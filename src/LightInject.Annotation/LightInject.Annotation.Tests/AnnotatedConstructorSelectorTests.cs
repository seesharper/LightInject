namespace LightInject.Tests
{
    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Xunit;

    
    public class AnnotatedConstructorSelectorTests : TestBase
    {
        [Fact]
        public void Execute_MultipleConstructors_UsesServiceNameFromAttribute()
        {
            var container = CreateContainer();
            ((ServiceContainer)container).EnableAnnotatedConstructorInjection();
            container.RegisterInstance(42, "SomeValue");
            container.RegisterInstance(84, "AnotherValue");
            var selector = new AnnotatedConstructorSelector(container.CanGetInstance);
            var constructorInfo = selector.Execute(typeof(FooWithMultipleParameterizedConstructors));
            Assert.Equal(typeof(int), constructorInfo.GetParameters()[0].ParameterType);
        }

        [Fact]
        public void Execute_MultipleConstructors_UsesParameterNameAsServiceNameWhenAttributeIsMissing()
        {
            var container = CreateContainer();
            ((ServiceContainer)container).EnableAnnotatedConstructorInjection();
            container.RegisterInstance("42", "SomeValue");
            container.RegisterInstance("84", "stringValue");
            var selector = new AnnotatedConstructorSelector(container.CanGetInstance);
            var constructorInfo = selector.Execute(typeof(FooWithMultipleParameterizedConstructors));
            Assert.Equal(typeof(string), constructorInfo.GetParameters()[0].ParameterType);
        }
    }
}