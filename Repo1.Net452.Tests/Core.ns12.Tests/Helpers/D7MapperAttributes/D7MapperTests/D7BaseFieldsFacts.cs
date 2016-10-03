using System.Collections.Generic;
using FluentAssertions;
using Repo1.Core.ns11.Drupal7Tools;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns12.Tests.Helpers.D7MapperAttributes.D7MapperTests
{
    public class D7BaseFieldsFacts
    {
        [Fact(DisplayName = "Maps base fields to Dictionary")]
        public void Case1()
        {
            var poco = new SampleClass
            {
                nid        = 1234,
                uid        = 91011,
                FileName   = "this becomes title",
                ExeNid     = 5678,
                ExeVersion = "1.645.45647",
                PartHash   = "45646-456465-4546-654654",
            };
            var dict = D7Mapper.ToObjectDictionary(poco);

            dict.BaseField("type", "sample_class");
            dict.BaseField("nid", poco.nid);
            dict.BaseField("title", poco.FileName);
            dict.BaseField("status", 1);
            dict.BaseField("uid", poco.uid);
            //dict.BaseField("language", "und");

            dict.ContainsKey("vid").Should().BeFalse("No vid if zero");
            poco.vid = 91012;

            dict = D7Mapper.ToObjectDictionary(poco);
            dict.BaseField("vid", poco.vid);
        }


        [Fact(DisplayName = "Maps base fields to node")]
        public void Case2()
        {
            var poco = new SampleClass();
            var dict = new Dictionary<string, object>
            {
                { "nid", 123 },
                { "vid", 456 },
                { "uid", 789 },
            };

            dict.SetNodeIDs(poco);

            poco.nid.Should().Be(123);
            poco.vid.Should().Be(456);
            poco.uid.Should().Be(789);
        }


        [D7Type(Key = "sample_class")]
        private class SampleClass
        {
            [NodeTitle]
            public string FileName { get; set; }

            [Value(Key = "field_exenid")]
            public int ExeNid { get; set; }

            [Value(Key = "field_fileversion")]
            public string ExeVersion { get; set; }

            [Value(Key = "field_filehash")]
            public string PartHash { get; set; }

            public int nid { get; set; }
            public int uid { get; set; }
            public int vid { get; set; }
            public string FullPathOrURL { get; set; }
        }
    }


    public static class D7BaseFieldsFactsExtensions
    {
        public static void BaseField(this Dictionary<string, object> dict, string key, object value)
        {
            dict.Should().ContainKey(key);
            dict[key].Should().Be(value);
        }
    }
}
