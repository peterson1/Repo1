using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;
using Repo1.WPF45.SDK.Configuration;

namespace Repo1.WPF45.SDK.Clients
{
    public class LocalFileUpdater1 : MachineProfilingRestClient1, ILocalFileUpdater
    {
        public LocalFileUpdater1(RestServerCredentials restServerCredentials, string configKey) : base(configKey, restServerCredentials)
        {
        }


        public R1Ping   PingNode  { get; set; }


        public async Task<R1Executable> GetLatestVersions()
        {
            //todo:  include last user activity in ping payload
            await AddProfileTo(PingNode);
            var dict = await Update(PingNode);
            if (dict == null) return null;
            
            UpdateLocalCfgAsNeeded(dict);

            return new R1Executable
            {
                FileVersion = dict.FieldValue("field_fileversion"),
                FileHash    = dict.FieldValue("field_filehash"),
            };
        }


        private void UpdateLocalCfgAsNeeded(Dictionary<string, object> dict)
        {
            var expctd = dict.FieldValue("field_expectedcfg");
            if (expctd.IsBlank()) return;

            var actual = Repo1Cfg.Read(_cfgKey);
            if (actual != expctd)
                Repo1Cfg.Rewrite(expctd, _cfgKey);
        }
    }
}
