using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightInject.Interception.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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
        public void GetProxyType_AdditionalInterfacesWithSameTypeAndMethodName_ImplementsBothInterfaces()
        {
            var proxyType = CreateProxyType(
                typeof(IMethodWithNoParameters),
                typeof(NamespaceA.IMethodWithNoParameters),
                typeof(NamespaceB.IMethodWithNoParameters));

            Assert.IsTrue(typeof(NamespaceA.IMethodWithNoParameters).IsAssignableFrom(proxyType));
            Assert.IsTrue(typeof(NamespaceB.IMethodWithNoParameters).IsAssignableFrom(proxyType));
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
            proxyDefinition.Implement(info => true, () => null);

            var proxyType = CreateProxyType(proxyDefinition);

            FieldInfo targetField = proxyType.GetField("interceptor0", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(typeof(Lazy<IInterceptor>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_SingleInterceptor_DeclaresPublicLazyInterceptorFactoryField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ITarget));
            proxyDefinition.Implement(info => true, () => null);

            var proxyType = CreateProxyType(proxyDefinition);
            
            FieldInfo targetField = proxyType.GetField("InterceptorFactory0", BindingFlags.Public | BindingFlags.Static);
            Assert.AreEqual(typeof(Func<IInterceptor>), targetField.FieldType);
        }

        [TestMethod]
        public void GetProxyType_NonGenericMethod_DeclaresPrivateStaticInterceptedMethodInfoField()
        {
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters));
            proxyDefinition.Implement(info => info.Name == "Execute", () => null);

            var proxyType = CreateProxyType(proxyDefinition);

            FieldInfo interceptedMethodInfoField = proxyType.GetField("ExecuteTargetMethodInfo0", BindingFlags.NonPublic | BindingFlags.Static);

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
        public void GetProxyType_OverloadedMethods_DeclaresUniqueInterceptedMethodFields()
        {
            var proxyType = CreateProxyType(typeof(IClassWithOverloadedMethods));

            var fields = proxyType.GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                     .Where(f => f.FieldType == typeof(TargetMethodInfo)).ToArray();

            Assert.AreNotEqual(fields[0].Name, fields[1].Name);
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
            var instance = CreateProxy<IMethodWithNoParameters>(interceptorMock.Object);
            MethodInfo expectedMethod = typeof(IMethodWithNoParameters).GetMethod("Execute");

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<IInvocationInfo>(ii => ii.Method == expectedMethod)));
        }
       
        [TestMethod]
        public void Execute_NonGenericMethod_PassesProxyInstanceToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithNoParameters>(interceptorMock.Object);

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<IInvocationInfo>(ii => ii.Proxy == instance)));
        }

        [TestMethod]
        public void Execute_NonGenericMethod_PassesArgumentArrayToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();            
            var instance = CreateProxy<IMethodWithNoParameters>(interceptorMock.Object);

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.Is<IInvocationInfo>(ii => ii.Arguments.GetType() == typeof(object[]))));
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeParameter_PassesValueToInterceptor()
        {            
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithReferenceTypeParameter>(interceptorMock.Object);

            instance.Execute("SomeValue");

            VerifyArgument(interceptorMock, "SomeValue");              
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeParameter_PassesValueToTarget()
        {            
            var targetMock = new Mock<IMethodWithReferenceTypeParameter>();
            var instance = CreateProxy(targetMock.Object);

            instance.Execute("SomeValue");

            targetMock.Verify(t => t.Execute("SomeValue"));
        }
       
        [TestMethod]
        public void Execute_MethodWithValueTypeParameter_PassesValueToInterceptor()
        {           
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithValueTypeParameter>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);        
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeParameter_PassesValueToTarget()
        {
            var targetMock = new Mock<IMethodWithValueTypeParameter>();

            var instance = CreateProxy(targetMock.Object);

            instance.Execute(42);

            targetMock.Verify(t => t.Execute(42));
        }

        [TestMethod]
        public void Execute_MethodWithNullableParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();            
            var instance = CreateProxy<IMethodWithNullableParameter>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }

        [TestMethod]
        public void Execute_MethodWithNullableParameter_PassesValueToTarget()
        {
            var targetMock = new Mock<IMethodWithNullableParameter>();

            var instance = CreateProxy(targetMock.Object);

            instance.Execute(42);

            targetMock.Verify(t => t.Execute(42));
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeOutParameter_ReturnsValueFromInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                           .Returns<IInvocationInfo>(ii => ii.Arguments[0] = 42);
            var instance = CreateProxy<IMethodWithValueTypeOutParameter>(interceptorMock.Object);
            
            int returnValue;
            instance.Execute(out returnValue);

            Assert.AreEqual(42, returnValue);
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeOutParameter_ReturnsValueFromTarget()
        {            
            var instance = CreateProxy<IMethodWithValueTypeOutParameter>(new MethodWithValueTypeOutParameter(42));
            
            int value;
            instance.Execute(out value);

            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeOutParameter_ReturnsValueFromInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                           .Returns<IInvocationInfo>(ii => ii.Arguments[0] = "AnotherValue");
            var instance = CreateProxy<IMethodWithReferenceTypeOutParameter>(interceptorMock.Object);

            string returnValue;
            instance.Execute(out returnValue);

            Assert.AreEqual("AnotherValue", returnValue);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeOutParameter_ReturnsValueFromTarget()
        {
            const string ValueToReturn = "SomeValue";
            var instance = CreateProxy<IMethodWithReferenceTypeOutParameter>(new MethodWithReferenceTypeOutParameter(ValueToReturn));

            string value;
            instance.Execute(out value);

            Assert.AreEqual(ValueToReturn, value);
        }


        [TestMethod]
        public void Execute_MethodWithValueTypeRefParameter_ReturnsValueFromInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                           .Returns<IInvocationInfo>(ii => ii.Arguments[0] = 42);
            var instance = CreateProxy<IMethodWithValueTypeRefParameter>(interceptorMock.Object);

            int returnValue = 84;
            
            instance.Execute(ref returnValue);

            Assert.AreEqual(42, returnValue);
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeRefParameter_ReturnsValueFromTarget()
        {            
            var instance = CreateProxy<IMethodWithValueTypeRefParameter>(new MethodWithValueTypeRefParameter());
            int value = 42;
            
            instance.Execute(ref value);

            Assert.AreEqual(84, value);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeRefParameter_ReturnsValueFromInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>()))
                           .Returns<IInvocationInfo>(ii => ii.Arguments[0] = "AnotherValue");
            var instance = CreateProxy<IMethodWithReferenceTypeRefParameter>(interceptorMock.Object);

            string returnValue = "SomeValue";

            instance.Execute(ref returnValue);

            Assert.AreEqual("AnotherValue", returnValue);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeRefParameter_ReturnsValueFromTarget()
        {
            const string ValueToReturn = "AnotherValue";
            var instance = CreateProxy<IMethodWithReferenceTypeRefParameter>(new MethodWithReferenceTypeRefParameter(ValueToReturn));
            
            string value = "SomeValue";
            instance.Execute(ref value);

            Assert.AreEqual(ValueToReturn, value);
        }

        #region Properties       

        [TestMethod]
        public void GetProxyType_InterfaceWithProperty_ProxyImplementsProperty()
        {            
            var proxyType = CreateProxyType(new ProxyDefinition(typeof(IClassWithProperty)));
            Assert.AreEqual(1, proxyType.GetProperties().Length);
        }
        
        [TestMethod]
        public void GetProxyType_AdditionalInterfaceWithProperty_DoesNotImplementProperty()
        {
            var proxyType = CreateProxyType(typeof(IMethodWithNoParameters), typeof(IClassWithProperty));
            Assert.AreEqual(0, proxyType.GetProperties().Length);
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
        public void GetProxyType_AdditionalInterfaceWithProperty_DoesNotImplementEvent()
        {
            var proxyType = CreateProxyType(typeof(IMethodWithNoParameters), typeof(IClassWithEvent));
            Assert.AreEqual(0, proxyType.GetEvents().Length);
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
            var instance = this.CreateProxyWithoutInterceptor(targetMock.Object);
            
            var result = instance.Execute();

            Assert.IsInstanceOfType(result, typeof(IProxy));           
        }

        [TestMethod]
        public void Execute_ProxyDefinitionWithoutMethodSelector_DoesNotInterceptObjectMethods()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var targetMock = new Mock<IMethodWithNoParameters>();
            ProxyDefinition proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters), () => targetMock.Object);
            proxyDefinition.Implement(() => interceptorMock.Object);
            var instance = CreateProxyFromDefinition<IMethodWithNoParameters>(proxyDefinition);
            instance.GetHashCode();
            instance.Equals(instance);
            instance.ToString();
            instance.GetType();
            interceptorMock.Verify(i => i.Invoke(It.IsAny<IInvocationInfo>()), Times.Never());
        }

        #region Generics

        [TestMethod]
        public void GetProxyType_MethodWithTypeLevelValueTypeGenericParameter_ImplementsInterface()
        {
            Type proxyType = CreateProxyType((typeof(IMethodWithTypeLevelGenericParameter<int>)));            
            Assert.IsTrue(typeof(IMethodWithTypeLevelGenericParameter<int>).IsAssignableFrom(proxyType));
        }
        
        [TestMethod]
        public void GetProxyType_OpenGenericMethod_DeclaresPrivateStaticOPenGenericInterceptedMethodInfoField()
        {
            Type proxyType = CreateProxyType((typeof(IMethodWithGenericParameter)));
            
            var field = proxyType.GetField(
                "ExecuteOpenGenericMethodInfo0", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.IsNotNull(field);
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

        [TestMethod]
        public void Execute_MethodWithContravariantTypeParameter_PassesValueToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var instance = CreateProxy<IMethodWithContravariantTypeParameter<int>>(interceptorMock.Object);

            instance.Execute(42);

            VerifyArgument(interceptorMock, 42);
        }

        [TestMethod]
        public void Execute_MethodWithCovariantTypeParameter_ReturnsValueFromInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns(42);
            var instance = CreateProxy<IMethodWithCovariantTypeParameter<int>>(interceptorMock.Object);

            var result = instance.Execute();

            Assert.AreEqual(42, result);
        }


        #endregion

        #region Interceptors

        [TestMethod]
        public void Execute_WithoutProceed_DoesCallTargetFactory()
        {
            int callCount = 0;
            var proxyDefinition = new ProxyDefinition(typeof(IMethodWithNoParameters), () =>
                { 
                    callCount++;
                    return null;
                });
            
            var interceptorMock = CreateInterceptorMock(null);            
            proxyDefinition.Implement(m => m.Name == "Execute", () => interceptorMock.Object);
            var instance = CreateProxyFromDefinition<IMethodWithNoParameters>(proxyDefinition);

            instance.Execute();

            Assert.AreEqual(0, callCount);
        }

     

        

        #endregion

        private T CreateProxyFromDefinition<T>(ProxyDefinition proxyDefinition)
        {                        
            Type proxyType = CreateProxyBuilder().GetProxyType(proxyDefinition);
            return (T)Activator.CreateInstance(proxyType);
        }


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
            var interceptor = CreateProceedingInterceptor();
            proxyDefinition.Implement(m => m.Name == "Execute", () => interceptor);
            Type proxyType = CreateProxyBuilder().GetProxyType(proxyDefinition);
            return (T)Activator.CreateInstance(proxyType, new Lazy<T>(() => target));
        }
        

        private T CreateProxyWithoutInterceptor<T>(T target)
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
                   It.Is<IInvocationInfo>(ii => Equals(ii.Arguments[0] , value))), Times.Once());
        }



        private Type CreateProxyType(ProxyDefinition proxyDefinition)
        {
            return CreateProxyBuilder().GetProxyType(proxyDefinition);
        }



        private Type CreateProxyType(Type targetType)
        {
            var proxyDefinition = new ProxyDefinition(targetType);
            proxyDefinition.Implement(m => m.IsDeclaredBy(targetType), () => null);
            return CreateProxyBuilder().GetProxyType(proxyDefinition);
        }

        private Type CreateProxyType(Type targetType, params Type[] additionalInterfaces)
        {
            var proxyDefinition = new ProxyDefinition(targetType, additionalInterfaces);
            proxyDefinition.Implement(m => m.IsDeclaredBy(targetType), () => null);
            foreach (Type additionalInterface in additionalInterfaces)
            {
                Type @interface = additionalInterface;
                proxyDefinition.Implement(m => m.IsDeclaredBy(@interface), () => null);
            }
            return CreateProxyBuilder().GetProxyType(proxyDefinition);
        }


        private static IInterceptor CreateProceedingInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns<IInvocationInfo>(ii => ii.Proceed());
            return interceptorMock.Object;
        }

        private static Mock<IInterceptor> CreateProceedingInterceptorMock()
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns<IInvocationInfo>(ii => ii.Proceed());
            return interceptorMock;
        }

        private static Mock<IInterceptor> CreateInterceptorMock(object returnValue)
        {
            var interceptorMock = new Mock<IInterceptor>();
            interceptorMock.Setup(i => i.Invoke(It.IsAny<IInvocationInfo>())).Returns(returnValue);
            return interceptorMock;
        }

        internal virtual IProxyBuilder CreateProxyBuilder()
        {
            return new ProxyBuilder();
        }
    }


}
