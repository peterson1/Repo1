using System.Windows;

namespace Repo1.LegacyClient.DemoWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var win         = new MainWindow();
            win.DataContext = new MainWindowVM();
            win.Show();
        }
    }
}
