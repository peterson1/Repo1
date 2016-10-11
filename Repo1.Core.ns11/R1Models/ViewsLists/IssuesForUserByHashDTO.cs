namespace Repo1.Core.ns11.R1Models.ViewsLists
{
    public class IssuesForUserByHashDTO : R1Issue, IR1ViewsListDTO
    {
        public string ViewsDisplayURL => "issues_for_current?display_id=page";
    }
}
