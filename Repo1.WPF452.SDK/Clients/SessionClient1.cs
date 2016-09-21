using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Models;

namespace Repo1.WPF452.SDK.Clients
{
    class SessionClient1 : SessionClientBase
    {
        public SessionClient1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }

        protected override Task<T> Get<T>(string resourceUrl)
        {
            throw new NotImplementedException();
        }

        protected override string GetComputerName()
        {
            throw new NotImplementedException();
        }

        protected override string GetExePath()
        {
            throw new NotImplementedException();
        }

        protected override string GetExeVersion()
        {
            throw new NotImplementedException();
        }

        protected override string GetLegacyCfgJson()
        {
            throw new NotImplementedException();
        }

        protected override string GetMacAndPrivateIPs()
        {
            throw new NotImplementedException();
        }

        protected override string GetRepo1CfgJson()
        {
            throw new NotImplementedException();
        }

        protected override HttpStatusCode? GetStatusCode<T>(T exception)
        {
            throw new NotImplementedException();
        }

        protected override string GetWindowsAccount()
        {
            throw new NotImplementedException();
        }

        protected override string GetWorkgroup()
        {
            throw new NotImplementedException();
        }

        protected override void OnError(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override Task<T> Post<T>(T objToPost, string resourceUrl)
        {
            throw new NotImplementedException();
        }

        protected override Task<T> Put<T>(T objToPost, string resourceUrl)
        {
            throw new NotImplementedException();
        }
    }
}
