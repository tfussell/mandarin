#pragma once

#include <Windows.h>

using namespace System;

namespace WindowsManagedApi {

public value struct Gdi32
{
    static IntPtr CreateCompatibleDC(IntPtr hDC)
	{
		return IntPtr(::CreateCompatibleDC((HDC)hDC.ToPointer()));
	}

    static bool DeleteDC(IntPtr hdc)
	{
		return ::DeleteDC((HDC)hdc.ToPointer()) != 0;
	}

    static IntPtr SelectObject(IntPtr hDC, IntPtr hObject)
	{
		return IntPtr(::SelectObject((HDC)hDC.ToPointer(), (HGDIOBJ)hObject.ToPointer()));
	}

    static bool DeleteObject(IntPtr hObject)
	{
		return ::DeleteObject((HGDIOBJ)hObject.ToPointer()) != 0;
	}
};

}