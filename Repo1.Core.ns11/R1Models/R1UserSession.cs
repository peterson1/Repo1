using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "user_session")]
    public class R1UserSession
    {
        [NodeTitle]                              public string    Description       { get; set; }
        [Value(Key = "field_publicip"        )]  public string    PublicIP          { get; set; }
        [Value(Key = "field_fileversion"     )]  public string    ExeVersion        { get; set; }
        [Value(Key = "field_filepath"        )]  public string    ExePath           { get; set; }
        [Value(Key = "field_uniquekey"       )]  public string    SessionKey        { get; set; }
        [Value(Key = "field_licensekey"      )]  public string    FutureLicenseKey  { get; set; }
        [Value(Key = "field_macandprivateips")]  public string    MacAndPrivateIPs  { get; set; }
        [Value(Key = "field_windowsaccount"  )]  public string    WindowsAccount    { get; set; }
        [Value(Key = "field_computername"    )]  public string    ComputerName      { get; set; }
        [Value(Key = "field_workgroup"       )]  public string    Workgroup         { get; set; }
        [Value(Key = "field_legacycfgjson"   )]  public string    LegacyCfgJson     { get; set; }
        [Value(Key = "field_repo1cfgjson"    )]  public string    Repo1CfgJson      { get; set; }
        [Value(Key = "field_expectedcfg"     )]  public string    ExpectedCfg       { get; set; }


        public int   nid   { get; set; }
        public int   uid   { get; set; }
        public int   vid   { get; set; }
    }
}
