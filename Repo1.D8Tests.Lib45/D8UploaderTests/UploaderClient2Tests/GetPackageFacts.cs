using FluentAssertions;
using Repo1.D8Uploader.Lib45.RestClients;
using Xunit;

namespace Repo1.D8Tests.Lib45.D8UploaderTests.UploaderClient2Tests
{
    public class GetPackageFacts
    {
        const string DEMO_FILENAME = "Repo1.SdkClient.DemoWPF.exe";

        [Fact(DisplayName = "Get Package Node: Demo")]
        public async void GetDemoPackage()
        {
            var sut = new UploaderClient2();

            var pkg = await sut.GetPackage(DEMO_FILENAME);

            pkg.nid.Should().BeGreaterThan(0);
        }
    }
}
