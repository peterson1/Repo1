using Repo1.D8Uploader.Lib45.Configuration;

namespace Repo1.D8Tests.Lib45.TestTools
{
    class TestCfgReader
    {
        internal static UploaderCfg LoadLocal()
        {
            CfgReader.FileName = "TestUploader_cfg.json";
            return CfgReader.ReadAndParseFromLocal();
        }

    }
}
