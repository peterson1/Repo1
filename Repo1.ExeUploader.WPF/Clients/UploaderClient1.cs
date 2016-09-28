using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.ExeUploader.WPF.DiskAccess;
using Repo1.WPF452.SDK.Archivers;
using Repo1.WPF452.SDK.Clients;
using Repo1.WPF452.SDK.Helpers;
using Repo1.WPF452.SDK.Helpers.EmbeddedResourceHelpers;

namespace Repo1.ExeUploader.WPF.Clients
{
    [ImplementPropertyChanged]
    class UploaderClient1 : SvcStackRestClient
    {
        private UploaderCfg       _upCfg;
        private DownloaderClient1 _downloadr;

        public UploaderClient1(DownloaderClient1 downloaderClient, UploaderCfg uploaderCfg) : base(uploaderCfg)
        {
            _upCfg     = uploaderCfg;
            _downloadr = downloaderClient;

            EmbeddedResrc.ExtractToFile<UploaderClient1>
                ("7za.dll", "Archivers", SevenZipper1.GetLocalBinariesDir());
        }


        internal async Task<R1Executable> GetExecutable(string exeFileName)
        {
            Status   = "Querying uploadables for this user ...";
            var list = await ViewsList<UploadablesForUserDTO>();
            if (list == null) return null;
            var exe = list.SingleOrDefault(x => x.FileName == exeFileName);
            return exe;
        }


        internal async Task<bool> UploadNew
            (R1Executable localExe, double? maxVolumeSizeMB)
        {
            IsBusy         = true;
            Status         = "Compressing ...";
            var tmpCopy    = CopyToTemp(localExe.FullPathOrURL);
            var splitParts = new List<R1SplitPart>();
            var partPaths  = await SevenZipper1.Compress(tmpCopy, null, maxVolumeSizeMB, ".data");

            for (int i = 0; i < partPaths.Count; i++)
            {
                Status = $"Uploading part {i + 1} of {partPaths.Count} ...";

                var r1Part = FilePart.ToR1Part(partPaths[i], 
                                localExe, i + 1, partPaths.Count);

                var node = await Create(r1Part, 
                      () => GetSplitPartIDsByHash(r1Part.PartHash));

                if (node == null) return false;
                splitParts.Add(r1Part);
            }

            var ok = await ValidateDownload(splitParts, localExe.FileHash);
            if (!ok)
            {
                //todo: delete corrupted uploaded parts
                return Alerter.Warn("Uploaded parts are invalid.", false);
            }

            IsBusy = false;
            return true;
        }


        private async Task<R1SplitPart> GetSplitPartIDsByHash(string partHash)
        {
            var list = await ViewsList<SplitPartIDsByHashDTO>(partHash);
            if (list == null) return null;
            if (list.Count > 1)
                Warn($"Split part w/ hash “{partHash}” was uploaded {list.Count} times.");

            return list.FirstOrDefault();
        }


        internal async Task<bool> Edit(R1Executable remoteExe, string versionChanges)
        {
            var dict = await Update(remoteExe, versionChanges);
            return dict != null;
        }


        private async Task<bool> ValidateDownload(List<R1SplitPart> splitParts, string expectedHash)
        {
            var downloaded = await _downloadr.DownloadAndExtract(splitParts, expectedHash);
            if (downloaded.IsBlank())
                return Alerter.Warn("Failed to download/assemble/validate the file.", false);

            //if (downloaded.SHA1ForFile() != expectedHash)
            //    return Alerter.Warn("Hash of downloaded file is different.", false);

            File.Delete(downloaded);

            Status = "Uploaded files validated successfully.";
            return true;
        }

        private string CopyToTemp(string filePath)
        {
            var uniq = "R1_Uploading_" + DateTime.Now.Ticks;
            var tmpD = Path.Combine(Path.GetTempPath(), uniq);
            Directory.CreateDirectory(tmpD);

            var fNme = Path.GetFileName(filePath);
            var path = Path.Combine(tmpD, fNme);
            File.Copy(filePath, path, true);
            return path;
        }
    }
}
