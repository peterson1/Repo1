using FluentAssertions;
using Repo1.D8Tests.Lib45.TestTools;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.RestClients;
using Xunit;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.D8UploaderTests.UploaderClient2Tests
{
    [Trait("Scope", "Acceptance")]
    public class GetPackageFacts
    {
        private UploaderCfg _cfg;
        private readonly ITestOutputHelper _out;

        const string DEMO_FILENAME = "Repo1.SdkClient.DemoWPF.exe";
        public GetPackageFacts(ITestOutputHelper testOutputHelper)
        {
            _out = testOutputHelper;
            CfgReader.FileName = "TestUploader_cfg.json";
            _cfg = CfgReader.ReadAndParseFromLocal();
        }


        [Fact(DisplayName = "Get Package Node: Demo")]
        public async void GetDemoPackage()
        {
            var sut = new UploaderClient2(_cfg);
            sut.MakeTestable(_out);

            var pkg = await sut.GetPackage(DEMO_FILENAME);

            pkg.Should().NotBeNull();
            pkg.nid.Should().BeGreaterThan(0);
            pkg.uid.Should().BeGreaterThan(0);
            pkg.FileName.Should().Be(DEMO_FILENAME);
        }
    }
}
