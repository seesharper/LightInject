namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.SampleLibrary;
    using LightInject.ServiceLocation;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void GetInstance_KnownService_ReturnsInstance()
        {
            IServiceLocator serviceLocator = CreateServiceLocator();
            var instance = serviceLocator.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_KnownNamedService_ReturnsInstance()
        {
            IServiceLocator serviceLocator = CreateServiceLocator();
            var instance = serviceLocator.GetInstance(typeof(IFoo), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_KnownServiceUsingNullKey_ReturnsInstance()
        {
            IServiceLocator serviceLocator = CreateServiceLocator();
            var instance = serviceLocator.GetInstance(typeof(IFoo), null);
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }
            
        [TestMethod]
        public void GetInstance_InvalidService_ThrowsException()
        {
            IServiceLocator serviceLocator = CreateServiceLocator();
            ExceptionAssert.Throws<ActivationException>(() => serviceLocator.GetInstance(typeof(IBar)));            
        }

        [TestMethod]
        public void GetAllInstances_MultipleServices_ReturnsAllInstances()
        {
            IServiceLocator serviceLocator = CreateServiceLocator();
            var instances = serviceLocator.GetAllInstances(typeof(IFoo));
            Assert.AreEqual(2, instances.Count());
        }

        private static IServiceLocator CreateServiceLocator()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            return new LightInjectServiceLocator(container);
        }

    }
}