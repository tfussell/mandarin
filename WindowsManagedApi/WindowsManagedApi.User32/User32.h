// Wma.User32.h

#pragma once

#include <Windows.h>
#include <WinUser.h>
#include <vcclr.h>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace WindowsManagedApi {
namespace User32 {
namespace Structures {

public value struct PROCESS_DPI_AWARENESS
{

};

public value struct FEEDBACK_TYPE
{

};

public value struct COLORREF
{

};

public value struct BLENDFUNCTION
{

};

}

namespace Enumerations {

[Flags]
public enum class WindowStyle : unsigned int
{
	Border = WS_BORDER,
	Caption = WS_CAPTION,
	Child = WS_CHILD,
	ChildWindow = WS_CHILDWINDOW,
	ClipChildren = WS_CLIPCHILDREN,
	ClipSiblings = WS_CLIPSIBLINGS,
	Disabled = WS_DISABLED,
	DialogFrame = WS_DLGFRAME,
	Group = WS_GROUP,
	HorizontalScroll = WS_HSCROLL,
	Iconic = WS_ICONIC,
	Maximize = WS_MAXIMIZE,
	MaximizeBox = WS_MAXIMIZEBOX,
	Minimize = WS_MINIMIZE,
	MinimizeBox = WS_MINIMIZEBOX,
	Overlapped = WS_OVERLAPPED,
	OverlappedWindow = WS_OVERLAPPEDWINDOW,
	Popup = WS_POPUP,
	PopupWindow = WS_POPUPWINDOW,
	SizeBox = WS_SIZEBOX,
	SystemMenu = WS_SYSMENU,
	TabStop = WS_TABSTOP,
	ThickFrame = WS_THICKFRAME,
	Tiled = WS_TILED,
	TiledWindow = WS_TILEDWINDOW,
	Visible = WS_VISIBLE,
	VerticalScroll = WS_VSCROLL
};

[Flags]
public enum class ExtendedWindowStyle
{
	AcceptFiles = WS_EX_ACCEPTFILES,
	AppWindow = WS_EX_APPWINDOW,
	ClientEdge = WS_EX_CLIENTEDGE,
	Composited = WS_EX_COMPOSITED,
	ContextHelp = WS_EX_CONTEXTHELP,
	ControlParent = WS_EX_CONTROLPARENT,
	DialogModalFrame = WS_EX_DLGMODALFRAME,
	Layered = WS_EX_LAYERED,
	LayoutRightToLeft = WS_EX_LAYOUTRTL,
	Left = WS_EX_LEFT,
	LeftScrollbar = WS_EX_LEFTSCROLLBAR,
	LeftToRightReading = WS_EX_LTRREADING,
	MdiChild = WS_EX_MDICHILD,
	NoActivate = WS_EX_NOACTIVATE,
	NoInheritLayout = WS_EX_NOINHERITLAYOUT,
	NoParentNotify = WS_EX_NOPARENTNOTIFY,
	//NoRedirectionBitmap = WS_EX_NOREDIRECTIONBITMAP,
	OverlappedWindow = WS_EX_OVERLAPPEDWINDOW,
	PaletteWindow = WS_EX_PALETTEWINDOW,
	Right = WS_EX_RIGHT,
	RightScrollbar = WS_EX_RIGHTSCROLLBAR,
	RightToLeftReading = WS_EX_RTLREADING,
	StaticEdge = WS_EX_STATICEDGE,
	ToolWindow = WS_EX_TOOLWINDOW,
	TopMost = WS_EX_TOPMOST,
	Transparent = WS_EX_TRANSPARENT,
	WindowEdge = WS_EX_WINDOWEDGE
};

public enum class AllowSetForegroundWindow : unsigned int
{
	Any = ASFW_ANY
};

public enum class ShowWindow : int
{
	///<Summary>Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.</Summary>
	ForceMinimize = 11,
	///<Summary>Hides the window and activates another window.</Summary>
	Hide = 0,
	///<Summary>Maximizes the specified window.</Summary>
	Maximize = 3,
	///<Summary>Minimizes the specified window and activates the next top-level window in the Z order.</Summary>
	Minimize = 6,
	///<Summary>Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.</Summary>
	Restore = 9,
	///<Summary>Activates the window and displays it in its current size and position.</Summary>
	Show = 5,
	///<Summary>Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.</Summary>
	ShowDefault = 10,
	///<Summary>Activates the window and displays it as a maximized window.</Summary>
	ShowMaximized = 3,
	///<Summary>Activates the window and displays it as a minimized window.</Summary>
	ShowMinimized = 2,
	///<Summary>Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.</Summary>
	ShowMinNoActive = 7,
	///<Summary>Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.</Summary>
	ShowNa = 8,
	///<Summary>Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.</Summary>
	ShowNoActivate = 4,
	///<Summary>ctivates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.</Summary>
	ShowNormal = 1
};

}

public ref struct Functions {

    /// <summary>
    /// Calculates the required size of the window rectangle, based on the desired client-rectangle size. The window rectangle can then be passed to the CreateWindow function to create a window whose client area is the desired size.
	/// To specify an extended window style, use the AdjustWindowRectEx function.
    /// </summary>
    /// <param name="rect">A Rectangle that contains the coordinates of the top-left and bottom-right corners of the desired client area. When the function returns, the structure contains the coordinates of the top-left and bottom-right corners of the window to accommodate the desired client area.</param>
	/// <param name="style">The window style of the window whose required size is to be calculated. Note that you cannot specify the WS_OVERLAPPED style.</param>
	/// <param name="menu">Indicates whether the window has a menu.</param>
	/// <returns>
	/// If the function succeeds, the return value is nonzero.
	/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// A client rectangle is the smallest rectangle that completely encloses a client area. A window rectangle is the smallest rectangle that completely encloses the window, which includes the client area and the nonclient area.
	/// The AdjustWindowRect function does not add extra space when a menu bar wraps to two or more rows.
	/// The AdjustWindowRect function does not take the WS_VSCROLL or WS_HSCROLL styles into account. To account for the scroll bars, call the GetSystemMetrics function with SM_CXVSCROLL or SM_CYHSCROLL.
	/// </remarks>
	static bool AdjustWindowRect(System::Drawing::Rectangle rect, Enumerations::WindowStyle style, bool menu)
	{
		throw gcnew NotImplementedException();
	}
	
    /// <summary>
    /// Calculates the required size of the window rectangle, based on the desired size of the client rectangle. The window rectangle can then be passed to the CreateWindowEx function to create a window whose client area is the desired size.
    /// </summary>
    /// <param name="rect">A Rectangle that contains the coordinates of the top-left and bottom-right corners of the desired client area. When the function returns, the structure contains the coordinates of the top-left and bottom-right corners of the window to accommodate the desired client area.</param>
	/// <param name="style">The window style of the window whose required size is to be calculated. Note that you cannot specify the WS_OVERLAPPED style.</param>
	/// <param name="menu">Indicates whether the window has a menu.</param>
	/// <param name="extendedStyle">The extended window style of the window whose required size is to be calculated.</param>
	/// <returns>
	/// If the function succeeds, the return value is nonzero.
	/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// A client rectangle is the smallest rectangle that completely encloses a client area. A window rectangle is the smallest rectangle that completely encloses the window, which includes the client area and the nonclient area.
	/// The AdjustWindowRectEx function does not add extra space when a menu bar wraps to two or more rows.
	/// The AdjustWindowRectEx function does not take the WS_VSCROLL or WS_HSCROLL styles into account. To account for the scroll bars, call the GetSystemMetrics function with SM_CXVSCROLL or SM_CYHSCROLL.
	/// </remarks>
	static bool AdjustWindowRectEx(System::Drawing::Rectangle rect, Enumerations::WindowStyle style, bool menu, Enumerations::ExtendedWindowStyle extendedStyle)
	{
		throw gcnew NotImplementedException();
	}

    /// <summary>
    /// Enables the specified process to set the foreground window using the SetForegroundWindow function. The calling process must already be able to set the foreground window. For more information, see Remarks later in this topic.
    /// </summary>
    /// <param name="processId">The identifier of the process that will be enabled to set the foreground window. If this parameter is ASFW_ANY, all processes will be enabled to set the foreground window.</param>
	/// <returns>
	/// If the function succeeds, the return value is nonzero.
	/// If the function fails, the return value is zero. The function will fail if the calling process cannot set the foreground window. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// The system restricts which processes can set the foreground window. A process can set the foreground window only if one of the following conditions is true:
	/// The process is the foreground process.
	/// The process was started by the foreground process.
	/// The process received the last input event.
	/// There is no foreground process.
	/// The foreground process is being debugged.
	/// The foreground is not locked (see LockSetForegroundWindow).
	/// The foreground lock time-out has expired (see SPI_GETFOREGROUNDLOCKTIMEOUT in SystemParametersInfo).
	/// No menus are active.
	/// A process that can set the foreground window can enable another process to set the foreground window by calling AllowSetForegroundWindow. The process specified by dwProcessId loses the ability to set the foreground window the next time the user generates input, unless the input is directed at that process, or the next time a process calls AllowSetForegroundWindow, unless that process is specified.
	/// </remarks>
	static bool AllowSetForegroundWindow(IntPtr processId)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI AnimateWindow(
		_In_  HWND hwnd,
		_In_  DWORD dwTime,
		_In_  DWORD dwFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632669(v=vs.85).aspx
	*/
	static bool AnimateWindow(IntPtr hwnd, unsigned int dwTime, unsigned int dwFlags)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI AnyPopup(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632670(v=vs.85).aspx
	*/
	static bool AnyPopup()
	{
		throw gcnew NotImplementedException();
	}

	/*
	UINT WINAPI ArrangeIconicWindows(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632671(v=vs.85).aspx
	*/
	static unsigned int ArrangeIconicWindows(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HDWP WINAPI BeginDeferWindowPos(
		_In_  int nNumWindows
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632672(v=vs.85).aspx
	*/
	static IntPtr BeginDeferWindowPos(int nNumWindows)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI BringWindowToTop(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632673(v=vs.85).aspx
	*/
	static bool BringWindowToTop(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI CalculatePopupWindowPosition(
		_In_      const POINT *anchorPoint,
		_In_      const SIZE *windowSize,
		_In_      UINT flags,
		_In_opt_  RECT *excludeRect,
		_Out_     RECT *popupWindowPosition
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd565861(v=vs.85).aspx
	*/
	static bool CalculatePopupWindowPosition(System::Drawing::Point anchorPoint, System::Drawing::Size windowSize, unsigned int flags, System::Drawing::Rectangle excludeRect, [Out] System::Drawing::Rectangle% popupWindowPosition)
	{
		throw gcnew NotImplementedException();
	}

	/*
	WORD WINAPI CascadeWindows(
		_In_opt_  HWND hwndParent,
		_In_      UINT wHow,
		_In_opt_  const RECT *lpRect,
		_In_      UINT cKids,
		_In_opt_  const HWND *lpKids
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632674(v=vs.85).aspx
	*/
	static int CascadeWindows(IntPtr hwndParent, unsigned int wHow, System::Drawing::Rectangle lpRect, unsigned int cKids, IntPtr lpKids)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI ChangeWindowMessageFilter(
		_In_  UINT message,
		_In_  DWORD dwFlag
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632675(v=vs.85).aspx
	*/
	static bool ChangeWindowMessageFilter(unsigned int message, unsigned int dwFlag)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI ChangeWindowMessageFilterEx(
		_In_         HWND hWnd,
		_In_         UINT message,
		_In_         DWORD action,
		_Inout_opt_  PCHANGEFILTERSTRUCT pChangeFilterStruct
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd388202(v=vs.85).aspx
	*/
	static bool ChangeWindowMessageFilterEx(IntPtr hWnd, unsigned int message, unsigned int action, PCHANGEFILTERSTRUCT% pChangeFilterStruct)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI ChildWindowFromPoint(
		_In_  HWND hWndParent,
		_In_  POINT Point
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632676(v=vs.85).aspx
	*/
	static IntPtr ChildWindowFromPoint(IntPtr hWndParent, System::Drawing::Point Point)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI ChildWindowFromPointEx(
		_In_  HWND hwndParent,
		_In_  POINT pt,
		_In_  UINT uFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632677(v=vs.85).aspx
	*/
	static IntPtr ChildWindowFromPointEx(IntPtr hwndParent, System::Drawing::Point pt, unsigned int uFlags)	
	{
		throw gcnew NotImplementedException();
	}

    /// <summary>
    /// Minimizes (but does not destroy) the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window to be minimized.</param>
	/// <returns>
	/// If the function succeeds, the return value is nonzero.
	/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// To destroy a window, an application must use the DestroyWindow function.
	/// See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632678(v=vs.85).aspx
	/// </remarks>
	static bool CloseWindow(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI CreateWindowEx(
		_In_      DWORD dwExStyle,
		_In_opt_  LPCTSTR lpClassName,
		_In_opt_  LPCTSTR lpWindowName,
		_In_      DWORD dwStyle,
		_In_      int x,
		_In_      int y,
		_In_      int nWidth,
		_In_      int nHeight,
		_In_opt_  HWND hWndParent,
		_In_opt_  HMENU hMenu,
		_In_opt_  HINSTANCE hInstance,
		_In_opt_  LPVOID lpParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632680(v=vs.85).aspx
	*/
	static IntPtr CreateWindowEx(unsigned int dwExStyle, String^ lpClassName, String^ lpWindowName, unsigned int dwStyle, int x, int y, int nWidth,int nHeight, _In_opt_ IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, UIntPtr lpParam)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HDWP WINAPI DeferWindowPos(
		_In_      HDWP hWinPosInfo,
		_In_      HWND hWnd,
		_In_opt_  HWND hWndInsertAfter,
		_In_      int x,
		_In_      int y,
		_In_      int cx,
		_In_      int cy,
		_In_      UINT uFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632681(v=vs.85).aspx
	*/
	static IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, unsigned int uFlags)	
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI DeregisterShellHookWindow(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms644979(v=vs.85).aspx
	*/
	static bool DeregisterShellHookWindow(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI DestroyWindow(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632682(v=vs.85).aspx
	*/
	static bool DestroyWindow(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI EndDeferWindowPos(
		_In_  HDWP hWinPosInfo
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633440(v=vs.85).aspx
	*/
	static bool EndDeferWindowPos(IntPtr hWinPosInfo)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI EndTask(
		_In_  HWND hWnd,
		_In_  BOOL fShutDown,
		_In_  BOOL fForce
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633492(v=vs.85).aspx
	*/
	static bool EndTask(IntPtr hWnd, bool fShutDown, bool fForce)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL CALLBACK EnumChildProc(
		_In_  HWND hwnd,
		_In_  LPARAM lParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633493(v=vs.85).aspx
	*/
	static bool EnumChildProc(IntPtr hwnd,LPARAM lParam)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI EnumChildWindows(
		_In_opt_  HWND hWndParent,
		_In_      WNDENUMPROC lpEnumFunc,
		_In_      LPARAM lParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633494(v=vs.85).aspx
	*/
	static bool EnumChildWindows(IntPtr hWndParent, WNDENUMPROC lpEnumFunc,LPARAM lParam)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI EnumThreadWindows(
		_In_  DWORD dwThreadId,
		_In_  WNDENUMPROC lpfn,
		_In_  LPARAM lParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633495(v=vs.85).aspx
	*/
	static bool EnumThreadWindows(unsigned int dwThreadId, WNDENUMPROC lpfn, LPARAM lParam)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL CALLBACK EnumThreadWndProc(
		_In_  HWND hwnd,
		_In_  LPARAM lParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633496(v=vs.85).aspx
	*/
	static bool EnumThreadWndProc(IntPtr hwnd, LPARAM lParam)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI EnumWindows(
		_In_  WNDENUMPROC lpEnumFunc,
		_In_  LPARAM lParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633497(v=vs.85).aspx
	*/
	static bool EnumWindows(WNDENUMPROC lpEnumFunc, LPARAM lParam)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL CALLBACK EnumWindowsProc(
		_In_  HWND hwnd,
		_In_  LPARAM lParam
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633498(v=vs.85).aspx
	*/
	static bool EnumWindowsProc(IntPtr hwnd, LPARAM lParam)
	{
		throw gcnew NotImplementedException();
	}

    /// <summary>
    /// Retrieves a handle to the top-level window whose class name and window name match the specified strings. This function does not search child windows. This function does not perform a case-sensitive search.
	/// To search child windows, beginning with a specified child window, use the FindWindowEx function.
    /// </summary>
    /// <param name="className">
	/// The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
	/// If className is String.Empty, it finds any window whose title matches the windowName parameter.
	/// </param>
	/// <param name="windowName">
	/// The window name (the window's title). If this parameter is String.Empty, all window names match.
	/// </param>
	/// <returns>
	/// If the function succeeds, the return value is a handle to the window that has the specified class name and window name.
	/// If the function fails, the return value is IntPtr.Zero. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// If the windowName parameter is not String.Empty, FindWindow calls the GetWindowText function to retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks for GetWindowText.
	/// </remarks>
	static IntPtr FindWindow(String^ className, String^ windowName)
	{
		IntPtr lpClassName = Marshal::StringToHGlobalUni(className);
		IntPtr lpWindowName = Marshal::StringToHGlobalUni(windowName);

		IntPtr result = IntPtr::Zero;

		try
		{
			HWND windowHandle = ::FindWindow(static_cast<LPCWSTR>(lpClassName.ToPointer()), static_cast<LPCWSTR>(lpWindowName.ToPointer()));
			result = IntPtr(windowHandle);
		}
		finally
		{
			Marshal::FreeHGlobal(lpClassName);
			Marshal::FreeHGlobal(lpWindowName);
		}

		return result;
	}

	/*
	HWND WINAPI FindWindowEx(
		_In_opt_  HWND hwndParent,
		_In_opt_  HWND hwndChildAfter,
		_In_opt_  LPCTSTR lpszClass,
		_In_opt_  LPCTSTR lpszWindow
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633500(v=vs.85).aspx
	*/
	static IntPtr FindWindowEx( _In_opt_ IntPtr hwndParent, _In_opt_ IntPtr hwndChildAfter, _In_opt_ String^ lpszClass, _In_opt_ String^ lpszWindow)	
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetAltTabInfo(
		_In_opt_   HWND hwnd,
		_In_       int iItem,
		_Inout_    PALTTABINFO pati,
		_Out_opt_  LPTSTR pszItemText,
		_In_       UINT cchItemText
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633501(v=vs.85).aspx
	*/
	static bool GetAltTabInfo( _In_opt_ IntPtr hwnd,int iItem, _Inout_ PALTTABINFO pati, _Out_opt_ String^ pszItemText,unsigned int cchItemText)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetAncestor(
		_In_  HWND hwnd,
		_In_  UINT gaFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633502(v=vs.85).aspx
	*/
	static IntPtr GetAncestor(IntPtr hwnd,unsigned int gaFlags)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetClientRect(
		_In_   HWND hWnd,
		_Out_  LPRECT lpRect
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633503(v=vs.85).aspx
	*/
	static bool GetClientRect(IntPtr hWnd, _Out_ LPRECT lpRect)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetDesktopWindow(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633504(v=vs.85).aspx
	*/
	static IntPtr GetDesktopWindow()
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetForegroundWindow(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633505(v=vs.85).aspx
	*/
	static IntPtr GetForegroundWindow()
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetGUIThreadInfo(
		_In_     DWORD idThread,
		_Inout_  LPGUITHREADINFO lpgui
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633506(v=vs.85).aspx
	*/
	static bool GetGUIThreadInfo(unsigned int idThread, LPGUITHREADINFO% lpgui)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetLastActivePopup(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633507(v=vs.85).aspx
	*/
	static IntPtr GetLastActivePopup(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetLayeredWindowAttributes(
		_In_       HWND hwnd,
		_Out_opt_  COLORREF *pcrKey,
		_Out_opt_  BYTE *pbAlpha,
		_Out_opt_  DWORD *pdwFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633508(v=vs.85).aspx
	*/
	static bool GetLayeredWindowAttributes(IntPtr hwnd, _Out_opt_ COLORREF pcrKey, _Out_opt_ BYTE pbAlpha, _Out_opt_ unsigned int pdwFlags)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetNextWindow(
		_In_  HWND hWnd,
		_In_  UINT wCmd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633509(v=vs.85).aspx
	*/
	static IntPtr GetNextWindow2(IntPtr hWnd, unsigned int wCmd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetParent(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633510(v=vs.85).aspx
	*/
	static IntPtr GetParent(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetProcessDefaultLayout(
		_Out_  DWORD *pdwDefaultLayout
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633511(v=vs.85).aspx
	*/
	static bool GetProcessDefaultLayout([Out] unsigned int% pdwDefaultLayout)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HRESULT WINAPI GetProcessDPIAwareness(
		_In_   HANDLE hprocess,
		_Out_  PROCESS_DPI_AWARENESS *value
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/dn302113(v=vs.85).aspx
	*/
	static int GetProcessDPIAwareness(IntPtr hprocess, [Out] Structures::PROCESS_DPI_AWARENESS% value)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetShellWindow(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633512(v=vs.85).aspx
	*/
	static IntPtr GetShellWindow()
	{
		throw gcnew NotImplementedException();
	}

	/*
	DWORD WINAPI GetSysColor(
		_In_  int nIndex
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms724371(v=vs.85).aspx
	*/
	static unsigned int GetSysColor(int nIndex)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetTitleBarInfo(
		_In_     HWND hwnd,
		_Inout_  PTITLEBARINFO pti
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633513(v=vs.85).aspx
	*/
	static bool GetTitleBarInfo(IntPtr hwnd, _Inout_ PTITLEBARINFO pti)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetTopWindow(
		_In_opt_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633514(v=vs.85).aspx
	*/
	static IntPtr GetTopWindow( _In_opt_ IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI GetWindow(
		_In_  HWND hWnd,
		_In_  UINT uCmd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633515(v=vs.85).aspx
	*/
	static IntPtr GetWindow(IntPtr hWnd, unsigned int uCmd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetWindowDisplayAffinity(
		_In_   HWND hWnd,
		_Out_  DWORD *dwAffinity
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd375338(v=vs.85).aspx
	*/
	static bool GetWindowDisplayAffinity(IntPtr hWnd, [Out] unsigned int dwAffinity)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetWindowInfo(
		_In_     HWND hwnd,
		_Inout_  PWINDOWINFO pwi
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633516(v=vs.85).aspx
	*/
	static bool GetWindowInfo(IntPtr hwnd, PWINDOWINFO% pwi)
	{
		throw gcnew NotImplementedException();
	}

	/*
	UINT WINAPI GetWindowModuleFileName(
		_In_   HWND hwnd,
		_Out_  LPTSTR lpszFileName,
		_In_   UINT cchFileNameMax
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633517(v=vs.85).aspx
	*/
	static unsigned int GetWindowModuleFileName(IntPtr hwnd, [Out] String^% lpszFileName, unsigned int cchFileNameMax)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetWindowPlacement(
		_In_     HWND hWnd,
		_Inout_  WINDOWPLACEMENT *lpwndpl
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633518(v=vs.85).aspx
	*/
	static bool GetWindowPlacement(IntPtr hWnd, WINDOWPLACEMENT% lpwndpl)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI GetWindowRect(
		_In_   HWND hWnd,
		_Out_  LPRECT lpRect
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633519(v=vs.85).aspx
	*/
	static bool GetWindowRect(IntPtr hWnd, [Out] System::Drawing::Rectangle% lpRect)
	{
		throw gcnew NotImplementedException();
	}

	/*
	int WINAPI GetWindowText(
		_In_   HWND hWnd,
		_Out_  LPTSTR lpString,
		_In_   int nMaxCount
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633520(v=vs.85).aspx
	*/
	static int GetWindowText(IntPtr hWnd, [Out] String^% lpString,int nMaxCount)
	{
		throw gcnew NotImplementedException();
	}

	/*
	int WINAPI GetWindowTextLength(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633521(v=vs.85).aspx
	*/
	static int GetWindowTextLength(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	DWORD WINAPI GetWindowThreadProcessId(
		_In_       HWND hWnd,
		_Out_opt_  LPDWORD lpdwProcessId
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633522(v=vs.85).aspx
	*/
	static unsigned int GetWindowThreadProcessId(IntPtr hWnd, UIntPtr lpdwProcessId)
	{
		throw gcnew NotImplementedException();
	}

	/*
	int WINAPI InternalGetWindowText(
		_In_   HWND hWnd,
		_Out_  LPWSTR lpString,
		_In_   int nMaxCount
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633523(v=vs.85).aspx
	*/
	static int InternalGetWindowText(IntPtr hWnd, [Out] String^% lpString,int nMaxCount)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsChild(
		_In_  HWND hWndParent,
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633524(v=vs.85).aspx
	*/
	static bool IsChild(IntPtr hWndParent, IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsGUIThread(
		_In_  BOOL bConvert
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633525(v=vs.85).aspx
	*/
	static bool IsGUIThread(bool bConvert)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsHungAppWindow(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633526(v=vs.85).aspx
	*/
	static bool IsHungAppWindow(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsIconic(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633527(v=vs.85).aspx
	*/
	static bool IsIconic(IntPtr hWnd)
	{
		return ::IsIconic((HWND)hWnd.ToInt32());
	}

	/*
	BOOL WINAPI IsProcessDPIAware(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/aa969261(v=vs.85).aspx
	*/
	static bool IsProcessDPIAware()
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsWindow(
		_In_opt_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633528(v=vs.85).aspx
	*/
	static bool IsWindow(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsWindowUnicode(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633529(v=vs.85).aspx
	*/
	static bool IsWindowUnicode(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsWindowVisible(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633530(v=vs.85).aspx
	*/
	static bool IsWindowVisible(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI IsZoomed(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633531(v=vs.85).aspx
	*/
	static bool IsZoomed(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI LockSetForegroundWindow(
		_In_  UINT uLockCode
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633532(v=vs.85).aspx
	*/
	static bool LockSetForegroundWindow(unsigned int uLockCode)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI LogicalToPhysicalPoint(
		_In_     HWND hWnd,
		_Inout_  LPPOINT lpPoint
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633533(v=vs.85).aspx
	*/
	static bool LogicalToPhysicalPoint(IntPtr hWnd, System::Drawing::Point% lpPoint)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI MoveWindow(
		_In_  HWND hWnd,
		_In_  int X,
		_In_  int Y,
		_In_  int nWidth,
		_In_  int nHeight,
		_In_  BOOL bRepaint
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633534(v=vs.85).aspx
	*/
	static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI OpenIcon(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633535(v=vs.85).aspx
	*/
	static bool OpenIcon(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI PhysicalToLogicalPoint(
		_In_     HWND hWnd,
		_Inout_  LPPOINT lpPoint
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633536(v=vs.85).aspx
	*/
	static bool PhysicalToLogicalPoint(IntPtr hWnd, System::Drawing::Point% lpPoint)
	{
		throw gcnew NotImplementedException();
	}

    /// <summary>
	/// [This function is not intended for general use. It may be altered or unavailable in subsequent versions of Windows.]
	/// Creates an array of handles to icons that are extracted from a specified file.
    /// </summary>
    /// <param name="lpszFile">The path and name of the file from which the icon(s) are to be extracted.</param>
	/// <param name="nIconIndex">The zero-based index of the first icon to extract. For example, if this value is zero, the function extracts the first icon in the specified file.</param>
	/// <param name="cxIcon">The horizontal icon size wanted. See Remarks.</param>
	/// <param name="cyIcon">The vertical icon size wanted. See Remarks.</param>
	/// <param name="phicon">A pointer to the returned array of icon handles.</param>
	/// <param name="piconid">A pointer to a returned resource identifier for the icon that best fits the current display device. The returned identifier is 0xFFFFFFFF if the identifier is not available for this format. The returned identifier is 0 if the identifier cannot otherwise be obtained.</param>
	/// <param name="nIcons">The number of icons to extract from the file. This parameter is only valid when extracting from .exe and .dll files.</param>
	/// <param name="flags">Specifies flags that control this function. These flags are the LR_* flags used by the LoadImage function.</param>
	/// <returns>
	/// If the phicon parameter is NULL and this function succeeds, then the return value is the number of icons in the file. If the function fails then the return value is 0.
	/// If the phicon parameter is not NULL and the function succeeds, then the return value is the number of icons extracted. Otherwise, the return value is 0xFFFFFFFF if the file is not found.
	/// </returns>
	/// <remarks>
	/// This function extracts from executable (.exe), DLL (.dll), icon (.ico), cursor (.cur), animated cursor (.ani), and bitmap (.bmp) files. Extractions from Windows 3.x 16-bit executables (.exe or .dll) are also supported.
	/// The cxIcon and cyIcon parameters specify the size of the icons to extract. Two sizes can be extracted by putting the first size in the LOWORD of the parameter and the second size in the HIWORD. For example, MAKELONG(24, 48) for both the cxIcon and cyIcon parameters would extract both 24 and 48 size icons.
	/// You must destroy all icons extracted by PrivateExtractIcons by calling the DestroyIcon function.
	/// This function was not included in the SDK headers and libraries until Windows XP Service Pack 1 (SP1) and Windows Server 2003. If you do not have a header file and import library for this function, you can call the function using LoadLibrary and GetProcAddress.
	/// </remarks>
	static unsigned int PrivateExtractIcons(String^ lpszFile, int nIconIndex, int cxIcon, int cyIcon, 
		[Out] IntPtr% phicon, [Out] unsigned int% piconid, unsigned int nIcons, unsigned int flags)
	{
		pin_ptr<const TCHAR> ulpszFile = PtrToStringChars(lpszFile);
		HICON iconHandle = { (HICON)phicon.ToPointer() };
		unsigned int upiconid = 0;

		UINT result = ::PrivateExtractIcons(ulpszFile, nIconIndex, cxIcon, cyIcon, &iconHandle, &upiconid, nIcons, flags);

		piconid = upiconid;

		return result;
	}

	/*
	HWND WINAPI RealChildWindowFromPoint(
		_In_  HWND hwndParent,
		_In_  POINT ptParentClientCoords
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633537(v=vs.85).aspx
	*/
	static IntPtr RealChildWindowFromPoint(IntPtr hwndParent, System::Drawing::Point ptParentClientCoords)
	{
		throw gcnew NotImplementedException();
	}

	/*
	UINT WINAPI RealGetWindowClass(
		_In_   HWND hwnd,
		_Out_  LPTSTR pszType,
		_In_   UINT cchType
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633538(v=vs.85).aspx
	*/
	static unsigned int RealGetWindowClass(IntPtr hwnd, [Out] String^% pszType, unsigned int cchType)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI RegisterShellHookWindow(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms644989(v=vs.85).aspx
	*/
	static bool RegisterShellHookWindow(IntPtr hWnd)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetForegroundWindow(
		_In_  HWND hWnd
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633539(v=vs.85).aspx
	*/
	static bool SetForegroundWindow(IntPtr hWnd)
	{
		return ::SetForegroundWindow((HWND)hWnd.ToInt32());
	}

	/*
	BOOL WINAPI SetLayeredWindowAttributes(
		_In_  HWND hwnd,
		_In_  COLORREF crKey,
		_In_  BYTE bAlpha,
		_In_  DWORD dwFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633540(v=vs.85).aspx
	*/
	static bool SetLayeredWindowAttributes(IntPtr hwnd, COLORREF crKey, unsigned char bAlpha, unsigned int dwFlags)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI SetParent(
		_In_      HWND hWndChild,
		_In_opt_  HWND hWndNewParent
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633541(v=vs.85).aspx
	*/
	static IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetProcessDefaultLayout(
		_In_  DWORD dwDefaultLayout
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633542(v=vs.85).aspx
	*/
	static bool SetProcessDefaultLayout(unsigned int dwDefaultLayout)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetProcessDPIAware(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633543(v=vs.85).aspx
	*/
	static bool SetProcessDPIAware()
	{
		throw gcnew NotImplementedException();
	}

	/*
	HRESULT WINAPI SetProcessDPIAwareness(
		_In_  PROCESS_DPI_AWARENESS value
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/dn302122(v=vs.85).aspx
	*/
	static int SetProcessDPIAwareness(Structures::PROCESS_DPI_AWARENESS value)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetSysColors(
		_In_  int cElements,
		_In_  const INT *lpaElements,
		_In_  const COLORREF *lpaRgbValues
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms724940(v=vs.85).aspx
	*/
	static bool SetSysColors(int cElements, int lpaElements, COLORREF lpaRgbValues)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetWindowDisplayAffinity(
		_In_  HWND hWnd,
		_In_  DWORD dwAffinity
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/dd375340(v=vs.85).aspx
	*/
	static bool SetWindowDisplayAffinity(IntPtr hWnd, unsigned int dwAffinity)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetWindowFeedbackSettings(
		_In_      HWND hwnd,
		_In_      FEEDBACK_TYPE feedback,
		_In_      DWORD flags,
		_In_      UINT32 size,
		_In_opt_  const VOID *configuration
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/hh405402(v=vs.85).aspx
	*/
	static bool SetWindowFeedbackSettings(IntPtr hwnd, Structures::FEEDBACK_TYPE feedback, unsigned int flags, unsigned int size, IntPtr configuration)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetWindowPlacement(
		_In_  HWND hWnd,
		_In_  const WINDOWPLACEMENT *lpwndpl
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633544(v=vs.85).aspx
	*/
	static bool SetWindowPlacement(IntPtr hWnd, WINDOWPLACEMENT lpwndpl)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetWindowPos(
		_In_      HWND hWnd,
		_In_opt_  HWND hWndInsertAfter,
		_In_      int X,
		_In_      int Y,
		_In_      int cx,
		_In_      int cy,
		_In_      UINT uFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx
	*/
	static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, unsigned int uFlags)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SetWindowText(
		_In_      HWND hWnd,
		_In_opt_  LPCTSTR lpString
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633546(v=vs.85).aspx
	*/
	static bool SetWindowText(IntPtr hWnd, String^ lpString)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI ShowOwnedPopups(
		_In_  HWND hWnd,
		_In_  BOOL fShow
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633547(v=vs.85).aspx
	*/
	static bool ShowOwnedPopups(IntPtr hWnd, bool fShow)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI ShowWindow(
		_In_  HWND hWnd,
		_In_  int nCmdShow
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
	*/
	static bool ShowWindow(IntPtr hWnd, int nCmdShow)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI ShowWindowAsync(
		_In_  HWND hWnd,
		_In_  int nCmdShow
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633549(v=vs.85).aspx
	*/
	static bool ShowWindowAsync(IntPtr hWnd, int nCmdShow)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI SoundSentry(void);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/aa969269(v=vs.85).aspx
	*/
	static bool SoundSentry()
	{
		throw gcnew NotImplementedException();
	}

	/*
	VOID WINAPI SwitchToThisWindow(
		_In_  HWND hWnd,
		_In_  BOOL fAltTab
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633553(v=vs.85).aspx
	*/
	static void SwitchToThisWindow(IntPtr hWnd, bool fAltTab)
	{
		throw gcnew NotImplementedException();
	}

	/*
	WORD WINAPI TileWindows(
		_In_opt_  HWND hwndParent,
		_In_      UINT wHow,
		_In_opt_  const RECT *lpRect,
		_In_      UINT cKids,
		_In_opt_  const HWND *lpKids
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633554(v=vs.85).aspx
	*/
	static int TileWindows(IntPtr hwndParent, unsigned int wHow, System::Drawing::Rectangle lpRect, unsigned int cKids, IntPtr lpKids)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI UpdateLayeredWindow(
		_In_      HWND hwnd,
		_In_opt_  HDC hdcDst,
		_In_opt_  POINT *pptDst,
		_In_opt_  SIZE *psize,
		_In_opt_  HDC hdcSrc,
		_In_opt_  POINT *pptSrc,
		_In_      COLORREF crKey,
		_In_opt_  BLENDFUNCTION *pblend,
		_In_      DWORD dwFlags
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633556(v=vs.85).aspx
	*/
	static bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, System::Drawing::Point pptDst, System::Drawing::Size psize, IntPtr hdcSrc, System::Drawing::Point pptSrc, Structures::COLORREF crKey, Structures::BLENDFUNCTION pblend, unsigned int dwFlags)
	{
		throw gcnew NotImplementedException();
	}

	/*
	BOOL WINAPI UpdateLayeredWindowIndirect(
		_In_  HWND hwnd,
		_In_  const UPDATELAYEREDWINDOWINFO *pULWInfo
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633557(v=vs.85).aspx
	*/
	static bool UpdateLayeredWindowIndirect(IntPtr hwnd, UPDATELAYEREDWINDOWINFO pULWInfo)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI WindowFromPhysicalPoint(
		_In_  POINT Point
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/aa969270(v=vs.85).aspx
	*/
	static IntPtr WindowFromPhysicalPoint(System::Drawing::Point Point)
	{
		throw gcnew NotImplementedException();
	}

	/*
	HWND WINAPI WindowFromPoint(
		_In_  POINT Point
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633558(v=vs.85).aspx
	*/
	static IntPtr WindowFromPoint(System::Drawing::Point Point)
	{
		throw gcnew NotImplementedException();
	}

	/*
	int CALLBACK WinMain(
		_In_  HINSTANCE hInstance,
		_In_  HINSTANCE hPrevInstance,
		_In_  LPSTR lpCmdLine,
		_In_  int nCmdShow
	);

	See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms633559(v=vs.85).aspx
	*/
	static int WinMain(HINSTANCE hInstance,HINSTANCE hPrevInstance,LPSTR lpCmdLine,int nCmdShow)
	{
		throw gcnew NotImplementedException();
	}
};


public ref struct Helpers {
public:
	static unsigned int GetWindowThreadProcessId(IntPtr windowHandle)
	{
		unsigned int processId;
		::GetWindowThreadProcessId((HWND)windowHandle.ToPointer(), (LPDWORD)&processId);
		return processId;
	}

#pragma push_macro("ExtractIcon")
#undef ExtractIcon
	static System::Drawing::Image^ ExtractIcon(String^ file, int iconIndex)
	{
		pin_ptr<const TCHAR> ulpszFile = PtrToStringChars(file);
		HICON iconHandle = { NULL };
		UINT upiconid = 0;
		int size = 1024;
		UINT result = 0;

		while(size > 8 && iconHandle == NULL)
		{
			result = ::PrivateExtractIcons(ulpszFile, iconIndex, size, size, &iconHandle, &upiconid, 1, 0);
			size /= 2;
		}

		System::Drawing::Image^ image = nullptr;

		if(result != 0 && result != 0xFFFFFFFF && iconHandle != NULL)
		{
			System::Drawing::Icon^ icon = System::Drawing::Icon::FromHandle(IntPtr(iconHandle));
			image = icon->ToBitmap();
			DestroyIcon(iconHandle);
		}

		return image;
	}

	static System::Drawing::Image^ ExtractIcon(String^ file)
	{
		return ExtractIcon(file, 0);
	}
#pragma pop_macro("ExtractIcon")
};

}
}
