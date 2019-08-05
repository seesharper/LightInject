namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.SampleLibrary;
    using Xunit;



    public class PropertySelectorTests
    {
        [Fact]
        public void Execute_PublicGetterAndSetter_IsReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithPropertyDependency));

            Assert.Single(result);
        }

        [Fact]
        public void Execute_PublicGetterWithPrivateSetter_IsNotReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithDependency));

            Assert.Empty(result);
        }

        [Fact]
        public void Execute_Inherited_IsReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithInheritedProperyDepenency));

            Assert.Single(result);
        }

        [Fact]
        public void Execute_Static_IsNotReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithStaticProperty));

            Assert.Empty(result);
        }
    }
}