namespace LightInject.Interception.Tests
{
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class ContainerInterceptionTests
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
            container.Register(targetMock.Object);
            container.Register(interceptorMock.Object);
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
            container.Register(targetMock.Object);
            container.Intercept(sr => sr.ServiceType == typeof(IMethodWithNoParameters), (factory, definition) => definition.Implement(() => interceptorMock.Object));
            var instance = container.GetInstance<IMethodWithNoParameters>();

            instance.Execute();

            interceptorMock.Verify(i => i.Invoke(It.IsAny<IInvocationInfo>()), Times.Once());
        }

       

    }
}