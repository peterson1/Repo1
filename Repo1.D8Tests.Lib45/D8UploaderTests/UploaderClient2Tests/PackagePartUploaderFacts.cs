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


        [Fact(DisplayName = "Upload File")]
        public async void UploadFile()
        {
            var sut = _ioc.Create<PackagePartUploader>();
            await sut.RequestWriteAccess();

            var fid = await sut.UploadFile(Sample._1KbFile.Path);

            fid.Should().BeGreaterThan(0);
        }
    }
}
