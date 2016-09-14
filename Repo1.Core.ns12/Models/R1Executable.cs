using Repo1.Core.ns12.Helpers.D7MapperAttributes;

namespace Repo1.Core.ns12.Models
{
    [D7Type(Key = "executable")]
    public class R1Executable
    {
        [NodeTitle]                           public string  FileName        { get; set; }
        [Value(Key = "field_filesize"      )] public long    FileSize        { get; set; }
        [Value(Key = "field_filehash"      )] public string  FileHash        { get; set; }
        [Value(Key = "field_fileversion"   )] public string  FileVersion     { get; set; }
        [Value(Key = "field_versionchanges")] public string  VersionChanges  { get; set; }

        public int     nid             { get; set; }
        public string  FullPathOrURL   { get; set; }
    }
}
