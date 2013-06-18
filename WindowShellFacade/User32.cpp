#include "User32.h"

namespace WindowsShellFacade {

IntPtr User32::SendMessage(IntPtr hWnd, UInt32 msg, UIntPtr wParam, IntPtr lParam)
{
	return IntPtr(::SendMessage((HWND)hWnd.ToPointer(), msg, (WPARAM)wParam.ToPointer(), (LPARAM)lParam.ToPointer()));
}

bool User32::ShowWindow(IntPtr hWnd, int nCmdShow)
{
	return ::ShowWindow((HWND)hWnd.ToPointer(), nCmdShow) != 0;
}

bool User32::SetForegroundWindow(IntPtr hWnd)
{
	return ::SetForegroundWindow((HWND)hWnd.ToPointer()) != 0;
}

IntPtr User32::FindWindow(String^ lpClassName, String^ lpWindowName)
{
	pin_ptr<const TCHAR> ulpClassName = PtrToStringChars(lpClassName);
	pin_ptr<const TCHAR> ulpWindowName = PtrToStringChars(lpWindowName);
	return IntPtr(::FindWindow(ulpClassName, ulpWindowName));
}

IntPtr User32::FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, String^ lpszClass, String^ lpszWindow)
{
	pin_ptr<const TCHAR> ulpszClass = PtrToStringChars(lpszClass);
	pin_ptr<const TCHAR> ulpszWindow = PtrToStringChars(lpszWindow);
	return IntPtr(::FindWindowEx((HWND)hwndParent.ToPointer(), (HWND)hwndChildAfter.ToPointer(), ulpszClass, ulpszWindow));
}

String^ User32::GetWindowText(IntPtr hWnd)
{
	TCHAR buffer[256];
	buffer[0] = '\0';
	Int32 length = ::GetWindowText((HWND)hWnd.ToPointer(), buffer, 256);
	return gcnew String(buffer);
}

UInt32 User32::GetWindowThreadProcessId(IntPtr hWnd)
{
	UInt32 processId;
	::GetWindowThreadProcessId((HWND)hWnd.ToPointer(), (LPDWORD)&processId);
	return processId;
}

void User32::SetWorkingArea(int left, int right, int top, int bottom)
{
	RECT workArea = { left, right, top, bottom };
	::SystemParametersInfo(SPI_SETWORKAREA, 0, (PVOID)&workArea, 0);
}

UInt32 User32::GetClassLongPtr32(IntPtr hWnd, int nIndex)
{
	return ::GetClassLong((HWND)hWnd.ToPointer(), nIndex);
}

UInt64 User32::GetClassLongPtr64(IntPtr hWnd, int nIndex)
{
	return ::GetClassLongPtr((HWND)hWnd.ToPointer(), nIndex);
}

unsigned int User32::GetClassLongPtr(IntPtr hWnd, int nIndex)
{
    return IntPtr::Size > 4 ? (unsigned int)GetClassLongPtr64(hWnd, nIndex) : GetClassLongPtr32(hWnd, nIndex);
}

bool User32::UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, Point pptDst, Size psize, IntPtr hdcSrc)
{
	POINT upptDst = { pptDst.X, pptDst.Y };
	SIZE upsize = { psize.Width, psize.Height };
	POINT upprSrc = { 0, 0 };
	BLENDFUNCTION pblend = { AC_SRC_OVER,  0, 255, AC_SRC_ALPHA };
	return ::UpdateLayeredWindow((HWND)hwnd.ToPointer(), (HDC)hdcDst.ToPointer(), &upptDst, &upsize, (HDC)hdcSrc.ToPointer(), &upprSrc, 0, &pblend, ULW_ALPHA) != 0;
}

IntPtr User32::GetDC(IntPtr hWnd)
{
	return IntPtr(::GetDC((HWND)hWnd.ToPointer()));
}

int User32::ReleaseDC(IntPtr hWnd, IntPtr hDC)
{
	return ::ReleaseDC((HWND)hWnd.ToPointer(), (HDC)hDC.ToPointer());
}

Image^ User32::ExtractIcon(String^ lpszFile)
{
	pin_ptr<const TCHAR> ulpszFile = PtrToStringChars(lpszFile);
	HICON iconHandle = { NULL };
	UINT upiconid = 0;
	int size = 1024;
	UINT result = 0;

	while(size > 8 && iconHandle == NULL)
	{
		result = ::PrivateExtractIcons(ulpszFile, 0, size, size, &iconHandle, &upiconid, 1, 0);
		size /= 2;
	}

	Image^ image = nullptr;

	if(result != 0 && result != 0xFFFFFFFF && iconHandle != NULL)
	{
		Icon^ icon = Icon::FromHandle(IntPtr(iconHandle));
		image = icon->ToBitmap();
		DestroyIcon(iconHandle);
	}

	return image;
}

IntPtr User32::GetDesktopWindow()
{
	return IntPtr(::GetDesktopWindow());
}

IntPtr User32::GetWindowDC(IntPtr ptr)
{
	return IntPtr(::GetWindowDC((HWND)ptr.ToPointer()));
}

int User32::GetWindowLong(IntPtr hWnd, int nIndex)
{
	return ::GetWindowLong((HWND)hWnd.ToPointer(), nIndex);
}

bool User32::IsIconic(IntPtr hWnd)
{
	return ::IsIconic((HWND)hWnd.ToPointer()) != 0;
}

}
