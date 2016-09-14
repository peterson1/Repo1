using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.Helpers.InputCommands;
using Repo1.Core.ns12.Models;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.ExeUploader.WPF.Clients;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.ExeUploader.WPF.DiskAccess;
using Repo1.WPF452.SDK.Helpers.InputCommands;

namespace Repo1.ExeUploader.WPF
{
    [ImplementPropertyChanged]
    class MainWindowVM
    {

        public MainWindowVM(UploaderCfg uploaderCfg, UploaderClient1 uploaderClient1)
        {
            Config    = uploaderCfg;
            Client    = uploaderClient1;
            LocalExe  = FindLocalExe();
            if (LocalExe == null) return;

            //Client.

            RefreshCmd = new CommandR1WPF(async x => await GetExeNode());
            RefreshCmd.ExecuteIfItCan();

            UploadCmd = new CommandR1WPF(async x =>
            {
                UploadCmd.CurrentLabel = "Uploading ...";
                LocalExe.VersionChanges = VersionChanges;
                await Client.UploadNew(LocalExe);
            },
            x => !VersionChanges.IsBlank(), "Upload");
        }


        public UploaderClient1  Client          { get; private set; }
        public R1Executable     LocalExe        { get; private set; }
        public R1Executable     RemoteExe       { get; private set; }
        public UploaderCfg      Config          { get; private set; }
        public string           Status          { get; private set; }
        public ICommandR1       RefreshCmd      { get; private set; }
        public ICommandR1       UploadCmd       { get; private set; }
        public bool             HasChanges      { get; private set; }
        public string           VersionChanges  { get; set; }


        private async Task GetExeNode()
        {
            Status    = $"Getting Exe info from {Config.ApiBaseURL} ...";
            RemoteExe = await Client.GetExecutable();
            if (RemoteExe == null) return;
            if (RemoteExe.FileHash == LocalExe.FileHash) return;

            LocalExe.nid = RemoteExe.nid;
            Status       = "Please describe version changes.";
            HasChanges   = true;
        }


        private R1Executable FindLocalExe()
        {
            var exes = ValidExeFile.FindAll();

            if (exes.Count != 1)
            {
                Status = (exes.Count == 0 ? "No" : "More than 1") + " .exe file found.";
                return null;
            }
            
            var exe = ValidExeFile.ToR1Exe(exes[0]);
            Status = $"Found: {exe.FileName}";
            return exe;
        }
    }
}
