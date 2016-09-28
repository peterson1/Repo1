using System;
using System.Threading.Tasks;

namespace Repo1.Core.ns12.Clients
{
    public interface ISessionClient : IRestClient
    {
        int           SendIntervalMins  { get; set; }
        string        ConfigKey         { get; set; }
        Func<string>  ReadLegacyCfg     { set; }

        Task  StartSessionUpdateLoop  (string userName, string password);
    }
}
