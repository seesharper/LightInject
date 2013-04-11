namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedConstructorDependencySelectorTests
    {
        [TestMethod]
        public void Execute_ClassWithoutAnnotatedProperties_ReturnsDependencies()
        {
            var selector = new AnnotatedConstructorDependencySelector();

            var result = selector.Execute(typeof(FooWithDependency).GetConstructors().First());

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Execute_ClassWithAnnotatedProperties_ReturnsDependencies()
        {
            var selector = new AnnotatedConstructorDependencySelector();

            var result = selector.Execute(typeof(FooWithAnnotatedDependency).GetConstructors().First());

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Execute_ClassWithNamedAnnotatedProperties_ReturnsServiceNameFromAttribute()
        {
            var selector = new AnnotatedConstructorDependencySelector();

            ConstructorDependency constructorDependency = selector.Execute(typeof(FooWithNamedAnnotatedDependency).GetConstructors().First()).FirstOrDefault();

            Assert.AreEqual("AnotherBar", constructorDependency.ServiceName);
        }
    }
}