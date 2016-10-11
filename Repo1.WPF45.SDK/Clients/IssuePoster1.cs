using System;
using System.Linq;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.Extensions.StringExtensions;
using Repo1.Core.ns11.R1Clients;
using Repo1.Core.ns11.R1Models;
using Repo1.Core.ns11.R1Models.ViewsLists;
using Repo1.WPF45.SDK.Serialization;

namespace Repo1.WPF45.SDK.Clients
{
    public class IssuePoster1 : SvcStackRestClient, IIssuePosterClient
    {
        private MachineProfiler1 _specs;
        private string           _lastHash;

        public IssuePoster1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            _specs = new MachineProfiler1(_creds);
        }


        public async Task PostError(string errorMessage,
            string configKey, Func<string> readLegacyCfg = null)
        {
            var issue = CastError(errorMessage);
            await _specs.AddProfileTo(issue, readLegacyCfg, configKey);
            _lastHash = issue.RecordHash = Json.Serialize(issue).SHA1ForUTF8();

            await Create(issue, LastPostedIssue);
        }


        public async Task<R1Issue> LastPostedIssue()
        {
            var list = await ViewsList<IssuesForUserByHashDTO>(_lastHash);
            return list?.FirstOrDefault() as R1Issue;
        }


        private R1Issue CastError(string errorMessage)
        {
            var issue         = new R1Issue();
            issue.Title       = errorMessage.Split(L.f.ToCharArray())[0];
            issue.Description = errorMessage;
            issue.Category    = IssueCategories.RuntimeError;
            issue.Status      = IssueStates.NeedsReview;
            issue.Priority    = IssuePriorities.High;
            return issue;
        }
    }
}
