using System;
using System.IO;
using System.Text;
using Repo1.Core.ns12.Configuration;
using Repo1.Core.ns12.Helpers.StringExtensions;
using Repo1.WPF452.SDK.Helpers.Cryptographers;
using Repo1.WPF452.SDK.Helpers.Serialization;
using static System.Environment;

namespace Repo1.WPF452.SDK.Configuration
{
    public class Repo1Cfg : DownloaderCfg
    {
        public void Save(string configKey)
        {
            var json = Json.Serialize(this);
            var pwd  = GetPassword(configKey);
            var encr = AESThenHMAC.SimpleEncryptWithPassword(json, pwd);
            File.WriteAllText(GetPath(configKey), encr);
        }


        public static void WriteBlank(string configKey)
            => new Repo1Cfg().Save(configKey);


        public static bool Found(string configKey)
            => File.Exists(GetPath(configKey));


        public static Repo1Cfg Parse(string configKey)
            => Json.Deserialize<Repo1Cfg>(Read(configKey));


        public static string Read(string configKey)
        {
            var path = GetPath(configKey);
            if (!File.Exists(path)) return null;
            var raw  = File.ReadAllText(path);
            var pwd  = GetPassword(configKey);
            return AESThenHMAC.SimpleDecryptWithPassword(raw, pwd);
        }


        private static string GetPath(string configKey)
        {
            var env = GetFolderPath(SpecialFolder.LocalApplicationData);
            return Path.Combine(env, "Repo1", GetFileName(configKey));
        }


        private static string GetFileName(string configKey)
            => configKey.SHA1ForUTF8() + ".cfg";


        private static string GetPassword(string configKey)
            => B64(configKey).SHA1ForUTF8();


        private static string B64(string text)
        {
            if (text.IsBlank()) return null;
            var byts = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(byts);
        }
    }
}
