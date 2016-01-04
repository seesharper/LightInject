using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace LightInject.xUnit2.Tests
{
    using System;
    using System.Runtime.CompilerServices;
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

        [Theory, InjectData("value1")]
        public void MethodWithServiceAndValue(IFoo foo, string value)
        {
            Assert.IsType<Foo>(foo);
            Assert.Equal("value1", value);
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

        [Theory]
        [InjectData(0, 0, 0)]
        [InjectData(1, 1, 2)]
        [InjectData(2, 2, 4)]
        [InjectData(5, 5, 10)]
        public void ShouldAddNumbers(ICalculator calculator, int first, int second, int expected)
        {
            int result = calculator.Add(first, second);
            Assert.Equal(expected, result);
        }


        internal static void Configure(IServiceContainer container)
        {            
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
            container.Register<ICalculator, Calculator>();            
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

    public class XunitBaseClassWithTest
    {
        [Theory, InjectData]
        public void ShouldUseConfiguredService(IFoo foo)
        {
            if (this is XunitTestsWithInheritedTestMethod)
            {
                Assert.IsType<AnotherFoo>(foo);
            }
            else
            {
                Assert.IsType<Foo>(foo);
            }                       
        }
        
        internal static void Configure(IServiceContainer container)
        {
            container.Register<IFoo, Foo>();
        }
    }

    public class XunitTestsWithInheritedTestMethod : XunitBaseClassWithTest
    {
        internal static void Configure(IServiceContainer container)
        {
            container.Register<IFoo, AnotherFoo>();
        }
    }



    public class XunitTestsWithConfigureMethodInDerivedClass : XunitBaseClass
    {
        [Theory, InjectData]
        public void ShouldBeAbleToReconfigureContainer(IFoo foo)
        {
            Assert.IsType<AnotherFoo>(foo);
        }

        internal new static void Configure(IServiceContainer container)
        {
            container.Register<IFoo, AnotherFoo>();
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


    public interface ICalculator
    {
        int Add(int first, int second);
    }

    public class Calculator : ICalculator
    {
        public int Add(int first, int second)
        {
            return first + second;
        }
    }
}
