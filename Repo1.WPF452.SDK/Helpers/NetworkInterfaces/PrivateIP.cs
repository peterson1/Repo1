using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Repo1.WPF452.SDK.Helpers.NetworkInterfaces
{
    public class PrivateIP
    {
        public static string ForMAC(string macAddress)
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .SingleOrDefault(x => x.GetPhysicalAddress().ToString() == macAddress);

            return nic?.GetIPProperties().UnicastAddresses.SingleOrDefault(x
                => x.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString();
        }
    }
}
