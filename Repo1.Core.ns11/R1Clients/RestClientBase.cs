﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Polly;
using Repo1.Core.ns11.Configuration;
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

        protected abstract void OnError(Exception ex);
        //public Action<Exception>  OnError    { get; set; }
        public Action<string>     OnWarning  { get; set; }

        protected abstract Task<T>  GetAsync    <T>(string resourceUrl);
        protected abstract Task<T>  PostAsync   <T>(T objToPost, string resourceUrl);
        protected abstract Task<T>  PutAsync    <T>(T objToPost, string resourceUrl);
        protected abstract Task<T>  DeleteAsync <T>(string resourceUrl);

        protected abstract HttpStatusCode? GetStatusCode<T>(T exception);
        protected abstract void DecodeHtmlInStrings<T>(T obj);

        protected abstract Task<Dictionary<string, object>>  Create  <T>(T objectToPost, Func<Task<T>> postedNodeGetter, bool isPublished);
        protected abstract Task<Dictionary<string, object>>  Update  <T>(T node, string revisionLog);
        protected abstract Task<bool>                        Delete  (int nodeID);

        protected abstract Task<List<T>> ViewsList<T>(params object[] args) where T : IR1ViewsListDTO, new();



        protected Task<T> GetTilOK<T>(string resourceURL)
        {
            var busyMsg = Status;
            return KeepTrying(() => 
            {
                Status = busyMsg;
                return GetAsync<T>(resourceURL);
            });
        }


        protected async Task<T> KeepTrying<T>(Func<Task<T>> action)
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
