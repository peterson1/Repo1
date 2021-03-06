﻿using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "split_part")]
    public class R1SplitPart : D7NodeBase
    {
        [NodeTitle]                           public string        FileName       { get; set; }
        [Node (Key = "field_executable"   )]  public R1Executable  Executable     { get; set; }
        [Value(Key = "field_fileversion"  )]  public string        ExeVersion     { get; set; }
        [Value(Key = "field_filehash"     )]  public string        PartHash       { get; set; }
        [Value(Key = "field_base64content")]  public string        Base64Content  { get; set; }
        [Value(Key = "field_partnumber"   )]  public int           PartNumber     { get; set; }
        [Value(Key = "field_totalparts"   )]  public int           TotalParts     { get; set; }

        public string   FullPathOrURL   { get; set; }
    }
}
