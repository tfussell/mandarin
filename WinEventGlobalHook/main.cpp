#include <Windows.h>

#include "main.h"
#include "WinEventHookNative.h"

namespace HookWrapper {

HookWrapperNative *globalHookWrapper = NULL;

void CALLBACK WinEventProc(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd,
	LONG idObject, LONG idChild, DWORD dwEventThread, DWORD dwmsEventTime)
{
	if (hwnd && idObject == OBJID_WINDOW && idChild == CHILDID_SELF && IsWindow(hwnd))
	{
		globalHookWrapper->NotifyWinEvent(hwnd, event);
	}
}

}
