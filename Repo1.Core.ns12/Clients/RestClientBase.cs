using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{
    public abstract class RestClientBase : IRestClient
    {
        protected RestServerCredentials _cfg;

        public RestClientBase(RestServerCredentials restServerCredentials)
        {
            _cfg = restServerCredentials;
        }

        public abstract Task<T> Get<T>(string resourceUrl);

        protected Task<List<T>> ViewsList<T>(string displayID, params object[] args)
        {
            var url = _cfg.ApiBaseURL.Slash("views").Slash(displayID);

            for (int i = 0; i < args.Length; i++)
                url += $"&args[{i}]={args[i]}";

            return PersistentGet<List<T>>(url);
        }


        private Task<T> PersistentGet<T>(string resourceURL)
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryForever(attempts
                => Delay(attempts), (ex, span) => OnRetry(ex, span));

            return policy.Execute(() => Get<T>(resourceURL));
        }


        private TimeSpan Delay(int retryAttempt)
            => TimeSpan.FromSeconds(Math.Max(5, retryAttempt));


        protected virtual void OnRetry(Exception exception, TimeSpan timespan)
        {
        }
    }
}
