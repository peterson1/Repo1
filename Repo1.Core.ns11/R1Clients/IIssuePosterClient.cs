using System;
using System.Threading.Tasks;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IIssuePosterClient
    {
        Task PostError(string errorMsg, string configKey);
        Func<string> ReadLegacyCfg { set; }
    }
}
