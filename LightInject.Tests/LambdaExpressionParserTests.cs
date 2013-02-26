 namespace LightInject.Tests
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
        public void CreateConstructionInfo_NewOperatorInConstructor_ReturnsConstructionInfo()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(new Bar());
            var ConstructionInfo = parser.Parse(e);
            Assert.AreEqual(typeof(FooWithDependency), ConstructionInfo.ImplementingType);
        }
                
        [TestMethod]
        public void CreateConstructionInfo_GetInstanceInConstructor_ReturnsConstructionInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>());
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateConstructionInfo_NamedGetInstanceInConstructor_ReturnsConstructionInfoWithServiceName()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>("SomeBar"));
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(ConstructionInfo.ConstructorDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateConstructionInfo_GetInstanceInObjectInitializer_ReturnsConstructionInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>() };
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.PropertyDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateConstructionInfo_NamedGetInstanceInObjectInitializer_ReturnsConstructionInfoWithServiceName()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>("SomeBar") };
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.PropertyDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(ConstructionInfo.PropertyDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateConstructionInfo_ExternalMethodCallInObjectInitializer_ReturnsDependencyAsExpression()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = this.GetInstance() };
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.PropertyDependencies[0].FactoryExpression != null);
        }
       
        [TestMethod]
        public void CreateConstructionInfo_GetAllInstancesInObjectInitializer_ReturnsConstructionInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerablePropertyDependency { Bars = f.GetAllInstances<IBar>() };
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.PropertyDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        public void CreateConstructionInfo_GetAllInstancesInConstructor_ReturnsConstructionInfoWithServiceType()
        {
            var parser = new ServiceContainer.LambdaExpressionParser();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerableDependency(f.GetAllInstances<IBar>());
            var ConstructionInfo = parser.Parse(e);
            Assert.IsTrue(ConstructionInfo.ConstructorDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }
                
        private IBar GetInstance()
        {
            return null;
        }
    }
}