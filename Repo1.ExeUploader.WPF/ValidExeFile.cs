using System;
using System.Collections.Generic;
using System.IO;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;

namespace Repo1.ExeUploader.WPF
{
    class ValidExeFile
    {
        internal static List<string> FindAll()
        {
            var list = new List<string>();
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var exes = Directory.GetFiles(dir, "*.exe");

            foreach (var exe in exes)
                if (IsUploadableExe(exe)) list.Add(exe);

            return list;
        }


        private static bool IsUploadableExe(string exePath)
        {
            var fName = Path.GetFileName(exePath);

            if (fName.Contains(typeof(App).Namespace)) return false;
            if (fName.Contains("vshost")) return false;
            if (AppDomain.CurrentDomain.FriendlyName == fName) return false;

            return true;
        }


        internal static R1Executable ToR1Exe(string exePath)
        {
            var r1e = new R1Executable();
            var inf = new FileInfo(exePath);

            r1e.FileName      = inf.Name;
            r1e.FileSize      = inf.Length;
            r1e.FileHash      = inf.SHA1ForFile();
            r1e.FileVersion   = inf.FileVersion();
            r1e.FullPathOrURL = exePath;

            return r1e;
        }
    }
}
