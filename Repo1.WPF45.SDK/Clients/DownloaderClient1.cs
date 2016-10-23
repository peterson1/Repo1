using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;
using Repo1.Core.ns11.R1Models.ViewsLists;
using Repo1.WPF45.SDK.Archivers;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;

namespace Repo1.WPF45.SDK.Clients
{
    [ImplementPropertyChanged]
    public class DownloaderClient1 : D7SvcStackClientBase, IDownloadClient
    {
        private DownloaderCfg  _dCfg;
        private string         _lastTempDir;

        public DownloaderClient1(DownloaderCfg downloaderCfg) : base(downloaderCfg)
        {
            _dCfg = downloaderCfg;
            OnError = ex => Warn(ex.Info(true, true));
        }


        public async Task<string> DownloadAndExtract(List<R1SplitPart> splitParts, string expectedHash)
        {
            var orderedParts = splitParts.OrderBy(x => x.PartNumber).ToList();
            _lastTempDir     = CreateTempFolder();
            for (int i = 0; i < orderedParts.Count; i++)
            {
                Status = $"Downloading part {i + 1} of {orderedParts.Count}";
                var part = orderedParts[i];
                var path = Path.Combine(_lastTempDir, part.FileName);

                var byts = await GetPartContentByHash(part.PartHash);
                if (byts == null) return null;

                File.WriteAllBytes(path, byts);
                if (path.SHA1ForFile() != part.PartHash)
                {
                    Warn("Expected PartHash did not match actual hash.");
                    return null;
                }

                part.FullPathOrURL = path;
            }

            Status = "Merging and decompressing downloaded file ...";
            var paths = orderedParts.Select(x => x.FullPathOrURL);
            List<string> list = null;
            try
            {
                list = await SevenZipper1.DecompressMultiPart(paths, _lastTempDir);
            }
            catch (Exception ex)
            {
                Warn("[Decompress Error] Downloaded file may be corrupted." + L.f + ex.Info());
                return null;
            }
            if (list == null)
            {
                Warn("[Decompress Fail] Downloaded file may be corrupted.");
                return null;
            }

            var exePath = Path.Combine(_lastTempDir, list[0]);
            if (exePath.SHA1ForFile() == expectedHash)
                return exePath;
            else
            {
                Warn("[Hash Mismatch] Downloaded file may be corrupted.");
                return null;
            }
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


        public async Task<List<R1SplitPart>> GetPartsList(string exeVersion, string macAddress)
        {
            var key = _dCfg.GetLicenseKey(macAddress);
            var list = await ViewsList<DownloadablesForUserDTO>(key, exeVersion);
            var trimmed = TrimPartsList(list);

            if (!ValidatePartsList(trimmed)) return null;

            return trimmed.Select(x => x as R1SplitPart).ToList();
        }


        private List<DownloadablesForUserDTO> TrimPartsList(List<DownloadablesForUserDTO> list)
        {
            if (list.Count == 0) return list;
            return list.Take(list.First().TotalParts).ToList();
        }


        private bool ValidatePartsList(List<DownloadablesForUserDTO> list)
        {
            if (list.Count == 0)
                throw new InvalidDataException("Parts list should not be empty.");

            var byTotalParts = list.GroupBy(x => x.TotalParts)
                                   .Select(x => x.First());

            if (byTotalParts.Count() != 1)
                throw new InvalidDataException("All parts should have same values for ‹TotalParts›.");

            var expctd = byTotalParts.First().TotalParts;
            var actual = list.Count;

            if (actual != expctd)
                throw new InvalidDataException($"Expected list to have {expctd} parts but had {actual}.");

            return true;
        }


        //protected override void OnError(Exception ex)
        //    => Warn(ex.Info(true, true));


        public void DeleteLastTempDir()
        {
            foreach (var file in Directory.GetFiles(_lastTempDir))
                File.Delete(file);

            Directory.Delete(_lastTempDir, true);
        }
    }
}
