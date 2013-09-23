using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
namespace LightInject.Interception.Tests
{
    using System.Reflection;

    using Moq;

    [TestClass]
    public class ProxyBuilderTests
    {
        [TestMethod]
        public void GetProxyType_SingleInterface_ImplementsInterface()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            
            var proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(typeof(ITarget).IsAssignableFrom(proxyType));
        }

        [TestMethod]
        public void GetProxyType_AdditionalInterface_ImplementsInterface()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget), typeof(IAdditionalInterface));

            var proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(typeof(IAdditionalInterface).IsAssignableFrom(proxyType));
        }

        [TestMethod]
        public void GetProxyType_ForProxyDefinition_DeclaresLazyTargetField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));

            var proxyType = CreateProxyType(proxyDefinition);

            FieldInfo targetField = proxyType.GetField("target", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(typeof(Lazy<ITarget>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_SingleInterface_DeclaresPrivateInitializerMethod()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));

            var proxyType = CreateProxyType(proxyDefinition);

            MethodInfo initializerMethod = proxyType.GetMethod("InitializeProxy", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(initializerMethod);
        }

        [TestMethod]
        public void GetProxyType_SingleInterceptor_DeclaresPrivateLazyInterceptorField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            proxyDefinition.Implement(info => true, () => new SampleInterceptor());

            var proxyType = CreateProxyType(proxyDefinition);

            FieldInfo targetField = proxyType.GetField("interceptor0", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(typeof(Lazy<IInterceptor>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_SingleInterceptor_DeclaresPublicLazyInterceptorFactoryField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            proxyDefinition.Implement(info => true, () => new SampleInterceptor());

            var proxyType = CreateProxyType(proxyDefinition);

            var test = proxyType.GetMethod("ToString");

            FieldInfo targetField = proxyType.GetField("InterceptorFactory0", BindingFlags.Public | BindingFlags.Static);
            Assert.AreEqual(typeof(Func<IInterceptor>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_NonGenericMethod_DeclaresPrivateStaticInterceptedMethodInfoField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Implement(info => info.Name == "Execute", () => new SampleInterceptor());

            var proxyType = CreateProxyType(proxyDefinition);

            FieldInfo interceptedMethodInfoField = proxyType.GetField("ExecuteInterceptedMethodInfo", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.IsNotNull(interceptedMethodInfoField);
        }

        [TestMethod]
        public void GetProxyType_WithoutTargetFactory_DeclaresConstructorWithLazyTargetParameter()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            var proxyType = CreateProxyType(proxyDefinition);
            var constructor = proxyType.GetConstructor(new[] { typeof(Lazy<ITarget>) });

            Assert.IsNotNull(constructor);
        }

        [TestMethod]
        public void GetProxyType_WithoutTargetFactory_DoesNotDeclareParameterlessConstructor()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            var proxyType = CreateProxyType(proxyDefinition);
            var constructor = proxyType.GetConstructor(Type.EmptyTypes);

            Assert.IsNull(constructor);
        }


        [TestMethod]
        public void GetProxyType_MustImplementProxyInterface()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget), () => null);
            
            var proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(typeof(IProxy).IsAssignableFrom(proxyType));        
        }

        [TestMethod]
        public void Execute_NoInterceptor_PassesGetHashCodeToTarget()
         {
            var target = new TargetWithGetHashCodeOverride(42);
            var proxyDefinition = new ProxyDefinition(typeof(ITarget), () => target);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (ITarget)Activator.CreateInstance(proxyType);
            
            var hashCode = instance.GetHashCode();

            Assert.AreEqual(42, hashCode);
        }

        [TestMethod]
        public void Execute_NonGenericMethod_PassesMethodInfoToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithNoParameters)Activator.CreateInstance(proxyType, (object)null);
            MethodInfo expectedMethod = typeof(IMethodWithNoParameters).GetMethod("Execute");

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<InvocationInfo>(ii => ii.Method == expectedMethod)));
        }

       

        [TestMethod]
        public void Execute_NonGenericMethod_PassesProxyInstanceToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithNoParameters)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<InvocationInfo>(ii => ii.Proxy == instance)));
        }

        [TestMethod]
        public void Execute_NonGenericMethod_PassesArgumentArrayToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithNoParameters)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<InvocationInfo>(ii => ii.Arguments.GetType() == typeof(object[]))));
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithReferenceTypeParameter));

            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithReferenceTypeParameter)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute("SomeValue");

            interceptorMock.Verify(
                i => i.Invoke(
                    It.Is<InvocationInfo>(ii => (string)ii.Arguments[0] == "SomeValue")));
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithValueTypeParameter));

            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithValueTypeParameter)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute(42);

            interceptorMock.Verify(
                i => i.Invoke(
                    It.Is<InvocationInfo>(ii => (int)ii.Arguments[0] == 42)));
        }

        [TestMethod]
        public void Execute_MethodWithNullableParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNullableParameter));

            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithNullableParameter)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute(42);

            interceptorMock.Verify(
                i => i.Invoke(
                    It.Is<InvocationInfo>(ii => (int)ii.Arguments[0] == 42)));
        }

        [TestMethod]
        public void Execute_MethodWithGenericParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithGenericParameter));

            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptorMock.Object);
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithGenericParameter)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute(42);

            interceptorMock.Verify(
                i => i.Invoke(
                    It.Is<InvocationInfo>(ii => (int)ii.Arguments[0] == 42)));
        }

        [TestMethod]
        public void GetProxyType_InterfaceWithProperty_ProxyImplementsProperty()
        {
            var proxyBuiler = new ProxyBuilder();
            var proxyDefinition = new ProxyDefinition(typeof(IClassWithReferenceTypeProperty));
            var proxyType = proxyBuiler.GetProxyType(proxyDefinition);
            Assert.AreEqual(1, proxyType.GetProperties().Length);
        }

        [TestMethod]
        public void GetProxyType_InterfaceWithEvent_ProxyImplementsEvent()
        {
            var proxyBuiler = new ProxyBuilder();
            var proxyDefinition = new ProxyDefinition(typeof(IClassWithEvent));
            var proxyType = proxyBuiler.GetProxyType(proxyDefinition);
            Assert.AreEqual(1, proxyType.GetEvents().Length);
        }

        [TestMethod]
        public void Execute_InterceptedMethodWithTargetReturnType_ReturnsProxy()
        {
            var proxyBuilder = CreateProxyBuilder();
            var interceptorMock = new Mock<IInterceptor>();           
            var targetMock = new Mock<IMethodWithTargetReturnType>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns(targetMock.Object);
            targetMock.Setup(t => t.Execute()).Returns(targetMock.Object);

            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithTargetReturnType));
            proxyDefinition.Implement(() => interceptorMock.Object);

            var proxyType = proxyBuilder.GetProxyType(proxyDefinition);

            var instance = (IMethodWithTargetReturnType)Activator.CreateInstance(proxyType, new Lazy<IMethodWithTargetReturnType>(() => targetMock.Object));

            var result = instance.Execute();

            Assert.IsInstanceOfType(result, typeof(IProxy));           
        }

        [TestMethod]
        public void Execute_NonInterceptedMethodWithTargetReturnType_ReturnsProxy()
        {
            var proxyBuilder = CreateProxyBuilder();         
            var targetMock = new Mock<IMethodWithTargetReturnType>();         
            targetMock.Setup(t => t.Execute()).Returns(targetMock.Object);
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithTargetReturnType));
            var proxyType = proxyBuilder.GetProxyType(proxyDefinition);
            var instance = (IMethodWithTargetReturnType)Activator.CreateInstance(proxyType, new Lazy<IMethodWithTargetReturnType>(() => targetMock.Object));

            var result = instance.Execute();

            Assert.IsInstanceOfType(result, typeof(IProxy));           
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
