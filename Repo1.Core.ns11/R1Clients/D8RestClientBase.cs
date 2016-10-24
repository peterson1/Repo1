using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal8Tools;
using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.Core.ns11.R1Clients
{
    public abstract class D8RestClientBase : RestClientBase
    {
        public D8RestClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        protected override async Task<Dictionary<string, object>> Create<T>(T objectToPost, Func<Task<T>> postedNodeGetter, bool isPublished = true)
        {
            await Task.Delay(1);
            return null;
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
            var url = Credentials.ApiBaseURL.Slash(new T().ResourceURL);

            for (int i = 0; i < args.Length; i++)
                url += $"&args[{i}]={args[i]}";

            var list = await GetTilOK<List<T>>(url);
            //var list = await GetAsync<List<T>>(url);

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
