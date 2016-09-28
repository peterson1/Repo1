using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Helpers.ExceptionExtensions;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Configuration;
using Repo1.WPF452.SDK.Helpers.ErrorHandlers;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;
using Repo1.WPF452.SDK.Helpers.R1ExecutableExtensions;

namespace Repo1.WPF452.SDK.Clients
{
    [ImplementPropertyChanged]
    public class Repo1Client : Repo1ClientBase1
    {
        internal const string API_URL = "https://repo1.nfshost.com/api1";

        private string _cfgKey;


        public Repo1Client(string configKey, int checkIntervalMins = 2) : base(checkIntervalMins)
        {
            _cfgKey             = configKey;
            _sessionr.ConfigKey = configKey;
            _cfg                = Repo1Cfg.Parse(configKey);
        }


        public Repo1Client(string userName, string password, string activationKey, 
            int checkIntervalMins = 2) 
            : base(userName, password, activationKey, checkIntervalMins, API_URL)
        {
        }


        protected override IClientValidator GetClientValidator()
            => new ClientValidator1(_cfgKey);


        protected override IDownloadClient GetDownloadClient()
            => new DownloaderClient1(_cfg);


        protected override IPingClient GetPingClient()
            => new PingClient1(_cfg);


        protected override ISessionClient GetSessionClient(int checkIntervalMins)
            => new SessionClient1(checkIntervalMins);


        protected override void RunOnNewThread(Task task, string threadLabel)
            => new Thread(async () => 
            {
                try   { await task; }
                catch (Exception ex) { ThreadedAlerter.Show(ex, threadLabel); }
            }
            ).Start();


        public override Action<string> OnWarning
        {
            protected get { return base.OnWarning; }
            set
            {
                base.OnWarning       = value;
                _downloadr.OnWarning = value;
                _sessionr .OnWarning = value;
                _validr   .OnWarning = value;
            }
        }


        protected override bool ReplaceCurrentExeWith(string replacementExePath)
        {
            var origPath    = Assembly.GetEntryAssembly().Location;
            var currExe     = new FileInfo(origPath);
            var retiredsDir = Path.Combine(currExe.DirectoryName, "retired");
            Directory.CreateDirectory(retiredsDir);

            var retiredName = currExe.NamePartOnly() + "_" + currExe.FileVersion();
            var retiredPath = Path.Combine(retiredsDir, retiredName);
            try
            {
                if (File.Exists(retiredPath)) File.Delete(retiredPath);
                currExe.MoveTo(retiredPath);
                File.Move(replacementExePath, origPath);
            }
            catch (Exception ex)
            {
                return Warn(ex.Info(), false);
            }
            return File.Exists(origPath);
        }


        protected override R1Executable GetCurrentR1Exe()
            => R1Exe.FromFile(Assembly.GetEntryAssembly().Location);
    }
}
