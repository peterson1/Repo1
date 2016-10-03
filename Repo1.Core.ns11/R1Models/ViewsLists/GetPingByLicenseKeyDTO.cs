namespace Repo1.Core.ns11.R1Models.ViewsLists
{
    public class GetPingByLicenseKeyDTO : R1Ping, IR1ViewsListDTO
    {
        public string ViewsDisplayURL => "pings_for_current?display_id=page";

        public int UserLicenseNid { get; set; }
    }
}
