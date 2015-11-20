namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Xunit;

    
    public class AnnotatedPropertyDependencySelectorTests
    {
        [Fact]
        public void Execute_ClassWithoutAnnotatedProperties_ReturnsZeroDependencies()
        {
            var selector = new AnnotatedPropertyDependencySelector(new PropertySelector());

            var result = selector.Execute(typeof(FooWithProperyDependency));

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void Execute_ClassWithAnnotatedProperties_ReturnsAnnotatedProperties()
        {
            var selector = new AnnotatedPropertyDependencySelector(new PropertySelector());

            var result = selector.Execute(typeof(FooWithAnnotatedProperyDependency));

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Execute_ClassWithNamedAnnotatedProperties_ReturnsServiceNameFromAttribute()
        {
            var selector = new AnnotatedPropertyDependencySelector(new PropertySelector());

            PropertyDependency propertyDependency = selector.Execute(typeof(FooWithNamedAnnotatedProperyDependency)).FirstOrDefault();

            Assert.Equal("AnotherBar", propertyDependency.ServiceName);
        }
    }
}