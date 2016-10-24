using Repo1.Core.ns11.Drupal8Tools;

namespace Repo1.Core.ns11.R1Models.D8Models
{
    public class D8PackagePart : D8NodeBase
    {
        public D8Package  Package         { get; set; }
        public string     PackageVersion  { get; set; }
        public string     PartHash        { get; set; }
        public int        PartNumber      { get; set; }
        public int        TotalParts      { get; set; }

        public string     FullPathOrURL   { get; set; }
    }
}
