using System.Threading.Tasks;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IClientValidator : IRestClient
    {
        Task<bool>  ValidateThisMachine  ();
        R1Ping      PingNode             { get; }
    }
}
