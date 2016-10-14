using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Repo1.Core.ns11.EventArguments;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IRepo1Client : INotifyPropertyChanged
    {
        void  StartUpdateChecker  (string tempUserName, string tempPassword);
        Task  PostRuntimeError    (string errorMessage);

        void  RaisePropertyChanged(string propertyName);

        event EventHandler<EArg<string>> StatusChanged;
        event EventHandler               UpdateInstalled;

        string           Status         { get; }
        Action<string>   OnWarning      { set; }
        Func<string>     ReadLegacyCfg  { set; }
    }
}
