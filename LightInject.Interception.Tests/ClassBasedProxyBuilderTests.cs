using System;

namespace LightInject.Interception.Tests
{
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClassBasedProxyBuilderTests
    {
        [TestMethod]
        public void GetProxyType_VirtualMethod_ReturnsSubclass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            Type proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(proxyType.IsSubclassOf(typeof(ClassWithVirtualMethod)));

        }

        [TestMethod]
        public void GetProxyType_InterceptedVirtualMethod_ReturnsSubClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            proxyDefinition.Implement(() => null, info => info.Name == "Execute");

            Type proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(proxyType.IsSubclassOf(typeof(ClassWithVirtualMethod)));
        }

        [TestMethod]
        public void GetProxyType_ClassProxyWithConstructorParameters_ReturnsProxyWithNamedConstructorParameters()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithConstructor));

            Type proxyType = CreateProxyType(proxyDefinition);

            var constructor = proxyType.GetConstructor(new Type[] { typeof(string) });

            Assert.AreEqual("value", constructor.GetParameters()[0].Name);

        }

        [TestMethod]
        public void GetProxyType_VirtualMethod_DoesNotDeclareTargetField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            Type proxyType = CreateProxyType(proxyDefinition);
            
            FieldInfo targetField = proxyType.GetField("target", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNull(targetField);
        }

        [TestMethod]
        public void GetProxyType_AdditionalInterface_ImplementsInterface()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod), typeof(IAdditionalInterface));

            var proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(typeof(IAdditionalInterface).IsAssignableFrom(proxyType));
        }

        [TestMethod]
        public void Execute_VirtualMethod_CallsMethodInBaseClass()
        {                        
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            proxyDefinition.Implement(() => new SampleInterceptor(), info => info.Name == "Execute");

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithVirtualMethod)Activator.CreateInstance(proxyType);

            proxy.Execute();
        }

        [TestMethod]
        public void Create_TargetWithConstructorParameter_PassesValueToBaseClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithConstructor));

            proxyDefinition.Implement(() => new SampleInterceptor(), info => info.Name == "Execute");

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithConstructor)Activator.CreateInstance(proxyType, "SomeValue");
            
            Assert.AreEqual("SomeValue", proxy.Value);
        }

        [TestMethod]
        public void Create_NonInterceptedTargetWithVirtualProperty_PassesValueToBaseClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualProperty));
            proxyDefinition.Implement(() => new SampleInterceptor(), info => false);
                

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithVirtualProperty)Activator.CreateInstance(proxyType);

            proxy.Value = "SomeValue";
            
            Assert.AreEqual("SomeValue", proxy.value);
        }

        [TestMethod]
        public void Create_InterceptedTargetWithVirtualProperty_PassesValueToBaseClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualProperty));
            
            proxyDefinition.Implement(() => new SampleInterceptor(), info => info.Name == "ToString");

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithVirtualProperty)Activator.CreateInstance(proxyType);

            proxy.Value = "SomeValue";

            Assert.AreEqual("SomeValue", proxy.value);
        }
       
        private Type CreateProxyType(ProxyDefinition proxyDefinition)
        {
            return CreateProxyBuilder().GetProxyType(proxyDefinition);
        }

        internal virtual IProxyBuilder CreateProxyBuilder()
        {
            return new ProxyBuilder();
        }
    }
}
