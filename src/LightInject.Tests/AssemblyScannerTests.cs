
using System.Reflection;

namespace LightInject.Tests
{
    using System;
    using System.Collections;
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
                   
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), compositionRootTypeExtractorMock, new CompositionRootExecutor(containerMock,t => compositionRootMock), new GenericArgumentMapper());
            assemblyScanner.Scan(typeof(IFoo).GetTypeInfo().Assembly, containerMock, lifetimeFactory, shouldRegister, new ServiceNameProvider().GetServiceName);
            return containerMock;
        }
        private MockContext<IServiceContainer> GetContainerMock(Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister, Func<Type, Type, string> serviceNameProvider)
        {
            var containerMock = new ContainerMock();
            var compositionRootMock = new CompositionRootMock();

            var compositionRootTypeExtractorMock = new TypeExtractorMock();
            compositionRootTypeExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);

            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), compositionRootTypeExtractorMock, new CompositionRootExecutor(containerMock, t => compositionRootMock), new GenericArgumentMapper());
            assemblyScanner.Scan(typeof(IFoo).GetTypeInfo().Assembly, containerMock, lifetimeFactory, shouldRegister, serviceNameProvider);
            return containerMock;
        }

        private MockContext<IServiceContainer> GetContainerMock(Type type)
        {
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            var concreteTypeExtractorMock = new TypeExtractorMock();
            concreteTypeExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new Type[] { type });
            var scanner = new AssemblyScanner(concreteTypeExtractorMock, compositionRootExtractorMock, null,
                new GenericArgumentMapper());
            var containerMock = new ContainerMock();

            scanner.Scan(typeof(IFoo).GetTypeInfo().Assembly, containerMock, () => null, (st, ip) => true, new ServiceNameProvider().GetServiceName);
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
            this.GetContainerMock(() => null, (s,t) => true).Assert(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "LightInject.SampleLibrary.AnotherFoo", null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresNamedUsingCustomServiceNameFunction()
        {
            this.GetContainerMock(() => null, (s, t) => true, (serviceName, implementingType) => "CustomName").Assert(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "CustomName", null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            this.GetContainerMock(() => null, (s, t) => true).Assert(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty, null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            this.GetContainerMock(() => null, (s,t) => true).Assert(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "LightInject.SampleLibrary.AnotherFoo", null), Invoked.Once);
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresAccordingToPredicate()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(AssemblyScannerTests).GetTypeInfo().Assembly, (s,i) => s == typeof(IFoo));

            Assert.Contains(container.AvailableServices, sr => sr.ServiceType == typeof(IFoo));
            Assert.DoesNotContain(container.AvailableServices, sr => sr.ServiceType == typeof(IBar));
        }

        [Fact]
        public void Scan_SampleAssembly_ConfiguresAllServicesByDefault()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(AssemblyScannerTests).GetTypeInfo().Assembly);

            Assert.Contains(container.AvailableServices, sr => sr.ServiceType == typeof(IFoo));
            Assert.Contains(container.AvailableServices, sr => sr.ServiceType == typeof(IBar));
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
                new CompositionRootExecutor(containerMock, t => compositionRootMock), new GenericArgumentMapper());
            
            assemblyScanner.Scan(typeof(AssemblyScannerTests).GetTypeInfo().Assembly, containerMock);

            compositionRootMock.Assert(c => c.Compose(containerMock), Invoked.Once);
        }

        [Fact]
        public void ScanUsingPredicate_SampleAssemblyWithCompositionRoot_DoesNotCallCompositionRoot()
        {
            var compositionRootMock = new CompositionRootMock();
            var containerMock = new ContainerMock();
            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(),
                new CompositionRootTypeExtractor(new CompositionRootAttributeExtractor()),
                new CompositionRootExecutor(containerMock, t => compositionRootMock), new GenericArgumentMapper());

            assemblyScanner.Scan(typeof(AssemblyScannerTests).GetTypeInfo().Assembly, containerMock, () => null, (s, t) => true, new ServiceNameProvider().GetServiceName);
            
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
            container.RegisterAssembly(typeof(Foo).GetTypeInfo().Assembly);
            Assert.DoesNotContain(container.AvailableServices, si => si.ImplementingType != null && si.ImplementingType.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), false));
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterAbstractTypes()
        {            
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(Foo).GetTypeInfo().Assembly);
            Assert.DoesNotContain(container.AvailableServices, si => si.ImplementingType == typeof(AbstractFoo));
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterSystemTypes()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(string).GetTypeInfo().Assembly);
            Assert.Empty(container.AvailableServices);
        }

        [Fact]
        public void Scan_SampleAssembly_DoesNotRegisterNestedPrivateTypes()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(Foo).GetTypeInfo().Assembly);
            Assert.DoesNotContain(container.AvailableServices, si => si.ImplementingType.Name == "NestedPrivateBar");
        }


        [Fact]
        public void Scan_HostAssembly_DoesNotConfigureInternalServices()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] { typeof(CompositionRootMock) });
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(ServiceContainer).GetTypeInfo().Assembly);
            var result = container.AvailableServices.Where(si => si.ImplementingType.Namespace == "LightInject");
            Assert.DoesNotContain(container.AvailableServices, si => si.ImplementingType != null && si.ImplementingType.Namespace == "LightInject");
        }
        
        //[Fact]
        //public void GetInstance_FallBackRegisteredInScannedAssembly_ReturnsInstance()
        //{            	        
        //    var container = new ServiceContainer();            
        //    var instance = container.GetInstance<SampleLibraryWithCompositionRootTypeAttribute.IBar>();
        //    Assert.NotNull(instance);            
        //}


