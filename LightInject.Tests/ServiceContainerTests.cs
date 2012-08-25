namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceContainerTests
    {
        #region Values

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.AreEqual("SomeValue", value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnsLastRegisteredValue()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            container.Register("AnotherValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.AreEqual("AnotherValue", value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnSameValue()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            var value1 = (string)container.GetInstance(typeof(string));
            var value2 = (string)container.GetInstance(typeof(string));
            Assert.AreSame(value1, value2);
        }

        [TestMethod]
        public void GetInstance_ValueTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register(42);
            var value = (int)container.GetInstance(typeof(int));
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void GetInstance_NamedValue_ReturnsNamedValue()
        {
            var container = CreateContainer();
            container.Register(42, "Answer");
            var value = (int)container.GetInstance(typeof(int), "Answer");
            Assert.AreEqual(42, value);
        }

        #endregion
        [TestMethod]
        public void GetInstance_OneService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, Foo>("AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_TwoNamedServices_ThrowsExceptionWhenRequestingDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo)), ErrorMessages.UnableToResolveType);
        }

        [TestMethod]
        public void GetInstance_DuplicateRegistration_ReturnsLastRegisteredService()
        {
            var container = CreateContainer();            
            container.Register<IFoo, Foo>();
            container.Register<IFoo>(new AnotherFoo());
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_UnknownGenericType_ThrowsExceptionWhenRequestingDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar<>), typeof(Bar<>));
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo<int>)), ErrorMessages.UnknownGenericDependency);
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsNamedInstanceAfterGettingDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.GetInstance(typeof(IFoo), "AnotherFoo");
            object defaultInstance = container.GetInstance(typeof(IFoo));            
            Assert.IsInstanceOfType(defaultInstance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_OneNamedService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_OneNamedClosedGenericService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>), "SomeFoo");
            object instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(Foo<int>));
        }

        [TestMethod]
        public void GetInstance_NamedService_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            object instance = container.GetInstance<IFoo>("SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(Foo<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsInstanceOfLastRegisteredType()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>));
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_NamedOpenGenericType_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsInstanceOfType(instance, typeof(Foo<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsTransientInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsClosedGenericInstancesIfPresent()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<string>), typeof(FooWithStringTypeParameter));
            var instance = container.GetInstance(typeof(IFoo<string>));
            Assert.IsInstanceOfType(instance, typeof(FooWithStringTypeParameter));
        }

        [TestMethod]
        public void GetInstance_DefaultAndNamedOpenGenericType_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_TwoNamedOpenGenericTypes_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericTypeWithDependency_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo<>), typeof(FooWithGenericDependency<>));
            var instance = (FooWithGenericDependency<IBar>)container.GetInstance(typeof(IFoo<IBar>));
            Assert.IsInstanceOfType(instance.Dependency, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_OpenGenericSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), LifeCycleType.Singleton);
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Singleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), LifeCycleType.Singleton);
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Singleton_CallsConstructorOnlyOnce()
        {
            var container = CreateContainer();
            Foo.Instances = 0;
            container.Register(typeof(IFoo), typeof(Foo), LifeCycleType.Singleton);            
            container.GetInstance(typeof(IFoo));
            container.GetInstance(typeof(IFoo));
            Assert.AreEqual(1, Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_NamedSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo", LifeCycleType.Singleton);
            var instance1 = container.GetInstance(typeof(IFoo), "SomeFoo");
            var instance2 = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.AreSame(instance1, instance2);
        }

        #region Func Services

        [TestMethod]
        public void GetInstance_Func_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            var factory = container.GetInstance(typeof(Func<IFoo>));
            Assert.IsInstanceOfType(factory, typeof(Func<IFoo>));
        }

        [TestMethod]
        public void GetInstance_FuncWithStringArgument_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            var factory = container.GetInstance(typeof(Func<string, IFoo>));
            Assert.IsInstanceOfType(factory, typeof(Func<string, IFoo>));
        }

        [TestMethod]
        public void GetInstance_Func_ReturnsSameInstance()
        {
            var container = CreateContainer();
            var factory1 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var factory2 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            Assert.AreSame(factory1, factory2);
        }

        [TestMethod]
        public void GetInstance_FuncWithStringArgument_ReturnsSameInstance()
        {
            var container = CreateContainer();
            var factory1 = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            var factory2 = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            Assert.AreSame(factory1, factory2);
        }

        [TestMethod]
        public void GetInstance_Func_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance = factory();
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_FuncWithStringArgument_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            var factory = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            var instance = factory("SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_FuncWithSingletonTarget_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(LifeCycleType.Singleton);
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_FuncWithTransientTarget_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreNotSame(instance1, instance2);
        }
        #endregion

        #region Func Factory

        [TestMethod]
        public void GetInstance_FuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo());
            var instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }
    
        [TestMethod]
        public void GetInstance_FuncFactory_ReturnsLastRegisteredFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithMultipleConstructors());
            container.Register<IFoo>(c => new FooWithMultipleConstructors(new Bar()));
            var instance = (FooWithMultipleConstructors)container.GetInstance(typeof(IFoo));            
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_NamedFuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), "SomeFoo");
            var instance = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_NamedSingletonFuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), "SomeFoo", LifeCycleType.Singleton);
            var firstInstance = container.GetInstance(typeof(IFoo), "SomeFoo");
            var secondInstance = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar>(c => new Bar());
            container.Register<IFoo>(c => new FooWithDependency(c.GetInstance<IBar>()));
            var instance = (FooWithDependency)container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithReferenceTypeDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithReferenceTypeDependency("SomeStringValue"));
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual("SomeStringValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithValueTypeDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithValueTypeDependency(42));
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithEnumDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithEnumDependency(Encoding.UTF8));
            var instance = (FooWithEnumDependency)container.GetInstance(typeof(IFoo));
            Assert.AreEqual(Encoding.UTF8, instance.Value);
        }

        [TestMethod]
        public void GetInstance_SingletonFuncFactory_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), LifeCycleType.Singleton);
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1, instance2);
        }

        #endregion

        #region IEnumerable

        [TestMethod]
        public void GetInstance_IEnumerable_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GenericGetAllInstances_UnknownService_ReturnsEmptyIEnumerable()
        {
            var container = CreateContainer();
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetInstance_IEnumerableWithReferenceTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register("SomeValue");
            container.Register("AnotherValue", "AnotherStringValue");
            var services = container.GetInstance<IEnumerable<string>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GetInstance_IEnumerableWithValueTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(1024);
            container.Register(2048, "AnotherInt");
            var services = container.GetInstance<IEnumerable<int>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GetInstance_IEnumerable_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instance1 = container.GetInstance<IEnumerable<IFoo>>();
            var instance2 = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreNotSame(instance1, instance2);
        }
               
        [TestMethod]
        public void GetAllInstances_NonGeneric_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetAllInstances_Generic_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetAllInstances_TwoOpenGenericServices_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetAllInstances_ClosedAndOpenGenericService_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.AreEqual(2, instances.Count());
        }


        [TestMethod]
        public void GetAllInstances_EnumerableWithRecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetAllInstances<IFoo>(), ex => ex.InnerException.InnerException.InnerException.Message == ErrorMessages.RecursiveDependency);
        }

        [TestMethod]
        public void Run()
        {
            for (int i = 0; i < 1; i++)
            {
                GetInstance_SingletonUsingMultipleThreads_ReturnsSameInstance();
            }
        }

        
        [TestMethod]
        public void GetInstance_SingletonUsingMultipleThreads_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), LifeCycleType.Singleton);
            Foo.Instances = 0;
            IList<IFoo> instances = new List<IFoo>();
            for (int i = 0; i < 100; i++)
            {
                RunParallel(container);
            }
                      
            Assert.AreEqual(1,Foo.Instances);
        }

        private static void RunParallel(IServiceContainer container)
        {
            Parallel.Invoke(
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>(),
                () => container.GetInstance<IFoo>());
        }

        #endregion

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}