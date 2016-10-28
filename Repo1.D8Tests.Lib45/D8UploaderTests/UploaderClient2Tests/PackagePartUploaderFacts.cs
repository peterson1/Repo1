using FluentAssertions;
using Repo1.D8Tests.Lib45.TestTools;
using Repo1.D8Uploader.Lib45.RestClients;
using Xunit;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.D8UploaderTests.UploaderClient2Tests
{
    [Trait("Uploader D8", "Localhost 452 - Write")]
    public class PackagePartUploaderFacts : AutoMockTestBase
    {
        public PackagePartUploaderFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact(DisplayName = "Save Part Node")]
        public async void SavePartNode()
        {
            var sut = _ioc.Create<PackagePartUploader>();
            var prt = _fke.D8PackagePart();
            await sut.RequestWriteAccess();

            var ok = await sut.SavePartNode(prt);

            ok.Should().BeTrue();
        }
    }
}
