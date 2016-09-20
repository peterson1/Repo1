using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public interface IDownloadClient : IRestClient
    {
        Task<List<R1SplitPart>>  GetPartsList        (string exeVersion, string macAddress);
        Task<string>             DownloadAndExtract  (List<R1SplitPart> splitParts, string expectedHash);
    }
}
