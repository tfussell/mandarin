#pragma once

#include <string>
#include <Windows.h>

using namespace System;

namespace WindowsShellFacade {

public ref struct Shell32
{
	value struct Sherb 
	{
		static const unsigned int NoConfirmation = 0x00000001;
		static const unsigned int NoProgressUi = 0x00000002;
		static const unsigned int NoSound = 0x00000004;
	};

	static int SHQueryRecycleBin()
	{
		SHQUERYRBINFO queryInfo = { 0 };
		queryInfo.cbSize = sizeof(queryInfo);
		std::wstring systemRoot = GetSystemRoot();
		if(::SHQueryRecycleBin(systemRoot.c_str(), &queryInfo) == S_OK)
			return (int)queryInfo.i64NumItems;
		return 0;
	}

    static bool SHEmptyRecycleBin(bool noConfirmation, bool noProgress, bool noSound)
	{
		DWORD dwFlags = 0;
		if(noConfirmation) dwFlags |= Sherb::NoConfirmation;
		if(noProgress) dwFlags |= Sherb::NoProgressUi;
		if(noSound) dwFlags |= Sherb::NoSound;

		std::wstring systemRoot = GetSystemRoot();
		HRESULT result = ::SHEmptyRecycleBin(NULL, systemRoot.c_str(), dwFlags);
		if(result == S_OK) return true;

		return false;
	}

private:
	static std::wstring GetSystemRoot()
	{
		TCHAR buffer[MAX_PATH];
		buffer[0] = '\0';
		int length = GetWindowsDirectory(buffer, MAX_PATH);

		if(length > 0)
		{
			int firstPathSeparatorIndex = -1;
			for(int i = 0; i < length; i++)
			{
				if(buffer[i] == '\\')
				{
					firstPathSeparatorIndex = i;
				}
			}

			if(firstPathSeparatorIndex != -1)
			{
				buffer[firstPathSeparatorIndex + 1] = '\0';
			}

			return std::wstring(buffer, buffer + firstPathSeparatorIndex + 1);
		}

		return std::wstring();
	}
};
}