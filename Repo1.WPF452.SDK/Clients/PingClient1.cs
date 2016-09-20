using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.Core.ns12.Models;

namespace Repo1.WPF452.SDK.Clients
{
    internal class PingClient1 : SvcStackRestClient, IPingClient
    {
        public PingClient1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        public async Task<R1Executable> SendAndGetLatestVersion(R1Ping pingNode)
        {
            var dict = await Update(pingNode);
            if (dict == null) return null;
            return new R1Executable
            {
                FileVersion = GetFieldValue("field_fileversion", dict),
                FileHash    = GetFieldValue("field_filehash"   , dict),
            };
        }


        private string GetFieldValue(string fieldNme, Dictionary<string, object> dict)
        {
            if (!dict.ContainsKey(fieldNme))
                OnError(new MissingMemberException($"Not in returned dict: “{fieldNme}”"));

            var jsonStr = dict[fieldNme].ToString();

            return jsonStr.Between("{und:[{value:", "}]}");
        }
    }
}
