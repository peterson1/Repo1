using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class GetPingByLicenseKeyDTO : R1Ping, IViewsListDTO
    {
        public string ViewsDisplayURL => "pings_for_current?display_id=page";

        public int UserLicenseNid { get; set; }
    }
}
