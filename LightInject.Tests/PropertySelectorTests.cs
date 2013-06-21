namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.SampleLibrary;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class PropertySelectorTests
    {
        [TestMethod] 
        public void Execute_PublicGetterAndSetter_IsReturned()
        {
            var propertySelector = new PropertySelector();
             
            var result = propertySelector.Execute(typeof(FooWithProperyDependency));

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Execute_PublicGetterWithPrivateSetter_IsNotReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithDependency));

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Execute_Inherited_IsReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithInheritedProperyDepenency));

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void Execute_Static_IsNotReturned()
        {
            var propertySelector = new PropertySelector();

            var result = propertySelector.Execute(typeof(FooWithStaticProperty));

            Assert.AreEqual(0, result.Count());
        }
    }
}