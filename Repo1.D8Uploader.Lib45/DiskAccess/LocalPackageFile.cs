using System;
using System.Collections.Generic;
using System.IO;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.Extensions.R1ModelExtensions;

namespace Repo1.D8Uploader.Lib45.DiskAccess
{
    public class LocalPackageFile
    {
        public static string Filter       { get; set; } = "*.exe";
        public static string AppNameSpace { get; set; }

        public static D8Package Find(string appNameSpace)
        {
            AppNameSpace = appNameSpace;
            var exePath  = "";

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                exePath = args[1];
                if (!File.Exists(exePath)) return Alerter.Warn(
                    $"Exe path from argument is invalid:{L.f}   {exePath}");
            }
            else
            {
                var exes = FindAll();
                if (exes.Count != 1) return Alerter.Warn((exes.Count == 0
                    ? "No" : "More than 1") + " .exe file found.");
                exePath = exes[0];
            }

            var exe = R1Package.FromFile(exePath);
            return exe;
        }


        private static List<string> FindAll()
        {
            var list = new List<string>();
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var exes = Directory.GetFiles(dir, Filter);

            foreach (var exe in exes)
                if (IsUploadableExe(exe)) list.Add(exe);

            return list;
        }


        private static bool IsUploadableExe(string exePath)
        {
            var fName = Path.GetFileName(exePath);

            if (fName.Contains(LocalPackageFile.AppNameSpace)) return false;
            if (fName.Contains("vshost")) return false;
            if (AppDomain.CurrentDomain.FriendlyName == fName) return false;

            return true;
        }
    }
}
