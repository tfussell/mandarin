#pragma once

#include <vector>

#include "NotifyIconNative.h"

namespace WindowsShellFacade {

class NotificationAreaNative
{
public:
	NotificationAreaNative();
	~NotificationAreaNative();

	void Update();

	std::vector<NotifyIconNative> &icons() { return icons_; }
	int icon_count() { return static_cast<int>(icons_.size()); }
	HWND window_handle() { return windowHandle; }
	DWORD process_id() { return processId; }

private:
	int QueryTBButtonCount();

	std::vector<NotifyIconNative> icons_;
	HWND windowHandle;
	DWORD processId;
};

}
