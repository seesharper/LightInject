﻿namespace LightInject.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using LightInject;
    using LightInject.SampleLibrary;
    using LightInject.SampleLibraryWithCompositionRoot;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AssemblyScannerTests
    {
        private Mock<IServiceContainer> GetContainerMock(Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            var containerMock = new Mock<IServiceContainer>();
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor());
            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock.Object, lifetimeFactory, shouldRegister);
            return containerMock;
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultService()
        {
            this.GetContainerMock(() => null, (s, t) => true).Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, null), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresServiceWithGivenLifeCycleType()
        {
            this.GetContainerMock(() => new PerScopeLifetime(), (s,t) => true).Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, It.IsAny<PerScopeLifetime>()), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_DoesNotConfigureServiceFilteredByDelegate()
        {
            this.GetContainerMock(() => new PerScopeLifetime(), (s,t) => t.Name != "Foo").Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, It.IsAny<PerScopeLifetime>()), Times.Never());
        }


        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedService()
        {
            this.GetContainerMock(() => null, (s,t) => true).Verify(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo", null), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            this.GetContainerMock(() => null, (s, t) => true).Verify(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty, null), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            this.GetContainerMock(() => null, (s,t) => true).Verify(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo", null), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssemblyWithCompositionRoot_CallsComposeMethodOnce()
        {
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor());
            var containerMock = new Mock<IServiceContainer>();
            SampleCompositionRoot.CallCount = 0;
            assemblyScanner.Scan(typeof(SampleCompositionRoot).Assembly, containerMock.Object, null, (s, t) => true);
            Assert.AreEqual(1, SampleCompositionRoot.CallCount);
        }

        [TestMethod]
        public void Scan_SampleAssemblyWithCompositionRoot_HandlesRegisterAssemblyWithinCompositionRoot()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(SampleCompositionRoot).Assembly);
            var instance = container.GetInstance<ICompositionRoot>();
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Scan_SampleAssembly_DoesNotRegisterCompilerGeneratedTypes()
        {
            FooWithCompilerGeneratedType foo = new FooWithCompilerGeneratedType();
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.IsFalse(container.AvailableServices.Any(si => si.ImplementingType != null && si.ImplementingType.IsDefined(typeof(CompilerGeneratedAttribute), false)));
        }

        [TestMethod]
        public void Scan_HostAssembly_DoesNotConfigureInternalServices()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(ServiceContainer).Assembly);
            var result = container.AvailableServices.Where(si => si.ImplementingType.Namespace == "LightInject");
            Assert.IsFalse(container.AvailableServices.Any(si => si.ImplementingType != null && si.ImplementingType.Namespace == "LightInject"));
        }

        
        [TestMethod]
        public void GetInstance_NoServices_CallsAssemblyScannerOnFirstRequest()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            try
            {
                serviceContainer.GetInstance<IFoo>();
            }
            catch
            {

            }

            finally
            {
                scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), It.IsAny<Func<ILifetime>>(), It.IsAny<Func<Type, Type, bool>>()), Times.Once());
            }
        }
          
        [TestMethod]
        public void Register_AssemblyFile_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly("*SampleLibrary.dll");
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), It.IsAny<Func<ILifetime>>(), It.IsAny<Func<Type, Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_Assembly_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), It.IsAny<Func<ILifetime>>(), It.IsAny<Func<Type, Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_Assembly_RegistersConcreteTypeWithoutBaseclass()
        {
            AssemblyScanner assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor());
            var serviceRegistryMock = new Mock<IServiceRegistry>();
            assemblyScanner.Scan(typeof(IFoo).Assembly, serviceRegistryMock.Object, () => null, (s,t) => true);

            serviceRegistryMock.Verify(r => r.Register(typeof(ConcreteFoo), typeof(ConcreteFoo), "ConcreteFoo", null));
        }

        [TestMethod]
        public void Register_Assembly_RegistersConcreteTypeWithBaseclass()
        {
            AssemblyScanner assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor());
            var serviceRegistryMock = new Mock<IServiceRegistry>();
            assemblyScanner.Scan(typeof(IFoo).Assembly, serviceRegistryMock.Object, () => null, (s,t) => true);

            serviceRegistryMock.Verify(r => r.Register(typeof(Foo), typeof(ConcreteFooWithBaseClass), "ConcreteFooWithBaseClass", null));


        }

        [TestMethod]
        public void Register_AssemblyWithFunc_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, (s,t) => true);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), It.IsAny<Func<ILifetime>>(), It.IsAny<Func<Type, Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_AssemblyWithFuncAndLifeCycle_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, () => new PerContainerLifetime(), (s,t) => true);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), It.IsAny<Func<ILifetime>>(), It.IsAny<Func<Type, Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_AssemblyWithLifetimeFactory_RegistersServicesWithGivenLifeTime()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(IFoo).Assembly, () => new PerContainerLifetime());

            var service = container.AvailableServices.FirstOrDefault(sr => sr.ServiceType == typeof(IFoo));

            Assert.IsInstanceOfType(service.Lifetime, typeof(PerContainerLifetime));
        }
    }
}
