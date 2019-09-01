using BenchmarkDotNet.Attributes;

namespace LightInject.Benchmarks
{
    [MemoryDiagnoser]
    public class BeginScopeBenchmarks
    {
        private IServiceContainer containerWithCurrentScopeEnabled;

        private IServiceContainer containerWithCurrentScopeDisabled;

        [GlobalSetup]
        public void Setup()
        {
            containerWithCurrentScopeEnabled = new ServiceContainer(new ContainerOptions() { EnableCurrentScope = true });
            containerWithCurrentScopeDisabled = new ServiceContainer(new ContainerOptions() { EnableCurrentScope = false });
        }

        [Benchmark]
        public void WithCurrentScopeEnabled()
        {
            using (var scope = containerWithCurrentScopeEnabled.BeginScope())
            {

            }
        }

        [Benchmark]
        public void WithCurrentScopeDisabled()
        {
            using (var scope = containerWithCurrentScopeDisabled.BeginScope())
            {

            }
        }
    }
}