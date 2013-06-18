#pragma once

namespace HookWrapper {

class HookWrapperNative;
extern HookWrapperNative *globalHookWrapper;

void CALLBACK WinEventProc(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd,
LONG idObject, LONG idChild, DWORD dwEventThread, DWORD dwmsEventTime);

}
