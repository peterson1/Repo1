namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class SplitPartContentByHashDTO : IViewsListDTO
    {
        public string ViewsDisplayURL => "split_part_views?display_id=page";


        public string   Base64Content   { get; set; }
    }
}
