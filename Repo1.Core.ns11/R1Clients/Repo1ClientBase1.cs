﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        private int _intervalMins;
        private bool _keepChecking;
        private bool _isChecking;
        protected IPingClient _pingr;
        protected IClientValidator _validr;
        protected ISessionClient _sessionr;
        protected IDownloadClient _downloadr;
        protected DownloaderCfg _cfg;
        protected string _cfgKey;


        public Repo1ClientBase1(string configKey, int checkIntervalMins)
        {
            _cfgKey = configKey;
            _cfg = ParseDownloaderCfg(configKey);
            _intervalMins = checkIntervalMins;
            _validr = GetClientValidator();
            _pingr = GetPingClient();
            _downloadr = GetDownloadClient();
            _sessionr = GetSessionClient(checkIntervalMins);
            _sessionr.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_sessionr.Status))
                    Status = _sessionr.Status;
            };
        }


        //public Repo1ClientBase1(string userName, 
        //                        string password, 
        //                        string activationKey, 
        //                        int checkIntervalMins, 
        //                        string apiBaseURL)
        //    : this(checkIntervalMins)
        //{
        //    _cfg = new DownloaderCfg
        //    {
        //        Username      = userName,
        //        Password      = password,
        //        ApiBaseURL    = apiBaseURL,
        //        ActivationKey = activationKey
        //    };
        //}



        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                PropertyChanged.Raise(nameof(Status));
                _statusChanged.Raise(_status);
            }
        }


        private EventHandler<EArg<string>> _statusChanged;
        public event EventHandler<EArg<string>> StatusChanged
        {
            add { _statusChanged -= value; _statusChanged += value; }
            remove { _statusChanged -= value; }
        }


        public virtual Action<string> OnWarning { protected get; set; }


        public Func<string> ReadLegacyCfg
        {
            set { _sessionr.ReadLegacyCfg = value; }
        }


        protected abstract IClientValidator GetClientValidator();
        protected abstract IPingClient GetPingClient();
        protected abstract IDownloadClient GetDownloadClient();
        protected abstract ISessionClient GetSessionClient(int checkIntervalMins);
        protected abstract R1Executable GetCurrentR1Exe();
        protected abstract bool ReplaceCurrentExeWith(string replacementExePath);
        protected abstract void RunOnNewThread(Task task, string threadLabel);
        protected abstract DownloaderCfg ParseDownloaderCfg(string configKey);


        protected T Warn<T>(string message, T returnVal = default(T))
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

            _isChecking = true;
            _keepChecking = true;
            RunOnNewThread(ExecuteUpdateCheckLoop(), "Update Checker Loop Thread");
        }



        private Task StartPingOnlyLoop()
            => _pingr.StartPingOnlyLoop(_validr.PingNode, _intervalMins);



        public void StopUpdateCheckLoop()
        {
            _keepChecking = false;
        }


        private async Task ExecuteUpdateCheckLoop()
        {
            var latest = default(R1Executable);
            var ping = _validr.PingNode;
            var current = GetCurrentR1Exe();
            Status = $"Currently running version [{current.FileVersion}].";

            while (_keepChecking)
            {
                Status = "Checking for newer version ...";
                latest = await _pingr.SendAndGetLatestVersion(ping);

                //todo: rewrite local Repo1Cfg if server's copy changed

                //todo: rewrite local legacyCfg if server's copy changed


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



        public async void StartSessionUpdateLoop(string userName, string password)
        {
            if (await _validr.ValidateThisMachine())
            {
                Status = "Machine validation successful.";
                //RunOnNewThread(StartPingOnlyLoop(), "Ping-only Loop Thread");
                StartUpdateCheckLoop();
            }
            else
            {
                Status = "Machine validation failed.";
                RunOnNewThread(_sessionr.StartSessionUpdateLoop(userName, password),
                                    "Session Loop Thread");
            }
        }
    }
}