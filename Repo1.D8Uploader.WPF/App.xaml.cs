using System.Windows;
using Autofac;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.WPF45.SDK.ErrorHandlers;

namespace Repo1.D8Uploader.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThreadedAlerter.CatchErrors(this);

            var cfg = CfgReader.ReadAndParseFromLocal();
            if (cfg == null)
            {
                MessageBox.Show($"Config not found in {CfgReader.ExpectedPath}");
                return;
            }

            var win = new MainWindow();
            using (var scope = Components.GetContainer(cfg).BeginLifetimeScope())
            {
                win.DataContext = scope.Resolve<MainWindowVM>();
            }
            win.Show();
        }
    }
}
