using System;
using System.ComponentModel;
using Repo1.Core.ns11.Configuration;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IRestClient : INotifyPropertyChanged
    {
        bool     IsBusy       { get; }
        string   Status       { get; }
        int      MaxRetries   { get; set; }
        //Task<T> Get  <T>(string resourceUrl);
        //Task<T> Add  <T>(T newObject);
        void RaisePropertyChanged(string propertyName);
        //Task<bool> Edit<T>(T node, string revisionLog = null);

        RestServerCredentials  Credentials   { get; set; }
        Action<string>         OnWarning     { get; set; }
        Action<Exception>      OnError       { get; set; }
        Action<string>         WriteToDebug  { get; set; }
    }
}
