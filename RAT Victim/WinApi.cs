﻿using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RAT_Victim
{
    internal static class WinApi
    {
        private const string User = "user32";
        private const string Kernel = "kernel32";

        [DllImport(User)]
        public static extern uint GetForegroundWindow();

        [DllImport(User)]
        public static extern bool LockWorkStation();

        [DllImport(User)]
        public static extern uint GetWindowThreadProcessId(uint hWnd, out uint processId);

        [DllImport(User)]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport(User)]
        public static extern int MessageBox(uint hWnd, string lpText, string lpCaption, uint uType);

        [DllImport(User)]
        public static extern int GetWindowText(uint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(Kernel)]
        public static extern uint GetLastError();

        [DllImport(Kernel)]
        public static extern bool TerminateProcess(uint hProcess, int uExitCode);

        [DllImport(Kernel)]
        public static extern uint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport(User)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, int dwExtraInfo);
    }
}