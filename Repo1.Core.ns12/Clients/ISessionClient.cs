using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public interface ISessionClient : IRestClient
    {
        Task StartTrackingLoop();
    }
}
