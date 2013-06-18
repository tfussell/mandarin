#include <Windows.h>

#include "WinEventHookManaged.h"
#include "WinEventHookNative.h"

namespace HookWrapper {

HookWrapperManaged::HookWrapperManaged() : native(NULL), hooked(false)
{
	native = new HookWrapperNative();
}

HookWrapperManaged::~HookWrapperManaged()
{
	this->!HookWrapperManaged();
}

HookWrapperManaged::!HookWrapperManaged()
{
	Shutdown();
}

void HookWrapperManaged::Shutdown_()
{
	if(hooked)
	{
		native->RemoveHook();
		winEventDelegateGcHandle.Free();
		delete native;
		native = NULL;
	}
	hooked = false;
}

static HookWrapperManaged::HookWrapperManaged()
{
	Instance = gcnew HookWrapperManaged();
}

void HookWrapperManaged::Initialize()
{
	if(!hooked)
	{
		native->AddHook();
		hooked = true;

		WindowChangedDelegateU^ eventDelegate = gcnew WindowChangedDelegateU(GlobalNotifyWinEvent);
		winEventDelegateGcHandle = GCHandle::Alloc(eventDelegate);
		IntPtr eventDelegateFunctionPointer = Marshal::GetFunctionPointerForDelegate(eventDelegate);
		native->SetWinEventCallback(static_cast<WINDOWCHANGECB>(eventDelegateFunctionPointer.ToPointer()));
	}
}

void HookWrapperManaged::Shutdown()
{
	Instance->Shutdown_();
}

void HookWrapperManaged::NotifyWinEvent(HWND hWnd, UINT eventId)
{
	switch(eventId)
	{
	case EVENT_OBJECT_CREATE:
		OnWindowCreated(IntPtr(hWnd));
		break;
	case EVENT_OBJECT_DESTROY:
		OnWindowDestroyed(IntPtr(hWnd));
		break;
	case EVENT_OBJECT_SHOW:
		OnWindowShown(IntPtr(hWnd));
		break;
	case EVENT_OBJECT_HIDE:
		OnWindowHidden(IntPtr(hWnd));
		break;
	}
}

void HookWrapperManaged::OnWindowCreated(IntPtr hWnd)
{
	WindowCreated(hWnd);
}

void HookWrapperManaged::OnWindowDestroyed(IntPtr hWnd)
{
	WindowDestroyed(hWnd);
}

void HookWrapperManaged::OnWindowShown(IntPtr hWnd)
{
	WindowShown(hWnd);
}

void HookWrapperManaged::OnWindowHidden(IntPtr hWnd)
{
	WindowHidden(hWnd);
}

void GlobalNotifyWinEvent(HWND hWnd, UINT eventId)
{
	HookWrapperManaged::Instance->NotifyWinEvent(hWnd, eventId);
}

}
