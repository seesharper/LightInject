namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using LightInject;
    using LightInject.SampleLibrary;
    using LightInject.SampleLibraryWithCompositionRoot;

    using LightMock;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    
    [TestClass]
    public class AssemblyScannerTests
    {                
        private MockContext<IServiceContainer> GetContainerMock(Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            var mockContext = new MockContext<IServiceContainer>();
            var containerMock = new ContainerMock(mockContext);            
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), new CompositionRootTypeExtractor(), new CompositionRootExecutor(containerMock));
            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock, lifetimeFactory, shouldRegister);
            return mockContext;
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultService()
        {
            this.GetContainerMock(() => null, (s, t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, null), Invoked.Once);
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresServiceWithGivenLifeCycleType()
        {
            this.GetContainerMock(() => new PerScopeLifetime(), (s,t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, The<ILifetime>.Is(i => i is PerScopeLifetime)), Invoked.Once);
        }

        [TestMethod]
        public void Scan_SampleAssembly_DoesNotConfigureServiceFilteredByDelegate()
        {
            this.GetContainerMock(() => new PerScopeLifetime(), (s, t) => t.Name != "Foo").Assert(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, The<ILifetime>.Is(i => i is PerScopeLifetime)), Invoked.Never);
        }


        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedService()
        {
            this.GetContainerMock(() => null, (s,t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo", null), Invoked.Once);
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            this.GetContainerMock(() => null, (s, t) => true).Assert(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty, null), Invoked.Once);
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            this.GetContainerMock(() => null, (s,t) => true).Assert(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo", null), Invoked.Once);
        }

        [TestMethod]
        public void Scan_SampleAssemblyWithCompositionRoot_CallsComposeMethodOnce()
        {
	        var containerMock = new ContainerMock(new MockContext<IServiceContainer>());
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), new CompositionRootTypeExtractor(), new CompositionRootExecutor(containerMock));
            SampleCompositionRoot.CallCount = 0;
            assemblyScanner.Scan(typeof(SampleCompositionRoot).Assembly, containerMock, null, (s, t) => true);
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
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.IsFalse(container.AvailableServices.Any(si => si.ImplementingType != null && si.ImplementingType.IsDefined(typeof(CompilerGeneratedAttribute), false)));
        }

        [TestMethod]
        public void Scan_SampleAssembly_DoesNotRegisterAbstractTypes()
        {            
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.IsFalse(container.AvailableServices.Any(si => si.ImplementingType == typeof(AbstractFoo)));
        }

        [TestMethod]
        public void Scan_SampleAssembly_DoesNotRegisterSystemTypes()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(string).Assembly);
            Assert.AreEqual(0, container.AvailableServices.Count());
        }

        [TestMethod]
        public void Scan_SampleAssembly_DoesNotRegisterNestedPrivateTypes()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.IsFalse(container.AvailableServices.Any(si => si.ImplementingType.Name == "NestedPrivateBar"));
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
        public void GetInstance_UnknownService_CallsAssemblyScannerBeforeInvokingRules()
        {
            List<string> sequence = new List<string>();
	        var mockContext = new MockContext<IAssemblyScanner>();
			mockContext.Arrange(m => m.Scan(The<Assembly>.IsAnyValue, The<IServiceRegistry>.IsAnyValue))
				.Callback<Assembly, IServiceRegistry>((a,r) => sequence.Add("Scan"));
            
	        var scannerMock = new AssemblyScannerMock(mockContext);

            var container = new ServiceContainer();
	        container.AssemblyScanner = scannerMock;
            container.RegisterFallback((type, s) => type.Name == "IFoo",
                request =>
                    {
                        sequence.Add("Fallback");
                        return new SampleLibraryWithCompositionRootTypeAttribute.Foo();
                    });
            container.GetInstance<SampleLibraryWithCompositionRootTypeAttribute.IFoo>();

            Assert.AreEqual("Scan", sequence[0]);
            Assert.AreEqual("Fallback", sequence[1]);
        }


#if NET || NET45         
        [TestMethod]
        public void Register_AssemblyFile_CallsAssemblyScanner()
        {
	        var mockContext = new MockContext<IAssemblyScanner>();
			var scannerMock = new AssemblyScannerMock(mockContext);
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly("*SampleLibrary.dll");
            mockContext.Assert(a => a.Scan(typeof(IFoo).Assembly,The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type,Type, bool>>.IsAnyValue), Invoked.Once);
        }
#endif
        [TestMethod]
        public void Register_Assembly_CallsAssemblyScanner()
        {
            var mockContext = new MockContext<IAssemblyScanner>();
            var scannerMock = new AssemblyScannerMock(mockContext);
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly);
            mockContext.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }

        [TestMethod]
        public void Register_Assembly_RegistersConcreteTypeWithoutBaseclass()
        {
            var mockContext = new MockContext<IServiceContainer>();
            var containerMock = new ContainerMock(mockContext);

            AssemblyScanner assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), new CompositionRootTypeExtractor(), new CompositionRootExecutor(containerMock));            
            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock, () => null, (s,t) => true);

            mockContext.Assert(r => r.Register(typeof(ConcreteFoo), typeof(ConcreteFoo), "ConcreteFoo", null));
        }

        [TestMethod]
        public void Register_Assembly_RegistersConcreteTypeWithBaseclass()
        {
            var mockContext = new MockContext<IServiceContainer>();
            var containerMock = new ContainerMock(mockContext);
            
            AssemblyScanner assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), new CompositionRootTypeExtractor(), new CompositionRootExecutor(containerMock));

            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock, () => null, (s,t) => true);

            mockContext.Assert(r => r.Register(typeof(Foo), typeof(ConcreteFooWithBaseClass), "ConcreteFooWithBaseClass", null));
        }

        [TestMethod]
        public void Register_AssemblyWithFunc_CallsAssemblyScanner()
        {
            var mockContext = new MockContext<IAssemblyScanner>();
            var scannerMock = new AssemblyScannerMock(mockContext);
            
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, (s,t) => true);
            mockContext.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }

        [TestMethod]
        public void Register_AssemblyWithFuncAndLifeCycle_CallsAssemblyScanner()
        {
            var mockContext = new MockContext<IAssemblyScanner>();
            var scannerMock = new AssemblyScannerMock(mockContext);
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, () => new PerContainerLifetime(), (s, t) => true);
            mockContext.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }
#if NET || NET45
        [TestMethod]
        public void Register_SearchPattern_CallsAssemblyScanner()
        {
            var mockContext = new MockContext<IAssemblyScanner>();
            var scannerMock = new AssemblyScannerMock(mockContext);
            

            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly("LightInject.SampleLibrary.dll");
            mockContext.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }
#endif

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
