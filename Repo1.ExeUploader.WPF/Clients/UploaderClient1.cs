using System;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Configuration;
using Repo1.WPF452.SDK.Archivers;
using Repo1.WPF452.SDK.Clients;

namespace Repo1.ExeUploader.WPF.Clients
{
    class UploaderClient1 : SvcStackRestClient
    {
        private UploaderCfg _upCfg;

        public UploaderClient1(UploaderCfg uploaderCfg) : base(uploaderCfg)
        {
            _upCfg = uploaderCfg;
        }


        internal async Task<R1Executable> GetExecutable()
        {
            var list = await ViewsList<UploadablesForUserDTO>(_upCfg.ExecutableNid);
            if (list == null) return null;
            return list.Single(x => x.nid == _upCfg.ExecutableNid);
        }


        internal async Task UploadNew(R1Executable localExe)
        {
            var archive = await SevenZipper1.Compress(localExe.FullPathOrURL);

            //  split

            //  upload

        }
    }
}
