using Repo1.Core.ns11.Drupal7Tools;
using Repo1.Core.ns11.Obfuscators;
using Repo1.Net452.Tests.Core.ns12.Tests.Helpers.D7MapperAttributes.D7MapperTests;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns11.Tests.Helpers.D7MapperAttributes.D7MapperTests
{
    public class InheritedPropertiesFacts
    {
        private FakeFactory _fke = new FakeFactory();


        [Fact(DisplayName = "Adds values from inherited properties")]
        public void Case1()
        {
            var obj      = new SampleClass1();
            obj.BaseProp = _fke.Text;
            obj.TopProp  = _fke.Text;

            var dict = D7Mapper.ToObjectDictionary(obj);
            dict.ScalarField("baseProp", obj.BaseProp);
            dict.ScalarField("topProp", obj.TopProp);
        }


        abstract class BaseClass1
        {
            [Value(Key = "baseProp")]
            public string BaseProp { get; set; }
        }


        [D7Type(Key = "sample_class1")]
        class SampleClass1 : BaseClass1
        {
            [Value(Key = "topProp")]
            public string TopProp { get; set; }
        }
    }
}
