using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
namespace LightInject.Interception.Tests
{
    using System.Collections.Generic;
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

        #region Properties       

        [TestMethod]
        public void GetProxyType_InterfaceWithProperty_ProxyImplementsProperty()
        {            
            var proxyType = CreateProxyType(new ProxyDefinition(typeof(IClassWithProperty)));
            Assert.AreEqual(1, proxyType.GetProperties().Length);
        }
        
        [TestMethod]
        public void SetValue_InterfaceWithProperty_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IClassWithProperty>(interceptorMock.Object, "set_Value");
            
            instance.Value = "SomeValue";

            VerifyArgument(interceptorMock, "SomeValue");
        }

        [TestMethod]
        public void GetValue_InterfaceWithProperty_ReturnsValueFromInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns("SomeValue");
            var instance = CreateProxy<IClassWithProperty>(interceptorMock.Object, "get_Value");

            var value = instance.Value;

            Assert.AreEqual("SomeValue", value);
        }    


        #endregion

        #region Events

        [TestMethod]
        public void GetProxyType_InterfaceWithEvent_ProxyImplementsEvent()
        {            
            var proxyType = CreateProxyType(new ProxyDefinition(typeof(IClassWithEvent)));

            Assert.AreEqual(1, proxyType.GetEvents().Length);
        }

        [TestMethod]
        public void AddHandler_InterfaceWithEvent_PassesSenderToInterceptor()
        {                        
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IClassWithEvent>(interceptorMock.Object, "add_SomeEvent");
            EventHandler<EventArgs> handler = (sender, args) => { };

            instance.SomeEvent += handler;

            VerifyArgument(interceptorMock, handler);            
        }


        [TestMethod]
        public void RemoveHandler_InterfaceWithEvent_PassesSenderToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IClassWithEvent>(interceptorMock.Object, "remove_SomeEvent");
            EventHandler<EventArgs> handler = (sender, args) => { };

            instance.SomeEvent -= handler;

            VerifyArgument(interceptorMock, handler);
        }

        #endregion

        [TestMethod]
        public void Execute_InterceptedMethodWithTargetReturnType_ReturnsProxy()
        {            
            var targetMock = new Mock<IMethodWithTargetReturnType>();
            var interceptorMock = new Mock<IInterceptor>();                       
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns(targetMock.Object);
            var instance = CreateProxy(interceptorMock.Object, targetMock.Object);

            var result = instance.Execute();

            Assert.IsInstanceOfType(result, typeof(IProxy));           
        }

        [TestMethod]
        public void Execute_NonInterceptedMethodWithTargetReturnType_ReturnsProxy()
        {            
            var targetMock = new Mock<IMethodWithTargetReturnType>();         
            targetMock.Setup(t => t.Execute()).Returns(targetMock.Object);
            var instance = CreateProxy(targetMock.Object);
            
            var result = instance.Execute();

            Assert.IsInstanceOfType(result, typeof(IProxy));           
        }

        #region Generics

        [TestMethod]
        public void GetProxyType_MethodWithTypeLevelValueTypeGenericParameter_ImplementsInterface()
        {
            Type proxyType = CreateProxyType(new ProxyDefinition(typeof(IMethodWithTypeLevelGenericParameter<int>)));            
            Assert.IsTrue(typeof(IMethodWithTypeLevelGenericParameter<int>).IsAssignableFrom(proxyType));
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeGenericParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithGenericParameter>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeGenericParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithGenericParameter>(interceptorMock.Object);

            instance.Execute("SomeValue");

            VerifyArgument(interceptorMock, "SomeValue");
        }

        [TestMethod]
        public void Execute_MethodWithGenericParameterThatHasClassConstraint_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithGenericParameterThatHasClassConstraint>(interceptorMock.Object);
            
            instance.Execute("SomeValue");

            VerifyArgument(interceptorMock, "SomeValue");
        }

        [TestMethod]
        public void Execute_MethodWithGenericParameterThatHasStructConstraint_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithGenericParameterThatHasStructConstraint>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }

        [TestMethod]
        public void Execute_MethodWithGenericParameterThatHasNewConstraint_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithGenericParameterThatHasNewConstraint>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }

        [TestMethod]
        public void Execute_MethodWithGenericParameterThatHasNestedConstraint_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithGenericParameterThatHasNestedContraint>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }

        [TestMethod]
        public void Execute_MethodWithTypeLevelValueTypeGenericParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithTypeLevelGenericParameter<int>>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }


        #endregion

        private T CreateProxy<T>(IInterceptor interceptor)
        {
            var proxyDefinition = new ProxyDefinition(typeof(T));
            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptor);
            Type proxyType = CreateProxyBuilder().GetProxyType(proxyDefinition);
            return (T)Activator.CreateInstance(proxyType, (object)null);
        }

        private T CreateProxy<T>(IInterceptor interceptor, string methodName)
        {
            var proxyDefinition = new ProxyDefinition(typeof(T));
            proxyDefinition.Implement(info => info.Name == methodName, () => interceptor);
            Type proxyType = CreateProxyBuilder().GetProxyType(proxyDefinition);
            return (T)Activator.CreateInstance(proxyType, (object)null);
        }

        private T CreateProxy<T>(T target)
        {
            var proxyDefinition = new ProxyDefinition(typeof(T));            
            Type proxyType = CreateProxyBuilder().GetProxyType(proxyDefinition);
            return (T)Activator.CreateInstance(proxyType, new Lazy<T>(() => target));
        }

        private T CreateProxy<T>(IInterceptor interceptor, T target)
        {
            var proxyDefinition = new ProxyDefinition(typeof(T));
            proxyDefinition.Implement(info => info.Name == "Execute", () => interceptor);
            Type proxyType = CreateProxyBuilder().GetProxyType(proxyDefinition);
            return (T)Activator.CreateInstance(proxyType, new Lazy<T>(() => target));
        }

        private void VerifyArgument<T>(Mock<IInterceptor> interceptorMock, T value)
        {                        
            interceptorMock.Verify(
               i => i.Invoke(
                   It.Is<InvocationInfo>(ii => Equals(ii.Arguments[0] , value))), Times.Once());
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
