using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;

namespace Repo1.WPF452.SDK.Clients
{
    internal class PingClient1 : SvcStackRestClient, IPingClient
    {
        private bool _isPinging;

        public PingClient1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        public async Task<R1Executable> SendAndGetLatestVersion(R1Ping pingNode)
        {
            var dict = await Update(pingNode);
            if (dict == null) return null;
            return new R1Executable
            {
                FileVersion = dict.FieldValue("field_fileversion"),
                FileHash    = dict.FieldValue("field_filehash"),
            };
        }


        public async Task StartPingOnlyLoop(R1Ping pingNode, int intervalMins)
        {
            if (_isPinging) return;
            _isPinging = true;

            while (true)
            {
                await Update(pingNode);
                await Task.Delay(1000 * intervalMins * 60);
            }
        }


        //private string GetFieldValue(string fieldNme, Dictionary<string, object> dict)
        //{
        //    if (!dict.ContainsKey(fieldNme))
        //        OnError(new MissingMemberException($"Not in returned dict: “{fieldNme}”"));

        //    var jsonStr = dict[fieldNme].ToString();

        //    return jsonStr.Between("{und:[{value:", "}]}");
        //}
    }
}
