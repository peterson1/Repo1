using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Archivers;
using Repo1.WPF452.SDK.Helpers;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;

namespace Repo1.WPF452.SDK.Clients
{
    [ImplementPropertyChanged]
    public class DownloaderClient1 : SvcStackRestClient
    {
        public DownloaderClient1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        public async Task<string> AssembleParts(List<R1SplitPart> splitParts)
        {
            var orderedParts = splitParts.OrderBy(x => x.PartNumber).ToList();
            var tempDir      = CreateTempFolder();
            for (int i = 0; i < orderedParts.Count; i++)
            {
                Status = $"Downloading part {i + 1} of {orderedParts.Count}";
                var part = orderedParts[i];
                var path = Path.Combine(tempDir, part.FileName);

                var byts = await GetPartContentByHash(part.PartHash);
                if (byts == null) return null;

                File.WriteAllBytes(path, byts);
                if (path.SHA1ForFile() != part.PartHash)
                    return Alerter.Warn("Expected PartHash did not match actual hash.");

                part.FullPathOrURL = path;
            }

            Status = "Merging and decompressing downloaded file ...";
            var paths = orderedParts.Select(x => x.FullPathOrURL);
            var list  = await SevenZipper1.DecompressMultiPart(paths, tempDir);
            if (list == null) return null;

            return list[0];
        }


        private async Task<byte[]> GetPartContentByHash(string partHash)
        {
            var list = await ViewsList<SplitPartContentByHashDTO>(partHash);
            if (list == null) return null;
            if (list.Count == 0)
                return Alerter.Warn($"No content found for ‹{partHash}›");

            return Convert.FromBase64String(list[0].Base64Content);
        }


        private string CreateTempFolder()
        {
            var uniq = "R1_Downloading_" + DateTime.Now.Ticks;
            var tmpD = Path.Combine(Path.GetTempPath(), uniq);
            Directory.CreateDirectory(tmpD);
            return tmpD;
        }
    }
}
