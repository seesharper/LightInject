namespace LightInject.Wcf.SampleLibrary.Implementation
{
    using System;
    using System.ServiceModel;

    public class Service : IService
    {
        public int Execute()
        {
            return 42;
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


    public interface IFoo {}
    

    public class Foo : IFoo
    {

        public static int InitializeCount = 0;

        public Foo()
        {
            InitializeCount++;
        }
    }
}