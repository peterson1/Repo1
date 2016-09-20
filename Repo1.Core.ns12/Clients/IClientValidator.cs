using System.Threading.Tasks;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public interface IClientValidator : IRestClient
    {
        Task<bool>  ValidateThisMachine  ();
        R1Ping      PingNode             { get; }
    }
}
