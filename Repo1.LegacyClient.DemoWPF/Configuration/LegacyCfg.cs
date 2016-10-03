using System.IO;
using Repo1.WPF45.SDK.Serialization;
using static System.Environment;

namespace Repo1.LegacyClient.DemoWPF.Configuration
{
    class LegacyCfg
    {
        public string  Username      { get; set; }
        public string  Password      { get; set; }
        public string  UniqueCfgKey  { get; set; }
        public string  LegacyValue1  { get; set; }
        public string  LegacyValue2  { get; set; }


        internal static LegacyCfg ReadAndParse()
            => Json.Deserialize<LegacyCfg>(Read());


        internal static string Read()
        {
            var env = GetFolderPath(SpecialFolder.LocalApplicationData);
            var prj = typeof(LegacyCfg).Assembly.GetName().Name;
            var pth = Path.Combine(env, prj, "legacyCfg.json");
            return File.ReadAllText(pth);
        }
    }
}
