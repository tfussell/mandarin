// WinEventHook.h

#pragma once

#include <Windows.h>

using namespace System;
using namespace System::Collections::Generic;

namespace WinEventHook {

	void CALLBACK HandleWinEvent(HWINEVENTHOOK hook, DWORD event, HWND hwnd,
		LONG idObject, LONG idChild,
		DWORD dwEventThread, DWORD dwmsEventTime);

	public ref class Hook {
	public:
		static Hook ^Singleton;

		Hook() : hHook(0)
		{
			if (Singleton == nullptr)
			{
				Singleton = this;
			}

			createdWindows = gcnew List<IntPtr>();
			destroyedWindows = gcnew List<IntPtr>();

			InstallHook();
		}

		~Hook()
		{
			RemoveHook();
		}

		bool HasCreatedWindow()
		{
			return createdWindows->Count > 0;
		}

		IntPtr PopCreatedWindow()
		{
			IntPtr front = createdWindows[0];
			createdWindows->RemoveAt(0);
			return front;
		}

		bool HasDestroyedWindow()
		{
			return destroyedWindows->Count > 0;
		}

		IntPtr PopDestroyedWindow()
		{
			IntPtr front = destroyedWindows[0];
			destroyedWindows->RemoveAt(0);
			return front;
		}

		void InstallHook()
		{
			hHook = SetWinEventHook(0x8000, 0x8001, 0, &::WinEventHook::HandleWinEvent, 0, 0, 0);
		}

		void RemoveHook()
		{
			if (hHook != 0)
			{
				UnhookWinEvent(hHook);
			}
		}

		void HandleWinEvent(HWND hwnd, DWORD event)
		{
			if (event == 0x8000)
			{
				createdWindows->Add(IntPtr(hwnd));
			}
			else if (event == 0x8001)
			{
				if (createdWindows->Contains(IntPtr(hwnd)))
				{
					createdWindows->Remove(IntPtr(hwnd));
				}
				else
				{
					destroyedWindows->Add(IntPtr(hwnd));
				}
			}
		}

	private:
		List<IntPtr> ^createdWindows;
		List<IntPtr> ^destroyedWindows;
		HWINEVENTHOOK hHook;
	};

	void CALLBACK HandleWinEvent(HWINEVENTHOOK hook, DWORD event, HWND hwnd,
		LONG idObject, LONG idChild,
		DWORD dwEventThread, DWORD dwmsEventTime)
	{
		if (idObject != 0 || idChild != 0) return;
		Hook::Singleton->HandleWinEvent(hwnd, event);
	}
}
