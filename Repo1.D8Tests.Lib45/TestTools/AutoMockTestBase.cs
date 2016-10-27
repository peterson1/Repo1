using Autofac.Extras.Moq;
using Repo1.D8Uploader.Lib45.Configuration;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.TestTools
{
    public class AutoMockTestBase : SimpleTestBase
    {
        protected AutoMock                   _ioc;
        protected UploaderCfg                _cfg;

        public AutoMockTestBase(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _cfg = TestCfgReader.LoadLocal();
            _ioc = Components.GetFactory(_cfg, _out);
        }
    }
}
