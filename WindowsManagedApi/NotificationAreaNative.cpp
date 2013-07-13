#include "NotificationAreaNative.h"

#include <CommCtrl.h>

namespace WindowsManagedApi {

HWND FindNotificationAreaWindowHandle()
{
    HWND hWnd = ::FindWindow(L"Shell_TrayWnd", NULL);
    if(hWnd)
    {
        hWnd = ::FindWindowEx(hWnd,NULL,L"TrayNotifyWnd", NULL);
        if(hWnd)
        {
            hWnd = ::FindWindowEx(hWnd,NULL,L"SysPager", NULL);
            if(hWnd)
            {                
                hWnd = ::FindWindowEx(hWnd, NULL,L"ToolbarWindow32", NULL);
            }
        }
    }
    return hWnd;
}

NotificationAreaNative::NotificationAreaNative() : windowHandle(NULL), processId(NULL)
{
	windowHandle = FindNotificationAreaWindowHandle();

	if(!windowHandle)
	{
		throw std::exception("NotificationAreaNative::NotificationAreaNative(): Can't find HWND of notification area.");
	}

	GetWindowThreadProcessId(windowHandle, &processId);

	if(!processId)
	{
		throw std::exception("NotificationAreaNative::NotificationAreaNative(): Can't find process identifier of notification area.");
	}

	Update();
}

NotificationAreaNative::~NotificationAreaNative()
{

}

int NotificationAreaNative::QueryTBButtonCount()
{
	return (int)::SendMessage(windowHandle, TB_BUTTONCOUNT, 0, 0);
}

void NotificationAreaNative::Update()
{
	int buttonCount = QueryTBButtonCount();

	while(buttonCount > static_cast<int>(icons_.size()))
	{
		icons_.push_back(NotifyIconNative(windowHandle, processId, static_cast<int>(icons_.size())));
	}

	if(buttonCount < static_cast<int>(icons_.size()))
	{
		icons_.erase(icons_.begin() + buttonCount, icons_.end());
	}
	/*
	for(int i = 0; i < buttonCount; i++)
	{
		icons_[i].Update();
	}
	*/
}

}