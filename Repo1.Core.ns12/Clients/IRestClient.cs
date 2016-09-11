using System.Threading.Tasks;

namespace Repo1.Core.ns12.Clients
{
    public interface IRestClient
    {
        Task<T> Get<T>(string resourceUrl);
    }
}
