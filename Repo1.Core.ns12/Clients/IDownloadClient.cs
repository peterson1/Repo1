using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public interface IDownloadClient
    {
        Task<List<R1SplitPart>>  GetPartsList   (string exeVersion);
        Task<string>             AssembleParts  (List<R1SplitPart> splitParts);
    }
}
