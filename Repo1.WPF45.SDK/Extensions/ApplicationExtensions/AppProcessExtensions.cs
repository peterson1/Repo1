﻿using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Repo1.WPF45.SDK.Extensions.ApplicationExtensions
{
    public static class AppProcessExtensions
    {
        public static void Relaunch(this Application app)
        {
            var origExe = Assembly.GetEntryAssembly().Location;

            Process.Start(origExe);

            app.Shutdown();
        }
    }
}
