using System.IO;
using Repo1.Core.ns11.R1Models;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;

namespace Repo1.WPF452.SDK.Helpers.R1ExecutableExtensions
{
    public class R1Exe
    {
        public static R1Executable FromFile(string exePath)
        {
            var r1e = new R1Executable();
            var inf = new FileInfo(exePath);
            if (!inf.Exists) return null;

            r1e.FileName      = inf.Name;
            r1e.FileSize      = inf.Length;
            r1e.FileHash      = inf.SHA1ForFile();
            r1e.FileVersion   = inf.FileVersion();
            r1e.FullPathOrURL = exePath;

            return r1e;
        }
    }
}
