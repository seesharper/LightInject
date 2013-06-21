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

    
}