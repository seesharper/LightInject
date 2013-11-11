namespace LightInject.Wcf.SampleLibrary.Implementation
{
    using System;

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