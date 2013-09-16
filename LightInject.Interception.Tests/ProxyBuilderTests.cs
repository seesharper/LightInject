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
            proxyDefinition.Intercept(() => new SampleInterceptor(), info => true);

            var proxyType = CreateProxyType(proxyDefinition);

            FieldInfo targetField = proxyType.GetField("interceptor0", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(typeof(Lazy<IInterceptor>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_SingleInterceptor_DeclaresPublicLazyInterceptorFactoryField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            proxyDefinition.Intercept(() => new SampleInterceptor(), info => true);

            var proxyType = CreateProxyType(proxyDefinition);

            var test = proxyType.GetMethod("ToString");

            FieldInfo targetField = proxyType.GetField("InterceptorFactory0", BindingFlags.Public | BindingFlags.Static);
            Assert.AreEqual(typeof(Func<IInterceptor>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_NonGenericMethod_DeclaresPrivateStaticInterceptedMethodInfoField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Intercept(() => new SampleInterceptor(), info => info.Name == "Execute");

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
            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithNoParameters)Activator.CreateInstance(proxyType, (object)null);
            MethodInfo expectedMethod = typeof(IMethodWithNoParameters).GetMethod("Execute");

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<InvocationInfo>(ii => ii.Method == expectedMethod)));
        }

        [TestMethod]
        public void Execute_NonGenericMethod_PassesProceedDelegateToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithNoParameters)Activator.CreateInstance(proxyType, (object)null);
            
            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<InvocationInfo>(ii => ii.Proceed.GetType() == typeof(Func<object>))));
        }

        [TestMethod]
        public void Execute_NonGenericMethod_PassesProxyInstanceToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
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
            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
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

            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
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

            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
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

            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
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

            proxyDefinition.Intercept(() => interceptorMock.Object, info => info.Name == "Execute");
            var proxyType = CreateProxyType(proxyDefinition);
            var instance = (IMethodWithGenericParameter)Activator.CreateInstance(proxyType, (object)null);

            instance.Execute(42);

            interceptorMock.Verify(
                i => i.Invoke(
                    It.Is<InvocationInfo>(ii => (int)ii.Arguments[0] == 42)));
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
