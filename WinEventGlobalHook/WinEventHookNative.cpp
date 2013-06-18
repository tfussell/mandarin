#include <Windows.h>

#include "WinEventHookNative.h"
#include "main.h"

namespace HookWrapper {

HookWrapperNative::HookWrapperNative() : winEventHookHandle_(NULL), winEventCallback_(NULL)
{

}

void HookWrapperNative::AddHook()
{
	globalHookWrapper = this;
	winEventHookHandle_ = SetWinEventHook(
		EVENT_MIN, EVENT_MAX,
		NULL, WinEventProc, 0, 0,
		WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
}

void HookWrapperNative::RemoveHook()
{
	if (winEventHookHandle_)
	{
		UnhookWinEvent(winEventHookHandle_);
	}
	globalHookWrapper = NULL;
}

void HookWrapperNative::NotifyWinEvent(HWND hWnd, UINT eventId)
{
	if(winEventCallback_ != NULL)
	{
		(*winEventCallback_)(hWnd, eventId);
	}
}

bool HookWrapperNative::FilterWindow(HWND hWnd)
{
	LONG style = GetWindowLong(hWnd, GWL_STYLE);
	if(!(style & WS_CHILD))
		return false;
	return true;
}

void HookWrapperNative::SetWinEventCallback(WINDOWCHANGECB callback)
{
	winEventCallback_ = callback;
}

}
