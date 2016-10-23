using System;
using System.IO;
using Repo1.Core.ns11.InputCommands;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.ReflectionTools;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.DiskAccess;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.InputCommands;
using System.Threading.Tasks;
using System.Windows;

namespace Repo1.D8Uploader.WPF
{
    class MainWindowVM
    {
        public MainWindowVM(UploaderCfg uploaderCfg, UploaderClient2 uploaderClient, DeleterClient2 deleterClient)
        {
            Config   = uploaderCfg;
            Uploader = uploaderClient;
            Deleter  = deleterClient;
            Title    = $"Repo 1 Uploader  :  “{Config.Username}”  :  {Config.ApiBaseURL}";
            LocalPkg = LocalPackageFile.Find(typeof(App).Namespace);
            if (LocalPkg == null) return;

            RefreshCmd = R1Command.Async(CompareWithRemote);
            UploadCmd  = R1Command.Async(PublishLocalExe,
                     x => HasChanges && !VersionChanges.IsBlank(), "Upload");
            UploadCmd.DisableWhenDone = true;

            RefreshCmd.ExecuteIfItCan();
        }

        public UploaderCfg      Config          { get; private set; }
        public UploaderClient2  Uploader        { get; private set; }
        public DeleterClient2   Deleter         { get; private set; }
        public string           Title           { get; private set; }
        public D8Package        LocalPkg        { get; private set; }
        public D8Package        RemotePkg       { get; private set; }
        public IR1Command       RefreshCmd      { get; private set; }
        public IR1Command       UploadCmd       { get; private set; }
        public bool             HasChanges      { get; private set; }
        public string           VersionChanges  { get; set; }
        public double           MaxPartSizeMB   { get; set; } = 0.5;


        private async Task CompareWithRemote()
        {
            RemotePkg = await Uploader.GetPackage(LocalPkg.FileName);
            if (RemotePkg == null) return;
            Deleter.Initialize(RemotePkg);

            if (RemotePkg.LatestVersion == LocalPkg.LatestVersion) return;
            if (RemotePkg.LatestHash    == LocalPkg.LatestHash   ) return;

            LocalPkg.nid = RemotePkg.nid;
            LocalPkg.uid = RemotePkg.uid;
            HasChanges = true;
        }


        private async Task PublishLocalExe()
        {
            Clipboard.SetText(VersionChanges);

            UploadCmd.CurrentLabel = "Uploading ...";
            var ok = await Uploader.UploadNew(LocalPkg, MaxPartSizeMB);
            if (!ok)
            {
                UploadCmd.CurrentLabel = "Uploading Error";
                return;
            }

            RemotePkg.FileName      = LocalPkg.FileName;
            RemotePkg.FileSize      = LocalPkg.FileSize;
            RemotePkg.LatestHash    = LocalPkg.LatestHash;
            RemotePkg.LatestVersion = LocalPkg.LatestVersion;

            ok = await Uploader.Edit(RemotePkg, VersionChanges);
            if (!ok)
            {
                UploadCmd.CurrentLabel = "Updating Error";
                return;
            }
            UploadCmd.CurrentLabel = "Upload Successful";
        }
    }
}
