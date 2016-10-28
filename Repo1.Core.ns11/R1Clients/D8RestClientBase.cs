using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal8Tools;
using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.Core.ns11.R1Clients
{
    public abstract class D8RestClientBase : PerseveringClientBase
    {
        private string _csrfToken;

        public D8RestClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }

        protected abstract Task<bool> EnableWriteMethods(string resourceUrl);



        public Task<bool> RequestWriteAccess()
            => KeepTrying(async () =>
            {
                var ok = await EnableWriteMethods("api1/user/login");
                Credentials.WasRejected = !ok;
                return ok;
            });


        protected Task<Dictionary<string, object>> PostNode<T>(T objectToPost, Func<Task<T>> postedNodeGetter)
            where T : D8NodeBase
        {
            var node = D8NodeMapper.Cast(objectToPost, Credentials.ApiBaseURL);
            //node.Remove("field_package");

            //todo: pass D8 SavedIDsCopier to PostTilOK
            return PostTilOK<T>(node, "entity/node?_format=hal_json", postedNodeGetter, null);
        }


        protected async Task<int> PostFile(string fileName, long fileSize, string base64Content)
        {
            var mapd = D8FileMapper.Cast(fileName, fileSize, base64Content, Credentials.ApiBaseURL);

            var dict = await PostTilOK<object>(mapd, "file?_format=hal_json", null, null);
            //var dict = await PostTilOK<object>(mapd, "entity/file?_format=hal_json", null, null);
            //var dict = await PostTilOK<object>(mapd, "api1/file", null, null);

            return 0;
        }


        protected override async Task<Dictionary<string, object>> Update<T>(T node, string revisionLog = null)
        {
            await Task.Delay(1);
            return null;
        }


        protected override async Task<bool> Delete(int nodeID)
        {
            await Task.Delay(1);
            return false;
        }


        protected async Task<List<T>> ViewsList<T>(params object[] args)
            where T: ID8ViewsList, new()
        {
            var url    = Credentials.ApiBaseURL.Slash(new T().ResourceURL);
            var filtrs = string.Join("/", args);

            var list = await GetTilOK<List<T>>(url.Slash(filtrs));

            if (list == null)
            {
                Warn($"ViewsList ‹{typeof(T)?.Name}› returned NULL.");
                return null;
            }

            //foreach (var item in list)
            //    DecodeHtmlInStrings(item);

            return list;
        }
    }
}
