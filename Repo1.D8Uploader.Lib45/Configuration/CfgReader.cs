using System.IO;
using ServiceStack.Text;
using static System.Environment;

namespace Repo1.D8Uploader.Lib45.Configuration
{
    public class CfgReader
    {
        const SpecialFolder SPECIAL_DIR = SpecialFolder.LocalApplicationData;

        public static string  SubFolder { get; set; } = @"Repo1\D8Uploader";
        public static string  FileName  { get; set; } = "config.json";

        public static string ExpectedPath
            => Path.Combine(GetFolderPath(SPECIAL_DIR), SubFolder, FileName);


        public static UploaderCfg ReadAndParseFromLocal()
        {
            if (!File.Exists(ExpectedPath)) return null;

            var json = File.ReadAllText(ExpectedPath);

            return JsonSerializer.DeserializeFromString<UploaderCfg>(json);
        }
    }
}
