using System;
using System.Threading.Tasks;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IClientValidator : IRestClient
    {
        R1Ping       PingNode             { get; }
        Func<string> ReadLegacyCfg        { set; }
        Task<bool>   ValidateThisMachine  ();
    }
}
