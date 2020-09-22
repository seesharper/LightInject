using System;
using LightMock;

namespace LightInject.Tests
{
    internal class CompositionRootExecutorMock : MockContext<ICompositionRootExecutor>, ICompositionRootExecutor
    {
        public void Execute(Type compositionRootType)
        {
            ((IInvocationContext<ICompositionRootExecutor>)this).Invoke(c => c.Execute(compositionRootType));
        }

        public void Execute<TCompositionRoot>(TCompositionRoot compositionRoot) where TCompositionRoot : ICompositionRoot
        {
            ((IInvocationContext<ICompositionRootExecutor>)this).Invoke(c => c.Execute(compositionRoot));
        }
    }
}