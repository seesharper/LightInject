namespace LightInject.Interception.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;

    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class ContainerInterceptionTests : TestBase
    {
        [TestMethod]         
        public void Intercept_Service_ReturnsProxyInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.Intercept(sr => sr.ServiceType == typeof(IFoo), factory => new SampleInterceptor());            
                      
            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void Intercept_ServiceUsingContainerResolvedInterceptor_ReturnsProxyInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.Register<IInterceptor, SampleInterceptor>();
            container.Intercept(sr => sr.ServiceType == typeof(IFoo), factory => factory.GetInstance<IInterceptor>());

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void Intercept_ServiceUsingContainerResolvedInterceptor_InvokesInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var targetMock = new Mock<IMethodWithNoParameters>();
            var container = new ServiceContainer();
            container.RegisterInstance(targetMock.Object);
            container.RegisterInstance(interceptorMock.Object);
            container.Intercept(sr => sr.ServiceType == typeof(IMethodWithNoParameters), factory => factory.GetInstance<IInterceptor>());

            var instance = container.GetInstance<IMethodWithNoParameters>();
            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.IsAny<IInvocationInfo>()), Times.Once());
        }

        [TestMethod]
        public void Intercept_Interceptor_DoesNotReturnProxyInstance()
        {
            var container = new ServiceContainer();            
            container.Register<IInterceptor, SampleInterceptor>();
            container.Intercept(sr => sr.ServiceType == typeof(IInterceptor), factory => factory.GetInstance<IInterceptor>());
            var instance = container.GetInstance<IInterceptor>();

            Assert.IsNotInstanceOfType(instance, typeof(IProxy));
        }               

        [TestMethod]
        public void Intercept_Service_PassesInvocationInfoToInterceptor()
        {
            var interceptorMock = new Mock<IInterceptor>();
            var targetMock = new Mock<IMethodWithNoParameters>();
            var container = new ServiceContainer();
            container.RegisterInstance(targetMock.Object);
            container.Intercept(sr => sr.ServiceType == typeof(IMethodWithNoParameters), (factory, definition) => definition.Implement(() => interceptorMock.Object));
            var instance = container.GetInstance<IMethodWithNoParameters>();

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.IsAny<IInvocationInfo>()), Times.Once());
        }

        [TestMethod]
        public virtual void GetInstance_InterceptorAfterDecorator_ReturnsProxy()
        {
            var container = CreateContainer();            
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            container.Decorate(typeof(IFoo), typeof(AnotherFooDecorator));
            container.Intercept(registration => registration.ServiceType == typeof(IFoo), factory => null);

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void GetInstance_ServiceWithConstructorArguments_ReturnsProxy()
        {
            var container = new ServiceContainer();                                               
            container.Register<int, IMethodWithValueTypeReturnValue>((factory, i) => new ClassWithConstructorArguments());
            container.Intercept(sr => sr.ServiceType == typeof(IMethodWithValueTypeReturnValue), info => new SampleInterceptor());
                        
            var instance = container.GetInstance<int, IMethodWithValueTypeReturnValue>(42);
            instance.Execute();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void GetInstance_WithNestedNewStatement_ReturnsProxy()
        {
            var container = new ServiceContainer();                                               
            container.Register<ClassWithConstructor>(factory => new ClassWithConstructor(new string(new char[] {'a'})));
            container.Intercept(sr => sr.ServiceType == typeof(ClassWithConstructor), info => new SampleInterceptor());

            var instance = container.GetInstance<ClassWithConstructor>();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void Intercept_Method_InvokesInterceptorOnlyForMatchingMethods()
        {
            var container = new ServiceContainer();
            var targetMock = new Mock<IClassWithTwoMethods>();
            container.RegisterInstance(targetMock.Object);
            var interceptedMethods = new List<string>();
            container.Intercept(m => m.Name == "FirstMethod",
                info =>
                    {
                        interceptedMethods.Add(info.Method.Name);
                        return null;
                    });
            var instance = container.GetInstance<IClassWithTwoMethods>();
            instance.FirstMethod();

            Assert.AreEqual(1, interceptedMethods.Count);
            Assert.AreEqual(interceptedMethods[0], "FirstMethod");
        }

        [TestMethod]
        public void GetInstance_InterceptedClass_ReturnsClassProxy()
        {
            var container = new ServiceContainer();
            container.Register<ClassWithVirtualMethod>();            
            container.Intercept(sr => sr.ServiceType == typeof(ClassWithVirtualMethod), factory => new SampleInterceptor());
            var instance = container.GetInstance<ClassWithVirtualMethod>();
            var test = instance.Execute();
            Console.WriteLine(test);
            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void GetInstance_InterceptedClassWithConstructorArguments_ReturnsClassProxy()
        {
            var container = new ServiceContainer();
            container.Register<ClassWithConstructor>();
            container.RegisterInstance("SomeValue");
            container.Intercept(sr => sr.ServiceType == typeof(ClassWithConstructor), factory => new SampleInterceptor());
            var instance = container.GetInstance<ClassWithConstructor>();
            var test = instance.Execute();
            Console.WriteLine(test);
            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void GetInstance_InterceptedClassRegisteredUsingInstanceFactory_ReturnsClassProxy()
        {
            var container = new ServiceContainer();
            container.Register(factory => new ClassWithConstructor("SomeValue"));
            container.Intercept(sr => sr.ServiceType == typeof(ClassWithConstructor), factory => null);
            var instance = container.GetInstance<ClassWithConstructor>();
            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod]
        public void GetInstance_InterceptedClassUsingObjectInitializer_SetsProperties()
        {
            var container = new ServiceContainer();
            container.Register(factory => new ClassWithPropertyAndVirtualMethod{Value = "SomeValue"});
            container.Intercept(sr => sr.ServiceType == typeof(ClassWithPropertyAndVirtualMethod), factory => null);
            var instance = container.GetInstance<ClassWithPropertyAndVirtualMethod>();
            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InterceptedClassWithUndeterminableImplementingType_ThrowsException()
        {
            var container = new ServiceContainer();
            container.Register(factory => CreateClassWithVirtualMethod());
            container.RegisterInstance("SomeValue");
            container.Intercept(sr => sr.ServiceType == typeof(ClassWithVirtualMethod), factory => new SampleInterceptor());
            container.GetInstance<ClassWithVirtualMethod>();

        }

        private static ClassWithVirtualMethod CreateClassWithVirtualMethod()
        {
            return new ClassWithVirtualMethod();
        }
    }
}