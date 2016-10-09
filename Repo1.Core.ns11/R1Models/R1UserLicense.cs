using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "user_license")]
    public class R1UserLicense : D7NodeBase
    {
        [NodeTitle]                           public string       Description     { get; set; }
        [User (Key = "field_downloader"   )]  public object       Downloader      { get; set; }
        [Value(Key = "field_macaddress"   )]  public string       MacAddress      { get; set; }
        [Node (Key = "field_executable"   )]  public R1Executable Executable      { get; set; }
        [Value(Key = "field_licensekey"   )]  public string       LicenseKey      { get; set; }
        [Value(Key = "field_base64content")]  public string       Base64Content   { get; set; }
    }
}
