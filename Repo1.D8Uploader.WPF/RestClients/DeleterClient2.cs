using System;
using Repo1.Core.ns11.Configuration;
using Repo1.D8Uploader.Lib45.RestClients;

namespace Repo1.D8Uploader.WPF.RestClients
{
    class DeleterClient2 : DeleterClientBase
    {
        public DeleterClient2(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }

        protected override void ShowUploadedsWindow()
        {
            throw new NotImplementedException();
        }
    }
}