#if NET40 || NET452 || NET46         
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
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly,The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue, The<Func<Type, Type, string>>.IsAnyValue), Invoked.Once);
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
            serviceContainer.RegisterAssembly(typeof(IFoo).GetTypeInfo().Assembly);
            scannerMock.Assert(a => a.Scan(typeof(IFoo).GetTypeInfo().Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue, The<Func<Type, Type, string>>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Register_OpenGeneric_DoesNotRegisterInvalidAbstraction()
        {
            GetContainerMock(typeof (List<>))
                .Assert(
                    c =>
                        c.Register(
                            typeof (ICollection), 
                            typeof (List<>), 
                            The<string>.IsAnyValue,
                            The<ILifetime>.IsAnyValue), Invoked.Never);                        
        }


        [Fact]
        public void Register_Assembly_RegistersConcreteTypeWithoutBaseclass()
        {            
            GetContainerMock(() => null, (s, t) => true).Assert(r => r.Register(typeof(ConcreteFoo), typeof(ConcreteFoo), "LightInject.SampleLibrary.ConcreteFoo", null));
        }

        [Fact]
        public void Register_Assembly_RegistersConcreteTypeWithBaseclass()
        {           
            GetContainerMock(() => null, (s, t) => true).Assert(r => r.Register(typeof(Foo), typeof(ConcreteFooWithBaseClass), "LightInject.SampleLibrary.ConcreteFooWithBaseClass", null));
        }

        [Fact]
        public void Register_AssemblyWithFunc_CallsAssemblyScanner()
        {
            var scannerMock = new AssemblyScannerMock();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).GetTypeInfo().Assembly, (s,t) => true);
            scannerMock.Assert(a => a.Scan(typeof(IFoo).GetTypeInfo().Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue, The<Func<Type, Type, string>>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Register_AssemblyWithFuncAndLifeCycle_CallsAssemblyScanner()
        {            
            var scannerMock = new AssemblyScannerMock();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock;
            serviceContainer.RegisterAssembly(typeof(IFoo).GetTypeInfo().Assembly, () => new PerContainerLifetime(), (s, t) => true);
            scannerMock.Assert(a => a.Scan(typeof(IFoo).GetTypeInfo().Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue, The<Func<Type, Type, string>>.IsAnyValue), Invoked.Once);
        }
#if NET40 || NET452 || NET46
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
            scannerMock.Assert(a => a.Scan(typeof(IFoo).Assembly, The<IServiceRegistry>.IsAnyValue, The<Func<ILifetime>>.IsAnyValue, The<Func<Type, Type, bool>>.IsAnyValue, The<Func<Type, Type, string>>.IsAnyValue), Invoked.Once);
        }
#endif

        [Fact]
        public void Register_AssemblyWithLifetimeFactory_RegistersServicesWithGivenLifeTime()
        {
            var container = new ServiceContainer();
            container.RegisterAssembly(typeof(IFoo).GetTypeInfo().Assembly, () => new PerContainerLifetime());

            var service = container.AvailableServices.FirstOrDefault(sr => sr.ServiceType == typeof(IFoo));

            Assert.IsAssignableFrom<PerContainerLifetime>(service.Lifetime);
        }
    }
}
