using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "user_session")]
    public class R1UserSession : R1MachineSpecsBase
    {
        [NodeTitle]                         public string    Description       { get; set; }
        [Value(Key = "field_uniquekey"  )]  public string    SessionKey        { get; set; }
        [Value(Key = "field_licensekey" )]  public string    FutureLicenseKey  { get; set; }
        [Value(Key = "field_expectedcfg")]  public string    ExpectedCfg       { get; set; }
    }
}
