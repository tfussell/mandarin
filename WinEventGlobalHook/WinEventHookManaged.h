#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

namespace HookWrapper {

public delegate void WindowChangedDelegateU(HWND hWnd, UINT eventId);
public delegate void WindowChangedDelegate(IntPtr hWnd);

class HookWrapperNative;

public ref class HookWrapperManaged
{
private:
	HookWrapperNative *native;
	bool hooked;
	GCHandle winEventDelegateGcHandle;

	HookWrapperManaged();
	~HookWrapperManaged();
	!HookWrapperManaged();
	void Shutdown_();

public:
	static HookWrapperManaged^ Instance;
	static HookWrapperManaged();

	event WindowChangedDelegate^ WindowCreated;
	event WindowChangedDelegate^ WindowDestroyed;
	event WindowChangedDelegate^ WindowShown;
	event WindowChangedDelegate^ WindowHidden;

	void Initialize();
	void Shutdown();

internal:
	void NotifyWinEvent(HWND hWnd, UINT eventId);

protected:
	void OnWindowCreated(IntPtr hWnd);
	void OnWindowDestroyed(IntPtr hWnd);
	void OnWindowShown(IntPtr hWnd);
	void OnWindowHidden(IntPtr hWnd);
};

void GlobalNotifyWinEvent(HWND hWnd, UINT eventId);

}
