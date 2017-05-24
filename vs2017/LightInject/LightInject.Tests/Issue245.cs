namespace LightInject.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;

    public class Issue245 : TestBase
    {
        [Fact]
        public void ShouldHandleFactoryDelegateWithClosure()
        {
            var container = CreateContainer();
            var factoryDelegate = CreateFactoryDelegate();
            container.Register(factoryDelegate);

            var foo = container.GetInstance<Foo>();

            Assert.IsType<Foo>(foo);
        }


        private Func<IServiceFactory, Foo> CreateFactoryDelegate()
        {
            var constructor = typeof (Foo).GetTypeInfo().GetConstructor(Type.EmptyTypes);
            var newExpression = Expression.New(constructor);
            var lambdaExpression = Expression.Lambda(newExpression, Expression.Parameter(typeof (IServiceFactory)));
            var compiledDelegate = lambdaExpression.Compile();
            var test = compiledDelegate.GetMethodInfo().GetParameters();
            return (Func<IServiceFactory, Foo>)compiledDelegate;
        }

        public class Foo
        {
            
        }

    }
}