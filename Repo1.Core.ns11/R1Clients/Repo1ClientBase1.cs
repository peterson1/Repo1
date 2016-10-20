using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.EventArguments;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public abstract class Repo1ClientBase1 : IRepo1Client
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler UpdateInstalled = delegate { };

        private   int                _intervalMins;
        private   bool               _isChecking;
        protected ILocalFileUpdater  _updatr;
        protected IClientValidator   _validr;
        protected ISessionClient     _sessionr;
        protected IDownloadClient    _downloadr;
        private   IIssuePosterClient _postr;
        protected DownloaderCfg      _cfg;
        protected string             _cfgKey;


        public Repo1ClientBase1(string configKey, int checkIntervalMins)
        {
            _cfgKey       = configKey;
            _cfg          = ParseDownloaderCfg(configKey);
            _intervalMins = checkIntervalMins;
            _validr       = GetClientValidator();
            _postr        = GetPosterClient();
            _updatr       = GetLocalFileUpdater(configKey);
            _sessionr     = GetSessionClient(checkIntervalMins);
            _downloadr    = GetDownloadClient();
            _sessionr.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_sessionr.Status))
                    Status = _sessionr.Status;
            };
        }


        private string _status;
        public  string  Status
        {
            get { return _status; }
            set
            {
                _status = value;
                PropertyChanged.Raise(nameof(Status));
                _statusChanged .Raise(_status);
            }
        }


        private      EventHandler<EArg<string>> _statusChanged;
        public event EventHandler<EArg<string>>  StatusChanged
        {
            add    { _statusChanged -= value; _statusChanged += value; }
            remove { _statusChanged -= value; }
        }


        public virtual Action<string> OnWarning { protected get; set; }


        public Func<string> ReadLegacyCfg
        {
            set
            {
                _validr  .ReadLegacyCfg = value;
                _postr   .ReadLegacyCfg = value;
                _updatr  .ReadLegacyCfg = value;
                _sessionr.ReadLegacyCfg = value;
            }
        }


        protected abstract IClientValidator   GetClientValidator    ();
        protected abstract ILocalFileUpdater  GetLocalFileUpdater   (string configKey);
        protected abstract IDownloadClient    GetDownloadClient     ();
        protected abstract ISessionClient     GetSessionClient      (int checkIntervalMins);
        protected abstract IIssuePosterClient GetPosterClient       ();
        protected abstract R1Executable       GetCurrentLocalExe    ();
        protected abstract bool               ReplaceCurrentExeWith (string replacementExePath);
        protected abstract void               RunOnNewThread        (Task task, string threadLabel);
        protected abstract DownloaderCfg      ParseDownloaderCfg    (string configKey);


        public async void StartUpdateChecker(string tempUserName, string tempPassword)
        {
            if (_isChecking) return;
            _isChecking = true;

            Status = $"Validating application license for “{_cfg?.Username}” ...";
            if (await _validr.ValidateThisMachine())
            {
                Status = "Machine validation successful.";
                _updatr.PingNode = _validr.PingNode;
                StartAuthenticatedUpdaterLoop();
            }
            else
            {
                Status = _cfg?.WasRejected ?? true
                    ? $"Server rejected the credentials for “{_cfg?.Username}”."
                    : $"{_cfg?.Username} is not licensed to use this app on this machine.";

                StartTemporarySessionUpdaterLoop(tempUserName, tempPassword);
            }
        }


        private void StartAuthenticatedUpdaterLoop()
            => RunOnNewThread(ExecuteUpdateCheckLoop(), "Update Checker Loop Thread");


        private void StartTemporarySessionUpdaterLoop(string tempUserName, string tempPassword)
            => RunOnNewThread(_sessionr.StartSessionUpdateLoop
                (tempUserName, tempPassword), "Session Loop Thread");



        private async Task ExecuteUpdateCheckLoop()
        {
            var current = GetCurrentLocalExe();
            var latest = default(R1Executable);
            while (true)
            {
                Status = $"Checking for version newer than [{current.FileVersion}] ...";
                latest = await _updatr.GetLatestVersions();

                if (current.FileHash == latest.FileHash)
                {
                    Status = $"Nothing new.  Will check again in {_intervalMins} minutes ...";
                    await Task.Delay(1000 * _intervalMins * 60);
                }
                else
                {
                    Status = $"Newer version found: [{latest.FileVersion}]. Downloading ...";
                    if (await DownloadAndSwap(latest))
                    {
                        UpdateInstalled?.Raise(this);
                        Status = "Updates downloaded and installed.  Ready to relaunch.";
                        return;
                    }
                }
            }
        }

        private async Task<bool> DownloadAndSwap(R1Executable latest)
        {
            var macAddr   = _validr.PingNode.RegisteredMacAddress;
            var partsList = await _downloadr.GetPartsList(latest.FileVersion, macAddr);
            if (partsList.Count == 0) return false;

            var exePath = await _downloadr.DownloadAndExtract(partsList, latest.FileHash);
            if (exePath.IsBlank()) return false;

            var ok = ReplaceCurrentExeWith(exePath);
            if (ok) _downloadr.DeleteLastTempDir();
            return ok;
        }






        public virtual void RaisePropertyChanged(string propertyName)
            => PropertyChanged.Raise(propertyName);


        public void PostRuntimeError(string errorMessage)
            => RunOnNewThread(_postr.PostError(errorMessage, _cfgKey), "PostRuntimeError thread");
    }
}
