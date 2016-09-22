using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Configuration;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Configuration;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;
using Repo1.WPF452.SDK.Helpers.NetworkInterfaces;

namespace Repo1.WPF452.SDK.Clients
{
    class SessionClient1 : SvcStackRestClient, ISessionClient
    {
        private bool          _isTracking;
        private DownloaderCfg _cfg;
        private Func<string>  _readLegacyCfg;


        public SessionClient1(DownloaderCfg downloaderCfg) : base(downloaderCfg)
        {
            _cfg = downloaderCfg;
        }


        public int SendIntervalMins { get; set; } = 2;



        public async Task StartTrackingLoop()
        {
            if (_isTracking) return;
            _isTracking = true;

            var recs = await FindSavedSessions();
            if (recs == null) return;
            if (recs.Count > 1) return;

            var savd = recs.SingleOrDefault();
            if (savd == null)
                savd = await PostNewSession();

            while (true)
            {
                //await Task.Delay(1000 * 60 * SendIntervalMins);
                await Task.Delay(1000 * 1);

                var sess = await GatherSessionInfo(savd);
                await Update(sess);
            }
        }




        private async Task<R1UserSession> PostNewSession()
        {
            var sess = await GatherSessionInfo();
            var dict = await Create(sess);
            sess.nid = int.Parse(dict["nid"].ToString());
            sess.uid = int.Parse(dict["uid"].ToString());
            sess.vid = int.Parse(dict["vid"].ToString());
            return sess;
        }


        private async Task<R1UserSession> GatherSessionInfo
            (R1UserSession savedNode = null)
        {
            var ssn              = new R1UserSession();
            ssn.nid              = savedNode?.nid ?? 0;
            ssn.uid              = savedNode?.uid ?? 0;
            ssn.vid              = savedNode?.vid ?? 0;
            ssn.PublicIP         = await GetPublicIP();

            ssn.MacAndPrivateIPs = GetMacAndPrivateIPs();
            ssn.ExeVersion       = GetExeVersion();
            ssn.ExePath          = GetExePath().Replace("\\", "/");

            ssn.WindowsAccount   = Environment.UserName;
            ssn.ComputerName     = Environment.MachineName;
            ssn.Workgroup        = GetWorkgroup();

            ssn.LegacyCfgJson    = B64(_readLegacyCfg?.Invoke());
            ssn.Repo1CfgJson     = B64(Repo1Cfg.Read(_cfg.ActivationKey));
            ssn.SessionKey       = GetSessionKey();

            ssn.Description      = ssn.WindowsAccount
                        + " on " + ssn.Workgroup
                           + "/" + ssn.ComputerName;
            return ssn;
        }


        private string B64(string text)
        {
            if (text.IsBlank()) return null;
            var byts = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(byts);
        }


        private string GetWorkgroup()
        {
            var nme = $"Win32_ComputerSystem.Name='{Environment.MachineName}'";
            var inf = new ManagementObject(nme);
            return inf["Workgroup"].ToString();
        }


        private string GetMacAndPrivateIPs()
        {
            var list = MacAddresses.List();

            for (int i = 0; i < list.Count; i++)
                list[i] += " : " + PrivateIP.ForMAC(list[i]);

            return string.Join(L.f, list);
        }


        private string GetSessionKey()
            => (string.Join(",", MacAddresses.List())
                               + GetExePath()
                               + _cfg.ActivationKey).SHA1ForUTF8();



        private async Task<List<R1UserSession>> FindSavedSessions()
        {
            var key = GetSessionKey();
            var list = await ViewsList<UserSessionsByKeyDTO>(key);
            if (list == null) return null;

            if (list.Count > 1)
                Warn($"More than 1 session matched key: “{key}”.");

            return list.Select(x => x as R1UserSession).ToList();
        }



        private string GetExeVersion()
            => new FileInfo(GetExePath()).FileVersion();


        private string GetExePath()
            => Assembly.GetEntryAssembly().Location;


        public Func<string> ReadLegacyCfg
            { set { _readLegacyCfg = value; } }
    }
}
