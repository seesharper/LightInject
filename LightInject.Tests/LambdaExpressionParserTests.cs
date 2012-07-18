 namespace DependencyInjector.Tests
{
    using System;
    using System.Collections.Generic;    
    using System.Linq.Expressions;
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LambdaExpressionParserTests
    {
        [TestMethod]
        public void CreateServiceInfo_NewOperatorInConstructor_ReturnsServiceInfo()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(new Bar());
            var serviceInfo = parser.Parse(e);
            Assert.AreEqual(typeof(FooWithDependency), serviceInfo.ImplementingType);
        }
                
        [TestMethod]
        public void CreateServiceInfo_GetInstanceInConstructor_ReturnsServiceInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>());
            var serviceInfo = parser.Parse(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateServiceInfo_NamedGetInstanceInConstructor_ReturnsServiceInfoWithServiceName()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>("SomeBar"));
            var serviceInfo = parser.Parse(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateServiceInfo_GetInstanceInObjectInitializer_ReturnsServiceInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>() };
            var serviceInfo = parser.Parse(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateServiceInfo_NamedGetInstanceInObjectInitializer_ReturnsServiceInfoWithServiceName()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>("SomeBar") };
            var serviceInfo = parser.Parse(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateServiceInfo_MethodCall_ThrowsInvalidOperationException()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => CreateFoo();
            ExceptionAssert.Throws<InvalidOperationException>(() => parser.Parse(e), ExpectedErrorMessages.InvalidFuncFactoryExpression);            
        }

        [TestMethod]
        public void CreateServiceInfo_GetAllInstancesInObjectInitializer_ReturnsServiceInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerablePropertyDependency { Bars = f.GetAllInstances<IBar>() };
            var serviceInfo = parser.Parse(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        public void CreateServiceInfo_GetAllInstancesInConstructor_ReturnsServiceInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerableDependency(f.GetAllInstances<IBar>());
            var serviceInfo = parser.Parse(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }
        
        private IFoo CreateFoo()
        {
            return null;
        }
    }
}