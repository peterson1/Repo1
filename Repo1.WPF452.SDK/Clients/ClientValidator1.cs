using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Configuration;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Models;
using Repo1.WPF452.SDK.Helpers.FileInfoExtensions;

namespace Repo1.WPF452.SDK.Clients
{
    public class ClientValidator1 : SvcStackRestClient, IClientValidator
    {
        private DownloaderCfg _dCfg;


        public ClientValidator1(DownloaderCfg downloaderCfg) : base(downloaderCfg)
        {
            _dCfg = downloaderCfg;
        }


        public R1Ping  PingNode  { get; private set; }


        public async Task<bool> ValidateThisMachine()
        {
            foreach (var macAdress in GetMacAddresses())
            {
                var dto = await GetPingByMacAddress(macAdress);
                if (dto == null) continue;

                PingNode = await AssemblePingNode(dto, macAdress);
                return true;
            }
            return false;
        }


        private async Task<R1Ping> AssemblePingNode(GetPingByLicenseKeyDTO pingDTO, string macAddress)
        {
            var publicIP = await GetPublicIP();

            pingDTO.UserLicense          = new R1UserLicense { nid = pingDTO.UserLicenseNid };
            pingDTO.PublicIP             = await GetPublicIP();
            pingDTO.PrivateIP            = GetPrivateIP(macAddress);
            pingDTO.InstalledVersion     = GetInstalledVersion();
            pingDTO.RegisteredMacAddress = macAddress;

            return pingDTO;
        }


        private async Task<GetPingByLicenseKeyDTO> GetPingByMacAddress(string macAddress)
        {
            var key  = _dCfg.GetLicenseKey(macAddress);
            var list = await ViewsList<GetPingByLicenseKeyDTO>(key);
            return list.SingleOrDefault();
        }


        private string GetInstalledVersion() => new FileInfo(
            Assembly.GetEntryAssembly().Location).FileVersion();


        private string GetPrivateIP(string macAddress)
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .SingleOrDefault(x => x.GetPhysicalAddress().ToString() == macAddress);

            return nic?.GetIPProperties().UnicastAddresses.SingleOrDefault(x 
                => x.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString();
        }


        private async Task<string> GetPublicIP()
        {
            const string url = "https://api.ipify.org?format=json";
            var resp = await GetTilOK<Dictionary<string, string>>(url);
            return resp["ip"].ToString();
        }


        private List<string> GetMacAddresses()
            => NetworkInterface.GetAllNetworkInterfaces()
                .Where (nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString()).ToList();
    }
}
