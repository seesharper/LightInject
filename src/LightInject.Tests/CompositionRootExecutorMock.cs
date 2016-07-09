using System;
using LightMock;

namespace LightInject.Tests
{
    using System.Threading.Tasks;

    internal class CompositionRootExecutorMock : MockContext<ICompositionRootExecutor>, ICompositionRootExecutor
    {
        public void Execute(Type compositionRootType)
        {
            ((IInvocationContext<ICompositionRootExecutor>) this).Invoke(c => c.Execute(compositionRootType));
        }

        public Task ExecuteAsync(Type compositionRootType)
        {
            ((IInvocationContext<ICompositionRootExecutor>) this).Invoke(c => c.ExecuteAsync(compositionRootType));
            return Task.CompletedTask;
        }
    }
}