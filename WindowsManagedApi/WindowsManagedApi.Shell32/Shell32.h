// Wma.Shell32.h

#pragma once

#include <string>

#include <Windows.h>
#include <ShellAPI.h>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace WindowsManagedApi {
namespace Shell32 {
namespace Structures {

}

namespace Enumerations {

}

public ref struct Functions 
{

};

public ref struct Helpers 
{
	value struct Sherb 
	{
		static const unsigned int NoConfirmation = 0x00000001;
		static const unsigned int NoProgressUi = 0x00000002;
		static const unsigned int NoSound = 0x00000004;
	};

	static int QueryRecycleBinNumItems()
	{
		SHQUERYRBINFO queryInfo = { 0 };
		queryInfo.cbSize = sizeof(queryInfo);
		std::wstring systemRoot = GetSystemRoot();
		if(::SHQueryRecycleBin(systemRoot.c_str(), &queryInfo) == S_OK)
			return (int)queryInfo.i64NumItems;
		return 0;
	}

    static bool EmptyRecycleBin(bool noConfirmation, bool noProgress, bool noSound)
	{
		DWORD dwFlags = 0;
		if(noConfirmation) dwFlags |= Sherb::NoConfirmation;
		if(noProgress) dwFlags |= Sherb::NoProgressUi;
		if(noSound) dwFlags |= Sherb::NoSound;

		std::wstring systemRoot = GetSystemRoot();
		HRESULT result = ::SHEmptyRecycleBin(NULL, systemRoot.c_str(), dwFlags);

		return result == S_OK;
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
}
