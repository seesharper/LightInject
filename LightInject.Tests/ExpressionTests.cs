namespace DependencyInjector.Tests
{
    using System;
    using System.Linq.Expressions;

    using LightInject;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionTests
    {
        [TestMethod]
        public void CreateServiceInfo_GetInstanceInConstructor_ReturnsServiceInfoWithServiceType()
        {
            var methodCallVisitor = new EmitServiceContainer.ServiceExpressionVisitor();
            Expression<Func<IServiceFactory, IFoo>> e = (f) => new FooWithDependency(f.GetInstance<IBar>());
            var serviceInfo = methodCallVisitor.CreateServiceInfo(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateServiceInfo_NamedGetInstanceInConstructor_ReturnsServiceInfoWithServiceName()
        {
            var methodCallVisitor = new EmitServiceContainer.ServiceExpressionVisitor();
            Expression<Func<IServiceFactory, IFoo>> e = (f) => new FooWithDependency(f.GetInstance<IBar>("SomeBar"));
            var serviceInfo = methodCallVisitor.CreateServiceInfo(e);
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(serviceInfo.ConstructorDependencies[0].ServiceName == "SomeBar");
        }

        [TestMethod]
        public void CreateServiceInfo_GetInstanceInObjectInitializer_ReturnsServiceInfoWithServiceType()
        {
            var methodCallVisitor = new EmitServiceContainer.ServiceExpressionVisitor();
            Expression<Func<IServiceFactory, IFoo>> e = (f) => new FooWithProperyDependency() { Bar = f.GetInstance<IBar>() };
            var serviceInfo = methodCallVisitor.CreateServiceInfo(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IBar));            
        }

        [TestMethod]
        public void CreateServiceInfo_NamedGetInstanceInObjectInitializer_ReturnsServiceInfoWithServiceName()
        {
            var methodCallVisitor = new EmitServiceContainer.ServiceExpressionVisitor();
            Expression<Func<IServiceFactory, IFoo>> e = (f) => new FooWithProperyDependency() { Bar = f.GetInstance<IBar>("SomeBar") };
            var serviceInfo = methodCallVisitor.CreateServiceInfo(e);
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceType == typeof(IBar));
            Assert.IsTrue(serviceInfo.PropertyDependencies[0].ServiceName == "SomeBar");
        }
    }
}