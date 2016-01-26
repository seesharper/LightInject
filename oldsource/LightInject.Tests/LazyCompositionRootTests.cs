namespace LightInject.Tests
{
    using LightInject.SampleLibraryWithCompositionRootTypeAttribute;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LazyCompositionRootTests
    {
        [TestMethod]
        public void Initialize_CompositionRootAttribute_ReturnsType()
        {
            var attribute = new CompositionRootTypeAttribute(typeof(CompositionRoot));

            Assert.AreEqual(typeof(CompositionRoot), attribute.CompositionRootType);
        }
                
        [TestMethod] 
        public void GetInstance_UnknownService_ExecutesCompositionRootInSourceAssembly()
        {
            CompositionRoot.CallCount = 0;
            var container = new ServiceContainer();

            container.GetInstance<IFoo>();

            Assert.AreEqual(1, CompositionRoot.CallCount);
        }


        

    }
}