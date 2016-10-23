using System;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.Core.ns11.R1Models.D8Models.D8ViewsLists;
using Repo1.WPF45.SDK.Clients;
using Repo1.WPF45.SDK.ErrorHandlers;

namespace Repo1.D8Uploader.Lib45.RestClients
{
    public class UploaderClient2 : D8SvcStackClientBase
    {
        public UploaderClient2(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            OnError = ex => ThreadedAlerter.Show(ex, "Uploader Client 2");
        }

        public async Task<D8Package> GetPackage(string packageFilename)
        {
            Status = "Querying uploadables for this user ...";
            var list = await ViewsList<UploadablesForUserView>();
            if (list == null) return null;
            var exe = list.SingleOrDefault(x => x.FileName == packageFilename);
            return exe;
        }


        //protected override void OnError(Exception ex)
        //    => ThreadedAlerter.Show(ex, "Uploader Client 2");
    }
}
