namespace LightInject.Tests
{
    using LightInject.SampleLibraryWithCompositionRoot;

    using LightMock;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    [TestClass]
    public class CompositionRootExecutorTests
    {
        [TestMethod]
        public void Execute_CompositionRootType_IsCreatedAndExecuted()
        {
            SampleCompositionRoot.CallCount = 0;

            var serviceContainerMock = new ContainerMock(new MockContext<IServiceContainer>());
            var executor = new CompositionRootExecutor(serviceContainerMock);
            executor.Execute(typeof(SampleCompositionRoot));

            Assert.AreEqual(1, SampleCompositionRoot.CallCount);
        }

        [TestMethod]
        public void Execute_CompositionRootType_IsCreatedAndExecutedOnlyOnce()
        {
            SampleCompositionRoot.CallCount = 0;
            var serviceContainerMock = new ContainerMock(new MockContext<IServiceContainer>());
            var executor = new CompositionRootExecutor(serviceContainerMock);
            executor.Execute(typeof(SampleCompositionRoot));
            executor.Execute(typeof(SampleCompositionRoot));
            Assert.AreEqual(1, SampleCompositionRoot.CallCount);
        }
    }
}