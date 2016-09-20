using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Repo1.WPF452.SDK.Helpers.ApplicationExtensions
{
    public static class BasicApplicationExtensions
    {
        public static void Relaunch (this Application app)
        {
            var origExe = Assembly.GetEntryAssembly().Location;

            Process.Start(origExe);

            app.Shutdown();
        }
    }
}
