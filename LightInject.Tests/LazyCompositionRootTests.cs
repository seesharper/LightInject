namespace LightInject.Tests
{
    using LightInject.SampleLibraryWithCompositionRootTypeAttribute;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LazyCompositionRootTests
    {
        [TestMethod] 
        public void GetInstance_UnknownService_ExecutesCompositionRootInSourceAssembly()
        {
            var container = new ServiceContainer();

            container.GetInstance<IFoo>();

            Assert.AreEqual(1, CompositionRoot.CallCount);
        }
    }
}