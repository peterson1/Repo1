using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.InputCommands;
using Repo1.Core.ns11.R1Models;
using Repo1.ExeUploader.WPF.Clients;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.ExeUploader.WPF.DiskAccess;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.Extensions.R1ModelExtensions;
using Repo1.WPF45.SDK.InputCommands;

namespace Repo1.ExeUploader.WPF
{
    [ImplementPropertyChanged]
    class MainWindowVM
    {

        public MainWindowVM(UploaderCfg uploaderCfg, UploaderClient1 uploaderClient1)
        {
            Config  = uploaderCfg;
            Client  = uploaderClient1;
            Title   = $"Repo 1 Uploader  :  “{Config.Username}”  :  {Config.ApiBaseURL}";

            LocalExe  = FindLocalExe();
            if (LocalExe == null) return;

            RefreshCmd = new CommandR1WPF(async x => await GetRemoteExe());
            RefreshCmd.ExecuteIfItCan();

            UploadCmd = new CommandR1WPF(async x => await PublishLocalExe(),
                    x => HasChanges && !VersionChanges.IsBlank(), "Upload");
        }


        public string           Title           { get; private set; }
        public UploaderClient1  Client          { get; private set; }
        public R1Executable     LocalExe        { get; private set; }
        public R1Executable     RemoteExe       { get; private set; }
        public UploaderCfg      Config          { get; private set; }
        public ICommandR1       RefreshCmd      { get; private set; }
        public ICommandR1       UploadCmd       { get; private set; }
        public bool             HasChanges      { get; private set; }
        public string           VersionChanges  { get; set; }
        public double           MaxPartSizeMB   { get; set; } = 0.5;


        private async Task PublishLocalExe()
        {
            Clipboard.SetText(VersionChanges);

            UploadCmd.CurrentLabel = "Uploading ...";
            var ok = await Client.UploadNew(LocalExe, MaxPartSizeMB);
            if (!ok)
            {
                UploadCmd.CurrentLabel = "Uploading Error";
                return;
            }

            RemoteExe.FileName       = LocalExe.FileName;
            RemoteExe.FileSize       = LocalExe.FileSize;
            RemoteExe.FileHash       = LocalExe.FileHash;
            RemoteExe.FileVersion    = LocalExe.FileVersion;

            ok = await Client.Edit(RemoteExe, VersionChanges);
            if (!ok)
            {
                UploadCmd.CurrentLabel = "Updating Error";
                return;
            }
            UploadCmd.CurrentLabel = "Done.";
        }


        private async Task GetRemoteExe()
        {
            RemoteExe = await Client.GetExecutable(LocalExe.FileName);
            if (RemoteExe == null) return;
            if (RemoteExe.FileVersion == LocalExe.FileVersion) return;
            if (RemoteExe.FileHash == LocalExe.FileHash) return;

            LocalExe.nid = RemoteExe.nid;
            LocalExe.uid = RemoteExe.uid;
            HasChanges   = true;
        }


        private R1Executable FindLocalExe()
        {
            var exePath = "";

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                exePath = args[1];
                if (!File.Exists(exePath)) return Alerter.Warn(
                    $"Exe path from argument is invalid:{L.f}   {exePath}");
            }
            else
            {
                var exes = ValidExeFile.FindAll();
                if (exes.Count != 1) return Alerter.Warn((exes.Count == 0 
                    ? "No" : "More than 1") + " .exe file found.");
                exePath = exes[0];
            }

            var exe = R1Exe.FromFile(exePath);
            return exe;
        }
    }
}
