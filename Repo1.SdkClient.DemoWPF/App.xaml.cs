using System.Windows;
using Repo1.WPF452.SDK.Clients;

namespace Repo1.SdkClient.DemoWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var repo = new Repo1Client("settings.cfg");
            if (repo == null) return;
            repo.CatchErrors(this);

            var win = new MainWindow();
            win.DataContext = new MainWindowVM(repo);
            win.Show();
        }
    }
}
