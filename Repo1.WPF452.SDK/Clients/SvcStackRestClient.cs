using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Helpers.ExceptionExtensions;
using Repo1.Core.ns12.Models;
using ServiceStack;

namespace Repo1.WPF452.SDK.Clients
{
    [ImplementPropertyChanged]
    public class SvcStackRestClient : RestClientBase
    {
        public SvcStackRestClient(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        public override Task<T> Get<T>(string resourceUrl)
            => CreateClient().GetAsync<T>(resourceUrl);



        protected override HttpStatusCode? GetStatusCode<T>(T ex)
            => GetStatus(ex as WebException)
            ?? (ex as WebServiceException)?.GetStatus();



        private HttpStatusCode? GetStatus(WebException wx)
        {
            if (wx == null) return null;
            switch (wx.Status)
            {
                case WebExceptionStatus.NameResolutionFailure:
                case WebExceptionStatus.ConnectFailure:
                case WebExceptionStatus.ReceiveFailure:
                case WebExceptionStatus.SendFailure:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.RequestCanceled:
                case WebExceptionStatus.ProtocolError:
                case WebExceptionStatus.ConnectionClosed:
                case WebExceptionStatus.TrustFailure:
                case WebExceptionStatus.SecureChannelFailure:
                case WebExceptionStatus.ServerProtocolViolation:
                case WebExceptionStatus.KeepAliveFailure:
                case WebExceptionStatus.Pending:
                case WebExceptionStatus.Timeout:
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    return HttpStatusCode.ServiceUnavailable;
            }
            return (wx.Response as HttpWebResponse)?.StatusCode;
        }


        protected override void OnError(Exception ex) 
            => MessageBox.Show(ex.Info());


        private JsonServiceClient CreateClient()
            => new JsonServiceClient(_cfg.ApiBaseURL)
            {
                UserName = _cfg.Username,
                Password = _cfg.Password,
                AlwaysSendBasicAuthHeader = true,
            };
    }
}
