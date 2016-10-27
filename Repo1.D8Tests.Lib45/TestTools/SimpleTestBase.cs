using Repo1.Core.ns11.Obfuscators;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.TestTools
{
    public class SimpleTestBase
    {
        protected readonly ITestOutputHelper _out;
        protected FakeFactory                _fke;

        public SimpleTestBase(ITestOutputHelper testOutputHelper)
        {
            _fke = new FakeFactory();
            _out = testOutputHelper;
        }
    }
}
