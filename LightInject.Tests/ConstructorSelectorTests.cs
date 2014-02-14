﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Tests
{
    using LightInject.SampleLibrary;
#if NETFX_CORE || WINDOWS_PHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    [TestClass]
    public class ConstructorSelectorTests : TestBase
    {
        [TestMethod]
        public void Execute_StaticConstructor_IsNotReturned()
        {
            var container = CreateContainer();
            var selector = new MostResolvableConstructorSelector(container.CanGetInstance);           
            ExceptionAssert.Throws<InvalidOperationException>(() => selector.Execute(typeof(FooWithStaticConstructor)));
        }

        [TestMethod]
        public void Execute_MultipleConstructors_ReturnsMostResolvableConstructor()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            var selector = new MostResolvableConstructorSelector(container.CanGetInstance);           
            var constructorInfo = selector.Execute(typeof(FooWithMultipleParameterizedConstructors));                        
            Assert.AreEqual(typeof(string), constructorInfo.GetParameters()[0].ParameterType);
        }

        [TestMethod]
        public void Execute_MultipleConstructors_ThrowsException()
        {
            var container = CreateContainer();            
            var selector = new MostResolvableConstructorSelector(container.CanGetInstance);

            ExceptionAssert.Throws<InvalidOperationException>(
                () => selector.Execute(typeof(FooWithMultipleParameterizedConstructors)),e => e.Message.StartsWith("No resolvable"));                        
        }

        [TestMethod]
        public void Execute_MultipleConstructors_UsesParameterNameAsServiceName()
        {
            var container = CreateContainer();
            container.RegisterInstance(42, "SomeValue");
            container.RegisterInstance(84, "IntValue");
            var selector = new MostResolvableConstructorSelector(container.CanGetInstance);
            var constructorInfo = selector.Execute(typeof(FooWithMultipleParameterizedConstructors));
            Assert.AreEqual(typeof(int), constructorInfo.GetParameters()[0].ParameterType);
        }

        [TestMethod]
        public void Execute_PrivateConstructor_IsNotReturned()
        {
            var container = CreateContainer();
            var selector = new MostResolvableConstructorSelector(container.CanGetInstance);
            ExceptionAssert.Throws<InvalidOperationException>(() => selector.Execute(typeof(FooWithPrivateConstructor)));
        }      
    }
}
