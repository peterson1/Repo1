using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Polly;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal7Tools;
using Repo1.Core.ns11.EventArguments;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Models.ViewsLists;

namespace Repo1.Core.ns11.R1Clients
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

        public Action<string> OnWarning { get; set; }

        protected abstract Task<T>  Get    <T>(string resourceUrl);
        protected abstract Task<T>  Post   <T>(T objToPost, string resourceUrl);
        protected abstract Task<T>  Put    <T>(T objToPost, string resourceUrl);
        protected abstract Task<T>  Delete <T>(string resourceUrl);

        protected abstract HttpStatusCode? GetStatusCode<T>(T exception);
        protected abstract void OnError(Exception ex);
        protected abstract void DecodeHtmlInStrings<T>(T obj);


        protected async Task<Dictionary<string, object>> Create<T>(T objectToPost, Func<Task<T>> postedNodeGetter, bool isPublished = true)
        {
            var mappd = D7Mapper.ToObjectDictionary(objectToPost);
            if (mappd == null) return null;
            mappd["status"] = isPublished ? 1 : 0;

            //var dict = await Post(mappd, "entity_node");
            //todo: check if posted before retrying
            //var dict = await KeepTrying(() => Post(mappd, "entity_node"));
            var dict = await KeepTrying(async () =>
            {
                Dictionary<string, object> resp = null;
                try
                {
                    resp = await Post(mappd, "entity_node");
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
                OnError(new ArgumentNullException("Wasn't expecting Dictionary<string, object> from PUT to be NULL."));

            dict.SetNodeIDs(node);

            return dict;
        }



        protected async Task<bool>  Delete (int nodeID)
        {
            await KeepTrying(() => Delete<string>($"entity_node/{nodeID}"));
            return true;
        }


        protected async Task<List<T>> ViewsList<T>(params object[] args)
            where T : IR1ViewsListDTO, new()
        {
            var displayID = new T().ViewsDisplayURL;
            var url = _creds.ApiBaseURL.Slash("views").Slash(displayID);

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


        protected Task<T> GetTilOK<T>(string resourceURL)
            //=> KeepTrying(() => Get<T>(resourceURL));
        {
            var busyMsg = Status;
            return KeepTrying(() => 
            {
                Status = busyMsg;
                return Get<T>(resourceURL);
            });
        }


        private async Task<T> KeepTrying<T>(Func<Task<T>> action)
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

            Warn(Status);
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


        protected bool Warn(string message, bool returnVal = false)
        {
            OnWarning?.Invoke(message);
            return returnVal;
        }
    }
}
