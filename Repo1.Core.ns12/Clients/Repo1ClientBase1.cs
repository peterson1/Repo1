using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Repo1.Core.ns12.Helpers;
using Repo1.Core.ns12.Helpers.PropertyChangedExtensions;

namespace Repo1.Core.ns12.Clients
{
    public abstract class Repo1ClientBase1 : IRepo1Client
    {
        const int INTERVAL_SEC = 2;

        public Repo1ClientBase1(string userName, string password, string apiBaseURL)
        {
            _pingr = GetPingClient();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private bool            _keepChecking;
        private bool            _isChecking;
        private IPingClient     _pingr;
        private IDownloadClient _downloadr;


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


        private      EventHandler<EArg<string>> _statusChanged;
        public event EventHandler<EArg<string>>  StatusChanged
        {
            add    { _statusChanged -= value; _statusChanged += value; }
            remove { _statusChanged -= value; }
        }

        protected abstract IPingClient GetPingClient();
        protected abstract void RunOnNewThread(Task task);


        public void StartUpdateCheckLoop()
        {
            if (_isChecking) return;
            _isChecking   = true;
            _keepChecking = true;
            RunOnNewThread(ExecuteUpdateCheckLoop());
        }


        public void StopUpdateCheckLoop()
        {
            _keepChecking = false;
        }


        private async Task ExecuteUpdateCheckLoop()
        {
            var latestVer = "";

            while (_keepChecking)
            {
                Status    = "Checking for newer version ...";
                var ping  = _pingr.GatherPingFields();
                latestVer = await _pingr.SendAndGetLatestVersion(ping);

                if (!_keepChecking) return;
                if (ping.InstalledVersion != latestVer) break;

                await Task.Delay(1000 * INTERVAL_SEC);
            }

            Status        = "Newer version found. Downloading ...";
            var partsList = await _downloadr.GetPartsList(latestVer);
            var exePath   = await _downloadr.AssembleParts(partsList);
        }



        public virtual void RaisePropertyChanged(string propertyName)
            => PropertyChanged.Raise(propertyName);
    }
}
