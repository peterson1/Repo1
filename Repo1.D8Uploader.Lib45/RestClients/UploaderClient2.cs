using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.Core.ns11.R1Models.D8Models.D8ViewsLists;
using Repo1.WPF45.SDK.Archivers;
using Repo1.WPF45.SDK.Clients;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.Extensions.R1ModelExtensions;
using Repo1.D8Uploader.Lib45.FileIO;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;

namespace Repo1.D8Uploader.Lib45.RestClients
{
    public class UploaderClient2 : D8SvcStackClientBase
    {
        private List<string>        _partPaths;
        private List<R1PackagePart> _pkgParts;
        private R1Package           _package;
        private PackagePartUploader _partUploadr;

        public UploaderClient2(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            OnError = ex => ThreadedAlerter.Show(ex, "Uploader Client 2");
            _partUploadr = new PackagePartUploader(restServerCredentials);
        }



        public async Task<R1Package> GetPackage(string packageFilename)
        {
            Status = "Querying uploadables for this user ...";
            var list = await ViewsList<UploadablesForUserView>();
            if (list == null) return null;
            var exe = list.SingleOrDefault(x => x.FileName == packageFilename);
            return exe;
        }


        public async Task<bool> UploadInParts(R1Package localPkg, double maxVolumeSizeMB)
        {
            var tmpCopy = CopyToTemp(localPkg.FullPathOrURL);
            _package    = localPkg;
            _partPaths  = await SevenZipper1.Compress(tmpCopy, null, maxVolumeSizeMB, ".data");
            _pkgParts = new List<R1PackagePart>();

            for (int i = 0; i < _partPaths.Count; i++)
            {
                var ok = await UploadPartByIndex(i);
                if (!ok) return false;
            }
            return true;
        }


        private async Task<bool> UploadPartByIndex(int partIndex)
        {
            var path            = _partPaths[partIndex];
            var part            = LocalFile.AsR1PackagePart(path);
            part.PartNumber     = partIndex + 1;
            part.TotalParts     = _partPaths.Count;
            part.Package        = _package;
            part.PackageVersion = _package.LatestVersion;
            part.FullPathOrURL  = path;

            return await _partUploadr.UploadAndAttachToNewNode(part);
        }




        public Task<bool> Edit(R1Package remotePkg, string versionChanges)
        {
            throw new NotImplementedException();
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
