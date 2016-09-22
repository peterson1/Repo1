using System;
using System.Threading.Tasks;

namespace Repo1.Core.ns12.Clients
{
    public interface ISessionClient : IRestClient
    {
        int           SendIntervalMins  { get; set; }
        Func<string>  ReadLegacyCfg     { set; }

        Task  StartTrackingLoop  ();
    }
}
