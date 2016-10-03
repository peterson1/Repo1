using System;
using System.ComponentModel;
using Repo1.Core.ns11.EventArguments;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IRepo1Client : INotifyPropertyChanged
    {
        void  StartUpdateCheckLoop   ();
        void  StopUpdateCheckLoop    ();
        void  StartSessionUpdateLoop (string userName, string password);

        void  RaisePropertyChanged(string propertyName);

        event EventHandler<EArg<string>> StatusChanged;
        event EventHandler               UpdateInstalled;

        string           Status         { get; }
        Action<string>   OnWarning      { set; }
        Func<string>     ReadLegacyCfg  { set; }
    }
}
