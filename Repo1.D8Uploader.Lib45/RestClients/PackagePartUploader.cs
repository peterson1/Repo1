using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.Core.ns11.R1Models.D8Models.D8ViewsLists;
using Repo1.WPF45.SDK.Clients;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.Extensions.FileInfoExtensions;

namespace Repo1.D8Uploader.Lib45.RestClients
{
    public class PackagePartUploader : D8SvcStackClientBase
    {
        public PackagePartUploader(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            OnError = ex => ThreadedAlerter.Show(ex, "Package Part Uploader");
        }


        //public async Task<bool> UploadAndAttachToNewNode(R1PackagePart part)
        //{
        //    var fid = await UploadFile(part.FullPathOrURL);

        //    var dict = await PostNode(part,
        //          () => GetPackagePartByHash(part.PartHash));

        //    if (dict == null) return false;
        //    return true;
        //}


        public async Task<bool> SavePartNode(R1PackagePart part)
        {
            var dict = await PostNode(part,
                  () => GetPackagePartByHash(part.PartHash));

            return dict != null;
        }


        //public async Task<int> UploadFile(string filePath)
        //{
        //    var file = new FileInfo(filePath);
        //    if (!file.Exists) return -1;
        //    return await PostFile(file.Name, file.Length, file.Base64Content());
        //}


        public async Task<R1PackagePart> GetPackagePartByHash(string partHash)
        {
            var parts = await ViewsList<PartsUploadedByUserView>(partHash);
            return parts?.FirstOrDefault();
        }
    }
}
