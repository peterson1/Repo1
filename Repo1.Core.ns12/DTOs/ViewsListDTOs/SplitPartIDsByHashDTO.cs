using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class SplitPartIDsByHashDTO : R1SplitPart, IViewsListDTO
    {
        public string ViewsDisplayURL => "split_part_views?display_id=page_1";
    }
}
