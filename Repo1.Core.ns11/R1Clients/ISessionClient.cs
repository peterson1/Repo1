using System;
using System.Threading.Tasks;

namespace Repo1.Core.ns11.R1Clients
{
    public interface ISessionClient : IRestClient
    {
        int           SendIntervalMins  { get; set; }
        string        ConfigKey         { get; set; }
        Func<string>  ReadLegacyCfg     { set; }

        Task  StartSessionUpdateLoop  (string userName, string password);
    }
}
