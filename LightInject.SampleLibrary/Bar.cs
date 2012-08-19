using System;

namespace LightInject.SampleLibrary
{
    public interface IBar
    {
    }

    public class Bar : IBar
    {
        [ThreadStatic]
        public static int InitializeCount = 0;

        public Bar()
        {
            InitializeCount++;
        }
    }

    public interface IBar<T> { }

    public class Bar<T> : IBar<T> { }

    public class BarWithFooDependency : IBar
    {
        public IFoo Foo { get; private set; }

        public BarWithFooDependency(IFoo foo)
        {
            Foo = foo;
        }
    }


    public class AnotherBar : IBar
    {
    }

    internal class BarFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            CallCount++;
            ServiceName = serviceRequest.ServiceName;
            return serviceRequest.CanProceed ? serviceRequest.Proceed() : new Bar();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof(IBar).IsAssignableFrom(serviceType);
        }

        public string ServiceName { get; private set; }

        public int CallCount { get; private set; }
    }
}