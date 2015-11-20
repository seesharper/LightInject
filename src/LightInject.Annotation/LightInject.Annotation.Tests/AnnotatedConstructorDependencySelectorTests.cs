namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Xunit;

    
    public class AnnotatedConstructorDependencySelectorTests
    {
        [Fact]
        public void Execute_ClassWithoutAnnotatedProperties_ReturnsDependencies()
        {
            var selector = new AnnotatedConstructorDependencySelector();

            var result = selector.Execute(typeof(FooWithDependency).GetConstructors().First());

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Execute_ClassWithAnnotatedProperties_ReturnsDependencies()
        {
            var selector = new AnnotatedConstructorDependencySelector();

            var result = selector.Execute(typeof(FooWithAnnotatedDependency).GetConstructors().First());

            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Execute_ClassWithNamedAnnotatedProperties_ReturnsServiceNameFromAttribute()
        {
            var selector = new AnnotatedConstructorDependencySelector();

            ConstructorDependency constructorDependency = selector.Execute(typeof(FooWithNamedAnnotatedDependency).GetConstructors().First()).FirstOrDefault();

            Assert.Equal("AnotherBar", constructorDependency.ServiceName);
        }
    }
}