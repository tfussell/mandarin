#pragma once

#include "NotifyIconNative.h"

using namespace System;
using namespace System::Drawing;

namespace WindowsManagedApi {
namespace NotificationArea {

public ref class NotifyIconManaged
{
public:
	NotifyIconManaged(NotifyIconNative *native)
	{
		this->native = native;
	}

	void Update()
	{
		this->native->Update();
	}

	property int Index
	{
		int get()
		{
			return native->index();
		}
	}

	property Bitmap^ IconImage
	{
		Bitmap^ get()
		{
			try
			{
				return Bitmap::FromHicon(IntPtr(native->icon_handle()));
			}
			catch (Exception^)
			{
				return nullptr;
			}
		}
	}

	property String^ TooltipText
	{
		String^ get()
		{
			return gcnew String(native->text().c_str());
		}
	}

private:
	NotifyIconNative *native;
};

}
}