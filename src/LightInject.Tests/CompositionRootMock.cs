using System.Threading.Tasks;
using LightMock;

namespace LightInject.Tests
{


    internal class CompositionRootMock : MockContext<ICompositionRoot>, ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            ((IInvocationContext<ICompositionRoot>) this).Invoke(c => c.Compose(serviceRegistry));
        }
    }

    internal class AnotherCompositionRootMock : MockContext<ICompositionRoot>, ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            ((IInvocationContext<ICompositionRoot>)this).Invoke(c => c.Compose(serviceRegistry));
        }
    }

    internal class AsyncCompositionRootMock : MockContext<IAsyncCompositionRoot>, IAsyncCompositionRoot
    {
        public Task ComposeAsync(IServiceRegistry serviceRegistry)
        {
            return ((IInvocationContext<IAsyncCompositionRoot>)this).Invoke(c => c.ComposeAsync(serviceRegistry));
        }
    }
}
