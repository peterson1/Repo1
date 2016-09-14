using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Polly;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Helpers.D7MapperAttributes;
using Repo1.Core.ns12.Helpers.PropertyChangedExtensions;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{

    public abstract class RestClientBase : IRestClient
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        protected RestServerCredentials _cfg;
        protected int                   _uid;



        public RestClientBase(RestServerCredentials restServerCredentials)
        {
            _cfg = restServerCredentials;
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; PropertyChanged.Raise(nameof(IsBusy), this); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; PropertyChanged.Raise(nameof(Status), this); }
        }


        protected abstract Task<T>          Get<T>            (string resourceUrl);
        protected abstract Task<T>          Post<T>           (T objToPost, string resourceUrl);
        protected abstract HttpStatusCode?  GetStatusCode<T>  (T exception);
        protected abstract void             OnError           (Exception ex);


        protected async Task<T> Add <T>(T objectToPost, bool isPublished = true)
        {
            var mappd = D7Mapper.ToObjectDictionary(objectToPost);
            if (mappd == null) return default(T);
            mappd["uid"]    = _uid;
            mappd["status"] = isPublished ? 1 : 0;

            //await Task.Delay(1000 * 10);

            var saved = await Post(mappd, "entity_node");
            if (saved == null) return default(T);

            TrySetServerGeneratedValues(objectToPost, saved);

            return objectToPost;
        }


        private void TrySetServerGeneratedValues<T>(T objectToPost, Dictionary<string, object> saved)
        {
            object nid;
            if (!saved.TryGetValue("nid", out nid)) return;
            var prop = typeof(T).GetRuntimeProperties().SingleOrDefault(x => x.Name == "nid");
            if (prop != null) prop.SetValue(objectToPost, int.Parse(nid.ToString()));
        }


        protected Task<List<T>> ViewsList<T>(params object[] args)
            where T : IViewsListDTO, new()
        {
            var displayID = new T().ViewsDisplayURL;
            var url = _cfg.ApiBaseURL.Slash("views").Slash(displayID);

            for (int i = 0; i < args.Length; i++)
                url += $"&args[{i}]={args[i]}";

            return PersistentGet<List<T>>(url);
        }


        private async Task<T> PersistentGet<T>(string resourceURL)
        {
            var policy = Policy.Handle<Exception>(x => IsRetryable(x))
                .WaitAndRetryForeverAsync(attempts
                => Delay(attempts), (ex, span) => OnRetry(ex, span));

            IsBusy = true;
            T result = default(T);
            try
            {
                result = await policy.ExecuteAsync(() => Get<T>(resourceURL));
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                IsBusy = false;
            }
            return result;
        }


        private TimeSpan Delay(int retryAttempt)
            => TimeSpan.FromSeconds(Math.Max(5, retryAttempt));


        protected virtual void OnRetry(Exception exception, TimeSpan timespan)
        {
        }


        private bool IsRetryable<T>(T exception)
        {
            var code = GetStatusCode(exception);
            if (!code.HasValue) return false;

            switch (code)
            {
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.Gone:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.Moved:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Redirect:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.TemporaryRedirect:
                    return true;

                default:
                    return false;
            }
        }

    }
}
