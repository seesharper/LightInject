namespace LightInject.Tests
{
    using System;
    using System.Diagnostics;
    using SampleLibrary;
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
        public void ToString_WithAllProperties_ReturnsEasyToReadRepresentation()
        {
            var sr = new ServiceRegistration();
            sr.ServiceType = typeof (IFoo);
            sr.ServiceName = "AnotherFoo";
            sr.ImplementingType = typeof (AnotherFoo);
            var toString = sr.ToString();
            Assert.Equal("ServiceType: 'LightInject.SampleLibrary.IFoo', ServiceName: 'AnotherFoo', ImplementingType: 'LightInject.SampleLibrary.AnotherFoo', Lifetime: 'Transient'", toString);
        }

        [Fact]
        public void ToString_WithNullProperty_ReturnsEasyToReadRepresentation()
        {
            var sr = new ServiceRegistration();
            sr.ServiceType = typeof(IFoo);
            sr.ServiceName = "AnotherFoo";
            sr.ImplementingType = null;
            var toString = sr.ToString();
            Assert.Equal("ServiceType: 'LightInject.SampleLibrary.IFoo', ServiceName: 'AnotherFoo', ImplementingType: '', Lifetime: 'Transient'", toString);
        }

        [Fact]
        public void Register_ServiceAfterFirstGetInstance_TracesWarning()
        {
            var container = new ServiceContainer();
            string message = null;
            SampleTraceListener sampleTraceListener = new SampleTraceListener(m => message = m);
            try
            {                
                Trace.Listeners.Add(sampleTraceListener);
                container.Register<IFoo, Foo>();
                container.GetInstance<IFoo>();
                container.Register<IFoo, Foo>();
                Assert.True(message.StartsWith("LightInject.ServiceContainer: Cannot overwrite existing serviceregistration"));
            }
            finally
            {
                Trace.Listeners.Remove(sampleTraceListener);
            }
            
        }
    }

    public class SampleTraceListener : TraceListener
    {
        private readonly Action<string> listener;

        public SampleTraceListener(Action<string> listener)
        {
            this.listener = listener;
        }

        public override void Write(string message)
        {
            listener(message);
        }

        public override void WriteLine(string message)
        {
            listener(message);
        }
    }
}