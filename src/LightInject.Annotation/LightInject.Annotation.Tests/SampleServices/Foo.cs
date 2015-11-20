using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LightInject.SampleLibrary
{
    using System.Threading.Tasks;

    public interface IFooWithProperty
    {
        string Value { get; }
    }

    public class FooWithProperty : IFooWithProperty
    {
        public string Value
        {
            get
            {
                return "SomeValue";
            }
        }
    }

    public class FooWithCompilerGeneratedType : IFoo
    {      
        public Func<string> SomeAction
        {
            get
            {
                string someString = "Somestring";
                return () => someString;
            }
        }

    }

#if NET45 || NET46

    public interface IAsyncFoo
    {
        Task<IBar> GetBar();
    }

    public class AsyncFoo : IAsyncFoo
    {
        private readonly Lazy<IBar> lazyBar;

        public AsyncFoo(Lazy<IBar> lazyBar)
        {
            this.lazyBar = lazyBar;
        }

        public async Task<IBar> GetBar()
        {
            await Task.Delay(10);
            return lazyBar.Value;
        }

        
    }

#endif    

    public interface IFoo { }


    public class SingletonFoo : IFoo
    {

        public static int Instances { get; set; }

        public SingletonFoo()
        {
            Instances++;
        }
    }

    

    public class Foo : IFoo
    {
        [ThreadStatic]
        private static int _instances;


        public static int Instances
        {
            get { return _instances; }
            set { _instances = value; }
        }

        public Foo()
        {
            Instances++;
        }
    }

    public class FooWithCallback : IFoo
    {
        public FooWithCallback(Action callback)
        {
            callback();
        }
    }


    public class LazyFoo : IFoo
    {

        public static int Instances { get; set; }

        public LazyFoo()
        {
            Instances++;
        }
    }

    public class DerivedFoo : Foo
    {
        
    }


    public class FooWithStaticDependency : IFoo
    {
        public static IBar Bar { get; set; }
    }

    public class FooMock : IFoo
    {
        
    }

    public class FooMock<T> : IFoo<T>{}


    public class AnotherFoo : IFoo { }

    public class FooWithDependency : IFoo
    {
        public FooWithDependency(IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; private set; }
    }

    public class AnotherFooWithDependency : IFoo
    {
        public AnotherFooWithDependency(IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; private set; }
    }

    public class FooWithAnnotatedDependency : IFoo
    {
        public FooWithAnnotatedDependency([Inject]IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; private set; }
    }

    public class FooWithNamedAnnotatedDependency : IFoo
    {
        public FooWithNamedAnnotatedDependency([Inject("AnotherBar")]IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; private set; }
    }

    public class FooWithEnumerableIFooDependency : IFoo
    {
        public IEnumerable<IFoo> FooList { get; private set; }

        public FooWithEnumerableIFooDependency(IEnumerable<IFoo> fooList)
        {
            FooList = fooList;
        }
    }
    
    public class FooWithEnumerableAndRegularDependency : IFoo
    {
        public IEnumerable<IBar> Bars { get; private set; }

        public IBar Bar { get; private set; }

        public FooWithEnumerableAndRegularDependency(IEnumerable<IBar> bars, IBar bar)
        {
            Bars = bars;
            Bar = bar;
        }
    }

    public class FooWithStaticProperty : IFoo
    {
        public static IBar Bar { get; set; }
    }


    public class FooWithSameDependencyTwice : IFoo
    {
        private readonly IBar m_bar1;

        private readonly IBar m_bar2;

        public FooWithSameDependencyTwice(IBar bar1, IBar bar2)
        {
            m_bar1 = bar1;
            m_bar2 = bar2;
        }

        public IBar Bar2
        {
            get
            {
                return this.m_bar2;
            }
        }

        public IBar Bar1
        {
            get
            {
                return this.m_bar1;
            }
        }
    }
    
    public class FooWithSampleServiceDependency : IFoo
    {
        public FooWithSampleServiceDependency(IBar bar, ISampleService sampleService)
        {
            SampleService = sampleService;
            Bar = bar;
        }
        public ISampleService SampleService { get; private set; }
        public IBar Bar { get; private set; }
    }


    public class BarWithSampleServiceDependency : IBar
    {
        public BarWithSampleServiceDependency(ISampleService sampleService)
        {
            SampleService = sampleService;
        }

        public ISampleService SampleService { get; private set; }
    }

    


    public class FooWithSamePropertyDependencyTwice : IFoo
    {
        public IBar Bar1 { get; set; }

        public IBar Bar2 { get; set; }
    }


    public class FooDecorator<T> : IFoo<T>
    {
        public FooDecorator(IFoo<T> foo)
        {
        }
    }


    public class ClosedGenericFooDecorator : IFoo<int>
    {
        public ClosedGenericFooDecorator(IFoo<int> foo)
        {
        }
    }

    public class FooDecoratorWithTargetAsPropertyDependency : IFoo
    {
        public IFoo Foo { get; set; }
    }



    public class AnotherFooDecorator<T> : IFoo<T>
    {
        public AnotherFooDecorator(IFoo<T> foo)
        {
        }
    }

    public class FooDecorator : IFoo
    {
        private readonly IFoo foo;

        public FooDecorator(IFoo foo)
        {
            this.foo = foo;
        }

        public IFoo DecoratedInstance
        {
            get
            {
                return this.foo;
            }
        }
    }

    public class BarDecorator : IBar
    {
        [ThreadStatic]
        public static int Instances;
        
        public BarDecorator(IBar bar)
        {
            Instances++;
        }
    }


    public class FooDecoratorWithDependency : IFoo
    {        
        public FooDecoratorWithDependency(IFoo foo, IBar bar)
        {
            Foo = foo;
            Bar = bar;
        }

        public IFoo Foo { get; private set; }

        public IBar Bar { get; private set; }
    }

    public class FooDecoratorWithDependencyFirst : IFoo
    {
        public FooDecoratorWithDependencyFirst(IBar bar, IFoo foo)
        {
            Foo = foo;
            Bar = bar;
        }

        public IFoo Foo { get; private set; }

        public IBar Bar { get; private set; }
    }


    public class AnotherFooDecorator : IFoo
    {
        private readonly IFoo foo;

        public AnotherFooDecorator(IFoo foo)
        {
            this.foo = foo;
        }

        public IFoo DecoratedInstance
        {
            get
            {
                return this.foo;
            }
        }
    }



    public class FooWithReferenceTypeDependency : IFoo
    {
        public FooWithReferenceTypeDependency(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }

    public class FooWithReferenceTypePropertyDependency : IFoo
    {
        public string Value { get; set; }
    }

    public class FooWithValueTypeDependency : IFoo
    {
        public FooWithValueTypeDependency(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }

    public class FooWithOneParameter : IFoo
    {
        public int Arg1 { get; private set; }

        public FooWithOneParameter(int arg1)
        {
            Arg1 = arg1;
        }
    }

    public class FooWithTwoParameters : IFoo
    {
        public int Arg1 { get; private set; }

        public int Arg2 { get; private set; }

        public FooWithTwoParameters(int arg1, int arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }

    public class FooWithThreeParameters : IFoo
    {
        public int Arg1 { get; private set; }

        public int Arg2 { get; private set; }

        public int Arg3 { get; private set; }

        public FooWithThreeParameters(int arg1, int arg2, int arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
    }

    public class FooWithFourParameters : IFoo
    {
        public int Arg1 { get; private set; }

        public int Arg2 { get; private set; }

        public int Arg3 { get; private set; }

        public int Arg4 { get; private set; }

        public FooWithFourParameters(int arg1, int arg2, int arg3, int arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
    }



    public class AnotherFooWithValueTypeDependency : IFoo
    {
        public AnotherFooWithValueTypeDependency(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }

    public class FooWithValueTypePropertyDependency : IFoo
    {
        public int Value { get; set; }
    }


    public class FooWithEnumDependency : IFoo
    {
        public FooWithEnumDependency(Encoding value)
        {
            Value = value;
        }

        public Encoding Value { get; private set; }
    }

    public class FooWithEnumPropertyDependency : IFoo
    {


        public Encoding Value { get; set; }
    }


    public class FooWithEnumerableDependency : IFoo
    {
        public FooWithEnumerableDependency(IEnumerable<IBar> bars)
        {
            Bars = bars;
        }

        public IEnumerable<IBar> Bars { get; private set; }
    }

    public class FooWithArrayDependency : IFoo
    {
        public FooWithArrayDependency(IBar[] bars)
        {
            Bars = bars;
        }

        public IEnumerable<IBar> Bars { get; private set; }
    }

    public class FooWithParamsArrayDependency : IFoo
    {
        public FooWithParamsArrayDependency(IBar[] bars)
        {
            Bars = bars;
        }

        public IEnumerable<IBar> Bars { get; private set; }
    }

    public class FooWithEnumerablePropertyDependency : IFoo
    {
        public FooWithEnumerablePropertyDependency()
        {            
        }

        public IEnumerable<IBar> Bars { get; set; }
    }

    public class FooWithRecursiveDependency : IFoo
    {
        public FooWithRecursiveDependency(IFoo foo)
        {
        }
    }

    public class BarWithPropertyDependency : IBar
    {
        public IFoo Foo { get; set; }
    }

    public interface IFoo<T1, T2> { }

    public class FooWithPartiallyClosedGenericInterface<T> : IFoo<T, string> { }

    public interface IFoo<T> { }

    public class Foo<T> : IFoo<T> { }

    public class AnotherFoo<T> : IFoo<T> { }

    public class FooWithGenericDependency<T> : IFoo<T>
    {        
        public FooWithGenericDependency(T dependency)
        {
            Dependency = dependency;
        }

        public T Dependency { get; private set; }        
    }

    public class FooWithOpenGenericDependency<T> : IFoo<T>
    {
        private readonly IBar<T> dependency;

        public FooWithOpenGenericDependency(IBar<T> dependency)
        {
            this.dependency = dependency;
        }

        public IBar<T> Dependency
        {
            get { return dependency; }
        }
    }

    public class FooWithSameOpenGenericDependencyTwice<T> : IFoo<T>
    {
        private readonly IBar<T> bar1;

        private readonly IBar<T> bar2;

        public FooWithSameOpenGenericDependencyTwice(IBar<T> bar1, IBar<T> bar2)
        {
            this.bar1 = bar1;
            this.bar2 = bar2;
        }

        public IBar<T> Bar1
        {
            get { return bar1; }
        }

        public IBar<T> Bar2
        {
            get { return bar2; }
        }
    }


    public class FooWithGenericPropertyDependency<T> : IFoo<T>
    {
        public T Dependency { get; set; }
    }


    public class FooWithStringTypeParameter : IFoo<string> {}
    

    public class FooWithLazyDependency : IFoo
    {
        public FooWithLazyDependency(Lazy<IBar> lazyService)
        {
            LazyService = lazyService;
        }

        public Lazy<IBar> LazyService { get; private set; }
    }

    public class FooWithTwoConstructors : IFoo
    {
        public FooWithTwoConstructors(int value)
        {
        }

        public FooWithTwoConstructors(string value)
        {
        }
    }

    public class FooWithIndexer
    {
        public object this[int index]
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
    }

    public class FooWithObjectProperty
    {
        public object Property { get; set; }
    }


    public class FooWithMultipleConstructors : IFoo
    {
        public FooWithMultipleConstructors()
        {
        }

        public FooWithMultipleConstructors(IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; private set; }
    }

    public class FooWithMultipleParameterizedConstructors
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }

        public FooWithMultipleParameterizedConstructors([Inject("SomeValue")]int intValue)
        {
            IntValue = intValue;
        }

        public FooWithMultipleParameterizedConstructors(string stringValue)
        {
            StringValue = stringValue;
        }
    }


    public class FooWithProperyDependency : IFoo
    {
        public IBar Bar { get; set; }
    }

    public class FooWithFuncPropertyDependency : IFoo
    {
        public Func<IBar> BarFunc { get; set; }
    }


    public class FooWithInheritedProperyDepenency : FooWithProperyDependency {}

    public class FooWithAnnotatedProperyDependency : IFoo
    {
        [Inject]
        public IBar Bar { get; set; }
    }

    public class FooWithNamedAnnotatedProperyDependency : IFoo
    {
        [Inject("AnotherBar")]
        public IBar Bar { get; set; }
    }

    public class FooWithFuncDependency : IFoo
    {
        public FooWithFuncDependency(Func<IBar> getBar)
        {
            GetBar = getBar;
        }
        public Func<IBar> GetBar { get; private set; } 
    }

    public class FooWithNamedFuncDependency : IFoo
    {
        public FooWithNamedFuncDependency(Func<string, IBar> getBar)
        {
            GetBar = getBar;
        }
        public Func<string,IBar> GetBar { get; private set; }
    }

    public class FooWithCustomFuncDependency : IFoo
    {
        public Func<string> StringFunc { get; private set; }

        public FooWithCustomFuncDependency(Func<string> stringFunc)
        {
            StringFunc = stringFunc;
        }
    }

    public class DisposableFoo : IFoo, IDisposable
    {
        public bool IsDisposed { get; private set; }
        
        public void Dispose()
        {
            IsDisposed = true;            
        }
    }

    public class ConcreteFoo
    {
        
    }

    public class ConcreteFooWithBaseClass : Foo
    {

    }


    public class LazyFooDecorator : IFoo
    {
        public Lazy<IFoo> Foo { get; private set; }

        public LazyFooDecorator(Lazy<IFoo> foo)
        {
            Foo = foo;
        }
    }

    public class AnotherLazyFooDecorator : IFoo
    {
        public Lazy<IFoo> Foo { get; private set; }

        public AnotherLazyFooDecorator(Lazy<IFoo> foo)
        {
            Foo = foo;
        }
    }

    public class FooWithConstructorAndPropertyDependency : IFoo
    {
        public FooWithConstructorAndPropertyDependency(IBar bar)
        {
            ConstructorInjectedBar = bar;
        }

        public IBar ConstructorInjectedBar { get; private set; }

        public IBar Bar { get; set; }
    }

    internal class InternalFooWithPublicConstructor : IFoo
    {
        public InternalFooWithPublicConstructor () {}
    }

    internal class InternalFooWithInternalConstructor : IFoo
    {
        internal InternalFooWithInternalConstructor() { }
    }



    public interface IFooFactory
    {
        IFoo CreateFoo();
    }

    public class FooFactory : IFooFactory
    {
        private readonly Func<IFoo> getFoo;

        public FooFactory(Func<IFoo> getFoo)
        {
            this.getFoo = getFoo;
        }

        public IFoo CreateFoo()
        {
            return getFoo();
        }
    }


    public class FooWithDependencyAndArgument : IFoo
    {
        public int Value { get; private set; }

        public IBar Bar { get; private set; }

        public FooWithDependencyAndArgument(IBar bar, int value)
        {
            Value = value;
            Bar = bar;
        }
    }


    public abstract class AbstractFoo : IFoo {}
    

    public class FooWithNestedPrivate : IFoo
    {
        private class NestedPrivateBar
        {
            
        }
    }

    public class FooWithGenericConstraint<T> : IFoo<T> where T:IBar
    {
        
    }

    public interface IFooWithGenericInterfaceConstraint<T,I> where T: IBar<I>
    {
        
    }

    public class FooWithGenericInterfaceConstraint<T,I> : IFooWithGenericInterfaceConstraint<T, I>
        where T : IBar<I>
    {
    }

    public class FooWithSameHashCode
    {
        public FooWithSameHashCode(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public override bool Equals(object obj)
        {
            return ((FooWithSameHashCode)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
    
    public class FooWithBrokenDependency
    {
        public FooWithBrokenDependency(IBar bar)
        { }
    }

    
    public class DisposableLifetime : ILifetime, IDisposable
    {
        public bool IsDisposed { get; set; }
        
        public void Dispose()
        {
            IsDisposed = true;
        }

        object ILifetime.GetInstance(Func<object> createInstance, Scope scope)
        {
            return createInstance();
        }
    }

    public class FooCollection<T> : ICollection<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
    }
}
