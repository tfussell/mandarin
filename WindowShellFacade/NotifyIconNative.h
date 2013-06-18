#pragma once

#include <string>
#include <Windows.h>

namespace WindowsShellFacade {

class NotifyIconNative
{
public:
	NotifyIconNative(HWND parentWindowHandle, DWORD parentProcessId, int index);
	~NotifyIconNative();

	void Update();

	std::wstring text() { return text_; }
	int index() { return index_; }
	HICON icon_handle() { return iconHandle; }

private:
	int index_;
	HWND windowHandle;
	DWORD parentProcess;
	HWND notificationAreaWindowHandle;
	DWORD notificationAreaProcessId;
	std::wstring text_;
	HICON iconHandle;
};

}
