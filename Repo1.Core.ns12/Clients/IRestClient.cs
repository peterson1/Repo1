using System.Threading.Tasks;

namespace Repo1.Core.ns12.Clients
{
    public interface IRestClient
    {
        bool IsBusy { get; }

        Task<T> Get<T>(string resourceUrl);
    }
}
