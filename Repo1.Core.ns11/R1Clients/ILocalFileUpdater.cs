using System;
using System.Threading.Tasks;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public interface ILocalFileUpdater
    {
        R1Ping              PingNode           { get; set; }
        Func<string>        ReadLegacyCfg      { set; }
        Task<R1Executable>  GetLatestVersions  ();
    }
}
