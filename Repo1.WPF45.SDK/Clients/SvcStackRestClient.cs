﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using PropertyChanged;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.R1Clients;
using Repo1.WPF45.SDK.HtmlTools;
using ServiceStack;
using ServiceStack.Text;

namespace Repo1.WPF45.SDK.Clients
{
    [ImplementPropertyChanged]
    public class SvcStackRestClient : RestClientBase
    {
        //internal MachineProfilingRestClient1 _specs;

        public SvcStackRestClient(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            ToggleAllowUnsafeHeaderParsing(true);
            JsConfig.ExcludeTypeInfo = true;
            //_specs = new MachineProfilingRestClient1(x => GetTilOK<Dictionary<string, string>>(x));
        }


        protected override Task<T>  Get  <T>(string resourceUrl)
            => CreateClient().GetAsync<T>(resourceUrl);


        protected override Task<T>  Delete  <T>(string resourceUrl)
            => CreateClient().DeleteAsync<T>(resourceUrl);


        protected override Task<T>  Post  <T>(T objToPost, string resourceUrl)
        {
            //var js = DynamicJson.Serialize(objToPost);
            //var js = ServiceStack.Text.JsonSerializer.SerializeToString(objToPost);
            return CreateClient().PostAsync<T>(resourceUrl, objToPost);
        }


        protected override Task<T>  Put  <T>(T objToPut, string resourceUrl)
        {
            //var js = DynamicJson.Serialize(objToPut);
            //var js = JsonSerializer.SerializeToString(objToPut);
            return CreateClient().PutAsync<T>(resourceUrl, objToPut);
        }


        protected override HttpStatusCode? GetStatusCode<T>(T ex)
            => GetStatus(ex as WebException)
            ?? (ex as WebServiceException)?.GetStatus();


        protected override void DecodeHtmlInStrings<T>(T obj)
            => HtmlDecoder.ReplaceStrings(obj);


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
            => new JsonServiceClient(_creds.ApiBaseURL)
            {
                UserName = _creds.Username,
                Password = _creds.Password,
                AlwaysSendBasicAuthHeader = true,
            };



        // from  http://stackoverflow.com/a/8523437
        // Enable/disable useUnsafeHeaderParsing.
        // See http://o2platform.wordpress.com/2010/10/20/dealing-with-the-server-committed-a-protocol-violation-sectionresponsestatusline/
        public static bool ToggleAllowUnsafeHeaderParsing(bool enable)
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
