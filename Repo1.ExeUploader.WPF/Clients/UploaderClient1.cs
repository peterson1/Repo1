using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.ExeUploader.WPF.DiskAccess;
using Repo1.WPF452.SDK.Archivers;
using Repo1.WPF452.SDK.Clients;

namespace Repo1.ExeUploader.WPF.Clients
{
    class UploaderClient1 : SvcStackRestClient
    {
        private UploaderCfg _upCfg;

        public UploaderClient1(UploaderCfg uploaderCfg) : base(uploaderCfg)
        {
            _upCfg = uploaderCfg;
        }


        internal async Task<R1Executable> GetExecutable()
        {
            var list = await ViewsList<UploadablesForUserDTO>(_upCfg.ExecutableNid);
            if (list == null) return null;
            var exe = list.Single(x => x.nid == _upCfg.ExecutableNid);
            _uid = exe.uid;
            return exe;
        }


        internal async Task<bool> UploadNew(
            R1Executable localExe, double? maxVolumeSizeMB = 0.5)
        {
            IsBusy = true;
            Status = "Compressing ...";
            var tmpCopy   = CopyToTemp(localExe.FullPathOrURL);
            var partsList = await SevenZipper1.Compress(tmpCopy, null, maxVolumeSizeMB, ".data");

            for (int i = 0; i < partsList.Count; i++)
            {
                Status = $"Uploading part {i + 1} of {partsList.Count} ...";

                var partPath = partsList[i];
                var r1Part = FilePart.ToR1Part(partPath, localExe);
                var node = await Add(r1Part);
                if (node == null) return false;
            }

            //  update exe node -- dito palang

            IsBusy = false;
            return true;
        }


        private string CopyToTemp(string filePath, string newExtension = ".bin")
        {
            var uniq = "Uploading_" + DateTime.Now.Ticks;
            var tmpD = Path.Combine(Path.GetTempPath(), uniq);
            Directory.CreateDirectory(tmpD);

            var fNme = Path.GetFileNameWithoutExtension(filePath) + newExtension;
            var path = Path.Combine(tmpD, fNme);
            File.Copy(filePath, path, true);
            return path;
        }
    }
}
