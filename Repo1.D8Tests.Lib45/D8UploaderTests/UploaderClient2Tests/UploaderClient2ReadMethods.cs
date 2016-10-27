using FluentAssertions;
using Repo1.D8Tests.Lib45.TestTools;
using Repo1.D8Uploader.Lib45.RestClients;
using Xunit;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.D8UploaderTests.UploaderClient2Tests
{
    [Trait("Uploader D8", "Localhost 452 - Read")]
    public class UploaderClient2ReadMethods : AutoMockTestBase
    {
        public UploaderClient2ReadMethods(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact(DisplayName = "Get Package Node: Demo")]
        public async void GetDemoPackage()
        {
            var sut = _ioc.Create<UploaderClient2>();

            var pkg = await sut.GetPackage(Sample.SdkClientDemo.FileName);

            pkg         .Should().NotBeNull();
            pkg.nid     .Should().BeGreaterThan(0);
            pkg.uid     .Should().BeGreaterThan(0);
            pkg.FileName.Should().Be(Sample.SdkClientDemo.FileName);
        }


        [Fact(DisplayName = "Get PackagePart by Hash")]
        public async void GetPackagePartByHash()
        {
            var sut  = _ioc.Create<PackagePartUploader>();
            var hash = Sample.PackagePart1.Hash;

            var part = await sut.GetPackagePartByHash(hash);

            part         .Should().NotBeNull();
            part.PartHash.Should().Be(hash);
            part.nid     .Should().BeGreaterThan(0);
        }
    }
}
