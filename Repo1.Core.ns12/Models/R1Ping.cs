using System;
using Repo1.Core.ns12.Helpers.D7MapperAttributes;

namespace Repo1.Core.ns12.Models
{
    [D7Type(Key = "ping")]
    public class R1Ping
    {
        [NodeTitle]                                public string        Title              { get; set; }
        [Node (Key = "field_userlicense"       )]  public R1UserLicense UserLicense        { get; set; }
        [Value(Key = "field_publicip"          )]  public string        PublicIP           { get; set; }
        [Value(Key = "field_privateip"         )]  public string        PrivateIP          { get; set; }
        [Value(Key = "field_installedversion"  )]  public string        InstalledVersion   { get; set; }
        [Value(Key = "field_lastactdescription")]  public string        LastActDescription { get; set; }
        [Value(Key = "field_lastacttimestamp‎ " )]  public DateTime      LastActTimestamp   { get; set; }

        public int    nid                  { get; set; }
        public int    uid                  { get; set; }
        public int    vid                  { get; set; }
        public string RegisteredMacAddress { get; set; }
        //public string  LatestExeVersion  { get; set; } do not place this here -- it'll confuse you
    }
}
