using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class UserSessionsByKeyDTO : R1UserSession, IViewsListDTO
    {
        public string ViewsDisplayURL => "user_session_views?display_id=page";
    }
}
