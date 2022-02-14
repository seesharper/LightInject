using Xunit;

namespace LightInject.Tests
{
    public class Issue237 : TestBase
    {
        [Fact]
        public void ShouldAutoResolveRemainingArguments()
        {
            var container = CreateContainer();
            container.Register<IBar1, Bar1>();
            container.Register<IBar2, Bar2>();
            container.Register<IFoo, Foo>();

            container.RegisterConstructorDependency((factory, info, runtimeArguments) => (string)(runtimeArguments[0]));
            var firstInstance = container.GetInstance<string, IFoo>("SomeValue");
            var secondInstance = container.GetInstance<string, IFoo>("AnotherValue");

            Assert.Equal("SomeValue", ((Foo)firstInstance).Value);
            Assert.Equal("AnotherValue", ((Foo)secondInstance).Value);
        }


        public interface IFoo
        {
        }
        public interface IBar1
        {
        }
        public interface IBar2
        {
        }

        public class Bar1 : IBar1
        {

        }

        public class Bar2 : IBar2
        {

        }

        public class Foo : IFoo
        {
            public string Value { get; private set; }
            public IBar1 Bar1 { get; private set; }
            public IBar2 Bar2 { get; private set; }

            public Foo(string value, IBar1 bar1, IBar2 bar2)
            {
                Value = value;
                Bar1 = bar1;
                Bar2 = bar2;
            }
        }
    }
}
