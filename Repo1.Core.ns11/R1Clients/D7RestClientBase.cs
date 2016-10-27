using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal7Tools;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Models.ViewsLists;

namespace Repo1.Core.ns11.R1Clients
{
    public abstract class D7RestClientBase : RestClientBase
    {
        public D7RestClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        protected async Task<Dictionary<string, object>> Create<T>(T objectToPost, Func<Task<T>> postedNodeGetter, bool isPublished = true)
        {
            var mappd = D7Mapper.ToObjectDictionary(objectToPost);
            if (mappd == null) return null;
            mappd["status"] = isPublished ? 1 : 0;

            var dict = await KeepTrying(async () =>
            {
                Dictionary<string, object> resp = null;
                try
                {
                    resp = await PostAsync(mappd, "entity_node");
                }
                catch (Exception ex)
                {
                    var savd = await postedNodeGetter.Invoke();
                    if (savd == null) throw ex;
                    if (resp == null)
                        resp = D7Mapper.CopyNodeIDs(savd);
                }
                return resp;
            });

            if (dict == null)
                OnError(new ArgumentNullException("Wasn't expecting Dictionary<string, object> from POST to be NULL."));

            dict.SetNodeIDs(objectToPost);

            return dict;
        }


        protected override async Task<Dictionary<string, object>> Update<T>(T node, string revisionLog = null)
        {
            var mappd = D7Mapper.ToObjectDictionary(node);
            if (mappd == null) return null;

            if (!mappd.ContainsKey("nid")) return null;
            int nid;
            if (!int.TryParse(mappd["nid"].ToString(), out nid)) return null;
            if (nid < 1) return null;

            if (!revisionLog.IsBlank())
            {
                mappd.Add("revision", 1);
                mappd.Add("log", revisionLog);
            }

            var dict = await KeepTrying(() => PutAsync(mappd, $"entity_node/{nid}"));

            if (dict == null)
                OnError(new ArgumentNullException("Wasn't expecting Dictionary<string, object> from PUT to be NULL."));

            dict.SetNodeIDs(node);

            return dict;
        }


        protected override async Task<bool> Delete(int nodeID)
        {
            await KeepTrying(() => DeleteAsync<string>($"entity_node/{nodeID}"));
            return true;
        }


        protected async Task<List<T>> ViewsList<T>(params object[] args)
            where T : IR1ViewsListDTO, new()
        {
            var displayID = new T().ViewsDisplayURL;
            var url = Credentials.ApiBaseURL.Slash("views").Slash(displayID);

            for (int i = 0; i < args.Length; i++)
                url += $"&args[{i}]={args[i]}";

            var list = await GetTilOK<List<T>>(url);

            if (list == null)
            {
                Warn($"ViewsList ‹{typeof(T)?.Name}› returned NULL.");
                return null;
            }

            foreach (var item in list)
                DecodeHtmlInStrings(item);

            return list;
        }
    }
}
