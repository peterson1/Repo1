using FluentAssertions;
using Repo1.D8Tests.Lib45.TestTools;
using Repo1.D8Uploader.Lib45.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.D8UploaderTests.MainWindowVmTests
{
    [Trait("Uploader D8", "Localhost 452 - Read")]
    public class MainWindowVmFacts : AutoMockTestBase
    {
        public MainWindowVmFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact(DisplayName = "Check non-registered file")]
        public async void NonRegistered()
        {
            var sut = _ioc.Create<MainWindowVmBase2>();

            await sut.CheckUploadability(Sample.NonRegistered.FilePath);

            sut.FileIsUploadable.Should().Be(false);
        }


        [Fact(DisplayName = "Check registered file")]
        public async void Registered()
        {
            var sut = _ioc.Create<MainWindowVmBase2>();

            await sut.CheckUploadability(Sample.SdkClientDemo.FilePath);

            sut.FileIsUploadable.Should().Be(true);
        }
    }
}
