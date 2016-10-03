using FluentAssertions;
using Repo1.Core.ns11.Drupal7Tools;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns12.Tests.Helpers.D7MapperAttributes.D7MapperTests
{
    public class NodeAttributeFacts
    {
        [Fact(DisplayName = "Maps nodeRef fields to Dictionary")]
        public void Case1()
        {
            var eraser = new Eraser
            {
                nid      = 5678,
                Material = "rubber"
            };
            var pencil = new Pencil { Eraser = eraser };
            var dict   = D7Mapper.ToObjectDictionary(pencil);

            dict.Should().ContainKey("field_eraser");
            var field = dict["field_eraser"].As<UndContainer<TargetIdField>>();
            field.und.Should().HaveCount(1);
            field.und[0].target_id.Should().Be(eraser.nid);
        }

        
        private class Eraser
        {
            public int     nid       { get; set; }
            public string  Material  { get; set; }
        }


        [D7Type(Key = "pencil")]
        private class Pencil
        {
            [Node(Key = "field_eraser")]
            public Eraser  Eraser  { get; set; }
        }
    }
}
