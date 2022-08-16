
using Xunit;

namespace LightInject.Tests
{
    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class ServiceContainerOptimizedTests : ServiceContainerTests
    {
        internal override IServiceContainer CreateContainer() 
            => new ServiceContainer((options => options.OptimizeForLargeObjectGraphs = true));
    }
}
