using System;
using Repo1.Core.ns12.Helpers.D7MapperAttributes;

namespace Repo1.Core.ns12.Models
{
    public class R1UserSession
    {
        [NodeTitle]                         public string    Description       { get; set; }
        [Value(Key = "field_publicip"   )]  public string    PublicIP          { get; set; }
        [Value(Key = "field_fileversion")]  public string    ExeVersion        { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    ExePath           { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    SessionKey        { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    MacAndPrivateIPs  { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    WindowsAccount    { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    ComputerName      { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    Workgroup         { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    LegacyCfgJson     { get; set; }
        [Value(Key = "field_xxxxxxxxxx" )]  public string    Repo1CfgJson      { get; set; }

        [Value(Key = "field_lastactdescription")]  public string     LastActDescription  { get; set; }
        [Value(Key = "field_lastacttimestamp‎ " )]  public DateTime   LastActTimestamp    { get; set; }

        public int   nid   { get; set; }
        public int   uid   { get; set; }
    }
}
