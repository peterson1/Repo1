using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public interface IDownloadClient : IRestClient
    {
        Task<List<R1SplitPart>>  GetPartsList        (string exeVersion, string macAddress);
        Task<string>             DownloadAndExtract  (List<R1SplitPart> splitParts, string expectedHash);
    }
}
