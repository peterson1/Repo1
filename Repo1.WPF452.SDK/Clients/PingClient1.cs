using System;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Models;

namespace Repo1.WPF452.SDK.Clients
{
    internal class PingClient1 : IPingClient
    {
        public R1Ping GatherPingFields()
        {
            throw new NotImplementedException();
        }

        public Task<string> SendAndGetLatestVersion(R1Ping pingData)
        {
            throw new NotImplementedException();
        }
    }
}
