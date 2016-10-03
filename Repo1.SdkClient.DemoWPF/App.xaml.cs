using System.Windows;
using Repo1.WPF45.SDK.Clients;

namespace Repo1.SdkClient.DemoWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var repo = new Repo1Client("C96DF613E23138A63F24BB139BF96037E27E6CDA");
            repo.CatchErrors(this);

            var win = new MainWindow();
            win.DataContext = new MainWindowVM(repo);
            win.Show();
        }
    }
}
