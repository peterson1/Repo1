using FluentAssertions;
using Repo1.Core.ns11.ReflectionTools;
using Xunit;

namespace Repo1.Net452.Tests.Core.ns11.Tests.ReflectionTools.NestedPropertyFinderTests
{
    public class InterfacePropertiesFacts
    {
        [Fact(DisplayName = "Non-nested property")]
        public void Case1()
        {
            var typ  = typeof(ISample1);
            var prop = typ.FindProperty(nameof(ISample1.NonNested));
            prop.Should().NotBeNull();
            prop.Name.Should().Be(nameof(ISample1.NonNested));
        }


        [Fact(DisplayName = "1-level Nested property")]
        public void Case2()
        {
            var typ  = typeof(ISampleChild);
            var prop = typ.FindProperty(nameof(ISampleParent.ParentProp));
            prop.Should().NotBeNull();
            prop.Name.Should().Be(nameof(ISampleParent.ParentProp));
        }


        interface ISample1
        {
            int NonNested { get; }
        }

        interface ISampleChild : ISampleParent
        {
            int ChildProp { get; }
        }

        interface ISampleParent
        {
            int ParentProp { get; }
        }
    }
}
