using Repo1.Core.ns12.Helpers.D7MapperAttributes;

namespace Repo1.Core.ns12.Models
{
    [D7Type(Key = "split_part")]
    public class R1SplitPart
    {
        [NodeTitle]                          public string   FileName      { get; set; }
        [Value(Key = "field_exenid"     )]   public int      ExeNid        { get; set; }
        [Value(Key = "field_fileversion")]   public string   ExeVersion    { get; set; }
        [Value(Key = "field_filehash"   )]   public string   PartHash      { get; set; }
        [Value(Key = "field_base64content")] public string   Base64Content { get; set; }

        public int      nid            { get; set; }
        public string   FullPathOrURL  { get; set; }
    }
}
