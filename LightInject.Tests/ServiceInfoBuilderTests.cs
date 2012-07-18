 namespace DependencyInjector.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceInfoBuilderTests
    {
        [TestMethod]
        public void CreateServiceInfo_NewOperatorInConstructor_ReturnsServiceInfo()
        {
            var serviceInfoBuilder = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(new Bar());
            var serviceInfo = serviceInfoBuilder.Parse(e);
            Assert.AreEqual(typeof(FooWithDependency), serviceInfo.ImplementingType);
        }
                
        [TestMethod]
        public void CreateServiceInfo_GetInstanceInConstructor_ReturnsServiceInfoWithServiceType()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>());
            var serviceInfo = methodCallVisitor.Parse(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateServiceInfo_NamedGetInstanceInConstructor_ReturnsServiceInfoWithServiceName()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>("SomeBar"));
            var serviceInfo = methodCallVisitor.Parse(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateServiceInfo_GetInstanceInObjectInitializer_ReturnsServiceInfoWithServiceType()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>() };
            var serviceInfo = methodCallVisitor.Parse(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateServiceInfo_NamedGetInstanceInObjectInitializer_ReturnsServiceInfoWithServiceName()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>("SomeBar") };
            var serviceInfo = methodCallVisitor.Parse(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateServiceInfo_MethodCall_ThrowsInvalidOperationException()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => CreateFoo();
            ExceptionAssert.Throws<InvalidOperationException>(() => methodCallVisitor.Parse(e), ExpectedErrorMessages.InvalidFuncFactoryExpression);            
        }

        [TestMethod]
        public void CreateServiceInfo_GetAllInstancesInObjectInitializer_ReturnsServiceInfoWithServiceType()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerablePropertyDependency { Bars = f.GetAllInstances<IBar>() };
            var serviceInfo = methodCallVisitor.Parse(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        public void CreateServiceInfo_GetAllInstancesInConstructor_ReturnsServiceInfoWithServiceType()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerableDependency(f.GetAllInstances<IBar>());
            var serviceInfo = methodCallVisitor.Parse(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        [Ignore]
        public void CreateServiceInfo()
        {
            var methodCallVisitor = new EmitServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IEnumerable<IFoo>>> e =
                f => new[] { f.GetInstance<IFoo>("SomeBar"), f.GetInstance<IFoo>("AnotherBar") };
            var serviceInfo = methodCallVisitor.Parse(e);

        }

        private IFoo CreateFoo()
        {
            return null;
        }

    }
}