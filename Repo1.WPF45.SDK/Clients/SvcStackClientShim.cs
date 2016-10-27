using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Configuration;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal8Tools;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.WPF45.SDK.Cryptographers;
using Repo1.WPF45.SDK.Serialization;
using ServiceStack;
using ServiceStack.Text;

namespace Repo1.WPF45.SDK.Clients
{
    public class SvcStackClientShim
    {
        //const string REPO1_NFS_CERT = "68BD712DFC7529ED73D2E5E3F1A4EB5DFBA50164";

        private RestServerCredentials _creds;
        private D8Cookie              _cookie;
        private string                _csrfToken;

        public SvcStackClientShim(RestServerCredentials restServerCredentials)
        {
            _creds = restServerCredentials;
            JsConfig.ExcludeTypeInfo = true;

            //Certificator.AllowFrom(REPO1_NFS_CERT);
        }


        public Task<T> GetAsync<T>(string resourceUrl)
            => BasicAuthClient().GetAsync<T>(resourceUrl);


        public Task<T> DeleteAsync<T>(string resourceUrl)
            => BasicAuthClient().DeleteAsync<T>(resourceUrl);


        public Task<T> BasicAuthPOST<T>(T objToPost, string resourceUrl)
            => BasicAuthClient().PostAsync<T>(resourceUrl, objToPost);


        public async Task<T> BasicAuthHALJsonPOST<T>(T objToPost, string resourceUrl)
        {
            var url  = _creds.ApiBaseURL.Slash(resourceUrl);
            var json = Json.Serialize(objToPost);
            var resp = await url.PostStringToUrlAsync(json, 
                "application/hal+json", "application/hal+json", r =>
                {
                    r.AddBasicAuth(_creds.Username, _creds.Password);
                    r.Headers["X-CSRF-Token"] = _csrfToken;
                });
            return Json.Deserialize<T>(resp);
        }


        public Task<T> CookieAuthPOST<T>(T objToPost, string resourceUrl)
        {
            var json = Json.Serialize(objToPost);
            return CookieAuthClient().PostAsync<T>(resourceUrl, objToPost);
            //var url = _creds.ApiBaseURL.Slash(resourceUrl);
            //var uri = new Uri(_creds.ApiBaseURL);

            //var res = await url.PostJsonToUrlAsync(objToPost, r =>
            //    {
            //        r.Accept = "application/hal+json";
            //        r.CookieContainer = new CookieContainer();
            //        r.CookieContainer.Add(new Cookie(_cookie.Name, _cookie.Id, resourceUrl, "NULL"));
            //    });
            //return Json.Deserialize<T>(res);
        }


        public Task<T> PutAsync<T>(T objToPut, string resourceUrl)
            => BasicAuthClient().PutAsync<T>(resourceUrl, objToPut);


        public HttpStatusCode? GetStatusCode<T>(T ex)
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


        internal async Task<bool> GetCsrfToken(string resourceUrl)
        {
            var client = new JsonServiceClient(_creds.ApiBaseURL);
            var body   = new { username = _creds.Username, password = _creds.Password };
            _cookie    = null;
            var json   = await client.PostAsync<string>(resourceUrl, body);
            if (!Json.TryDeserialize<D8Cookie>(json, out _cookie)) return false;

            _csrfToken = await CookieAuthClient().GetAsync<string>("rest/session/token");
            return !_csrfToken.IsBlank();
        }


        private JsonServiceClient BasicAuthClient()
            => new JsonServiceClient(_creds.ApiBaseURL)
            {
                UserName = _creds.Username,
                Password = _creds.Password,
                AlwaysSendBasicAuthHeader = true
            };


        private JsonServiceClient CookieAuthClient()
        {
            if (_cookie == null)
                throw new InvalidOperationException("POST a Session Cookie request first before performing an CookieAuth operation.");

            var client = new JsonServiceClient(_creds.ApiBaseURL);
            client.SetCookie(_cookie.Name, _cookie.Id);
            //client.SetCookie(_cookie.Id, _cookie.Name + "sdasdas");
            return client;
        }



        // from  http://stackoverflow.com/a/8523437
        // Enable/disable useUnsafeHeaderParsing.
        // See http://o2platform.wordpress.com/2010/10/20/dealing-with-the-server-committed-a-protocol-violation-sectionresponsestatusline/
        public bool ToggleAllowUnsafeHeaderParsing(bool enable)
        {
            //Get the assembly that contains the internal class
            Assembly assembly = Assembly.GetAssembly(typeof(SettingsSection));
            if (assembly != null)
            {
                //Use the assembly in order to get the internal type for the internal class
                Type settingsSectionType = assembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (settingsSectionType != null)
                {
                    //Use the internal static property to get an instance of the internal settings class.
                    //If the static instance isn't created already invoking the property will create it for us.
                    object anInstance = settingsSectionType.InvokeMember("Section",
                    BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
                    if (anInstance != null)
                    {
                        //Locate the private bool field that tells the framework if unsafe header parsing is allowed
                        FieldInfo aUseUnsafeHeaderParsing = settingsSectionType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null)
                        {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, enable);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
