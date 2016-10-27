using System.Collections.Generic;
using FluentAssertions;
using Repo1.Core.ns11.Drupal8Tools;
using Repo1.Core.ns11.Obfuscators;
using Repo1.D8Tests.Lib45.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace Repo1.D8Tests.Lib45.CoreD8ToolsTests.D8ContentMapperTests
{
    [Trait("Core", "Unit")]
    public class D8ContentMapperFacts : SimpleTestBase
    {
        public D8ContentMapperFacts(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact(DisplayName = "Sets D8 Content Title")]
        public void SetsContentTitle()
        {
            var obj  = _fke.SampleClass1();
            var dict = D8NodeMapper.Cast(obj, "");

            dict.MustHave("title", "value", obj.TheTitle);
        }


        [Fact(DisplayName = "Sets D8 Content Type")]
        public void SetsContentType()
        {
            var obj  = _fke.SampleClass1();
            var dict = D8NodeMapper.Cast(obj, "");

            dict.MustHave("type", "target_id", obj.D8TypeName);
        }


        [Fact(DisplayName = "Sets D8 Integer Property")]
        public void SetsIntegerProperty()
        {
            var obj  = _fke.SampleClass1();
            var dict = D8NodeMapper.Cast(obj, "");

            dict.MustHave("field_first_number", "value", obj.FirstNumber);
        }


        [Fact(DisplayName = "Sets D8 Text Property")]
        public void SetsTextProperty()
        {
            var obj = _fke.SampleClass1();
            var dict = D8NodeMapper.Cast(obj, "");

            dict.MustHave("field_first_text", "value", obj.FirstText);
        }
    }


    class SampleClass1 : D8NodeBase
    {
        public override string D8TypeName => "sample_class_1";

        [ContentTitle     ]  public string  TheTitle     { get; set; }
        [_("first_number")]  public int     FirstNumber  { get; set; }
        [_("first_text")  ]  public string  FirstText    { get; set; }
    }


    static class FakeFactoryExtensions
    {
        public static SampleClass1 SampleClass1(this FakeFactory fke)
            => new D8ContentMapperTests.SampleClass1
            {
                TheTitle    = fke.Text,
                FirstNumber = fke.Int(1, 99999),
                FirstText   = fke.Text,
            };
    }


    static class DictionaryExtensions
    {
        public static void MustHave<T>(this Dictionary<string, object> dict,
            string fieldKey, string subKey, T expctdValue)
        {
            dict.Should().NotBeNull();

            dict.Should().ContainKey(fieldKey);
            var sub = dict[fieldKey] as List<Dictionary<string, object>>;
            sub           .Should().NotBeNull();
            sub           .Should().HaveCount(1);
            sub[0]        .Should().ContainKey(subKey);
            sub[0][subKey].Should().Be(expctdValue);

            dict.MustHaveLinks();
        }


        public static void MustHaveLinks(this Dictionary<string, object> dict)
        {
            dict.Should().ContainKey("_links");
            var lnx = dict["_links"] as Dictionary<string, object>;
            lnx.Should().NotBeNull();

            lnx.Should().ContainKey("type");
            var typ = lnx["type"] as Dictionary<string, object>;
            typ.Should().NotBeNull();

            typ.Should().ContainKey("href");
            var href = typ["href"].ToString();
            href.Should().Contain("rest/type/node/");
        }
    }
}
