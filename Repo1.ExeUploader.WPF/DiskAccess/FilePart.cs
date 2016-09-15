using System.IO;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;

namespace Repo1.ExeUploader.WPF.DiskAccess
{
    class FilePart
    {
        internal static R1SplitPart ToR1Part(string partPath, 
            R1Executable r1Exe, int partNumber, int totalParts)
        {
            var part = new R1SplitPart();
            var inf  = new FileInfo(partPath);

            part.FileName      = inf.Name;
            part.ExeNid        = r1Exe.nid;
            part.ExeVersion    = r1Exe.FileVersion;
            part.PartHash      = inf.SHA1ForFile();
            part.FullPathOrURL = partPath;
            part.Base64Content = inf.Base64Content();
            part.PartNumber    = partNumber;
            part.TotalParts    = totalParts;

            return part;
        }
    }
}
