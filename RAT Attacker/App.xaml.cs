using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RAT_Attacker
{
    public partial class App
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var form = new Form
            {
                Width = 800,
                Height = 450,
                MinimumSize = new Size(400, 225),
                Icon = new Icon(GetResourceStream(new Uri("pack://application:,,,/RAT Attacker;component/Icons/shell32_240.ico")).Stream),
                Text = "Error!"
            };

            var textBox = new TextBox
            {
                ReadOnly = true,
                Text = e.Exception.ToString(),
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font(new FontFamily("Consolas"), 12)
            };

            form.Controls.Add(textBox);
            form.Show();
            SystemSounds.Hand.Play();
            textBox.SelectionLength = 0;
        }
    }
}
