using System;
using System.ComponentModel;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IRestClient : INotifyPropertyChanged
    {
        bool    IsBusy      { get; }
        string  Status      { get; }
        int     MaxRetries  { get; set; }
        //Task<T> Get  <T>(string resourceUrl);
        //Task<T> Add  <T>(T newObject);
        void RaisePropertyChanged(string propertyName);
        //Task<bool> Edit<T>(T node, string revisionLog = null);

        Action<string>     OnWarning  { get; set; }
        Action<Exception>  OnError    { get; set; }
    }
}
