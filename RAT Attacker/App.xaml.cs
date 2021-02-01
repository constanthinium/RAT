using System.Windows;
using System.Windows.Threading;

namespace RAT_Attacker
{
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = MessageBox.Show(e.Exception.ToString(), "Unhandled exception. Leave app running?",
                MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes;
        }
    }
}
