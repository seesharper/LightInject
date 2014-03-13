namespace LightInject.Wcf.SampleLibrary.Implementation
{
    using System;
    using System.Runtime.Remoting.Messaging;
    using System.ServiceModel;
    using System.Threading.Tasks;

    public class Service : IService
    {
        public int Execute()
        {
            return 42;
        }
    }

    public class ServiceWithFuncDependency : IService
    {
        private readonly Func<IFoo> fooFactory;

        public ServiceWithFuncDependency(Func<IFoo> fooFactory)
        {
            this.fooFactory = fooFactory;
        }

        public int Execute()
        {
             Task.Run(
                async () =>
                    {
                        // Force the fooFactory to be executed on another thread.
                        await Task.Delay(10);
                        Foo = fooFactory();
                    }).Wait();
            return 42;
        }

        public IFoo Foo { get; private set; }
        
    }


    public class AsyncService : IAsyncService
    {
        private readonly Func<IFoo> fooFactory;

        public AsyncService(Func<IFoo> fooFactory)
        {
            this.fooFactory = fooFactory;
        }

        public async Task<IFoo> Execute()
        {
            await Task.Delay(10);

            return fooFactory(); //<--This code is executed on another thread.                                     
        }
    }

    public class ServiceWithSameDependencyTwice : IServiceWithSameDependencyTwice
    {
        public ServiceWithSameDependencyTwice(IFoo foo1, IFoo foo2)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PerCallInstanceAndSingleConcurrency : IPerCallInstanceAndSingleConcurrency
    {
        public PerCallInstanceAndSingleConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PerCallInstanceAndMultipleConcurrency : IPerCallInstanceAndMultipleConcurrency
    {
        public PerCallInstanceAndMultipleConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PerCallInstanceAndReentrantConcurrency : IPerCallInstanceAndReentrantConcurrency
    {
        public PerCallInstanceAndReentrantConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PerSessionInstanceAndSingleConcurrency : IPerSessionInstanceAndSingleConcurrency
    {
        public PerSessionInstanceAndSingleConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PerSessionInstanceAndMultipleConcurrency : IPerSessionInstanceAndMultipleConcurrency
    {
        public PerSessionInstanceAndMultipleConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PerSessionInstanceAndReentrantConcurrency : IPerSessionInstanceAndReentrantConcurrency
    {
        public PerSessionInstanceAndReentrantConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class SingleInstanceAndSingleConcurrency : ISingleInstanceAndSingleConcurrency
    {
        public SingleInstanceAndSingleConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SingleInstanceAndMultipleConcurrency : ISingleInstanceAndMultipleConcurrency
    {
        public SingleInstanceAndMultipleConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class SingleInstanceAndReentrantConcurrency : ISingleInstanceAndReentrantConcurrency
    {
        public SingleInstanceAndReentrantConcurrency(IFoo foo)
        {
        }

        public int Execute()
        {
            return 42;
        }
    }


    

    public class Foo : IFoo
    {

        public static int InitializeCount = 0;

        public Foo()
        {
            InitializeCount++;
        }
    }
}