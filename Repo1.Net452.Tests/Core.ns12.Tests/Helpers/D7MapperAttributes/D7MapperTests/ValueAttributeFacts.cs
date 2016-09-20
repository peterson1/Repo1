using System.Collections.Generic;
using FluentAssertions;
using Repo1.Core.ns12.Helpers.D7MapperAttributes;
using Repo1.Core.ns12.Helpers.D7MapperAttributes.UndFields;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns12.Tests.Helpers.D7MapperAttributes.D7MapperTests
{
    public class ValueAttributeFacts
    {
        [Fact(DisplayName = "Maps scalar fields to Dictionary")]
        public void Case1()
        {
            var poco = new SampleClass
            {
                nid = 1234,
                uid = 91011,
                FileName = "this becomes title",
                ExeNid = 5678,
                ExeVersion = "1.645.45647",
                PartHash = "45646-456465-4546-654654",
            };
            var dict = D7Mapper.ToObjectDictionary(poco);

            dict.BaseField("type", "sample_class");
            dict.BaseField("nid", poco.nid);
            dict.BaseField("title", poco.FileName);
            dict.BaseField("status", 1);
            dict.BaseField("uid", poco.uid);

            dict.ScalarField("field_exenid", poco.ExeNid);
            dict.ScalarField("field_fileversion", poco.ExeVersion);
            dict.ScalarField("field_filehash", poco.PartHash);
        }




        [D7Type(Key = "sample_class")]
        private class SampleClass
        {
            [NodeTitle]
            public string   FileName    { get; set; }

            [Value(Key = "field_exenid"     )]
            public int      ExeNid      { get; set; }

            [Value(Key = "field_fileversion")]
            public string   ExeVersion  { get; set; }

            [Value(Key = "field_filehash"   )]
            public string   PartHash    { get; set; }


            public int      nid            { get; set; }
            public int      uid            { get; set; }
            public string   FullPathOrURL  { get; set; }
        }
    }



    public static class ValueAttributeFactsExtensions
    {
        public static void BaseField(this Dictionary<string, object> dict, string key, object value)
        {
            dict.Should().ContainKey(key);
            dict[key].Should().Be(value);
        }

        public static void ScalarField(this Dictionary<string, object> dict, string key, object value)
        {
            dict.Should().ContainKey(key);
            var field = dict[key].As<UndContainer<ValueField>>();
            field.und[0].value.Should().Be(value);
        }
    }
}
