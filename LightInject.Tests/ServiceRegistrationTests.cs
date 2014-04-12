namespace LightInject.Tests
{
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceRegistrationTests
    {
        [TestMethod] 
        public void Equals_SameTypeSameServiceName_AreEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

            var secondRegistration = new ServiceRegistration();
            secondRegistration.ServiceType = typeof(int);
            secondRegistration.ServiceName = string.Empty;

            Assert.AreEqual(firstRegistration, secondRegistration);
        }

        [TestMethod]
        public void Equals_SameTypeDifferentServiceName_AreNotEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

            var secondRegistration = new ServiceRegistration();
            secondRegistration.ServiceType = typeof(int);
            secondRegistration.ServiceName = "SomeName";

            Assert.AreNotEqual(firstRegistration, secondRegistration);
        }

        [TestMethod]
        public void Equals_DifferentTypeSameServiceName_AreNotEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

            var secondRegistration = new ServiceRegistration();
            secondRegistration.ServiceType = typeof(string);
            secondRegistration.ServiceName = string.Empty;

            Assert.AreNotEqual(firstRegistration, secondRegistration);
        }

        [TestMethod]
        public void Equals_ComparedToNull_AreNotEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

           
            Assert.IsFalse(firstRegistration.Equals(null));
        }
    
        [TestMethod]
        public void GetInstance_OverrideImplementingType_CreateInstanceOfOverriddenImplementingType()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();

            container.Override(
                sr => sr.ServiceType == typeof(IFoo),
                (factory, registration) =>
                    {
                        registration.ImplementingType = typeof(AnotherFoo);
                        return registration;
                    });

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }
    }
}