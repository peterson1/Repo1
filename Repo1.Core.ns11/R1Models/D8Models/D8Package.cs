using Repo1.Core.ns11.Drupal8Tools;

namespace Repo1.Core.ns11.R1Models.D8Models
{
    public class D8Package : D8NodeBase
    {
        public string   FileName       { get; set; }
        public int      FileSize       { get; set; }
        public string   LatestVersion  { get; set; }
        public string   LatestHash     { get; set; }
    }
}
