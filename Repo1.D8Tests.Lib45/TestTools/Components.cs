using Autofac;
using Autofac.Extras.Moq;
using Moq;
using Repo1.Core.ns11.Configuration;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.D8Uploader.Lib45.ViewModels;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.TestTools
{
    class Components
    {
        internal static AutoMock GetFactory(UploaderCfg cfg, ITestOutputHelper testOutputHelper)
        {
            var ioc = AutoMock.GetLoose();

            ioc.Provide(cfg as UploaderCfg);
            ioc.Provide(cfg as RestServerCredentials);

            var uploader = ioc.Create<UploaderClient2>();
            uploader.MakeTestable(testOutputHelper);
            ioc.Provide(uploader);
            
            var mainVm = new Mock<MainWindowVmBase2>(uploader) { CallBase = true };
            ioc.Provide(mainVm.Object);

            return ioc;
        }
    }
}
