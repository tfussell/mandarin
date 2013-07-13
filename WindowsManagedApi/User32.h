#pragma once

#include <Windows.h>
#include <vcclr.h>

using namespace System;
using namespace System::Text;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;

namespace WindowsManagedApi {

public ref struct User32 
{
public:
	ref class WM
    {
	public:
        static const UInt32 CLOSE = 0x0010;
        static const UInt32 GETICON = 0x007F;
        static const UInt32 KEYDOWN = 0x0100;
        static const UInt32 COMMAND = 0x0111;
        static const UInt32 USER = 0x0400; // 0x0400 - 0x7FFF
        static const UInt32 APP = 0x8000; // 0x8000 - 0xBFFF
    };

	ref struct TB
    {
	public: 
		static const UInt32 GETBUTTON = WM::USER + 23;
        static const UInt32 BUTTONCOUNT = WM::USER + 24;
        static const UInt32 CUSTOMIZE = WM::USER + 27;
        static const UInt32 GETBUTTONTEXTA = WM::USER + 45;
        static const UInt32 GETBUTTONTEXTW = WM::USER + 75;
    };

	ref struct Spi
	{
		static const Int32 SetWorkArea = 47;
	};

	ref struct Ulw
	{
		static const Int32 ColorKey = 0x00000001;
		static const Int32 Alpha = 0x00000002;
		static const Int32 Opaque = 0x00000004;
	};

	ref struct Ac
	{
		static const byte SrcOver = 0x00;
		static const byte SrcAlpha = 0x01;
	};

	ref struct Sw
	{ 
		static const Int32 ShowNormal = 1;
	};

    static IntPtr SendMessage(IntPtr hWnd, UInt32 msg, UIntPtr wParam, IntPtr lParam);
    static bool ShowWindow(IntPtr hWnd, int nCmdShow);
    static bool SetForegroundWindow(IntPtr hWnd);
    static IntPtr FindWindow(String^ lpClassName, String^ lpWindowName);
    static IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, String^ lpszClass, String^ lpszWindow);
    static String^ GetWindowText(IntPtr hWnd);
    static UInt32 GetWindowThreadProcessId(IntPtr hWnd);
    static void SetWorkingArea(int left, int right, int top, int bottom);
	static unsigned int GetClassLongPtr(IntPtr hWnd, int nIndex);
    static bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, Point pptDst, Size psize, IntPtr hdcSrc);
    static IntPtr GetDC(IntPtr hWnd);
    static int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    static Image^ ExtractIcon(String^ lpszFile, int index);
    static IntPtr GetDesktopWindow();
    static IntPtr GetWindowDC(IntPtr ptr);
    static int GetWindowLong(IntPtr hWnd, int nIndex);
    static bool IsIconic(IntPtr hWnd);

private:
	static UInt32 GetClassLongPtr32(IntPtr hWnd, int nIndex);
    static UInt64 GetClassLongPtr64(IntPtr hWnd, int nIndex);
};
}
