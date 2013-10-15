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
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(new Bar());
            var constructionInfo = builder.Execute(e);
            Assert.AreEqual(typeof(FooWithDependency), constructionInfo.ImplementingType);
        }
                
        [TestMethod]
        public void CreateConstructionInfo_GetInstanceInConstructor_ReturnsConstructionInfoWithServiceType()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>());
            var constructionInfo = builder.Execute(e);
            Assert.IsTrue(constructionInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateConstructionInfo_NamedGetInstanceInConstructor_ReturnsConstructionInfoWithServiceName()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithDependency(f.GetInstance<IBar>("SomeBar"));
            var ConstructionInfo = builder.Execute(e);
            Assert.IsTrue(ConstructionInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(ConstructionInfo.ConstructorDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateConstructionInfo_GetInstanceInObjectInitializer_ReturnsConstructionInfoWithServiceType()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>() };
            var constructionInfo = builder.Execute(e);
            Assert.IsTrue(constructionInfo.PropertyDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateConstructionInfo_NamedGetInstanceInObjectInitializer_ReturnsConstructionInfoWithServiceName()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>("SomeBar") };
            var constructionInfo = builder.Execute(e);
            Assert.IsTrue(constructionInfo.PropertyDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(constructionInfo.PropertyDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateConstructionInfo_ExternalMethodCallInObjectInitializer_ReturnsDependencyAsExpression()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithProperyDependency { Bar = this.GetInstance() };
            var constructionInfo = builder.Execute(e);
            Assert.IsTrue(constructionInfo.PropertyDependencies[0].FactoryExpression != null);
        }
       
        [TestMethod]
        public void CreateConstructionInfo_GetAllInstancesInObjectInitializer_ReturnsConstructionInfoWithServiceType()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerablePropertyDependency { Bars = f.GetAllInstances<IBar>() };
            var constructionInfo = builder.Execute(e);
            Assert.IsTrue(constructionInfo.PropertyDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        public void CreateConstructionInfo_GetAllInstancesInConstructor_ReturnsConstructionInfoWithServiceType()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, IFoo>> e = f => new FooWithEnumerableDependency(f.GetAllInstances<IBar>());
            var constructionInfo = builder.Execute(e);
            Assert.IsTrue(constructionInfo.ConstructorDependencies[0].ServiceType == typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        public void CreateConstructionInfo_WithParameters_ReturnsExpression()
        {
            var builder = new LambdaConstructionInfoBuilder();
            Expression<Func<IServiceFactory, int , IFoo>> e = (f,a) => new FooWithValueTypeDependency(a);
            var constructionInfo = builder.Execute(e);
            Assert.IsNotNull(constructionInfo.FactoryDelegate);
        }
        
        private IBar GetInstance()
        {
            return null;
        }
    }
}