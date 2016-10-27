using System.IO;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;

namespace Repo1.D8Uploader.Lib45.FileIO
{
    public class LocalFile
    {
        public static R1Package AsR1Package(string filePath)
        {
            var pkg = new R1Package();
            var inf = new FileInfo(filePath);
            if (!inf.Exists) return null;

            pkg.FileName      = inf.Name;
            pkg.FileSize      = inf.Length;
            pkg.LatestVersion = inf.FileVersion();
            pkg.LatestHash    = inf.SHA1ForFile();
            pkg.FullPathOrURL = filePath;

            return pkg;
        }


        public static R1PackagePart AsR1PackagePart(string filePath)
        {
            var part = new Core.ns11.R1Models.D8Models.R1PackagePart();
            var inf  = new FileInfo(filePath);
            if (!inf.Exists) return null;

            part.PartHash      = inf.SHA1ForFile();
            part.FullPathOrURL = filePath;

            return part;
        }
    }
}
