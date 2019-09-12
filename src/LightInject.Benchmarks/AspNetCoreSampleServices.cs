using System;
using System.Threading;

namespace LightInject.Benchmarks
{

    public interface ISingleton1
    {
        void DoSomething();
    }


    public interface ISingleton2
    {
        void DoSomething();
    }


    public interface ISingleton3
    {
        void DoSomething();
    }

    public class Singleton1 : ISingleton1
    {
        private static int counter;

        public Singleton1()
        {
            System.Threading.Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
            Console.WriteLine("Hello");
        }
    }

    public class Singleton2 : ISingleton2
    {
        private static int counter;

        public Singleton2()
        {
            System.Threading.Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
            Console.WriteLine("Hello");
        }
    }

    public class Singleton3 : ISingleton3
    {
        private static int counter;

        public Singleton3()
        {
            System.Threading.Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
            Console.WriteLine("Hello");
        }
    }

    public interface IScopedService1
    {
        void DoSomething();
    }

    public class ScopedService1 : IScopedService1
    {
        private static int counter;

        public ScopedService1()
        {
            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public interface IScopedService2
    {
        void DoSomething();
    }

    public class ScopedService2 : IScopedService2
    {
        private static int counter;

        public ScopedService2()
        {
            //Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public interface IScopedService3
    {
        void DoSomething();
    }

    public class ScopedService3 : IScopedService3
    {
        private static int counter;

        public ScopedService3()
        {
            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public interface IScopedService4
    {
        void DoSomething();
    }

    public class ScopedService4 : IScopedService4
    {
        private static int counter;

        public ScopedService4()
        {
            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public interface IScopedService5
    {
        void DoSomething();
    }

    public class ScopedService5 : IScopedService5
    {
        private static int counter;

        public ScopedService5()
        {
            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public interface IRepositoryTransient1
    {
        void DoSomething();
    }

    public interface IRepositoryTransient2
    {
        void DoSomething();
    }

    public interface IRepositoryTransient3
    {
        void DoSomething();
    }

    public interface IRepositoryTransient4
    {
        void DoSomething();
    }

    public interface IRepositoryTransient5
    {
        void DoSomething();
    }

    public class RepositoryTransient1 : IRepositoryTransient1
    {
        private static int counter;



        public RepositoryTransient1(ISingleton1 singleton, IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (singleton == null)
            {
                throw new ArgumentNullException(nameof(singleton));
            }

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public class RepositoryTransient2 : IRepositoryTransient2
    {
        private static int counter;

        public RepositoryTransient2(ISingleton1 singleton, IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (singleton == null)
            {
                throw new ArgumentNullException(nameof(singleton));
            }

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public class RepositoryTransient3 : IRepositoryTransient3
    {
        private static int counter;

        public RepositoryTransient3(ISingleton1 singleton, IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (singleton == null)
            {
                throw new ArgumentNullException(nameof(singleton));
            }

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public class RepositoryTransient4 : IRepositoryTransient4
    {
        private static int counter;

        public RepositoryTransient4(ISingleton1 singleton, IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (singleton == null)
            {
                throw new ArgumentNullException(nameof(singleton));
            }

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public class RepositoryTransient5 : IRepositoryTransient5
    {
        private static int counter;

        public RepositoryTransient5(ISingleton1 singleton, IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (singleton == null)
            {
                throw new ArgumentNullException(nameof(singleton));
            }

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public void DoSomething()
        {
        }
    }

    public class TestController1 : IDisposable
    {
        private static int counter;

        private static int disposeCount;

        public TestController1(IRepositoryTransient1 transient1)
        { }

        public TestController1(
            IRepositoryTransient1 transient1,
            IRepositoryTransient2 repositoryTransient2,
            IRepositoryTransient3 repositoryTransient3,
            IRepositoryTransient4 repositoryTransient4,
            IRepositoryTransient5 repositoryTransient5)
        {
            if (transient1 == null)
            {
                throw new ArgumentNullException(nameof(transient1));
            }

            if (repositoryTransient2 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient2));
            }

            if (repositoryTransient3 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient3));
            }

            if (repositoryTransient4 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient4));
            }

            if (repositoryTransient5 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public static int DisposeCount
        {
            get { return disposeCount; }
            set { disposeCount = value; }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Interlocked.Increment(ref disposeCount);
        }
    }

    public class TestController2 : IDisposable
    {
        private static int counter;
        private static int disposeCount;

        public TestController2(
            IRepositoryTransient1 transient1,
            IRepositoryTransient2 repositoryTransient2,
            IRepositoryTransient3 repositoryTransient3,
            IRepositoryTransient4 repositoryTransient4,
            IRepositoryTransient5 repositoryTransient5)
        {
            if (transient1 == null)
            {
                throw new ArgumentNullException(nameof(transient1));
            }

            if (repositoryTransient2 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient2));
            }

            if (repositoryTransient3 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient3));
            }

            if (repositoryTransient4 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient4));
            }

            if (repositoryTransient5 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public static int DisposeCount
        {
            get { return disposeCount; }
            set { disposeCount = value; }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Interlocked.Increment(ref disposeCount);
        }
    }

    public class TestController3 : IDisposable
    {
        private static int counter;
        private static int disposeCount;

        public TestController3(
            IRepositoryTransient1 transient1,
            IRepositoryTransient2 repositoryTransient2,
            IRepositoryTransient3 repositoryTransient3,
            IRepositoryTransient4 repositoryTransient4,
            IRepositoryTransient5 repositoryTransient5)
        {
            if (transient1 == null)
            {
                throw new ArgumentNullException(nameof(transient1));
            }

            if (repositoryTransient2 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient2));
            }

            if (repositoryTransient3 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient3));
            }

            if (repositoryTransient4 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient4));
            }

            if (repositoryTransient5 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient5));
            }

            Interlocked.Increment(ref counter);
        }

        public static int Instances
        {
            get { return counter; }
            set { counter = value; }
        }

        public static int DisposeCount
        {
            get { return disposeCount; }
            set { disposeCount = value; }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Interlocked.Increment(ref disposeCount);
        }
    }
}