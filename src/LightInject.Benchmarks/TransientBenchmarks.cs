namespace LightInject.BenchMarks
{
    using BenchmarkDotNet.Attributes;
    using DryIoc;
    using Grace.DependencyInjection;

    public class TransientBenchmarks
    {
        private readonly ServiceContainer container = new ServiceContainer();
        private readonly DryIoc.Container dryIocContainer = new Container();

        private readonly Grace.DependencyInjection.DependencyInjectionContainer graceContainer =
            new DependencyInjectionContainer();

        public TransientBenchmarks()
        {
            container
                .Register<ITransient1, Transient1>()
                .Register<ITransient2, Transient2>()
                .Register<ITransient3, Transient3>();

            dryIocContainer.Register<ITransient1, Transient1>();
            dryIocContainer.Register<ITransient2, Transient2>();
            dryIocContainer.Register<ITransient3, Transient3>();

            graceContainer.Configure(ioc =>
            {
                ioc.Export<Transient1>().As<ITransient1>();
                ioc.Export<Transient2>().As<ITransient2>();
                ioc.Export<Transient3>().As<ITransient3>();
            });
        }

        [Benchmark]
        public object DryIoc()
        {
            return dryIocContainer.Resolve(typeof(ITransient1));
        }

        [Benchmark]
        public object Grace()
        {
            return graceContainer.Locate(typeof(ITransient1));
        }

        [Benchmark(Baseline = true)]
        public object LightInject()
        {
            return container.GetInstance(typeof(ITransient1));
        }
    }



    public interface ITransient1
    {

    }

    public interface ITransient2
    {
    }

    public interface ITransient3
    {
    }

    public class Transient1 : ITransient1
    {
    }

    public class Transient2 : ITransient2
    {
    }

    public class Transient3 : ITransient3
    {
    }
}