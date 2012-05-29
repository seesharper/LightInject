using System;
using System.Text;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjector.Tests
{
    [TestClass]
    public class ConstructorInjectionTests
    {
        [TestMethod]
        public void GetInstance_KnownDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_UnKnownDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.GetInstance<IFoo>();            
        }

        [TestMethod]
        public void GetInstance_OpenGenericDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericDependency<>));
            var instance = (FooWithGenericDependency<IBar>)container.GetInstance<IFoo<IBar>>();
            Assert.IsInstanceOfType(instance.Dependency, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithDependency>();
            var instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Request);
            container.Register<IFoo, FooWithDependency>();
            var instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingleonDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Singleton);
            container.Register<IFoo, FooWithDependency>();
            var instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithSameDependencyTwice>();
            var instance = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingletonDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Singleton);
            container.Register<IFoo, FooWithSameDependencyTwice>();
            var instance = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsSameDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Request);
            container.Register<IFoo, FooWithSameDependencyTwice>();
            var instance = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependenciesForMultipleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(LifeCycleType.Request);
            container.Register<IFoo, FooWithSameDependencyTwice>();
            var instance1 = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            var instance2 = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreNotEqual(instance1.Bar1, instance2.Bar2);
        }

        [TestMethod]
        public void GetInstance_ValueTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(int), 42);
            container.Register<IFoo, FooWithValueTypeDependency>();
            var instance = (FooWithValueTypeDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_EnumDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(Encoding), Encoding.UTF8);
            container.Register<IFoo, FooWithEnumDependency>();
            var instance = (FooWithEnumDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(Encoding.UTF8, instance.Value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(string), "SomeValue");
            container.Register<IFoo, FooWithReferenceTypeDependency>();
            var instance = (FooWithReferenceTypeDependency)container.GetInstance<IFoo>();
            Assert.AreEqual("SomeValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_RequestLifeCycle_CallConstructorsOnDependencyOnlyOnce()
        {
            var container = CreateContainer();
            Bar.InitializeCount = 0;
            container.Register(typeof(IBar), typeof(Bar), LifeCycleType.Request);
            container.Register(typeof(IFoo), typeof(FooWithSameDependencyTwice));
            container.GetInstance<IFoo>();
            Assert.AreEqual(1, Bar.InitializeCount);
        }

        [TestMethod]
        public void GetInstance_MultipleContructors_UsesConstructorWithMostParameters()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithMultipleConstructors));
            container.Register(typeof(IBar), typeof(Bar));
            var foo = (FooWithMultipleConstructors)container.GetInstance<IFoo>();
            Assert.IsNotNull(foo.Bar);
        }


        private static IContainer CreateContainer()
        {
            return new EmitServiceContainer();
        }



    }
}