﻿using System.Net;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.R1Clients;
using Repo1.WPF45.SDK.HtmlTools;

namespace Repo1.WPF45.SDK.Clients
{
    [ImplementPropertyChanged]
    public abstract class D7SvcStackClientBase : D7RestClientBase
    {
        private SvcStackClientShim _svcStack;

        public D7SvcStackClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            _svcStack = new SvcStackClientShim(restServerCredentials);
            _svcStack.ToggleAllowUnsafeHeaderParsing(true);
        }

        protected override Task<T>  GetAsync    <T>(             string resourceUrl) => _svcStack.GetAsync<T>(resourceUrl);
        protected override Task<T>  DeleteAsync <T>(             string resourceUrl) => _svcStack.DeleteAsync<T>(resourceUrl);
        protected override Task<T>  PostAsync   <T>(T objToPost, string resourceUrl) => _svcStack.BasicAuthPOST<T>(objToPost, resourceUrl);
        protected override Task<T>  PutAsync    <T>(T objToPut , string resourceUrl) => _svcStack.PutAsync <T>(objToPut , resourceUrl);

        protected override HttpStatusCode?  GetStatusCode       <T>(T ex)   => _svcStack  .GetStatusCode(ex);
        protected override void             DecodeHtmlInStrings <T>(T obj)  => HtmlDecoder.ReplaceStrings(obj);
    }




}
