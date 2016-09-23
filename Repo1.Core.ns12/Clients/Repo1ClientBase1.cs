using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Repo1.Core.ns12.Configuration;
using Repo1.Core.ns12.Helpers;
using Repo1.Core.ns12.Helpers.PropertyChangedExtensions;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public abstract class Repo1ClientBase1 : IRepo1Client
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler                UpdateInstalled = delegate { };


        public Repo1ClientBase1(string userName, 
                                string password, 
                                string activationKey, 
                                int checkIntervalMins, 
                                string apiBaseURL)
        {
            _cfg = new DownloaderCfg
            {
                Username      = userName,
                Password      = password,
                ApiBaseURL    = apiBaseURL,
                ActivationKey = activationKey
            };

            _intervalMins = checkIntervalMins;
            _validr       = GetClientValidator();
            _pingr        = GetPingClient();
            _downloadr    = GetDownloadClient();
            _sessionr     = GetSessionClient(checkIntervalMins);
        }


        private int                _intervalMins;
        private bool               _keepChecking;
        private bool               _isChecking;
        protected IPingClient      _pingr;
        protected IClientValidator _validr;
        protected ISessionClient   _sessionr;
        protected IDownloadClient  _downloadr;
        protected DownloaderCfg    _cfg;


        private string _status;
        public string   Status
        {
            get { return _status; }
            set
            {
                _status = value;
                PropertyChanged.Raise(nameof(Status));
                _statusChanged.Raise(_status);
            }
        }


        private      EventHandler<EArg<string>> _statusChanged;
        public event EventHandler<EArg<string>>  StatusChanged
        {
            add    { _statusChanged -= value; _statusChanged += value; }
            remove { _statusChanged -= value; }
        }


        public virtual Action<string>  OnWarning  { protected get; set; }


        public Func<string> ReadLegacyCfg
        {
            set { _sessionr.ReadLegacyCfg = value; }
        }


        protected abstract IClientValidator  GetClientValidator    ();
        protected abstract IPingClient       GetPingClient         ();
        protected abstract IDownloadClient   GetDownloadClient     ();
        protected abstract ISessionClient    GetSessionClient      (int checkIntervalMins);
        protected abstract R1Executable      GetCurrentR1Exe       ();
        protected abstract bool              ReplaceCurrentExeWith (string replacementExePath);
        protected abstract void              RunOnNewThread        (Task task, string threadLabel);


        protected T Warn <T>(string message, T returnVal = default(T))
        {
            OnWarning?.Invoke(message);
            return returnVal;
        }


        public async void StartUpdateCheckLoop()
        {
            if (_isChecking) return;
            Status = $"Validating application license for “{_cfg.Username}” ...";
            if (!(await _validr.ValidateThisMachine()))
            {
                Status = _cfg.WasRejected
                    ? $"Server rejected the credentials for “{_cfg.Username}”."
                    : $"{_cfg.Username} is not licensed to use this app on this machine.";
                return;
            }

            _isChecking   = true;
            _keepChecking = true;
            RunOnNewThread(ExecuteUpdateCheckLoop(), "Update Checker Loop Thread");
        }


        public void StopUpdateCheckLoop()
        {
            _keepChecking = false;
        }


        private async Task ExecuteUpdateCheckLoop()
        {
            var latest  = default(R1Executable);
            var ping    = _validr.PingNode;
            var current = GetCurrentR1Exe();
            Status = $"Currently running version [{current.FileVersion}].";

            while (_keepChecking)
            {
                Status = "Checking for newer version ...";
                latest = await _pingr.SendAndGetLatestVersion(ping);

                if (!_keepChecking) return;
                if (current.FileHash == latest.FileHash)
                {
                    Status = $"Nothing new.  Will check again in {_intervalMins} minutes ...";
                    await Task.Delay(1000 * _intervalMins * 60);
                }
                else
                {
                    Status = $"Newer version found: [{latest.FileVersion}]. Downloading ...";
                    if (await DownloadAndSwap(latest, ping))
                    {
                        UpdateInstalled?.Raise(this);
                        Status = "Updates downloaded and installed.  Ready to relaunch.";
                        return;
                    }
                }
            }
        }

        private async Task<bool> DownloadAndSwap(R1Executable latest, R1Ping ping)
        {
            var partsList = await _downloadr.GetPartsList
                (latest.FileVersion, ping.RegisteredMacAddress);
            if (partsList.Count == 0) return false;

            var exePath = await _downloadr.DownloadAndExtract(partsList, latest.FileHash);
            if (exePath.IsBlank()) return false;

            return ReplaceCurrentExeWith(exePath);
        }




        public virtual void RaisePropertyChanged(string propertyName)
            => PropertyChanged.Raise(propertyName);


        public void StartTrackingUserSession()
            => RunOnNewThread(_sessionr.StartTrackingLoop(), "Session Loop Thread");
    }
}
