using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Helpers.D7MapperAttributes.UndFields;
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
                FileVersion = dict.FieldValue("field_fileversion"),
                FileHash    = dict.FieldValue("field_filehash"),
            };
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
