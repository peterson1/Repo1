namespace Repo1.Core.ns11.R1Models.ViewsLists
{
    public class UserSessionsByKeyDTO : R1UserSession, IR1ViewsListDTO
    {
        public string ViewsDisplayURL => "user_session_views?display_id=page";
    }
}
