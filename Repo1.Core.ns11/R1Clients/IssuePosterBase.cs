using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.R1Models;

namespace Repo1.Core.ns11.R1Clients
{
    public abstract class IssuePosterBase : RestClientBase, IIssuePosterClient
    {
        public IssuePosterBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
        }


        public async Task PostError(Exception ex, string errorCaughtBy)
        {
            var issue = CastError(ex, errorCaughtBy);
            AddMachineProfile(issue);
            await Create(issue, GetDuplicateNode);
        }


        private R1Issue CastError(Exception ex, string errorCaughtBy)
        {
            var issue         = new R1Issue();
            issue.Title       = ex?.Message ?? "Exception is NULL";
            issue.Description = ex?.Info(true, true);
            issue.Category    = IssueCategories.RuntimeError;
            issue.Status      = IssueStates.NeedsReview;
            issue.Priority    = IssuePriorities.High;
            return issue;
        }


        private void AddMachineProfile(R1Issue issue)
        {
            throw new NotImplementedException();
        }


        private Task<R1Issue> GetDuplicateNode()
        {
            throw new NotImplementedException();
        }
    }
}
