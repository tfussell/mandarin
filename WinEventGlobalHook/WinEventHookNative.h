#pragma once

typedef void (__stdcall *WINDOWCHANGECB)(HWND hWnd, UINT eventId);

namespace HookWrapper {

class HookWrapperNative
{
public:
	HookWrapperNative();
	~HookWrapperNative() {}

	void AddHook();
	void RemoveHook();
	void SetWinEventCallback(WINDOWCHANGECB callback);
	void NotifyWinEvent(HWND hWnd, UINT eventId);

private:
	bool FilterWindow(HWND hWnd);

	HWINEVENTHOOK winEventHookHandle_;
	WINDOWCHANGECB winEventCallback_;
};

}
