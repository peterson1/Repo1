using System.Net;
using System.Windows;
using Autofac;
using Repo1.ExeUploader.WPF.ComponentRegistry;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.WPF452.SDK.Helpers.ErrorHandlers;

namespace Repo1.ExeUploader.WPF
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
            using (var scope = IoC.GetContainer(cfg).BeginLifetimeScope())
            {
                win.DataContext = scope.Resolve<MainWindowVM>();
            }
            win.Show();
        }
    }
}
