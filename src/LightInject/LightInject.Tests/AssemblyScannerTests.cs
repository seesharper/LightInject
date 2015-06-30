using System.Management.Instrumentation;
using System.Reflection;

namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using LightInject;
    using LightInject.SampleLibrary;
    
    using LightMock;

    using Xunit;
    
    
    
    public class AssemblyScannerTests
    {                
        private MockContext<IServiceContainer> GetContainerMock(Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {            
            var containerMock = new ContainerMock();
            var compositionRootMock = new CompositionRootMock();     
            
            var compositionRootTypeExtractorMock = new TypeExtractorMock();
            compositionRootTypeExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
                   
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), compositionRootTypeExtractorMock, new CompositionRootExecutor(containerMock,t => compositionRootMock));
            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock, lifetimeFactory, shouldRegister);
            return containerMock;
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresDefaultService()
        {
            GetContainerMock(() => null, (s, t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresServiceWithGivenLifeCycleType()
        {
            this.GetContainerMock(() => new PerScopeLifetime(), (s,t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, The<ILifetime>.Is(i => i is PerScopeLifetime)), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotConfigureServiceFilteredByDelegate()
        {
            this.GetContainerMock(() => new PerScopeLifetime(), (s, t) => t.Name != "Foo").Assert(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, The<ILifetime>.Is(i => i is PerScopeLifetime)), Invoked.Never);
        }


        [Fact]
        public void Scan_SampleAssembly_ConfiguresNamedService()
        {
            this.GetContainerMock(() => null, (s,t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo", null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            this.GetContainerMock(() => null, (s, t) => true).Assert(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty, null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            this.GetContainerMock(() => null, (s,t) => true).Assert(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo", null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresAccordingToPredicate()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(AssemblyScannerTests).Assembly, (s,i) => s == typeof(IFoo));

            Assert.True(container.AvailableServices.Any(sr => sr.ServiceType == typeof(IFoo)));
            Assert.False(container.AvailableServices.Any(sr => sr.ServiceType == typeof(IBar)));
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresAllServicesByDefault()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(AssemblyScannerTests).Assembly);

            Assert.True(container.AvailableServices.Any(sr => sr.ServiceType == typeof(IFoo)));
            Assert.True(container.AvailableServices.Any(sr => sr.ServiceType == typeof(IBar)));
        }



        [Fact]
        public void Scan_SampleAssemblyWithCompositionRoot_CallsComposeMethodOnce()
        {
            var compositionRootMock = new CompositionRootMock();
            var containerMock = new ContainerMock();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new []{typeof(CompositionRootMock)});
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(),
                compositionRootExtractorMock,
                new CompositionRootExecutor(containerMock, t => compositionRootMock));
            
            assemblyScanner.Scan(typeof(AssemblyScannerTests).Assembly, containerMock);

            compositionRootMock.Assert(c => c.Compose(containerMock), Invoked.Once);
        }

        [Fact]
        public void ScanUsingPredicate_SampleAssemblyWithCompositionRoot_DoesNotCallCompositionRoot()
        {
            var compositionRootMock = new CompositionRootMock();
            var containerMock = new ContainerMock();
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(),
                new CompositionRootTypeExtractor(new CompositionRootAttributeExtractor()),
                new CompositionRootExecutor(containerMock, t => compositionRootMock));

            assemblyScanner.Scan(typeof(AssemblyScannerTests).Assembly, containerMock, () => null, (s, t) => true);
            
            compositionRootMock.Assert(c => c.Compose(containerMock), Invoked.Never);
        }


        //[TestMethod, Ignore]
        //public void Scan_SampleAssemblyWithCompositionRoot_HandlesRegisterAssemblyWithinCompositionRoot()
        //{
        //    var container = new ServiceContainer();
        //    container.RegisterAssembly(typeof(SampleCompositionRoot).Assembly);
        //    var instance = container.GetInstance<ICompositionRoot>();
        //    Assert.NotNull(instance);
        //}

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterCompilerGeneratedTypes()
        {         
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.False(container.AvailableServices.Any(si => si.ImplementingType != null && si.ImplementingType.IsDefined(typeof(CompilerGeneratedAttribute), false)));
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterAbstractTypes()
        {            
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.False(container.AvailableServices.Any(si => si.ImplementingType == typeof(AbstractFoo)));
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterSystemTypes()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(string).Assembly);
            Assert.Equal(0, container.AvailableServices.Count());
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterNestedPrivateTypes()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(Foo).Assembly);
            Assert.False(container.AvailableServices.Any(si => si.ImplementingType.Name == "NestedPrivateBar"));
        }


        [Fact]
        public void Scan_HostAssembly_DoesNotConfigureInternalServices()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(ServiceContainer).Assembly);
            var result = container.AvailableServices.Where(si => si.ImplementingType.Namespace == "LightInject");
            Assert.False(container.AvailableServices.Any(si => si.ImplementingType != null && si.ImplementingType.Namespace == "LightInject"));
        }
        
        //[Fact]
        //public void GetInstance_FallBackRegisteredInScannedAssembly_ReturnsInstance()
        //{            	        
        //    var container = new ServiceContainer();            
        //    var instance = container.GetInstance<SampleLibraryWithCompositionRootTypeAttribute.IBar>();
        //    Assert.NotNull(instance);            
        //}


