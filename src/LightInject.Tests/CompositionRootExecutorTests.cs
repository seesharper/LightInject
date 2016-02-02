namespace LightInject.Tests
{
    using System.Threading.Tasks;
    using LightMock;
    using Xunit;

    public class CompositionRootExecutorTests
    {
        [Fact]
        public void Execute_CompositionRootType_IsCreatedAndExecuted()
        {            
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            AsyncCompositionRootMock asyncCompositionRootMock = new AsyncCompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock, (t) => asyncCompositionRootMock);
            executor.Execute(typeof(CompositionRootMock));
            compositionRootMock.Assert(c => c.Compose(The<IServiceContainer>.IsAnyValue), Invoked.Once);            
        }

        [Fact]
        public void Execute_CompositionRootType_IsCreatedAndExecutedOnlyOnce()
        {            
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            AsyncCompositionRootMock asyncCompositionRootMock = new AsyncCompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock, (t) => asyncCompositionRootMock);
            executor.Execute(typeof(CompositionRootMock));
            executor.Execute(typeof(CompositionRootMock));
            compositionRootMock.Assert(c => c.Compose(The<IServiceContainer>.IsAnyValue), Invoked.Once);           
        }

        [Fact]
        public async Task ExecuteAsync_CompositionRootType_IsCreatedAndExecuted()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            AsyncCompositionRootMock asyncCompositionRootMock = new AsyncCompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock, (t) => asyncCompositionRootMock);
            await executor.ExecuteAsync(typeof(AsyncCompositionRootMock));
            asyncCompositionRootMock.Assert(c => c.ComposeAsync(The<IServiceContainer>.IsAnyValue), Invoked.Once);            
        }

        [Fact]
        public async Task ExecuteAsync_CompositionRootType_IsCreatedAndExecutedOnlyOnce()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            AsyncCompositionRootMock asyncCompositionRootMock = new AsyncCompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock, (t) => asyncCompositionRootMock);
            await executor.ExecuteAsync(typeof(AsyncCompositionRootMock));
            await executor.ExecuteAsync(typeof(AsyncCompositionRootMock));
            asyncCompositionRootMock.Assert(c => c.ComposeAsync(The<IServiceContainer>.IsAnyValue), Invoked.Once);            
        }

        [Fact]
        public async Task ExecuteAsync_CompositionRootType_IsCreatedAndExecutedOnlyOnceWithParallel()
        {
            CompositionRootMock compositionRootMock = new CompositionRootMock();
            AsyncCompositionRootMock asyncCompositionRootMock = new AsyncCompositionRootMock();
            var serviceContainerMock = new ContainerMock();
            var executor = new CompositionRootExecutor(serviceContainerMock, (t) => compositionRootMock, (t) => asyncCompositionRootMock);

            var first = executor.ExecuteAsync(typeof(AsyncCompositionRootMock));
            var second = executor.ExecuteAsync(typeof(AsyncCompositionRootMock));

            await Task.WhenAll(first, second);

            asyncCompositionRootMock.Assert(c => c.ComposeAsync(The<IServiceContainer>.IsAnyValue), Invoked.Once);            
        }
    }
}