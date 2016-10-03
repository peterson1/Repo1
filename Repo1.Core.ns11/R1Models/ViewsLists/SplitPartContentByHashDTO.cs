namespace Repo1.Core.ns11.R1Models.ViewsLists
{
    public class SplitPartContentByHashDTO : IR1ViewsListDTO
    {
        public string ViewsDisplayURL => "split_part_views?display_id=page";


        public string Base64Content { get; set; }
    }
}
