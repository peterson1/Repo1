using System.Threading.Tasks;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IPingClient
    {
        Task<R1Executable>  SendAndGetLatestVersion  (R1Ping pingData);
        Task                StartPingOnlyLoop        (R1Ping pingNode, int intervalMins);
    }
}
