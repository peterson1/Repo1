using System;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;
using Repo1.Core.ns11.R1Models.ViewsLists;
using Repo1.WPF45.SDK.Configuration;
using Repo1.WPF45.SDK.NetworkTools;

namespace Repo1.WPF45.SDK.Clients
{
    internal class ClientValidator1 : MachineProfilingRestClient1, IClientValidator
    {
        private string _cfgKey;

        public ClientValidator1(string configKey) : base(Repo1Cfg.Parse(configKey))
        {
            _cfgKey = configKey;
        }


        public R1Ping PingNode { get; private set; }


        public async Task<bool> ValidateThisMachine()
        {
            if (_creds == null)
                return Warn("Credentials object is NULL.");

            foreach (var macAdress in MacAddresses.List())
            {
                var dto = await GetPingByMacAddress(macAdress);

                if (_creds.WasRejected)
                    return Warn("Server rejected the credentials.");

                if (dto == null)
                {
                    Warn($"Unregisted MAC address: {macAdress}");
                    continue;
                }

                PingNode = await AssemblePingNode(dto, macAdress);
                return true;
            }
            return Warn("No valid MAC addresses found.");
        }


        private async Task<R1Ping> AssemblePingNode(GetPingByLicenseKeyDTO pingDTO, string macAddress)
        {
            //todo: pass readLegacyCfg method here
            await AddProfileTo(pingDTO, _cfgKey);

            pingDTO.UserLicense          = new R1UserLicense { nid = pingDTO.UserLicenseNid };
            pingDTO.RegisteredMacAddress = macAddress;
            pingDTO.PrivateIP            = GetPrivateIP(macAddress);
            pingDTO.InstalledVersion     = GetExeVersion();
            pingDTO.ExpectedCfg          = Repo1Cfg.EXPECTED_KEY_IGNORE_ME;

            return pingDTO;
        }


        private async Task<GetPingByLicenseKeyDTO> GetPingByMacAddress(string macAddress)
        {
            var cfg = _creds as DownloaderCfg;
            if (cfg == null)
            {
                Warn($"Config is not a ‹{typeof(DownloaderCfg).Name}›");
                return null;
            }

            var key = cfg.GetLicenseKey(macAddress);
            var list = await ViewsList<GetPingByLicenseKeyDTO>(key);
            return list?.SingleOrDefault();
        }


        // ignore errors
        protected override void OnError(Exception ex) { }


        //private string GetInstalledVersion() => new FileInfo(
        //    Assembly.GetEntryAssembly().Location).FileVersion();
    }
}