#if NET || NET45 || NET46         
        [Fact]
        public void Register_AssemblyFileWithoutCompositionRoot_CallsAssemblyScanner()
        {	        
			var scannerMock = new AssemblyScannerMock();
            var serviceContainer = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            serviceContainer.CompositionRootTypeExtractor = compositionRootExtractorMock;
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly("*LightInject.Tests.dll");
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly,The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Scan_AssemblyFileWithCompositionRoot_CallsScanMethodMethod()
        {
            var container = new ServiceContainer();
            var compositionRootTypeExtractorMock = new TypeExtractorMock();
            compositionRootTypeExtractorMock.Arrange(t => t.Execute(The<Assembly>.IsAnyValue)).Returns(new [] {typeof(CompositionRootMock)});
            container.CompositionRootTypeExtractor = compositionRootTypeExtractorMock;
            var assemblyScannerMock = new AssemblyScannerMock();
            container.AssemblyScanner = assemblyScannerMock;

            container.RegisterAssembly("LightInject.Tests.dll");
            assemblyScannerMock.Assert(a => a.Scan(The<Assembly>.IsAnyValue, container), Invoked.Once);
        }
#endif
        [Fact]
        public void Register_Assembly_CallsAssemblyScanner()
        {            
            var scannerMock = new AssemblyScannerMock();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);

            var serviceContainer = new ServiceContainer();
            serviceContainer.CompositionRootTypeExtractor = compositionRootExtractorMock;
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly);
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Register_Assembly_RegistersConcreteTypeWithoutBaseclass()
        {            
            GetContainerMock(() => null, (s, t) => true).Assert(r => r.Register(typeof(ConcreteFoo), typeof(ConcreteFoo), "ConcreteFoo", null));
        }

        [Fact]
        public void Register_Assembly_RegistersConcreteTypeWithBaseclass()
        {           
            GetContainerMock(() => null, (s, t) => true).Assert(r => r.Register(typeof(Foo), typeof(ConcreteFooWithBaseClass), "ConcreteFooWithBaseClass", null));
        }

        [Fact]
        public void Register_AssemblyWithFunc_CallsAssemblyScanner()
        {
            var scannerMock = new AssemblyScannerMock();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, (s,t) => true);
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Register_AssemblyWithFuncAndLifeCycle_CallsAssemblyScanner()
        {            
            var scannerMock = new AssemblyScannerMock();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, () => new PerContainerLifetime(), (s, t) => true);
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }
#if NET || NET45 || NET46
        [Fact]
        public void Register_SearchPattern_CallsAssemblyScanner()
        {            
            var scannerMock = new AssemblyScannerMock();            
            var serviceContainer = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            serviceContainer.CompositionRootTypeExtractor = compositionRootExtractorMock;
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly("LightInject.Tests.dll");
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue), Invoked.Once);
        }
#endif

        [Fact]
        public void Register_AssemblyWithLifetimeFactory_RegistersServicesWithGivenLifeTime()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(IFoo).Assembly, () => new PerContainerLifetime());

            var service = container.AvailableServices.FirstOrDefault(sr => sr.ServiceType == typeof(IFoo));

            Assert.IsAssignableFrom(typeof(PerContainerLifetime), service.Lifetime);
        }
    }
}
