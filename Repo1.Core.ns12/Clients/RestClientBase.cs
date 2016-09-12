using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Polly;
using Repo1.Core.ns12.DTOs.ViewsListDTOs;
using Repo1.Core.ns12.Helpers.PropertyChangedExtensions;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.Clients
{

    public abstract class RestClientBase : IRestClient, INotifyPropertyChanged
    {
        protected RestServerCredentials _cfg;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


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


        public abstract Task<T> Get<T>(string resourceUrl);
        protected abstract HttpStatusCode? GetStatusCode<T>(T exception);
        protected abstract void OnError(Exception ex);

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
