using System;
using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "ping")]
    public class R1Ping : R1MachineSpecsBase
    {
        [NodeTitle]                                public string        Title              { get; set; }
        [Node (Key = "field_userlicense"       )]  public R1UserLicense UserLicense        { get; set; }
        [Value(Key = "field_privateip"         )]  public string        PrivateIP          { get; set; }
        [Value(Key = "field_installedversion"  )]  public string        InstalledVersion   { get; set; }
        [Value(Key = "field_lastactdescription")]  public string        LastActDescription { get; set; }
        [Value(Key = "field_lastacttimestamp"  )]  public DateTime      LastActTimestamp   { get; set; }
        [Value(Key = "field_expectedcfg"       )]  public string        ExpectedCfg        { get; set; }

        public string RegisteredMacAddress { get; set; }
        //public string  LatestExeVersion  { get; set; } do not place this here -- it'll confuse you
    }
}
