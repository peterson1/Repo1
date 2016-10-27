using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal8Tools;
using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.Core.ns11.R1Clients
{
    public abstract class D8RestClientBase : RestClientBase
    {
        private string _csrfToken;

        public D8RestClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }

        protected abstract Task<bool> EnableWriteMethods(string resourceUrl);


        protected async Task<int> UploadFile(string fileName, string base64Content)
        {
            var fileEntty = D8FileMapper.Cast(fileName, base64Content, Credentials.ApiBaseURL);
            await Task.Delay(1);
            return 0;
        }



        public Task<bool> RequestWriteAccess()
            => KeepTrying(async () =>
            {
                var ok = await EnableWriteMethods("api1/user/login");
                Credentials.WasRejected = !ok;
                return ok;
            });


        protected async Task<Dictionary<string, object>> Create<T>(T objectToPost, Func<Task<T>> postedNodeGetter)
            where T : D8NodeBase
        {
            var node = D8NodeMapper.Cast(objectToPost, Credentials.ApiBaseURL);
            node.Remove("field_package");

            var dict = await KeepTrying(async () =>
            {
                Dictionary<string, object> resp = null;
                try
                {
                    resp = await PostAsync(node, "entity/node?_format=hal_json");
                }
                catch (Exception ex)
                {
                    var savd = await postedNodeGetter.Invoke();
                    if (savd == null) throw ex;
                    if (resp == null)
                    {
                        //resp = D7Mapper.CopyNodeIDs(savd);
                        throw new NotImplementedException();
                    }
                }
                return resp;
            });

            if (dict == null)
                OnError(new ArgumentNullException("Wasn't expecting Dictionary<string, object> from POST to be NULL."));

            //dict.SetNodeIDs(objectToPost);

            return dict;
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
