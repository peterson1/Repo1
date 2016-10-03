using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo1.Core.ns11.R1Models.ViewsLists
{
    public class DownloadablesForUserDTO : R1SplitPart, IR1ViewsListDTO
    {
        public string ViewsDisplayURL => "downloadables_for_current_user?display_id=page";
    }
}
