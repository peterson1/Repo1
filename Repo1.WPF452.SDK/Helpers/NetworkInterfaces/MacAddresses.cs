using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Repo1.WPF452.SDK.Helpers.NetworkInterfaces
{
    public class MacAddresses
    {
        public static List<string> List()
            => NetworkInterface.GetAllNetworkInterfaces()
                .Where  (nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select (nic => nic.GetPhysicalAddress().ToString())
                .ToList ();
    }
}
