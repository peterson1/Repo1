using System.Threading.Tasks;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public interface IPingClient
    {
        Task<R1Executable>  SendAndGetLatestVersion  (R1Ping pingData);
    }
}
