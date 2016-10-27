using System;
using System.IO;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.D8Uploader.Lib45.FileIO;
using Repo1.WPF45.SDK.Extensions.R1ModelExtensions;

namespace Repo1.D8Uploader.Lib45.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class MainWindowVmBase2
    {
        private UploaderClient2 _uploader;

        public MainWindowVmBase2(UploaderClient2 uploaderClient)
        {
            _uploader = uploaderClient;
        }


        public R1Package  RemotePkg         { get; private set; }
        public R1Package  LocalPkg          { get; private set; }
        public bool       FileIsUploadable  { get; private set; }
        public bool       UploadSuccessful  { get; private set; }

        public string     VersionChanges    { get; set; }
        public double     MaxPartSizeMB     { get; set; } = 0.5;


        public async Task CheckUploadability(string filePath)
        {
            FileIsUploadable = false;

            if (!File.Exists(filePath)) return;
            LocalPkg = LocalFile.AsR1Package(filePath);
            RemotePkg = await _uploader.GetPackage(LocalPkg.FileName);

            if (RemotePkg               == null                  ) return;
            if (RemotePkg.LatestVersion == LocalPkg.LatestVersion) return;
            if (RemotePkg.LatestHash    == LocalPkg.LatestHash   ) return;

            FileIsUploadable = true;
        }


        public async Task UploadFile()
        {
            UploadSuccessful = false;
            UploadSuccessful = await _uploader.UploadInParts(LocalPkg, MaxPartSizeMB);
        }
    }
}
