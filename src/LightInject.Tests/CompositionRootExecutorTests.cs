namespace LightInject.Tests
{
    using LightMock;
    using Xunit;

    public class CompositionRootExecutorTests
    {
        [Fact]
        public void Execute_CompositionRootType_IsCreatedAndExecuted()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock);
            executor.Execute(typeof(CompositionRootMock));
            compositionRootMock.Assert(c => c.Compose(The<IServiceContainer>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Execute_CompositionRootType_IsCreatedAndExecutedOnlyOnce()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock);
            executor.Execute(typeof(CompositionRootMock));
            executor.Execute(typeof(CompositionRootMock));
            compositionRootMock.Assert(c => c.Compose(The<IServiceContainer>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Execute_Composition_IsExecuted()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock);
            executor.Execute(compositionRootMock);
            compositionRootMock.Assert(c => c.Compose(The<IServiceContainer>.IsAnyValue), Invoked.Once);
        }

        [Fact]
        public void Execute_Composition_IsExecutedOnlyOnce()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock);
            executor.Execute(compositionRootMock);
            executor.Execute(compositionRootMock);
            compositionRootMock.Assert(c => c.Compose(The<IServiceContainer>.IsAnyValue), Invoked.Once);
        }
    }
}
