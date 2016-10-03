using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Repo1.WPF45.SDK.NetworkTools
{
    public class MacAddresses
    {
        public static List<string> List()
            => NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up
                           && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
                           && nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .ToList();
    }
}
