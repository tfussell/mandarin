#pragma once

#include "NotificationAreaNative.h"
#include "NotifyIconManaged.h"

using namespace System;
using namespace System::Collections::Generic;

namespace WindowsShellFacade {

public ref class NotificationAreaManaged
{
public:
	NotificationAreaManaged()
	{
		native = new NotificationAreaNative();
		icons = gcnew List<NotifyIconManaged^>();
	}

	~NotificationAreaManaged()
	{
		delete native;
	}

	void Update()
	{
		native->Update();
		UpdateManagedIcons();
	}

	property List<NotifyIconManaged^>^ Icons
	{
		List<NotifyIconManaged^>^ get()
		{
			return icons;
		}
	}

	property int IconCount
	{
		int get()
		{
			return native->icon_count();
		}
	}

	property IntPtr WindowHandle
	{
		IntPtr get()
		{
			return IntPtr(native->window_handle());
		}
	}

	property UInt32 ProcessId
	{
		UInt32 get()
		{
			return UInt32(native->process_id());
		}
	}

private:
	void UpdateManagedIcons()
	{
		if(static_cast<int>(native->icons().size()) > icons->Count)
		{
			for(int i = icons->Count; i != native->icons().size(); i++)
			{
				icons->Add(gcnew NotifyIconManaged(&native->icons()[i]));
			}
		}

		for(int i = 0; i < icons->Count; i++)
		{
			icons[i]->Update();
		}
	}

	NotificationAreaNative *native;
	List<NotifyIconManaged^>^ icons;
};

}