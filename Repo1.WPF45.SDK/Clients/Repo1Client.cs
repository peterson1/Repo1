using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;
using Repo1.WPF45.SDK.Configuration;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;
using Repo1.WPF45.SDK.Extensions.R1ModelExtensions;

namespace Repo1.WPF45.SDK.Clients
{
    [ImplementPropertyChanged]
    public class Repo1Client : Repo1ClientBase1
    {
        internal const string API_URL = "https://repo1.nfshost.com/api1";



        public Repo1Client(string configKey, int checkIntervalMins = 2) : base(configKey, checkIntervalMins)
        {
            _sessionr.ConfigKey = configKey;
        }



        protected override IClientValidator GetClientValidator()
            => new ClientValidator1(_cfgKey);


        protected override IDownloadClient GetDownloadClient()
            => new DownloaderClient1(_cfg);


        protected override IPingClient GetPingClient()
            => new PingClient1(_cfg);


        protected override ISessionClient GetSessionClient(int checkIntervalMins)
            => new SessionClient1(checkIntervalMins);


        protected override IIssuePosterClient GetPosterClient()
            => new IssuePoster1(_cfg);


        protected override R1Executable GetCurrentR1Exe()
            => R1Exe.FromFile(Assembly.GetEntryAssembly().Location);


        protected override DownloaderCfg ParseDownloaderCfg(string configKey)
            => Repo1Cfg.Parse(configKey);


        protected override void RunOnNewThread(Task task, string threadLabel)
            => new Thread(async () =>
            {
                try { await task; }
                catch (Exception ex) { ThreadedAlerter.Show(ex, threadLabel); }
            }
            ).Start();


        public override Action<string> OnWarning
        {
            protected get { return base.OnWarning; }
            set
            {
                base.OnWarning = value;
                _downloadr.OnWarning = value;
                _sessionr.OnWarning = value;
                _validr.OnWarning = value;
            }
        }


        protected override bool ReplaceCurrentExeWith(string replacementExePath)
        {
            var origPath = Assembly.GetEntryAssembly().Location;
            var currExe = new FileInfo(origPath);
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
    }
}
