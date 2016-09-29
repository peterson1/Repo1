using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Configuration;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Configuration;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;
using Repo1.WPF452.SDK.Helpers.NetworkInterfaces;

namespace Repo1.WPF452.SDK.Clients
{
    public class ClientValidator1 : SvcStackRestClient, IClientValidator
    {
        public ClientValidator1(string configKey) : base(Repo1Cfg.Parse(configKey))
        {
        }


        public R1Ping  PingNode  { get; private set; }


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
            pingDTO.UserLicense          = new R1UserLicense { nid = pingDTO.UserLicenseNid };
            pingDTO.PublicIP             = await GetPublicIP();
            pingDTO.PrivateIP            = PrivateIP.ForMAC(macAddress);
            pingDTO.InstalledVersion     = GetInstalledVersion();
            pingDTO.RegisteredMacAddress = macAddress;

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

            var key  = cfg.GetLicenseKey(macAddress);
            var list = await ViewsList<GetPingByLicenseKeyDTO>(key);
            return list?.SingleOrDefault();
        }


        // ignore errors
        protected override void OnError(Exception ex) { }


        private string GetInstalledVersion() => new FileInfo(
            Assembly.GetEntryAssembly().Location).FileVersion();
    }
}
