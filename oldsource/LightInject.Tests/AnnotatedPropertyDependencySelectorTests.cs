namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedPropertyDependencySelectorTests
    {
        [TestMethod]
        public void Execute_ClassWithoutAnnotatedProperties_ReturnsZeroDependencies()
        {
            var selector = new AnnotatedPropertyDependencySelector(new PropertySelector());

            var result = selector.Execute(typeof(FooWithProperyDependency));

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Execute_ClassWithAnnotatedProperties_ReturnsAnnotatedProperties()
        {
            var selector = new AnnotatedPropertyDependencySelector(new PropertySelector());

            var result = selector.Execute(typeof(FooWithAnnotatedProperyDependency));

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Execute_ClassWithNamedAnnotatedProperties_ReturnsServiceNameFromAttribute()
        {
            var selector = new AnnotatedPropertyDependencySelector(new PropertySelector());

            PropertyDependency propertyDependency = selector.Execute(typeof(FooWithNamedAnnotatedProperyDependency)).FirstOrDefault();

            Assert.AreEqual("AnotherBar", propertyDependency.ServiceName);
        }
    }
}