using System.IO;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.Helpers.InputCommands;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Clients;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.WPF452.SDK.Helpers.InputCommands;

namespace Repo1.ExeUploader.WPF
{
    [ImplementPropertyChanged]
    class MainWindowVM
    {
        private UploaderClient1 _client;
        private R1Executable    _localExe;
        private R1Executable    _remoteExe;

        public MainWindowVM(UploaderCfg uploaderCfg, UploaderClient1 uploaderClient1)
        {
            Config     = uploaderCfg;
            _client    = uploaderClient1;
            _localExe  = FindLocalExe();
            if (_localExe == null) return;

            RefreshCmd = new CommandR1WPF(async x => await GetExeNode());
            RefreshCmd.ExecuteIfItCan();
        }


        public UploaderCfg  Config      { get; private set; }
        public string       Status      { get; private set; }
        public ICommandR1   RefreshCmd  { get; set; }


        private async Task GetExeNode()
        {
            Status = $"Getting Exe info from {Config.ApiBaseURL} ...";
            //var r1Exe = await QueryD7ExeNode(_cfg, _targetExe);
            //await Task.Delay(1000 * 1);
            _remoteExe = await _client.GetExecutable();
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
