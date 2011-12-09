using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.SampleLibrary
{
    public interface IFoo { }

    public class Foo : IFoo { }

    public class AnotherFoo : IFoo { }

    public class FooWithDependency : IFoo
    {
        public FooWithDependency(IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; set; }
    }

    public class FooWithEnumerableDependency : IFoo
    {
        public FooWithEnumerableDependency(IEnumerable<IBar> bars)
        {
            Bars = bars;
        }

        public IEnumerable<IBar> Bars { get; private set; }
    }

    public class FooWithRecursiveDependency : IFoo
    {
        public FooWithRecursiveDependency(IFoo foo)
        {
        }
    }
                
    public interface IFoo<T> { }

    public class Foo<T> : IFoo<T> { }

    public class AnotherFoo<T> : IFoo<T> { }

    public class FooWithGenericDependency<T> : IFoo<T>
    {
        private readonly T _dependency;

        public FooWithGenericDependency(T dependency)
        {
            _dependency = dependency;
        }

        public T Dependency
        {
            get { return _dependency; }
        }
    }

    public class FooWithLazyDependency : IFoo
    {
        public FooWithLazyDependency(Lazy<IBar> lazyService)
        {
            LazyService = lazyService;
        }

        public Lazy<IBar> LazyService { get; private set; }
    }

    public class FooFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            ServiceRequest = serviceRequest;
            CallCount++;
            ServiceName = serviceRequest.ServiceName;
            return serviceRequest.CanProceed ? serviceRequest.Proceed() : new Foo();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof(IFoo).IsAssignableFrom(serviceType);
        }

        public ServiceRequest ServiceRequest { get; private set; }

        public string ServiceName { get; set; }

        public int CallCount { get; private set; }
    }

    public class GenericFooFactory : IFactory 
    {        
        
        
        public object GetInstance(ServiceRequest serviceRequest)
        {
            ServiceRequest = serviceRequest;
            return serviceRequest.CanProceed ? serviceRequest.Proceed() : new AnotherFoo<int>();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType.IsGenericType;
        }

        public ServiceRequest ServiceRequest { get; private set; }
    }


    public class FooWithMultipleConstructors
    {
        public FooWithMultipleConstructors()
        {
        }

        public FooWithMultipleConstructors(IBar bar)
        {
        }
    }
}
