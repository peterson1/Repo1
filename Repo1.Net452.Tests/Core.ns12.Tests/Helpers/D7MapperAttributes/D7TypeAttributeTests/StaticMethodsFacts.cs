using FluentAssertions;
using Repo1.Core.ns12.Helpers.D7MapperAttributes;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns12.Tests.Helpers.D7MapperAttributes.D7TypeAttributeTests
{
    public class StaticMethodsFacts
    {
        [Fact(DisplayName = "Gets key attribute value from type")]
        public void Case1()
        {
            D7Type.GetKey<SampleClass1>().Should().Be("sample_key");
        }


        [D7Type(Key = "sample_key")]
        private class SampleClass1
        {

        }
    }
}
