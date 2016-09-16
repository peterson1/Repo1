using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.ExeUploader.WPF.DiskAccess;
using Repo1.WPF452.SDK.Archivers;
using Repo1.WPF452.SDK.Clients;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;
using Repo1.WPF452.SDK.Helpers;
using PropertyChanged;

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
        }


        internal async Task<R1Executable> GetExecutable()
        {
            Status   = "Querying uploadables for this user ...";
            var list = await ViewsList<UploadablesForUserDTO>(_upCfg.ExecutableNid);
            if (list == null) return null;
            var exe = list.Single(x => x.nid == _upCfg.ExecutableNid);
            _uid = exe.uid;
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

                var node = await Create(r1Part);
                if (node == null) return false;
                splitParts.Add(r1Part);
            }

            var ok = await ValidateDownload(splitParts, localExe.FileHash);
            if (!ok) return Alerter.Warn("Uploaded parts are invalid.", false);

            IsBusy = false;
            return true;
        }


        internal async Task<bool> Edit(R1Executable remoteExe, string versionChanges)
        {
            var dict = await Update(remoteExe, versionChanges);
            return dict != null;
        }


        private async Task<bool> ValidateDownload(List<R1SplitPart> splitParts, string expectedHash)
        {
            var downloaded = await _downloadr.AssembleParts(splitParts);
            if (downloaded.IsBlank())
                return Alerter.Warn("Failed to download/assemble the file.", false);

            if (downloaded.SHA1ForFile() != expectedHash)
                return Alerter.Warn("Hash of downloaded file is different.", false);

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
