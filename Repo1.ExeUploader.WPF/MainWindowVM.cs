using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.Helpers.InputCommands;
using Repo1.Core.ns12.Models;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.ExeUploader.WPF.Clients;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.ExeUploader.WPF.DiskAccess;
using Repo1.WPF452.SDK.Helpers.InputCommands;
using Repo1.WPF452.SDK.Helpers;
using Repo1.WPF452.SDK.Helpers.R1ExecutableExtensions;

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
            RemoteExe = await Client.GetExecutable();
            if (RemoteExe == null) return;
            if (RemoteExe.FileHash == LocalExe.FileHash) return;

            LocalExe.nid = RemoteExe.nid;
            HasChanges   = true;
        }


        private R1Executable FindLocalExe()
        {
            var exes = ValidExeFile.FindAll();

            if (exes.Count != 1) return Alerter.Warn((exes.Count == 0 
                ? "No" : "More than 1") + " .exe file found.");

            //var exe = ValidExeFile.ToR1Exe(exes[0]);
            var exe = R1Exe.FromFile(exes[0]);
            return exe;
        }
    }
}
