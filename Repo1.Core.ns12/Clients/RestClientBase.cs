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


        protected RestServerCredentials _creds;



        public RestClientBase(RestServerCredentials restServerCredentials)
        {
            _creds = restServerCredentials;
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

        public Action<string>    OnWarning    { get; set; }

        protected abstract Task<T>          Get  <T>          (string resourceUrl);
        protected abstract Task<T>          Post <T>          (T objToPost, string resourceUrl);
        protected abstract Task<T>          Put  <T>          (T objToPost, string resourceUrl);
        protected abstract HttpStatusCode?  GetStatusCode<T>  (T exception);
        protected abstract void             OnError           (Exception ex);


        protected Task<Dictionary<string, object>> Create <T>(T objectToPost, bool isPublished = true)
        {
            var mappd = D7Mapper.ToObjectDictionary(objectToPost);
            if (mappd == null) return null;
            mappd["status"] = isPublished ? 1 : 0;

            return KeepTrying(() => Post(mappd, "entity_node"));
        }


        protected T Warn<T>(string message, T returnVal = default(T))
        {
            OnWarning?.Invoke(message);
            return returnVal;
        }


        protected async Task<Dictionary<string, object>> Update<T>(T node, string revisionLog = null)
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

            var dict = await KeepTrying(() => Put(mappd, $"entity_node/{nid}"));

            if (dict == null)
                OnError(new ArgumentNullException("Wasn't expecting Dictionary<string, object> from PUT  to be NULL."));

            return dict;
        }



        private void TrySetServerGeneratedValues<T>(T objectToPost, Dictionary<string, object> saved)
        {
            object nid;
            if (!saved.TryGetValue("nid", out nid)) return;
            var prop = typeof(T).GetRuntimeProperties().SingleOrDefault(x => x.Name == "nid");
            if (prop != null) prop.SetValue(objectToPost, int.Parse(nid.ToString()));
        }


        protected async Task<List<T>> ViewsList<T>(params object[] args)
            where T : IViewsListDTO, new()
        {
            var displayID = new T().ViewsDisplayURL;
            var url = _creds.ApiBaseURL.Slash("views").Slash(displayID);

            for (int i = 0; i < args.Length; i++)
                url += $"&args[{i}]={args[i]}";

            //var list = await KeepTrying(() => Get<List<T>>(url));
            var list = await GetTilOK<List<T>>(url);

            if (list == null)
                OnError(new ArgumentNullException("Wasn't expecting ViewsList to return NULL."));

            return list;
        }


        protected Task<T> GetTilOK<T>(string resourceURL) 
            => KeepTrying(() => Get<T>(resourceURL));


        private async Task<T>  KeepTrying  <T>(Func<Task<T>> action)
        {
            var policy = Policy.Handle<Exception>(x => IsRetryable(x))
                .WaitAndRetryForeverAsync(attempts
                => Delay(attempts), (ex, span) => OnRetry(ex, span));

            IsBusy = true;
            T result = default(T);
            try
            {
                result = await policy.ExecuteAsync(action);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("got string starting with: Invalid credentials"))
                    _creds.WasRejected = true;

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
            Status = exception.Message + L.F 
                   + $"Retrying in {timespan.TotalSeconds} seconds ...";
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


        public virtual void RaisePropertyChanged(string propertyName)
            => PropertyChanged.Raise(propertyName);
    }
}
