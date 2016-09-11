using System.IO;
using ServiceStack.Text;
using static System.Environment;

namespace Repo1.ExeUploader.WPF.Configuration
{
    class CfgReader
    {
        const SpecialFolder SPECIAL_DIR = SpecialFolder.LocalApplicationData;
        const string CFG_PATH           = @"Repo1\Uploader\config.json";

        internal static string ExpectedPath 
            => Path.Combine(GetFolderPath(SPECIAL_DIR), CFG_PATH);


        internal static UploaderCfg ReadAndParseFromLocal()
        {
            if (!File.Exists(ExpectedPath)) return null;

            var json = File.ReadAllText(ExpectedPath);

            return JsonSerializer.DeserializeFromString<UploaderCfg>(json);
        }
    }
}
