using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class DownloadablesForUserDTO : R1SplitPart, IViewsListDTO
    {
        public string ViewsDisplayURL => "downloadables_for_current_user?display_id=page";
    }
}
