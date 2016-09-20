using System;
using System.ComponentModel;
using Repo1.Core.ns12.Helpers;

namespace Repo1.Core.ns12.Clients
{
    public interface IRepo1Client : INotifyPropertyChanged
    {
        void  StartUpdateCheckLoop();
        void  StopUpdateCheckLoop();
        void  RaisePropertyChanged(string propertyName);

        event EventHandler<EArg<string>> StatusChanged;
        event EventHandler               UpdateInstalled;

        string  Status { get; }

        Action<string>  OnWarning  { set; }
    }
}
