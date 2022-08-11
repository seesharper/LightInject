using System.Linq;
using Xunit;

namespace LightInject.Tests
{
    public class Issue257 : TestBase
    {
        [Fact]
        public void ShouldHandleMoreThanNineImplementationsForSameInterface()
        {
            var container = new ServiceContainer();
            container.Register<Foo0>();
            container.Register<Foo1>();
            container.Register<Foo2>();
            container.Register<Foo3>();
            container.Register<Foo4>();
            container.Register<Foo5>();
            container.Register<Foo6>();
            container.Register<Foo7>();
            container.Register<Foo8>();

            // Causes System.ArgumentException in
            // ILGenerator.Emit(OpCode code, Type type)
            // when creating an ArrayAccess expression
            // using a non-int32 type as index (in this
            // case, an sbyte)
            container.Register<Foo9>();

            var allInstances = container.GetAllInstances<IFoo>().ToList();
            Assert.Equal(10, allInstances.Count);
        }


        public interface IFoo { }
        public class Foo0 : IFoo { }
        public class Foo1 : IFoo { }
        public class Foo2 : IFoo { }
        public class Foo3 : IFoo { }
        public class Foo4 : IFoo { }
        public class Foo5 : IFoo { }
        public class Foo6 : IFoo { }
        public class Foo7 : IFoo { }
        public class Foo8 : IFoo { }
        public class Foo9 : IFoo { }
    }

}