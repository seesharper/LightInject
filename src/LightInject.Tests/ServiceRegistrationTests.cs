namespace LightInject.Tests
{
    using System;

    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using SampleLibrary;

    using Xunit;

    public class ServiceRegistrationTests
    {
        [Fact]
        public void Equals_SameTypeSameServiceName_AreEqual()
        {
            var firstRegistration = new ServiceRegistration
            {
                ServiceType = typeof(int),
                ServiceName = string.Empty
            };

            var secondRegistration = new ServiceRegistration
            {
                ServiceType = typeof(int),
                ServiceName = string.Empty
            };

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

            Assert.IsAssignableFrom<AnotherFoo>(instance);
        }

        [Fact]
        public void GetInstance_OverrideLifeTime_CallsFactoryOnlyOnce()
        {
            var container = new ServiceContainer();

            container.Register<A>();
            container.Register<B>();
            container.RegisterSingleton<C>();

            container.Override(sr => sr.ServiceType == typeof(C), (f, r) =>
            {
                r.Lifetime = new PerContainerLifetime();
                return r;
            });

            var a = container.GetInstance<A>();
            Assert.Same(a.C, a.B.C);
        }

        public class A
        {
            public A(B b, C c)
            {
                B = b;
                C = c;
            }

            public B B { get; }
            public C C { get; }
        }

        public class B
        {
            public B(C c)
            {
                C = c;
            }

            public C C { get; }
        }


        public class C
        {
            public C()
            {
                Thread.Sleep(100);
            }
        }


        [Fact]
        public void ToString_WithAllProperties_ReturnsEasyToReadRepresentation()
        {
            var sr = new ServiceRegistration();
            sr.ServiceType = typeof(IFoo);
            sr.ServiceName = "AnotherFoo";
            sr.ImplementingType = typeof(AnotherFoo);
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
        public void ToString_WithLifetime_ReturnsEasyToReadRepresentation()
        {
            var sr = new ServiceRegistration();
            sr.ServiceType = typeof(IFoo);
            sr.ServiceName = "AnotherFoo";
            sr.Lifetime = new PerContainerLifetime();
            var toString = sr.ToString();
            Assert.Equal("ServiceType: 'LightInject.SampleLibrary.IFoo', ServiceName: 'AnotherFoo', ImplementingType: '', Lifetime: 'LightInject.PerContainerLifetime'", toString);
        }


        [Fact]
        public void Register_ServiceAfterFirstGetInstance_TracesWarning()
        {
            string message = null;
            var container = new ServiceContainer(new ContainerOptions { LogFactory = t => m => message = m.Message });

            //SampleTraceListener sampleTraceListener = new SampleTraceListener(m => message = m);
            try
            {
                ///  Trace.Listeners.Add(sampleTraceListener);
                container.Register<IFoo, Foo>();
                container.GetInstance<IFoo>();
                container.Register<IFoo, Foo>();
                Assert.StartsWith("Cannot overwrite existing serviceregistration", message);
            }
            finally
            {
                //Trace.Listeners.Remove(sampleTraceListener);
            }
        }

        [Fact]
        public void Register_ImplementingTypeNotImplementingServiceType_ThrowsException()
        {
            var container = new ServiceContainer();
            Assert.Throws<ArgumentOutOfRangeException>("implementingType",
                () => container.Register(typeof(IFoo), typeof(Bar)));

        }

        [Fact]
        public void Register_GenericImplementingTypeWithMissingArgument_ThrowsException()
        {
            var container = new ServiceContainer();
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => container.Register(typeof(IFoo), typeof(Foo<>)));
            Assert.Equal("implementingType", exception.ParamName);
            Assert.StartsWith("The generic parameter(s) T found in type", exception.Message);
        }

        [Fact]
        public void Register_GenericImplementingTypeWithMultipleMissingArguments_ThrowsException()
        {
            var container = new ServiceContainer();
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => container.Register(typeof(object), typeof(Foo<,>)));
            Assert.Equal("implementingType", exception.ParamName);
            Assert.StartsWith("The generic parameter(s) T1,T2 found in type", exception.Message);
        }
    }

    //public class SampleTraceListener : TraceListener
    //{
    //    private readonly Action<string> listener;

    //    public SampleTraceListener(Action<string> listener)
    //    {
    //        this.listener = listener;
    //    }

    //    public override void Write(string message)
    //    {
    //        listener(message);
    //    }

    //    public override void WriteLine(string message)
    //    {
    //        listener(message);
    //    }
    //}
}