namespace LightInject.Tests
{
    using System;
    using System.Text;    
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PropertyInjectionTests
    {
        [TestMethod]
        public void GetInstance_KnownDependency_InjectsPropertyDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithProperyDependency>();
            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]        
        public void GetInstance_UnKnownDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();            
            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetInstance<IFoo>(), ErrorMessages.UnknownPropertyDependency);            
        }

        [TestMethod]
        public void GetInstance_OpenGenericPropertyDependency_InjectsPropertyDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericPropertyDependency<>));
            var instance = (FooWithGenericPropertyDependency<IBar>)container.GetInstance<IFoo<IBar>>();
            Assert.IsInstanceOfType(instance.Dependency, typeof(Bar));
        }
        
        [TestMethod]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithProperyDependency>();
            var instance1 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Request);
            container.Register<IFoo, FooWithProperyDependency>();
            var instance1 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingleonDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Singleton);
            container.Register<IFoo, FooWithProperyDependency>();
            var instance1 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingletonDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Singleton);
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsSameDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Request);
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependenciesForMultipleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Request);
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance1 = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            var instance2 = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance1.Bar1, instance2.Bar2);
        }

        [TestMethod]
        public void GetInstance_ValueTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(42);           
            container.Register<IFoo, FooWithValueTypePropertyDependency>();
            var instance = (FooWithValueTypePropertyDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_EnumDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(Encoding.UTF8);
            container.Register<IFoo, FooWithEnumPropertyDependency>();
            var instance = (FooWithEnumPropertyDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(Encoding.UTF8, instance.Value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            container.Register<IFoo, FooWithReferenceTypePropertyDependency>();
            var instance = (FooWithReferenceTypePropertyDependency)container.GetInstance<IFoo>();
            Assert.AreEqual("SomeValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithInitializer_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register<IFoo>(f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>() });
            var instance = (FooWithProperyDependency)container.GetInstance(typeof(IFoo));
            Assert.IsNotNull(instance.Bar);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithoutInitializer_ReturnsInstanceWithoutDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo>(f => new FooWithProperyDependency());
            var instance = (FooWithProperyDependency)container.GetInstance(typeof(IFoo));
            Assert.IsNull(instance.Bar);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithStringConstantInitializer_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register<IFoo>(f => new FooWithReferenceTypePropertyDependency { Value = "SomeValue" });
            var instance = (FooWithReferenceTypePropertyDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual("SomeValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_RequestLifeCycle_CallConstructorsOnDependencyOnlyOnce()
        {
            var container = CreateContainer();
            Bar.InitializeCount = 0;
            container.Register(typeof(IBar), typeof(Bar), LifeCycleType.Request);
            container.Register(typeof(IFoo), typeof(FooWithSamePropertyDependencyTwice));
            container.GetInstance<IFoo>();
            Assert.AreEqual(1, Bar.InitializeCount);
        }

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}