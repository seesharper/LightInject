using Xunit;
namespace LightInject.Tests
{
    
    public class ConstructorInjectionOptimizedTests : ConstructorInjectionTests
    {
        internal override IServiceContainer CreateContainer() 
            => new ServiceContainer((options => options.OptimizeForLargeObjectGraphs = true));
    }
}
