using System.Collections.Generic;
using FluentAssertions;
using Repo1.Core.ns11.Drupal7Tools;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns11.Tests.Drupal7Tools.D7MapperTests
{
    [Trait("Core", "Unit")]
    public class TaxonomyMapperFacts
    {
        [Fact(DisplayName = "Maps term ids for a taxonomy vocab")]
        public void Case1()
        {
            var dict = new Dictionary<string, object>();
            var obj  = new SampleClass1 { Enum1 = SampleEnum1.Val2 };

            D7TaxonomyMapper.Map(dict, obj);

            dict.Should().ContainKey("field_enum1");
            var field = dict["field_enum1"].As<UndContainer<TermIdField>>();
            field.und.Should().HaveCount(1);
            field.und[0].tid.Should().Be((int)SampleEnum1.Val2);
        }


        //[Vocabulary(Key = "sample_enum1")]
        enum SampleEnum1
        {
            Val1 = 12,
            Val2 = 34,
            Val3 = 56
        }


        class SampleClass1
        {
            [Term(Key = "field_enum1")]
            public SampleEnum1 Enum1 { get; set; }
        }
    }
}
