namespace LightInject.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using LightInject.SampleLibrary;
    using LightInject.Xunit;

    using global::Xunit;
    using global::Xunit.Extensions;

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

        
        public void MethodWithMissingService(int value)
        {
            
        }

        [Fact]
        public void MissingServiceThrowsException()
        {
            var attribute = new InjectDataAttribute();
            var method = typeof(XunitTestsWithConfigureMethod).GetMethod("MethodWithMissingService");
            Assert.Throws<InvalidOperationException>(() => attribute.GetData(method, new Type[] { typeof(int) }));
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
        [ScopedTheory, InjectData]
        public void MethodWithScopedArgument(IFoo foo)
        {
            Assert.NotNull(foo);
        }

        [ScopedTheory, InjectData]
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