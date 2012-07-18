using System.Transactions;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependencyInjector.Tests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void GetInstance_FactoryWithTransientLifeCycle_ReturnsSingletonInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));
            var instance1 = container.GetInstance<IFactory>();
            var instance2 = container.GetInstance<IFactory>();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_FactoryWithRequestLifeCycle_ReturnsSingletonInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory), LifeCycleType.Request);
            var instance1 = container.GetInstance<IFactory>();
            var instance2 = container.GetInstance<IFactory>();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_FactoryWithSingletonLifeCycle_ReturnsSingletonInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory), LifeCycleType.Singleton);
            var instance1 = container.GetInstance<IFactory>();
            var instance2 = container.GetInstance<IFactory>();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_UnknownService_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(new Foo());
            factoryMock.Setup(f => f.CanGetInstance(typeof(IFoo), string.Empty)).Returns(true);
            container.Register(factoryMock.Object);
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_KnownService_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(new AnotherFoo());
            factoryMock.Setup(f => f.CanGetInstance(typeof(IFoo), string.Empty)).Returns(true);
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(factoryMock.Object);
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_UnknownValueType_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(42);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), string.Empty)).Returns(true);
            container.Register(factoryMock.Object);
            var instance = container.GetInstance<int>();
            Assert.AreEqual(42, instance);
        }

        [TestMethod]
        public void GetInstance_KnownValueType_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), string.Empty)).Returns(true);
            container.Register(2048);
            container.Register(factoryMock.Object);
            var instance = container.GetInstance<int>();
            Assert.AreEqual(1024, instance);
        }

        [TestMethod]
        public void GetInstance_KnownService_CanProceedAndReturnDecorator()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.Register(typeof(IFoo), typeof(Foo));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_UnknownService_ServiceNameIsPassedToCanGetInstanceMethod()
        {
            var container = CreateContainer();
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), It.IsAny<string>())).Returns(true);
            container.Register(factoryMock.Object);
            container.GetInstance<int>("SomeServiceName");
            factoryMock.Verify(f => f.CanGetInstance(typeof(int), "SomeServiceName"));
        }

        [TestMethod]
        public void GetInstance_UnknownService_ServiceNameIsPassedToGetInstanceMethod()
        {
            var container = CreateContainer();
            ServiceRequest serviceRequest = null;
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024).Callback<ServiceRequest>(sr => serviceRequest = sr);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), It.IsAny<string>())).Returns(true);
            container.Register(factoryMock.Object);
            container.GetInstance<int>("SomeServiceName");
            Assert.AreEqual("SomeServiceName", serviceRequest.ServiceName);
        }

        [TestMethod]
        public void GetInstance_UnknownService_ServiceTypeIsPassedToGetInstanceMethod()
        {
            var container = CreateContainer();
            ServiceRequest serviceRequest = null;
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024).Callback<ServiceRequest>(sr => serviceRequest = sr);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), It.IsAny<string>())).Returns(true);
            container.Register(factoryMock.Object);
            container.GetInstance<int>("SomeServiceName");
            Assert.AreEqual(typeof(int), serviceRequest.ServiceType);
        }

        [TestMethod]
        public void GetInstance_DefaultService_PassesEmptyServiceNameToCanCreateInstanceMethod()
        {
            var container = CreateContainer();
            container.Register(42, "SomeServiceName");            
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), It.IsAny<string>())).Returns(true);
            container.Register(factoryMock.Object);
            container.GetInstance<int>();
            factoryMock.Verify(f => f.CanGetInstance(typeof(int), string.Empty));
        }

        [TestMethod]
        public void GetInstance_UnknownService_CannotProceed()
        {
            var container = CreateContainer();
            ServiceRequest serviceRequest = null;
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024).Callback<ServiceRequest>(sr => serviceRequest = sr);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), It.IsAny<string>())).Returns(true);
            container.Register(factoryMock.Object);
            container.GetInstance<int>();
            Assert.IsFalse(serviceRequest.CanProceed);
        }

        [TestMethod]
        public void GetInstance_KnownService_CanProceed()
        {
            var container = CreateContainer();
            container.Register(42);
            ServiceRequest serviceRequest = null;
            var factoryMock = new Mock<IFactory>();
            factoryMock.Setup(f => f.GetInstance(It.IsAny<ServiceRequest>())).Returns(1024).Callback<ServiceRequest>(sr => serviceRequest = sr);
            factoryMock.Setup(f => f.CanGetInstance(typeof(int), It.IsAny<string>())).Returns(true);
            container.Register(factoryMock.Object);
            container.GetInstance<int>();
            Assert.IsTrue(serviceRequest.CanProceed);
        }

        [TestMethod]
        public void GetInstance_TransactionScopedInstance_ReturnsSameInstance()
        {
            var container = CreateContainer(); 
            container.Register(typeof(IFactory), typeof(TransactionScopedFactory));
            using (new TransactionScope())
            {
                var firstInstance = container.GetInstance<IFoo>();
                var secondInstance = container.GetInstance<IFoo>();
                Assert.AreSame(firstInstance, secondInstance);
            }
        }

        [TestMethod]
        public void GetInstance_TransactionScopedInstanceUsingProceed_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(TransactionScopedFactoryUsingProceed));
            container.Register(typeof(IFoo), typeof(Foo));
            using (new TransactionScope())
            {
                var firstInstance = container.GetInstance<IFoo>();
                var secondInstance = container.GetInstance<IFoo>();
                Assert.AreSame(firstInstance, secondInstance);
            }
        }

        private static IServiceContainer CreateContainer()
        {
            return new EmitServiceContainer();
        } 
    }
}