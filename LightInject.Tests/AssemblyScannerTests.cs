namespace LightInject.Tests
{
    using LightInject;
    using LightInject.SampleLibrary;
    using LightInject.SampleLibraryWithCompositionRoot;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AssemblyScannerTests
    {
        private Mock<IServiceContainer> GetContainerMock()
        {
            var containerMock = new Mock<IServiceContainer>();
            var assemblyScanner = new AssemblyScanner();
            assemblyScanner.Scan(typeof(IFoo).Assembly, containerMock.Object);
            return containerMock;
        }
        
        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultService()
        {            
            this.GetContainerMock().Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty), Times.Once());
        }
        
        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedService()
        {            
            this.GetContainerMock().Verify(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo"), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            this.GetContainerMock().Verify(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            this.GetContainerMock().Verify(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo"), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembkyWithCompositionRoot_CallsComposeMethod()
        {            
            var assemblyScanner = new AssemblyScanner();
            Mock<IServiceContainer> containerMock = new Mock<IServiceContainer>();
            SampleCompositionRoot.CallCount = 0;
            assemblyScanner.Scan(typeof(SampleCompositionRoot).Assembly, containerMock.Object);
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
                scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>()), Times.Once());                     
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
                scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>()), Times.Once());
            }
        }

        [TestMethod]
        public void Register_AssemblyFile_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.Scan("*SampleLibrary.dll");
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<IServiceRegistry>()), Times.Once());
        }
    }
}
