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
             
            var result = propertySelector.Execute(typeof(FooWithProperyDependency));

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Execute_PublicGetterWithPrivateSetter_IsNotReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithDependency));

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void Execute_Inherited_IsReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithInheritedProperyDepenency));

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Execute_Static_IsNotReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithStaticProperty));

            Assert.Equal(0, result.Count());
        }
    }
}