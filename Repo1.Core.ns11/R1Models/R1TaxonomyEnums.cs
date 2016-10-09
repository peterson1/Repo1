using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [Vocabulary(Key = "issue_categories")]
    public enum IssueCategories
    {
        RuntimeError   = 1,
        BugReport      = 2,
        FeatureRequest = 3,
        SupportRequest = 4,
    }


    [Vocabulary(Key = "issue_states")]
    public enum IssueStates
    {
        NeedsReview             = 5,
        NowFixing               = 6,
        Fixed                   = 7,
        Postponed               = 8,
        Postponed_NeedsMoreInfo = 9,
        Closed_Duplicate        = 10,
    }


    [Vocabulary(Key = "issue_priorities")]
    public enum IssuePriorities
    {
        Critical = 12,
        High     = 13,
        Medium   = 14,
        Low      = 15,
    }
}
