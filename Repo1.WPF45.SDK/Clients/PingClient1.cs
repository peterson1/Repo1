using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;

namespace Repo1.WPF45.SDK.Clients
{
    internal class PingClient1 : SvcStackRestClient, IPingClient
    {

        public PingClient1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        public async Task<R1Executable> SendAndGetLatestVersion(R1Ping pingNode)
        {
            var dict = await Update(pingNode);
            if (dict == null) return null;
            return new R1Executable
            {
                FileVersion = dict.FieldValue("field_fileversion"),
                FileHash    = dict.FieldValue("field_filehash"),
            };
        }

    }
}
