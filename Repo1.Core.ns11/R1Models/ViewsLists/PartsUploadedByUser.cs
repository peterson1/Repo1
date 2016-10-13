using System;

namespace Repo1.Core.ns11.R1Models.ViewsLists
{
    public class PartsUploadedByUser : R1SplitPart, IR1ViewsListDTO
    {
        public string ViewsDisplayURL => "uploadables_for_current_user?display_id=page_1";

        public int       ExecutableNid  { get; set; }
        public DateTime  PostDate       { get; set; }
    }
}
