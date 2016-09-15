using System;
using Repo1.Core.ns12.Helpers.D7MapperAttributes;

namespace Repo1.Core.ns12.Models
{
    public class R1Ping
    {
        [NodeTitle]                                public string        Title              { get; set; }
        [Node (Key = "field_userlicense"       )]  public R1UserLicense UserLicense        { get; set; }
        [Value(Key = "field_publicip"          )]  public string        PublicIP           { get; set; }
        [Value(Key = "field_privateip"         )]  public string        PrivateIP          { get; set; }
        [Value(Key = "field_installedversion"  )]  public string        InstalledVersion   { get; set; }
        [Value(Key = "field_lastactdescription")]  public string        LastActDescription { get; set; }
        [Value(Key = "field_lastacttimestamp‎ " )]  public DateTime      LastActTimestamp   { get; set; }

        public int nid { get; set; }
    }
}
