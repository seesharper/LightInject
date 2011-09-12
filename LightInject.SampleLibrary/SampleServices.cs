using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightInject.SampleLibrary
{
    public interface IService {}

    public class Service : IService { }

    public class AnotherService : IService { }

    public class ServiceWithDependency : IService
    {
        public ServiceWithDependency(IFoo foo)
        {
            Foo = foo;
        }

        public IFoo Foo { get; private set; }
    }

    public class ServiceWithSingletonDependency : IService
    {
        public ServiceWithSingletonDependency(ISingleton singleton)
        {
            Singleton = singleton;
        }

        public ISingleton Singleton { get; private set; }
    }

    public class ServiceWithEnumerableDependency : IService
    {
        private readonly IEnumerable<IFoo> _foos;

        public ServiceWithEnumerableDependency(IEnumerable<IFoo> foos)
        {
            _foos = foos;
        }

        public IEnumerable<IFoo> Foos
        {
            get { return _foos; }
        }
    }



    

    public class ServiceWithLazyDependency : IService
    {
        public ServiceWithLazyDependency(Lazy<IFoo> lazyService)
        {
            LazyService = lazyService;
        }

        public Lazy<IFoo> LazyService { get; private set; }
    }

    public interface IFoo {}
    
    public class Foo : IFoo {}
    
    public class Foo1 : IFoo {}
    
    public class Foo2 : IFoo {}
    
    public class Foo3 : IFoo {}
    
    public class FooWithDependency : IFoo
    {
        public FooWithDependency(IService service) {}        
    }


    public interface ISingleton {}

    public class Singleton : ISingleton {} 

    public class  ServiceWithRecursiveDependency : IService
    {
        public ServiceWithRecursiveDependency(IFoo foo) {}        
    }

    
    public class CloneableClass : ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
    
    public interface IService<T> {}

    public class Service<T> : IService<T> { }    
    public class AnotherOpenGenericClass<T> : IService<T> {}



    public class ServiceWithGenericDependency<T> : IService<T>
    {
        private readonly T _dependency;

        public ServiceWithGenericDependency(T dependency)
        {
            _dependency = dependency;
        }

        public T Dependency
        {
            get { return _dependency; }
        }
    }

    public class ClosedGenericClass : IService<string> {}

}
