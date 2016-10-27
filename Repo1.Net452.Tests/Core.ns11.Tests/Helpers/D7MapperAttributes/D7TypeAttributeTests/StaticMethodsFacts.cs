using FluentAssertions;
using Repo1.Core.ns11.Drupal7Tools;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns12.Tests.Helpers.D7MapperAttributes.D7TypeAttributeTests
{
    [Trait("Core", "Unit")]
    public class StaticMethodsFacts
    {
        [Fact(DisplayName = "D7Type.GetKey<T>()")]
        public void Case1()
        {
            D7Type.GetKey<SampleClass1>().Should().Be("sample_key");
        }

        [Fact(DisplayName = "D7Type.GetKey<T>() from subclass")]
        public void Case2()
        {
            D7Type.GetKey<SubClass1>().Should().Be("sample_key");
        }


        [D7Type(Key = "sample_key")]
        private class SampleClass1
        {

        }

        private class SubClass1 : SampleClass1
        {

        }
    }
}
