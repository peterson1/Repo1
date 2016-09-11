using System.Windows;
using Autofac;
using Repo1.ExeUploader.WPF.ComponentRegistry;
using Repo1.ExeUploader.WPF.Configuration;

namespace Repo1.ExeUploader.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
