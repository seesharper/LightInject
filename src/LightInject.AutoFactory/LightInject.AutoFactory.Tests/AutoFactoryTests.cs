using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LightInject.AutoFactory.Tests
{
    public class AutoFactoryTests
    {
        [Fact]
        public void ShoudGetInstanceUsingFactory()
        {
            var factoryInstance = CreateFactory<IFooFactory>();

            var instance = factoryInstance.GetFoo(42);

            Assert.IsType<Foo>(instance);
        }

        [Fact]
        public void ShouldGetNamedInstanceUsingFactory()
        {
            var factoryInstance = CreateFactory<IFooFactory>();

            var instance = factoryInstance.GetAnotherFoo(42);

            Assert.IsType<AnotherFoo>(instance);
        }



        private TFactory CreateFactory<TFactory>()
        {
            AutoFactoryBuilder autoFactoryBuilder = CreateFactoryBuilder();
            var factoryType = autoFactoryBuilder.GetFactoryType(typeof (TFactory));
            var container = new ServiceContainer();
            container.RegisterConstructorDependency((factory, parameter, arguments) => (int)arguments[0]);
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");

            var factoryInstance = (TFactory) Activator.CreateInstance(factoryType, container);
            return factoryInstance;
        }

        protected virtual AutoFactoryBuilder CreateFactoryBuilder()
        {
            return new AutoFactoryBuilder(new ServiceNameResolver());
        }
    }


    public interface IFooFactory
    {
        IFoo GetFoo(int value);

        IFoo GetAnotherFoo(int value);
    }
  
    public interface IFoo { }

    public class Foo : IFoo
    {
        public Foo(int value)
        {
        }
    }

    public class AnotherFoo : IFoo
    {
        public AnotherFoo(int value)
        {
        }
    }

    public class FooFactory : IFooFactory
    {
        private readonly IServiceFactory serviceFactory;

        public FooFactory(IServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

        public IFoo GetFoo(int value )
        {
            return serviceFactory.GetInstance<int, IFoo>(value);
        }

        public IFoo GetAnotherFoo(int value)
        {
            return serviceFactory.GetInstance<int, IFoo>(value, "AnotherFoo");
        }
    }
}
