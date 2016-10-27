using Repo1.Core.ns11.Drupal8Tools;

namespace Repo1.Core.ns11.R1Models.D8Models
{
    public class R1Package : D8NodeBase
    {
        public override string D8TypeName => "package";

        [ContentTitle       ] public string   FileName       { get; set; }
        [_("file_size"     )] public long     FileSize       { get; set; }
        [_("latest_version")] public string   LatestVersion  { get; set; }
        [_("latest_hash"   )] public string   LatestHash     { get; set; }

        public string   FullPathOrURL  { get; set; }
    }
}
