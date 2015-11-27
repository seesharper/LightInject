using System;

namespace LightInject.Interception.Tests
{
    using System.Reflection;

    using Xunit;

    [Collection("Interception")]
    public class ClassBasedProxyBuilderTests
    {
        [Fact]
        public void GetProxyType_VirtualMethod_ReturnsSubclass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            Type proxyType = CreateProxyType(proxyDefinition);

            Assert.True(proxyType.IsSubclassOf(typeof(ClassWithVirtualMethod)));

        }

        [Fact]
        public void GetProxyType_InterceptedVirtualMethod_ReturnsSubClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            proxyDefinition.Implement(() => null, info => info.Name == "Execute");

            Type proxyType = CreateProxyType(proxyDefinition);

            Assert.True(proxyType.IsSubclassOf(typeof(ClassWithVirtualMethod)));
        }

        [Fact]
        public void GetProxyType_ClassProxyWithConstructorParameters_ReturnsProxyWithNamedConstructorParameters()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithConstructor));

            Type proxyType = CreateProxyType(proxyDefinition);

            var constructor = proxyType.GetConstructor(new Type[] { typeof(string) });

            Assert.Equal("value", constructor.GetParameters()[0].Name);

        }

        [Fact]
        public void GetProxyType_VirtualMethod_DoesNotDeclareTargetField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            Type proxyType = CreateProxyType(proxyDefinition);
            
            FieldInfo targetField = proxyType.GetField("target", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.Null(targetField);
        }

        [Fact]
        public void GetProxyType_AdditionalInterface_ImplementsInterface()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod), typeof(IAdditionalInterface));

            var proxyType = CreateProxyType(proxyDefinition);

            Assert.True(typeof(IAdditionalInterface).IsAssignableFrom(proxyType));
        }

        [Fact]
        public void Execute_VirtualMethod_CallsMethodInBaseClass()
        {                        
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            proxyDefinition.Implement(() => new SampleInterceptor(), info => info.Name == "Execute");

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithVirtualMethod)Activator.CreateInstance(proxyType);

            proxy.Execute();
        }

        [Fact]
        public void Create_TargetWithConstructorParameter_PassesValueToBaseClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithConstructor));

            proxyDefinition.Implement(() => new SampleInterceptor(), info => info.Name == "Execute");

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithConstructor)Activator.CreateInstance(proxyType, "SomeValue");
            
            Assert.Equal("SomeValue", proxy.Value);
        }

        [Fact]
        public void Create_NonInterceptedTargetWithVirtualProperty_PassesValueToBaseClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualProperty));
            proxyDefinition.Implement(() => new SampleInterceptor(), info => false);
                

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithVirtualProperty)Activator.CreateInstance(proxyType);

            proxy.Value = "SomeValue";
            
            Assert.Equal("SomeValue", proxy.value);
        }

        [Fact]
        public void Create_InterceptedTargetWithVirtualProperty_PassesValueToBaseClass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualProperty));
            
            proxyDefinition.Implement(() => new SampleInterceptor(), info => info.Name == "ToString");

            Type proxyType = CreateProxyType(proxyDefinition);

            var proxy = (ClassWithVirtualProperty)Activator.CreateInstance(proxyType);

            proxy.Value = "SomeValue";

            Assert.Equal("SomeValue", proxy.value);
        }

        [Fact]
        public void Create_TargetWithEventWithoutInterceptingAddRemove_CanCreateProxyType()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithEvent));
            proxyDefinition.Implement(() => new SampleInterceptor(), method => method.Name == "ToString");            
            Type proxyType = CreateProxyType(proxyDefinition);
            Assert.True(typeof(ClassWithEvent).IsAssignableFrom(proxyType));

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
