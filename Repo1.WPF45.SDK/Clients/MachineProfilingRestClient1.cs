using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Models;
using Repo1.WPF45.SDK.Configuration;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;
using Repo1.WPF45.SDK.NetworkTools;
using Repo1.WPF45.SDK.UacTools;

namespace Repo1.WPF45.SDK.Clients
{
    public class MachineProfilingRestClient1 : D7SvcStackClient
    {
        protected string _cfgKey;

        internal MachineProfilingRestClient1(string configKey, RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            _cfgKey = configKey;
        }


        public Func<string>  ReadLegacyCfg  { private get; set; }


        internal async Task AddProfileTo(R1MachineSpecsBase targetObj)
        {
            var publicIpJob = GetPublicIP();
            var offlineJobs = RunOfflineJobs(targetObj);
            try
            {
                await Task.WhenAll(publicIpJob, offlineJobs);
            }
            catch (Exception ex)
            {
                targetObj.MacAndPrivateIPs = ex.Info(true, true);
                return;
            }
            targetObj.PublicIP = await publicIpJob;
        }


        private async Task<bool> RunOfflineJobs(R1MachineSpecsBase obj)
        {
            obj.ExePath          = SafeGet(() => GetExePath()            );
            obj.MacAndPrivateIPs = SafeGet(() => GetMacAndPrivateIPs()   );
            obj.ExeVersion       = SafeGet(() => GetExeVersion()         );
            obj.WindowsAccount   = SafeGet(() => Environment.UserName    );
            obj.ComputerName     = SafeGet(() => Environment.MachineName );
            obj.Workgroup        = SafeGet(() => GetWorkgroup()          );
            obj.LegacyCfgJson    = SafeGet(() => ReadLegacyCfg?.Invoke() );
            obj.Repo1CfgJson     = SafeGet(() => Repo1Cfg.Read(_cfgKey));
            obj.IsAdminUser      = CurrentWindowsUser.IsAdmin();

            await Task.Delay(1);
            return true;
        }


        private string SafeGet(Func<string> method)
        {
            try                  { return method.Invoke(); }
            catch (Exception ex) { return ex.Message; }
        }


        internal string GetExePath()
            => Assembly.GetEntryAssembly()?.Location
            ?? Assembly.GetExecutingAssembly().Location;


        internal string GetMacAndPrivateIPs()
        {
            var list = MacAddresses.List();

            for (int i = 0; i < list.Count; i++)
                list[i] += " : " + PrivateIP.ForMAC(list[i]);

            return string.Join(L.f, list);
        }


        internal string GetExeVersion()
            => new FileInfo(GetExePath()).FileVersion();


        internal string GetWorkgroup()
        {
            var nme = $"Win32_ComputerSystem.Name='{Environment.MachineName}'";
            var inf = new ManagementObject(nme);
            return inf["Workgroup"].ToString();
        }


        internal string GetPrivateIP(string macAddress) 
            => PrivateIP.ForMAC(macAddress);


        internal async Task<string> GetPublicIP()
        {
            const string url = "https://api.ipify.org?format=json";
            var resp = await GetTilOK<Dictionary<string, string>>(url);
            return resp["ip"].ToString();
        }
    }
}
