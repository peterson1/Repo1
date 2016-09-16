using System.ComponentModel;
using System.Threading.Tasks;

namespace Repo1.Core.ns12.Clients
{
    public interface IRestClient : INotifyPropertyChanged
    {
        bool    IsBusy  { get; }
        string  Status  { get; }
        //Task<T> Get  <T>(string resourceUrl);
        //Task<T> Add  <T>(T newObject);
        void RaisePropertyChanged(string propertyName);
        //Task<bool> Edit<T>(T node, string revisionLog = null);
    }
}
