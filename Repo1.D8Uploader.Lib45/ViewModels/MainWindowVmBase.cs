using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.InputCommands;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.DiskAccess;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.WPF45.SDK.InputCommands;

namespace Repo1.D8Uploader.Lib45.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class MainWindowVmBase
    {
        public MainWindowVmBase(UploaderCfg uploaderCfg, UploaderClient2 uploaderClient, DeleterClientBase deleterClient)
        {
            Config   = uploaderCfg;
            Uploader = uploaderClient;
            Deleter  = deleterClient;
            Title    = $"Repo 1 Uploader  :  “{Config.Username}”  :  {Config.ApiBaseURL}";
            LocalPkg = LocalPackageFile.Find(AppNameSpace);
            if (LocalPkg == null) return;

            RefreshCmd = R1Command.Async(CompareWithRemote);
            UploadCmd  = R1Command.Async(PublishLocalExe,
                     x => HasChanges && !VersionChanges.IsBlank(), "Upload");
            UploadCmd.DisableWhenDone = true;

            RefreshCmd.ExecuteIfItCan();
        }

        public UploaderCfg      Config          { get; protected set; }
        public UploaderClient2  Uploader        { get; protected set; }
        public DeleterClientBase   Deleter         { get; protected set; }
        public string           Title           { get; protected set; }
        public D8Package        LocalPkg        { get; protected set; }
        public D8Package        RemotePkg       { get; protected set; }
        public IR1Command       RefreshCmd      { get; protected set; }
        public IR1Command       UploadCmd       { get; protected set; }
        public bool             HasChanges      { get; protected set; }
        public string           VersionChanges  { get; set; }
        public double           MaxPartSizeMB   { get; set; } = 0.5;


        protected abstract void    SetClipboardText (string text);
        protected abstract string  AppNameSpace     { get; }


        private async Task CompareWithRemote()
        {
            RemotePkg = await Uploader.GetPackage(LocalPkg.FileName);
            if (RemotePkg == null) return;
            Deleter.Initialize(RemotePkg);

            if (RemotePkg.LatestVersion == LocalPkg.LatestVersion) return;
            if (RemotePkg.LatestHash    == LocalPkg.LatestHash) return;

            LocalPkg.nid = RemotePkg.nid;
            LocalPkg.uid = RemotePkg.uid;
            HasChanges = true;
        }


        private async Task PublishLocalExe()
        {
            SetClipboardText(VersionChanges);

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
