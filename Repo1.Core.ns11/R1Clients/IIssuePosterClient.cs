using System;
using System.Threading.Tasks;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IIssuePosterClient
    {
        Task PostError(Exception ex, string errorCaughtBy, string configKey, Func<string> readLegacyCfg = null);
    }
}
