namespace LightInject.BenchMarks
{
    using BenchmarkDotNet.Attributes;

    internal class SingletonBenchmarks : ContainerTests
    {
        [Benchmark(Baseline = true)]
        public ISingleton1 LightInject()
        {
            return LightInjectContainer.GetInstance<ISingleton1>();
        }

        [Benchmark]
        public ISingleton1 DryIoc()
        {
            return LightInjectContainer.GetInstance<ISingleton1>();
        }

        [Benchmark]
        public ISingleton1 Grace()
        {
            return GraceContainer.Locate<ISingleton1>();
        }

    }

    public interface ISingleton1
    {

    }

    public interface ISingleton2
    {
    }

    public interface ISingleton3
    {
    }

    public class Singleton1 : ISingleton1
    {
    }

    public class Singleton2 : ISingleton2
    {
    }

    public class Singleton3 : ISingleton3
    {
    }
}