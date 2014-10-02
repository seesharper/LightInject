//using LightInject.SampleLibraryWithInternalClasses;

namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization.Formatters;
    using System.Security;
    
    using System.Text;
    using System.Threading.Tasks;

    using LightInject;
    using LightInject.SampleLibrary;
    using LightInject.SampleLibraryWithCompositionRootTypeAttribute;

    using Microsoft.VisualStudio.TestTools.UnitTesting;    
    using Foo = LightInject.SampleLibrary.Foo;
    using IFoo = LightInject.SampleLibrary.IFoo;
    using IBar = LightInject.SampleLibrary.IBar;
    using Bar = LightInject.SampleLibrary.Bar;
    [TestClass]
    public class ServiceContainerTests : TestBase
    {
        #region Values       
        
        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.AreEqual("SomeValue", value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnsLastRegisteredValue()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.RegisterInstance("AnotherValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.AreEqual("AnotherValue", value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeValue_ReturnSameValue()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            var value1 = (string)container.GetInstance(typeof(string));
            var value2 = (string)container.GetInstance(typeof(string));
            Assert.AreSame(value1, value2);
        }

        [TestMethod]
        public void GetInstance_ValueTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.RegisterInstance(42);
            var value = (int)container.GetInstance(typeof(int));
            Assert.AreEqual(42, value);
        }
        

        [TestMethod]
        public void GetInstance_NamedValue_ReturnsNamedValue()
        {
            var container = CreateContainer();
            container.RegisterInstance(42, "Answer");
            var value = (int)container.GetInstance(typeof(int), "Answer");
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void GetInstance_ValueTypeAsSingeton_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register<int>(factory => 42, new PerContainerLifetime());
            var value = container.GetInstance<int>();
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void GetInstance_ValueTypeAsPerScope_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register<int>(factory => 42, new PerScopeLifetime());
            using (container.BeginScope())
            {
                var value = container.GetInstance<int>();
                Assert.AreEqual(42, value);    
            }
        }

        #endregion

        [TestMethod]
        public void GetInstance_ConcreteService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<Foo>();

            var instance = container.GetInstance<Foo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_ConcreteServiceUsingNonGenericMethod_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(Foo));

            var instance = container.GetInstance<Foo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_ConcreteSingletonServiceUsingNonGenericMethod_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(Foo), new PerContainerLifetime());

            var first = container.GetInstance<Foo>();
            var second = container.GetInstance<Foo>();
            Assert.AreSame(first, second);
        }



        [TestMethod]
        public void GetInstance_ConcreteServiceAsSingleton_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register<Foo>(new PerContainerLifetime());

            var firstInstance = container.GetInstance<Foo>();
            var secondInstance = container.GetInstance<Foo>();

            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_UnknownService_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();

            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IBar>());
        }
        
        
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
            container.RegisterInstance<IFoo>(new AnotherFoo());
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
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Singleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Singleton_CallsConstructorOnlyOnce()
        {
            var container = CreateContainer();
            Foo.Instances = 0;
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());            
            container.GetInstance(typeof(IFoo));
            container.GetInstance(typeof(IFoo));
            Assert.AreEqual(1, Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_NamedSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo", new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo), "SomeFoo");
            var instance2 = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_PerScopeService_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<IFoo>();
                var instance2 = container.GetInstance<IFoo>();
                Assert.AreSame(instance1, instance2);
            }            
        }

        [TestMethod]
        public void GetInstance_PerScopeServiceOutSideOfScope_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());

            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetInstance<IFoo>(), e => e.Message == ErrorMessages.GetInstanceOutSideScope);
        }

        [TestMethod]
        public void GetInstance_GenericServiceWithPerScopeLifetime_DoesNotShareLifetimeInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerScopeLifetime());
            using(container.BeginScope())
            {
                var intInstance = container.GetInstance<IFoo<int>>();
                var stringInstance = container.GetInstance<IFoo<string>>();
                Assert.IsInstanceOfType(intInstance, typeof(IFoo<int>));
                Assert.IsInstanceOfType(stringInstance, typeof(IFoo<string>));
            }            
        }

        [TestMethod]
        public void GetInstance_PerRequestService_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerRequestLifeTime());
            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<IFoo>();
                var instance2 = container.GetInstance<IFoo>();
                Assert.AreNotSame(instance1, instance2);
            }
        }

        [TestMethod]
        public void GetInstance_DisposablePerRequestServiceOutsideScope_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, DisposableFoo>(new PerRequestLifeTime());

            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetInstance<IFoo>(),
                exception => exception.Message == ErrorMessages.DisposableOutsideScope);

        }

        #region Array
        
        [TestMethod]
        public void GetInstance_Array_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IFoo[]>();
            Assert.AreEqual(2, services.Length);
        }

        #endregion

        #region List

        [TestMethod]
        public void GetInstance_List_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IList<IFoo>>();
            Assert.AreEqual(2, services.Count);
        }

        #endregion

        #region Collection

        [TestMethod]
        public void GetInstance_Collection_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<ICollection<IFoo>>();
            Assert.AreEqual(2, services.Count);
        }

        #endregion

