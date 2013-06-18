#include <Windows.h>
#include <CommCtrl.h>

#include "NotifyIconNative.h"

namespace WindowsShellFacade {

NotifyIconNative::NotifyIconNative(HWND parentWindowHandle, DWORD parentProcessId, int index) : index_(index)
{
	notificationAreaWindowHandle = parentWindowHandle;
	notificationAreaProcessId = parentProcessId;
}

struct TBBUTTONFixed
{
	int iBitmap;
	int idCommand;
	unsigned char fsState;
	unsigned char fsStyle;
	unsigned char bReserved1;
	unsigned char bReserved2;
	unsigned char bReserved3;
	unsigned char bReserved4;
	unsigned char bReserved5;
	unsigned char bReserved6;
	int dwData;
	int iString;
};

void NotifyIconNative::Update()
{
	const size_t bufferSize = 0x100;
	unsigned char localBuffer[bufferSize];
	memset(localBuffer, 0, bufferSize);

	int accessFlags = PROCESS_VM_WRITE | PROCESS_VM_READ | PROCESS_VM_OPERATION;
	HANDLE processHandle = OpenProcess(accessFlags, FALSE, notificationAreaProcessId);

	if(processHandle)
	{
		void *remoteBuffer = VirtualAllocEx(processHandle, NULL, bufferSize, MEM_COMMIT, PAGE_READWRITE);

		if(remoteBuffer)
		{
			if(SendMessage(notificationAreaWindowHandle, TB_GETBUTTON, index_, (LPARAM)remoteBuffer))
			{
				int bytesRead = 0;
				if(ReadProcessMemory(processHandle, remoteBuffer, localBuffer, 32, (SIZE_T *)&bytesRead) == TRUE)
				{
					int buttonDwData;
					memcpy(&buttonDwData, (unsigned char *)localBuffer + 16, sizeof(int));
					int commandId;
					memcpy(&commandId, (unsigned char*)localBuffer + 4, sizeof(int));
					
					if(ReadProcessMemory(processHandle, (LPCVOID)buttonDwData, localBuffer, 48, (SIZE_T *)&bytesRead) == TRUE)
					{
						memcpy(&windowHandle, (unsigned char *)localBuffer + 0, sizeof(int));
						memcpy(&iconHandle, (unsigned char *)localBuffer + 24, sizeof(int));
						GetWindowThreadProcessId(windowHandle, &parentProcess);
						int length = static_cast<int>(SendMessage(notificationAreaWindowHandle, TB_GETBUTTONTEXTW, commandId, (LPARAM)remoteBuffer));
						ReadProcessMemory(processHandle, (LPCVOID)remoteBuffer, localBuffer, length * sizeof(wchar_t), NULL);
						text_ = std::wstring((wchar_t*)localBuffer, (wchar_t*)localBuffer + length);
					}
					else
					{
						throw gcnew System::Exception();
					}
				}
				else
				{
					throw gcnew System::Exception();
				}
			}
			else
			{
				throw gcnew System::Exception();
			}

			VirtualFreeEx(processHandle, remoteBuffer, NULL, MEM_RELEASE);
		}
		else
		{
			throw gcnew System::Exception();
		}

		CloseHandle(processHandle);
	}
	else
	{
		throw gcnew System::Exception();
	}
}

NotifyIconNative::~NotifyIconNative()
{

}

}