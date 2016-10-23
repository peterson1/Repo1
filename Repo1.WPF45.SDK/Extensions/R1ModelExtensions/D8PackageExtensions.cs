using System.IO;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;

namespace Repo1.WPF45.SDK.Extensions.R1ModelExtensions
{
    class D8PackageExtensions
    {
    }


    public class R1Package
    {
        public static D8Package FromFile(string filePath)
        {
            var pkg = new D8Package();
            var inf = new FileInfo(filePath);
            if (!inf.Exists) return null;

            pkg.FileName      = inf.Name;
            pkg.FileSize      = inf.Length;
            pkg.LatestVersion = inf.FileVersion();
            pkg.LatestHash    = inf.SHA1ForFile();
            pkg.FullPathOrURL = filePath;

            return pkg;
        }
    }
}
