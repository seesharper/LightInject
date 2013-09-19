using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Tests
{
    using LightInject.SampleLibrary;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    [TestClass]
    public class ConstructorSelectorTests
    {
        [TestMethod]
        public void Execute_StaticConstructor_IsNotReturned()
        {
            var selector = new ConstructorSelector();
           
            var constructorInfo = selector.Execute(typeof(FooWithStaticConstructor));

            Assert.IsNull(constructorInfo);
        }

        [TestMethod]
        public void Execute_MultipleConstructors_ConstructorWithMostParametersIsReturned()
        {
            var selector = new ConstructorSelector();

            var constructorInfo = selector.Execute(typeof(FooWithMultipleConstructors));

            Assert.AreEqual(1, constructorInfo.GetParameters().Count());
        }

        [TestMethod]
        public void Execute_PrivateConstructor_IsNotReturned()
        {
            var selector = new ConstructorSelector();

            var constructorInfo = selector.Execute(typeof(FooWithPrivateConstructor));

            Assert.IsNull(constructorInfo);
        }
        
    }
}
