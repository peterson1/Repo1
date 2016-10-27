using FluentAssertions;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.D8Tests.Lib45.TestTools;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.WPF45.SDK.Extensions.R1ModelExtensions;
using Xunit;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.D8UploaderTests.UploaderClient2Tests
{
    [Trait("Uploader D8", "Localhost 452 - Write")]
    public class UploaderClient2WriteMethods : AutoMockTestBase
    {
        public UploaderClient2WriteMethods(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact(DisplayName = "Post New Part")]
        public async void PostNewPart()
        {
            var sut  = _ioc.Create<PackagePartUploader>();
            var part = _fke.D8PackagePart();
            await sut.RequestWriteAccess();

            var ok   = await sut.UploadAndAttachToNewNode(part);

            ok.Should().BeTrue();
        }


        [Fact(DisplayName = "Request Write Access: Success")]
        public async void RequestWriteAccess_Success()
        {
            var sut = _ioc.Create<UploaderClient2>();
            var ok  = await sut.RequestWriteAccess();
            ok.Should().BeTrue();
            _cfg.WasRejected.Should().BeFalse();
        }


        [Fact(DisplayName = "Request Write Access: Invalid Login")]
        public async void RequestWriteAccess_WrongPwd()
        {
            var sut = _ioc.Create<UploaderClient2>();
            sut.Credentials.Username = "wrong user name";
            var ok = await sut.RequestWriteAccess();
            ok.Should().BeFalse();
            _cfg.WasRejected.Should().BeTrue();
        }


        //[Fact(DisplayName = "UploadInParts: Demo (success)")]
        //public async void UploadInParts()
        //{
        //    var pkg = R1Package.FromFile(Sample.SdkClientDemo.FilePath);
        //    //todo: create downloader instance here, then check before hand that this version is not yet uploaded
        //    var sut = _ioc.Create<UploaderClient2>();

        //    var res = await sut.UploadInParts(pkg, 0.5);

        //    res.Should().BeTrue();
        //    //todo: use downloader to check that parts were uploaded
        //}
    }
}
