using Repo1.Core.ns11.Drupal7Tools;

namespace Repo1.Core.ns11.R1Models
{
    [D7Type(Key = "executable")]
    public class R1Executable : D7NodeBase
    {
        [NodeTitle]                        public string  FileName        { get; set; }
        [Value(Key = "field_filesize"   )] public long    FileSize        { get; set; }
        [Value(Key = "field_filehash"   )] public string  FileHash        { get; set; }
        [Value(Key = "field_fileversion")] public string  FileVersion     { get; set; }

        public string  FullPathOrURL   { get; set; }
    }
}
