using System;
using Repo1.Core.ns12.Helpers;

namespace Repo1.Core.ns12.Clients
{
    public interface IRepo1Client
    {
        void  StartUpdateCheckLoop();
        void  StopUpdateCheckLoop();
        event EventHandler<EArg<string>> StatusChanged;
    }
}
