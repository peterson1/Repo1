using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Helpers.ExceptionExtensions;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;
using Repo1.WPF452.SDK.Helpers.R1ExecutableExtensions;

namespace Repo1.WPF452.SDK.Clients
{
    [ImplementPropertyChanged]
    public class Repo1Client : Repo1ClientBase1
    {
        public Repo1Client(string userName, string password, string activationKey, 
            int checkIntervalMins = 2,
            string apiBaseURL = "https://repo1.nfshost.com/api1") 
            : base(userName, password, activationKey, checkIntervalMins, apiBaseURL)
        {
        }

        protected override IClientValidator GetClientValidator()
            => new ClientValidator1(_cfg);


        protected override IDownloadClient GetDownloadClient()
            => new DownloaderClient1(_cfg);


        protected override IPingClient GetPingClient()
            => new PingClient1(_cfg);


        protected override void RunOnNewThread(Task task)
            => new Thread(async () => await task).Start();


        public override Action<string> OnWarning
        {
            protected get { return base.OnWarning; }
            set
            {
                base.OnWarning = value;
                _downloadr.OnWarning = value;
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
