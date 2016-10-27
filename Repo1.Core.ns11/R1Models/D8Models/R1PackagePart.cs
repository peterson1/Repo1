using Repo1.Core.ns11.Drupal8Tools;

namespace Repo1.Core.ns11.R1Models.D8Models
{
    public class R1PackagePart : D8NodeBase
    {
        public override string D8TypeName => "package_part";

        [ContentTitle       ] public string     Title           => $"{Package?.FileName} v{PackageVersion} : {PartNumber}/{TotalParts}";
        [_("package"       )] public R1Package  Package         { get; set; }
        [_("latest_version")] public string     PackageVersion  { get; set; }
        [_("part_file_hash")] public string     PartHash        { get; set; }
        [_("part_number"   )] public int        PartNumber      { get; set; }
        [_("total_parts"   )] public int        TotalParts      { get; set; }

        public string   FullPathOrURL   { get; set; }
    }
}
