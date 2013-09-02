namespace LightInject.Tests
{
    using System;

    using LightInject.SampleLibrary;  
    using LightInject.Mocking;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class MockingTests
    {
        [TestMethod]
        public void GetInstance_ExistingService_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();                        
            container.StartMocking<IFoo>(() => new FooMock());

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>();
        }

        [TestMethod]
        public void GetInstance_NamedExistingService_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");            
            container.StartMocking<IFoo>(() => new FooMock(), "SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>("SomeFoo");
        }

        [TestMethod]
        public void GetInstance_Dependency_ReturnsInstanceWithMockDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IBar, Bar>();                        
            container.StartMocking<IBar>(() => new BarMock());

            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            
            Assert.IsInstanceOfType(instance.Bar, typeof(BarMock));

            container.EndMocking<IBar>();
        }


        [TestMethod]
        public void GetInstance_ExistingSingletonService_InheritsLifetime()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            
            container.StartMocking<IFoo>(() => new FooMock());

            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();

            Assert.AreSame(firstInstance, secondInstance);
            container.EndMocking<IFoo>();
        }

        [TestMethod]
        public void GetInstance_ExistingSingletonService_RestoresLifetime()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());

            container.StartMocking<IFoo>(() => new FooMock());
            container.GetInstance<IFoo>();
            container.EndMocking<IFoo>();
            
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();

            Assert.AreSame(firstInstance, secondInstance);
        }

     

        [TestMethod]
        public void GetInstance_NamedService_DoesNotMockDefaultService()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");            
            container.StartMocking<IFoo>(() => new FooMock(), "AnotherFoo");

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
            container.EndMocking<IFoo>("AnotherFoo");
        }

        [TestMethod]
        public void GetInstance_DefaultService_DoesNotMockNamedService()
        {
            var container = CreateContainer();            
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            container.Register<IFoo, Foo>();
            container.StartMocking<IFoo>(() => new FooMock());

            var instance = container.GetInstance<IFoo>("AnotherFoo");

            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
            container.EndMocking<IFoo>();
        }


        [TestMethod]
        public void GetInstance_ExistingServiceAfterGetInstance_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.GetInstance<IFoo>();
            
            container.StartMocking<IFoo>(() => new FooMock());

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>();
        }


        [TestMethod]
        public void GetInstance_NamedExistingServiceAfterGetInstance_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            container.GetInstance<IFoo>("SomeFoo");            
            container.StartMocking<IFoo>(() => new FooMock(), "SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>("SomeFoo");
        }

        [TestMethod]
        public void GetInstance_UnknownService_RetursMockInstance()
        {
            var container = CreateContainer();                        
            container.StartMocking<IFoo>(() => new FooMock());

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>();
        }

        [TestMethod]
        public void GetInstance_NamedUnknownService_RetursMockInstance()
        {
            var container = CreateContainer();            
            container.StartMocking<IFoo>(() => new FooMock(), "SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>("SomeFoo");
        }

        [TestMethod]
        public void GetInstance_ExistingServiceAfterEndMocking_ReturnsOriginalInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            
            container.StartMocking<IFoo>(() => new FooMock());
            container.GetInstance<IFoo>();
            container.EndMocking<IFoo>();
            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_NamedExistingServiceAfterEndMocking_ReturnsOriginalInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            
            container.StartMocking<IFoo>(() => new FooMock(), "SomeFoo");
            container.GetInstance<IFoo>("SomeFoo");
            container.EndMocking<IFoo>("SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }
              
        [TestMethod]
        public void GetInstance_RegisterServiceAfterMockingIsStarted_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();            
            container.StartMocking<IFoo>(() => new FooMock());
            
            container.Register<IFoo, Foo>();
            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>();
        }

        [TestMethod]
        public void GetInstance_NamedServiceUsingMockType_ReturnsMockedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            container.StartMocking(typeof(IFoo), "SomeFoo", typeof(FooMock));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>("SomeFoo");
        }

        [TestMethod]
        public void GetInstance_ServiceUsingMockType_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.StartMocking(typeof(IFoo), typeof(FooMock));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooMock));
            container.EndMocking<IFoo>();
        }

        [TestMethod]
        public void GetInstance_OpenGenericServiceUsingMockType_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.StartMocking(typeof(IFoo<>), typeof(FooMock<>));

            var instance = container.GetInstance<IFoo<int>>();

            Assert.IsInstanceOfType(instance, typeof(FooMock<int>));
            container.EndMocking(typeof(IFoo<>));
        }

        [TestMethod]
        public void EndMocking_UnknownService_ThrowsMeaningfulException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            ExceptionAssert.Throws<InvalidOperationException>(container.EndMocking<IBar>, ErrorMessages.UnknownMockService);
        }


        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}