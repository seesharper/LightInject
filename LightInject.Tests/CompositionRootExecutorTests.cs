namespace LightInject.Tests
{
    using LightInject.SampleLibraryWithCompositionRoot;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    [TestClass]
    public class CompositionRootExecutorTests
    {
        [TestMethod]
        public void Execute_CompositionRootType_IsCreatedAndExecuted()
        {
            SampleCompositionRoot.CallCount = 0;
            var serviceContainerMock = new Mock<IServiceContainer>();
            var executor = new CompositionRootExecutor(serviceContainerMock.Object);
            executor.Execute(typeof(SampleCompositionRoot));

            Assert.AreEqual(1, SampleCompositionRoot.CallCount);
        }

        [TestMethod]
        public void Execute_CompositionRootType_IsCreatedAndExecutedOnlyOnce()
        {
            SampleCompositionRoot.CallCount = 0;
            var serviceContainerMock = new Mock<IServiceContainer>();
            var executor = new CompositionRootExecutor(serviceContainerMock.Object);
            executor.Execute(typeof(SampleCompositionRoot));
            executor.Execute(typeof(SampleCompositionRoot));
            Assert.AreEqual(1, SampleCompositionRoot.CallCount);
        }
    }
}