using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;
using Repo1.Core.ns11.R1Models.ViewsLists;
using Repo1.WPF45.SDK.Configuration;
using Repo1.WPF45.SDK.NetworkTools;

namespace Repo1.WPF45.SDK.Clients
{
    internal class SessionClient1 : MachineProfilingRestClient1, ISessionClient
    {
        private bool _isTracking;


        public SessionClient1(string configKey, int sendIntervalMins) : base(configKey, null)
        {
            SendIntervalMins = sendIntervalMins;
        }


        public int SendIntervalMins { get; set; } = 2;


        public string ConfigKey { get; set; }



        public async Task StartSessionUpdateLoop(string userName, string password)
        {
            _creds = new RestServerCredentials
            {
                Username = userName,
                Password = password,
                ApiBaseURL = Repo1Client.API_URL,
            };

            if (_isTracking) return;
            _isTracking = true;

            Status = "FindSavedSessions ...";
            var recs = await FindSavedSessions();
            if (recs == null) return;
            if (recs.Count > 1) return;

            var savd = recs.SingleOrDefault();
            if (savd == null)
                savd = await PostNewSession();

            if (!Repo1Cfg.Found(ConfigKey))
                Repo1Cfg.WriteBlank(ConfigKey);

            while (true)
            {
                await Task.Delay(1000 * 60 * SendIntervalMins);

                var sess = await GatherSessionInfo(savd);
                var newCfg = await SendAndGetNewCfg(sess);

                if (newCfg != sess.Repo1CfgJson)
                    Repo1Cfg.Rewrite(newCfg, ConfigKey);
            }
        }


        private async Task<string> SendAndGetNewCfg(R1UserSession session)
        {
            var dict = await Update(session);
            return dict.FieldValue("field_expectedcfg");
        }


        private async Task<R1UserSession> PostNewSession()
        {
            Status = "PostNewSession ...";
            var sess = await GatherSessionInfo();
            var dict = await Create(sess, async () =>
            {
                var list = await FindSavedSessions();
                return list?.FirstOrDefault();
            });
            return sess;
        }


        private async Task<R1UserSession> GatherSessionInfo
            (R1UserSession savedNode = null)
        {
            var exePath = GetExePath();
            var ssn = new R1UserSession();
            ssn.nid = savedNode?.nid ?? 0;
            ssn.uid = savedNode?.uid ?? 0;
            ssn.vid = savedNode?.vid ?? 0;

            await AddProfileTo(ssn);

            //ssn.PublicIP = await GetPublicIP();
            //ssn.MacAndPrivateIPs = GetMacAndPrivateIPs();
            //ssn.ExeVersion = GetExeVersion();
            //ssn.ExePath = exePath.Replace("\\", "/");
            //ssn.WindowsAccount = Environment.UserName;
            //ssn.ComputerName = Environment.MachineName;
            //ssn.Workgroup = GetWorkgroup();
            //ssn.LegacyCfgJson = _readLegacyCfg?.Invoke();
            //ssn.Repo1CfgJson = Repo1Cfg.Read(ConfigKey);

            ssn.ExpectedCfg = Repo1Cfg.EXPECTED_KEY_IGNORE_ME;

            ssn.SessionKey = GetSessionKey();
            ssn.FutureLicenseKey = GetFutureLicenseKey();
            ssn.Description = NoExt(exePath)
                        + " on " + ssn.Workgroup
                           + "/" + ssn.ComputerName
                           + "/" + ssn.WindowsAccount;
            return ssn;
        }


        private string NoExt(string filePath)
            => Path.GetFileNameWithoutExtension(filePath);


        private string B64(string text)
        {
            if (text.IsBlank()) return null;
            var byts = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(byts);
        }



        private string GetFutureLicenseKey()
        {
            var rCfg = Repo1Cfg.Parse(ConfigKey);
            if (rCfg == null) return null;
            var list = MacAddresses.List();

            for (int i = 0; i < list.Count; i++)
                list[i] = rCfg.GetLicenseKey(list[i]);

            return string.Join(L.f, list);
        }


        private string GetSessionKey()
            => (string.Join(",", MacAddresses.List())
                               + GetExePath()
                               + ConfigKey).SHA1ForUTF8();



        private async Task<List<R1UserSession>> FindSavedSessions()
        {
            var key = GetSessionKey();
            var list = await ViewsList<UserSessionsByKeyDTO>(key);
            if (list == null) return null;

            if (list.Count > 1)
                Warn($"More than 1 session matched key: “{key}”.");

            return list.Select(x => x as R1UserSession).ToList();
        }
    }
}
