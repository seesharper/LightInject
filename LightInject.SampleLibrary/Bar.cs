using System;

namespace LightInject.SampleLibrary
{
    public class FooWithStaticConstructor
    {
        private FooWithStaticConstructor()
        {
        }

        static FooWithStaticConstructor()
        {
            
        }
    }
    
    public class FooWithPrivateConstructor
    {
        private FooWithPrivateConstructor()
        {
        }
    }

    
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

    public class BarMock : IBar
    {
        
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

    public class BrokenBar : IBar 
    { 
        public BrokenBar()
        {
            throw new BrokenBarException();
        }
    }

    public class BrokenBarException : Exception { }
}