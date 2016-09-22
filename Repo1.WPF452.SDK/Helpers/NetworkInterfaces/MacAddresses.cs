using System.Collections.Generic;
using System.Linq;
using Repo1.Core.ns12.Helpers.StringExtensions;
using System.Net.NetworkInformation;

namespace Repo1.WPF452.SDK.Helpers.NetworkInterfaces
{
    public class MacAddresses
    {
        public static List<string> List()
            => NetworkInterface.GetAllNetworkInterfaces()
                .Where  (nic => nic.OperationalStatus == OperationalStatus.Up
                             && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
                             && nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .Select (nic => nic.GetPhysicalAddress().ToString())
                .ToList ();
    }
}
