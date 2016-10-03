using FluentAssertions;
using Repo1.WPF45.SDK.HtmlTools;
using Xunit;

namespace Repo1.Net452.Tests.WPF452.SDK.Tests.Helpers.HtmlTools.HtmlDecoderTests
{
    public class HtmlDecoderFacts
    {
        [Fact(DisplayName = "Decodes all string properties")]
        public void Case1()
        {
            var obj = new SampleClass1
            {
                Text1 = "Admin&#039;s Fats1 Ping"
            };

            HtmlDecoder.ReplaceStrings(obj);

            obj.Text1.Should().Be("Admin's Fats1 Ping");
        }


        class SampleClass1
        {
            public int     Number  { get; set; }
            public string  Text1   { get; set; }
            public string  Text2   { get; set; }
        }
    }
}
