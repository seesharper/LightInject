using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace LightInject.xUnit2.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using LightInject;
    using LightInject.SampleLibrary;

    using Xunit;

    public class XunitTestsWithConfigureMethod
    {
        [Theory, InjectData]
        public void MethodWithOneArgument(IFoo foo)
        {
            Assert.NotNull(foo);
        }

        [Theory, InjectData]
        public void MethodWithTwoArguments(IFoo foo, IBar bar)
        {
            Assert.NotNull(foo);
            Assert.NotNull(bar);
        }


        [Theory, InjectData("SomeValue")]
        public void MethodWithSingleValue(string value)
        {
            Assert.Equal("SomeValue", value);
        }

        [Theory, InjectData("Value1", "Value2")]
        public void MethodWithMultipleValues(string value1, string value2)
        {
            Assert.Equal("Value1", value1);
            Assert.Equal("Value2", value2);
        }


        [Theory, InjectData("value1"), InjectData("value2")]
        public void MethodWithMultipleInjectData(string value)
        {
            Assert.True(value == "value1" || value == "value2");
        }




        public void MethodWithMissingService(int value)
        {

        }

        [Fact]
        public void MissingServiceThrowsException()
        {
            var attribute = new InjectDataAttribute();
            var method = typeof(XunitTestsWithConfigureMethod).GetMethod("MethodWithMissingService");

            Assert.Throws<InvalidOperationException>(() => attribute.GetData(method));
        }

        internal static void Configure(IServiceContainer container)
        {
            Console.WriteLine("Configure" + Thread.CurrentThread.ManagedThreadId);
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();            
        }
    }


    public class XunitBaseClass
    {
        internal static void Configure(IServiceContainer container)
        {
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
        }
    }

    public class XunitTestsWithConfigureMethodInBaseClass : XunitBaseClass
    {
        [Theory, InjectData]
        public async void MethodWithOneArgument(IFoo foo)
        {
            await Task.Delay(100);
            Assert.NotNull(foo);
        }
    }

    public class XUnitTestsWithScopedServices
    {
        [Theory, Scoped, InjectData]
        public void MethodWithScopedArgument(IFoo foo)
        {
            Assert.NotNull(foo);
        }

        [Theory, Scoped, InjectData]
        public void MethodWithTwoScopedArguments(IFoo foo, IBar bar)
        {
            Assert.NotNull(foo);
        }

        internal static void Configure(IServiceContainer container)
        {
            container.Register<IFoo, DisposableFoo>(new PerScopeLifetime());
            container.Register<IBar, Bar>(new PerScopeLifetime());
        }
    }
}
