namespace LightInject.Tests
{
    using System;
    using LightInject.SampleLibrary;

    using Xunit;

    
    public class ServiceRegistrationTests
    {
        [Fact] 
        public void Equals_SameTypeSameServiceName_AreEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

            var secondRegistration = new ServiceRegistration();
            secondRegistration.ServiceType = typeof(int);
            secondRegistration.ServiceName = string.Empty;

            Assert.Equal(firstRegistration, secondRegistration);
        }

        [Fact]
        public void Equals_SameTypeDifferentServiceName_AreNotEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

            var secondRegistration = new ServiceRegistration();
            secondRegistration.ServiceType = typeof(int);
            secondRegistration.ServiceName = "SomeName";

            Assert.NotEqual(firstRegistration, secondRegistration);
        }

        [Fact]
        public void Equals_DifferentTypeSameServiceName_AreNotEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

            var secondRegistration = new ServiceRegistration();
            secondRegistration.ServiceType = typeof(string);
            secondRegistration.ServiceName = string.Empty;

            Assert.NotEqual(firstRegistration, secondRegistration);
        }

        [Fact]
        public void Equals_ComparedToNull_AreNotEqual()
        {
            var firstRegistration = new ServiceRegistration();
            firstRegistration.ServiceType = typeof(int);
            firstRegistration.ServiceName = string.Empty;

           
            Assert.False(firstRegistration.Equals(null));
        }
    
        [Fact]
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

            Assert.IsAssignableFrom(typeof(AnotherFoo), instance);
        }

        [Fact]
        public void Register_ImplementingTypeNotImplementingServiceType_ThrowsException()
        {
            var container = new ServiceContainer();
            Assert.Throws<ArgumentOutOfRangeException>("implementingType",
                () => container.Register(typeof (IFoo), typeof (Bar)));

        }

        [Fact]
        public void Register_GenericImplementingTypeWithMissingArgument_ThrowsException()
        {
            var container = new ServiceContainer();
            Assert.Throws<ArgumentOutOfRangeException>(() => container.Register(typeof (IFoo), typeof (Foo<>)));
        }
    }
}