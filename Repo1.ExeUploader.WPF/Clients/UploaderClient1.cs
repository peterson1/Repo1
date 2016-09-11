using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Configuration;

namespace Repo1.ExeUploader.WPF.Clients
{
    class UploaderClient1 : RestClientBase
    {
        const string VIEWS_EXE_BY_USER = "sadfa sdf asfasdfasdfasdf sdf";

        private UploaderCfg _upCfg;

        public UploaderClient1(UploaderCfg uploaderCfg) : base(uploaderCfg)
        {
            _upCfg = uploaderCfg;
        }


        internal async Task<R1Executable> GetExecutable()
        {
            var list = await ViewsList<R1Executable>(VIEWS_EXE_BY_USER);
            return list.Single(x => x.nid == _upCfg.ExecutableNid);
        }


        public override Task<T> Get<T>(string resourceUrl) 
            => CreateClient().GetAsync<T>(resourceUrl);


        private ServiceStack.JsonServiceClient CreateClient()
            => new ServiceStack.JsonServiceClient(_upCfg.ApiBaseURL)
            {
                UserName = _upCfg.Username,
                Password = _upCfg.Password,
                AlwaysSendBasicAuthHeader = true,
            };
    }
}
