using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class UploadablesForUserDTO : R1Executable, IViewsListDTO
    {
        public string ViewsDisplayURL => "uploadables_for_current_user?display_id=page";

        public int uid { get; set; }
    }
}
