namespace LightInject.Tests
{
    using System;

    using LightInject;
    using LightInject.SampleLibrary;
    using LightInject.SampleLibraryWithCompositionRoot;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AssemblyScannerTests
    {
        private Mock<IServiceContainer> GetContainerMock(LifeCycleType lifeCycleType, Func<Type, bool> shouldRegister)
        {
            var containerMock = new Mock<IServiceContainer>();
            var assemblyScanner = new AssemblyScanner();
            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock.Object, lifeCycleType, shouldRegister);
            return containerMock;
        }
        
        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultService()
        {            
            this.GetContainerMock(LifeCycleType.Transient, t => true).Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, LifeCycleType.Transient), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresServiceWithGivenLifeCycleType()
        {
            this.GetContainerMock(LifeCycleType.Request, t => true).Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, LifeCycleType.Request), Times.Once());
        }
        
        [TestMethod]
        public void Scan_SampleAssembly_DoesNotConfigureServiceFilteredByDelegate()
        {
            this.GetContainerMock(LifeCycleType.Request, t => t.Name != "Foo").Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty, LifeCycleType.Request), Times.Never());
        }


        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedService()
        {
            this.GetContainerMock(LifeCycleType.Transient, t => true).Verify(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo", LifeCycleType.Transient), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            this.GetContainerMock(LifeCycleType.Transient, t => true).Verify(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty, LifeCycleType.Transient), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            this.GetContainerMock(LifeCycleType.Transient, t => true).Verify(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo", LifeCycleType.Transient), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembkyWithCompositionRoot_CallsComposeMethod()
        {            
            var assemblyScanner = new AssemblyScanner();
            var containerMock = new Mock<IServiceContainer>();
            SampleCompositionRoot.CallCount = 0;
            assemblyScanner.Scan(typeof(SampleCompositionRoot).Assembly, containerMock.Object, LifeCycleType.Transient, t => true);
            Assert.AreEqual(1, SampleCompositionRoot.CallCount);
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
                scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), LifeCycleType.Transient, It.IsAny<Func<Type,bool>>()), Times.Once());                     
            }                             
        }

        [TestMethod]
        public void GetInstance_NoServices_CallsAssemblyScannerOnlyOnce()
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
                try
                {
                    serviceContainer.GetInstance<IFoo>();
                }
                catch
                {

                }
            }
            finally
            {
                scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), LifeCycleType.Transient, It.IsAny<Func<Type, bool>>()), Times.Once());
            }
        }

        [TestMethod]
        public void Register_AssemblyFile_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly("*SampleLibrary.dll");
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), LifeCycleType.Transient, It.IsAny<Func<Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_Assembly_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), LifeCycleType.Transient, It.IsAny<Func<Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_AssemblyWithFunc_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly, t => true);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), LifeCycleType.Transient, It.IsAny<Func<Type, bool>>()), Times.Once());
        }

        [TestMethod]
        public void Register_AssemblyWithFuncAndLifeCycle_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.RegisterAssembly(typeof(IFoo).Assembly,LifeCycleType.Singleton, t => true);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>(), LifeCycleType.Singleton, It.IsAny<Func<Type, bool>>()), Times.Once());
        }

    }
}
