namespace LightInject.Tests
{
    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedConstructorSelectorTests : TestBase
    {
        [TestMethod]
        public void Execute_MultipleConstructors_UsesServiceNameFromAttribute()
        {
            var container = CreateContainer();
            ((ServiceContainer)container).EnableAnnotatedConstructorInjection();
            container.RegisterInstance(42, "SomeValue");
            container.RegisterInstance(84, "AnotherValue");
            var selector = new AnnotatedConstructorSelector(container.CanGetInstance);
            var constructorInfo = selector.Execute(typeof(FooWithMultipleParameterizedConstructors));
            Assert.AreEqual(typeof(int), constructorInfo.GetParameters()[0].ParameterType);
        }

        [TestMethod]
        public void Execute_MultipleConstructors_UsesParameterNameAsServiceNameWhenAttributeIsMissing()
        {
            var container = CreateContainer();
            ((ServiceContainer)container).EnableAnnotatedConstructorInjection();
            container.RegisterInstance("42", "SomeValue");
            container.RegisterInstance("84", "stringValue");
            var selector = new AnnotatedConstructorSelector(container.CanGetInstance);
            var constructorInfo = selector.Execute(typeof(FooWithMultipleParameterizedConstructors));
            Assert.AreEqual(typeof(string), constructorInfo.GetParameters()[0].ParameterType);
        }
    }
}