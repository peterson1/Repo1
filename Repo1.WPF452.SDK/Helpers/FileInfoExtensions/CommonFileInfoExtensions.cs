using System.Diagnostics;
using System.IO;

namespace Repo1.WPF452.SDK.Helpers.FileInfoExtensions
{
    public static class CommonFileInfoExtensions
    {
        public static string SHA1ForBytes(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists) return null;
            var algo = new HashLib.Crypto.SHA1();
            var byts = File.ReadAllBytes(fileInfo.FullName);
            var hash = algo.ComputeBytes(byts);
            return hash.ToString().ToLower();
        }


        public static string FileVersion(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists) return null;
            var ver = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
            return ver.FileVersion;
        }
    }
}