#if NET45 || NETFX_CORE || WINDOWS_PHONE
        #region ReadOnly Collection

        [TestMethod]
        public void GetInstance_ReadOnlyCollection_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IReadOnlyCollection<IFoo>>();
            Assert.AreEqual(2, services.Count);
        }

        [TestMethod]
        public void GetInstance_GenericFooWithReadOnlyCollection_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo<IReadOnlyCollection<IBar>>, FooWithGenericDependency<IReadOnlyCollection<IBar>>>();

            var instance = (FooWithGenericDependency<IReadOnlyCollection<IBar>>)container.GetInstance<IFoo<IReadOnlyCollection<IBar>>>();

            Assert.AreEqual(1, instance.Dependency.Count);
        }


        #endregion

        #region ReadOnly List

        [TestMethod]
        public void GetInstance_ReadOnlyList_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IReadOnlyList<IFoo>>();
            Assert.AreEqual(2, services.Count);
        }


        #endregion
#endif
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
        public void GetInstance_FuncWithSingletonTarget_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
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
        public void GetInstance_FuncFactoryValueType_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<int>(c => 42);
            var instance = container.GetInstance(typeof(int));
            Assert.AreEqual(42, instance);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryValueTypeWithLifetime_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            using (container.BeginScope())
            {
                container.Register<int>(c => 42, new PerContainerLifetime());
                var instance = container.GetInstance(typeof(int));
                Assert.AreEqual(42, instance);    
            }            
        }

        [TestMethod]
        public void GetInstance_FuncFactoryReferenceTypeWithLifetime_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            using (container.BeginScope())
            {
                container.Register<string>(c => "SomeValue", new PerContainerLifetime());
                var instance = container.GetInstance(typeof(string));
                Assert.AreEqual("SomeValue", instance);
            }
        }


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
            container.Register<IFoo>(c => new Foo(), "SomeFoo", new PerContainerLifetime());
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
            container.Register<IFoo>(c => new Foo(), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithMethodCall_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(factory => GetFoo());
            var foo = container.GetInstance<IFoo>();
            Assert.IsNotNull(foo);
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithArrayDependency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IBar, AnotherBar>("AnotherBar");
            container.Register<IFoo>(factory => new FooWithArrayDependency(container.GetAllInstances<IBar>().ToArray()));
            var instance = (FooWithArrayDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(2, instance.Bars.Count());
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithArrayInitializer_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
           
            container.Register<IFoo>(factory => new FooWithArrayDependency(new[] { factory.GetInstance<IBar>() }));
            var instance = (FooWithArrayDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(1, instance.Bars.Count());
        }

        [TestMethod]
        public void GetInstance_FuncFactoryWithParamsArrayInitializer_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            container.Register<IFoo>(factory => new FooWithParamsArrayDependency(new[] { factory.GetInstance<IBar>() }));
            var instance = (FooWithParamsArrayDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(1, instance.Bars.Count());
        }

        [TestMethod]
        public void GetInstance_SingletonFuncFactoryWithMethodCall_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(factory => GetFoo(), new PerContainerLifetime());
            var instance1 = container.GetInstance<IFoo>();
            var instance2 = container.GetInstance<IFoo>();
            Assert.AreSame(instance1, instance2);
        }

        private IFoo GetFoo()
        {
            return new Foo();
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
            container.RegisterInstance("SomeValue");
            container.RegisterInstance("AnotherValue", "AnotherStringValue");
            var services = container.GetInstance<IEnumerable<string>>();
            Assert.AreEqual(2, services.Count());
        }

        [TestMethod]
        public void GetInstance_IEnumerableWithValueTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.RegisterInstance(1024);
            container.RegisterInstance(2048, "AnotherInt");
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
        public void GetInstance_UsingServicePredicate_ReturnsInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo());
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(IFoo));
        }

        [TestMethod]
        public void GetInstance_PerContainerLifetimeUsingServicePredicate_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo(), new PerContainerLifetime());
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_UsingFallback_DoesNotReuseLifetimeAcrossServices()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => true, request => Activator.CreateInstance(request.ServiceType), new PerContainerLifetime());
            var foo = container.GetInstance(typeof(Foo));
            var bar = container.GetInstance(typeof(Bar));            
            Assert.IsInstanceOfType(foo, typeof(Foo));
            Assert.IsInstanceOfType(bar, typeof(Bar));
        }


        [TestMethod]
        public void CanGetInstance_KnownService_ReturnsTrue()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var canCreateInstance = container.CanGetInstance(typeof(IFoo), string.Empty);
            Assert.IsTrue(canCreateInstance);
        }
        [TestMethod]
        public void CanGetInstance_UnknownService_ReturnFalse()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var canCreateInstance = container.CanGetInstance(typeof(IBar), string.Empty);
            Assert.IsFalse(canCreateInstance);
        }      

        [TestMethod]
        public void GetInstance_RegisterAfterGetInstance_ReturnsDependencyOfSecondRegistration()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IBar, Bar>();
            container.GetInstance<IFoo>();            
            //container.Register<IBar, AnotherBar>();

            //var instance = (FooWithDependency)container.GetInstance<IFoo>();

            //Assert.IsInstanceOfType(instance.Bar, typeof(AnotherBar));
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
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());
            Foo.Instances = 0;
            IList<IFoo> instances = new List<IFoo>();
            for (int i = 0; i < 100; i++)
            {
                RunParallel(container);
            }
                      
            Assert.AreEqual(1,Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_UnknownDependencyService_DoesNotThrowRecursiveDependencyExceptionOnSecondAttempt()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            try
            {
                container.GetInstance<IFoo>();
            }
            catch (Exception)
            {
                var ex = ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>(), e => !e.InnerException.Message.Contains("Recursive"));
            }
        }

        [TestMethod]
        public void BeginResolutionScope_TwoContainers_ResolutionContextIsScopedPerContainer()
        {
            var firstContainer = CreateContainer();
            var secondContainer = CreateContainer();
            
            using (Scope firstResolutionScope = firstContainer.BeginScope())
            {
                using (Scope secondResolutionScope = secondContainer.BeginScope())
                {
                    Assert.AreNotSame(firstResolutionScope, secondResolutionScope);
                }
            }
        }

        [TestMethod]
        public void Dispose_ServiceContainer_DisposesDisposableLifeTimeInstances()
        {
            var lifetime = new DisposableLifetime();
            
            using (var container = new ServiceContainer())
            {               
                container.Register<IFoo, Foo>(lifetime);                
            }
            Assert.IsTrue(lifetime.IsDisposed);
        }


        [TestMethod]
        public void Dispose_ServiceContainerWithDisposablePerContainerLifetimeService_DisposesInstance()
        {
            DisposableFoo foo;
            using (var container = new ServiceContainer())
            {
                container.Register<IFoo, DisposableFoo>(new PerContainerLifetime());                
                foo = (DisposableFoo)container.GetInstance<IFoo>();                
            }

            Assert.IsTrue(foo.IsDisposed);
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


        [TestMethod]
        public void TryGetInstance_UnknownService_ReturnsNull()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            var instance = container.TryGetInstance<IFoo>();

            Assert.IsNull(instance);
        }

        [TestMethod]
        public void TryGetInstance_UnknownService_IsAvailableAfterRegistration()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            container.TryGetInstance<IFoo>();
            container.Register<IFoo, Foo>();

            var instance = container.TryGetInstance<IFoo>();

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TryGetInstance_UnknownNamedService_IsAvailableAfterRegistration()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            container.TryGetInstance<IFoo>("Foo");
            container.Register<IFoo, Foo>("Foo");

            var instance = container.TryGetInstance<IFoo>("Foo");

            Assert.IsNotNull(instance);
        }


        [TestMethod]
        public void TryGetInstance_UnknownNamedService_ReturnsNull()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            var instance = container.TryGetInstance<IFoo>("SomeFoo");

            Assert.IsNull(instance);
        }

        [TestMethod]
        public void TryGetInstance_KnownService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();

            var instance = container.TryGetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void TryGetInstance_KnownServiceWithUnknownDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
           
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());            
        }

        [TestMethod]
        public void GetInstance_LazyService_ReturnsInstance()
        {            
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();

            var lazyInstance = container.GetInstance<Lazy<IFoo>>();

            Assert.IsInstanceOfType(lazyInstance, typeof(Lazy<IFoo>));
        }

        [TestMethod]
        public void GetInstance_LazyService_DoesNotCreateTarget()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            Foo.Instances = 0;

            container.GetInstance<Lazy<IFoo>>();

            Assert.AreEqual(0, Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_LazyService_CreatesTargetWhenValuePropertyIsAccessed()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            
            var instance = container.GetInstance<Lazy<IFoo>>();

            Assert.IsInstanceOfType(instance.Value, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_TypedFactory_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFooFactory, FooFactory>();

            var factory = container.GetInstance<IFooFactory>();
            var instance = factory.CreateFoo();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_ServiceWithGenericConstraint_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>));
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo<int>)), ErrorMessages.UnknownGenericDependency);
        }

        [TestMethod]
        public void GetInstance_ServiceWithGenericInterfaceConstraint_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(
                typeof(IFooWithGenericInterfaceConstraint<,>),
                typeof(FooWithGenericInterfaceConstraint<,>));
            container.Register(typeof(IBar<>), typeof(Bar<>));

            container.GetInstance<IFooWithGenericInterfaceConstraint<IBar<string>, string>>();

        }

        [TestMethod]
        public void GetAllInstances_ServiceWithGenericConstraint_ReturnsOnlyMatchingInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>));
            container.Register(typeof(IFoo<>), typeof(Foo<>), "AnotherFoo");

            var instances = container.GetAllInstances<IFoo<int>>();
            
            Assert.AreEqual(1, instances.Count());
        }

        [TestMethod]
        public void GetAllInstances_ServiceWithGenericConstraint_ReturnsAllMatchingInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>));
            container.Register(typeof(IFoo<>), typeof(Foo<>), "AnotherFoo");

            var instances = container.GetAllInstances<IFoo<IBar>>();

            Assert.AreEqual(2, instances.Count());
        }
       
        [TestMethod]
        public void RegisterFrom_CompositionRoot_RegistersService()
        {
            var container = CreateContainer();
            container.RegisterFrom<CompositionRoot>();
            Assert.AreEqual(1, container.AvailableServices.Count());
        }
        
        [TestMethod]
        public void GetInstance_SingletonInstance_EmitterIsProperlyPoppedOnConstructorException()
        {
            var container = CreateContainer();
            container.Register<IBar, BrokenBar>(new PerContainerLifetime());
            container.Register<FooWithBrokenDependency>(new PerContainerLifetime());
            
            try
            {
                container.GetInstance<FooWithBrokenDependency>();
            }
            catch(BrokenBarException)
            {
            }

            try
            {
                container.GetInstance<FooWithBrokenDependency>();
            }
            catch (BrokenBarException)
            {
            }
        }

        #region Internal Classes

        [TestMethod]
        public void GetInstance_InternalClassWithInternalConstructor_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();
            container.Register<IFoo, InternalFooWithInternalConstructor>();
            var exception = ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
            StringAssert.Contains(exception.InnerException.Message, "Missing public constructor for Type");            
        }

        [TestMethod]
        public void GetInstance_InternalClassWithPublicConstructor_ReturnInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, InternalFooWithPublicConstructor>();
            var instance = container.GetInstance<IFoo>();
            Assert.IsNotNull(instance);
        }
     
        #endregion
    }

    public interface IFooFactoryWithArgument
    {
        IFoo CreateFoo(int value);
    }

 
}