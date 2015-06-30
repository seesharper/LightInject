using System;
using LightMock;

namespace LightInject.Tests
{
    public class CompositionRootExecutorMock : MockContext<ICompositionRootExecutor>, ICompositionRootExecutor
    {
        public void Execute(Type compositionRootType)
        {
            ((IInvocationContext<ICompositionRootExecutor>) this).Invoke(c => c.Execute(compositionRootType));
        }
    }
}