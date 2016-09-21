using System;
using System.Threading.Tasks;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public abstract class SessionClientBase : RestClientBase, ISessionClient
    {
        private bool _isTracking;

        public SessionClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }

        protected abstract string  GetMacAndPrivateIPs ();
        protected abstract string  GetRepo1CfgJson     ();
        protected abstract string  GetLegacyCfgJson    ();
        protected abstract string  GetWorkgroup        ();
        protected abstract string  GetComputerName     ();
        protected abstract string  GetWindowsAccount   ();
        protected abstract string  GetExePath          ();
        protected abstract string  GetExeVersion       ();


        private Task<R1UserSession> GetSavedSession(string sessionKey)
        {
            throw new NotImplementedException();
        }


        public async Task StartTrackingLoop()
        {
            if (_isTracking) return;
            _isTracking = true;

            var sess = await GatherSessionInfo();
            var savd = await GetSavedSession(sess.SessionKey);
        }


        private async Task<R1UserSession> GatherSessionInfo()
        {
            var ssn              = new R1UserSession();
            ssn.PublicIP         = await GetPublicIP();

            ssn.MacAndPrivateIPs = GetMacAndPrivateIPs();
            ssn.ExeVersion       = GetExeVersion();
            ssn.ExePath          = GetExePath();

            ssn.WindowsAccount   = GetWindowsAccount();
            ssn.ComputerName     = GetComputerName();
            ssn.Workgroup        = GetWorkgroup();

            ssn.LegacyCfgJson    = GetLegacyCfgJson();
            ssn.Repo1CfgJson     = GetRepo1CfgJson();

            ssn.SessionKey       = (ssn.MacAndPrivateIPs 
                                 +  ssn.ExePath).SHA1ForUTF8();

            ssn.Description      = ssn.WindowsAccount 
                        + " on " + ssn.Workgroup  
                          + "\\" + ssn.ComputerName;
            return ssn;
        }
    }
}
