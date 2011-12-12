using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjector.Tests
{    
    [TestClass]
    public class ServiceContainerTests
    {        
        [TestMethod]
        public void GetInstance_OneService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof (IFoo), typeof (Foo));
            object instance = container.GetInstance(typeof (IFoo));        
            Assert.IsInstanceOfType(instance,typeof(Foo));
        }
                
        [TestMethod]
        public void GetInstance_TwoServices_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo),"AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo),"AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
        }

        [TestMethod]
        public void GetInstance_OneNamedService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo),"SomeFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void Instance_OneNamedClosedGenericService_ReturnsDefaultService()
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
            container.Register(typeof(IFoo),typeof(Foo),"SomeFoo");
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
        public void GetInstance_OpenGenericType_ReturnsTransientInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreNotSame(instance1,instance2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_UnknownOpenGenericType_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo),typeof(Foo));
            container.GetInstance<IFoo<int>>();
        }

        [TestMethod]
        public void GetInstance_DefaultAndNamedOpenGenericType_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>),"AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo<int>));
        }

        [TestMethod]
        public void GetInstance_TwoNamedOpenGenericTypes_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>),"SomeFoo");
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
            container.RegisterAsSingleton(typeof(IFoo<>), typeof(Foo<>));
            var instance1 = container.GetInstance(typeof (IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.AreSame(instance1,instance2);
        }
          
        [TestMethod]
        public void GetInstance_Singleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IFoo), typeof(Foo));
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1,instance2);
        }
                             
        [TestMethod]
        public void GetInstance_Func_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory = container.GetInstance(typeof(Func<IFoo>));
            Assert.IsInstanceOfType(factory, typeof(Func<IFoo>));
        }

        [TestMethod]
        public void GetInstance_Func_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance = factory();
            Assert.IsInstanceOfType(instance,typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_Func_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory1 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var factory2 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            Assert.AreSame(factory1,factory2);
        }

        [TestMethod, Ignore]
        public void GetInstance_FuncWithSingletonTarget_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IFoo), typeof(Foo));
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreSame(instance1,instance2);
        }

        [TestMethod]
        public void GetInstance_FuncWithTransientTarget_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo());
            var instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }
        
        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar>(c => new Bar());       
            container.Register<IFoo>(c => new FooWithDependency(container.GetInstance<IBar>()));
            var instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(FooWithDependency));
        }

        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsSingletonInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton<IBar>(() => new Bar());
            container.RegisterAsSingleton<IFoo>(() => new FooWithDependency(container.GetInstance<IBar>()));
            var instance = container.GetInstance(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(FooWithDependency));
        }

        [TestMethod]
        public void GetInstance_SingletonFuncFactory_ReturnsSingleInstance()
        {                        
            var container = CreateContainer();
            container.RegisterAsSingleton<IFoo>(() => new Foo());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.AreSame(instance1,instance2);
        }

        [TestMethod]
        public void GetInstance_CustomFactory_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory),typeof(FooFactory));
            var instance1 = container.GetInstance<IFactory>();
            var instance2 = container.GetInstance<IFactory>();
            Assert.AreEqual(instance1, instance2);
        }
        
        [TestMethod]
        public void GetInstance_CustomFactory_ServiceNameIsPassedToFactory()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));            
            var factory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance<IFoo>("SomeServiceName");
            Assert.AreEqual("SomeServiceName",factory.ServiceName);
            container.GetInstance<IFoo>("AnotherServiceName");
            Assert.AreEqual("AnotherServiceName", factory.ServiceName);
        }
        
        [TestMethod]
        public void GetInstance_CustomFactoryWithUnNamedAndNamedServiceRequest_ServiceNameIsPassedToFactory()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));
            var factory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance<IFoo>();
            Assert.AreEqual("", factory.ServiceName);
            container.GetInstance<IFoo>("AnotherServiceName");
            Assert.AreEqual("AnotherServiceName", factory.ServiceName);
        }

        [TestMethod]
        public void GetInstance_CustomFactoryWithUnknownService_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));
            var instance = container.GetInstance<IFoo>();
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void GetInstance_CustomFactory_CallsProceedWhenImplementationIsAvailable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.Register(typeof(IFoo), typeof(AnotherFoo));            
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFoo));

        }

        [TestMethod]
        public void GetInstance_LazyService_ReturnsLazyInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo),typeof(Foo));
            var lazyInstance = container.GetInstance<Lazy<IFoo>>();
            Assert.IsInstanceOfType(lazyInstance, typeof(Lazy<IFoo>));
        }

        [TestMethod]
        public void GetInstance_LazyService_InstanceCreatedWhenValueIsAccessed()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var lazyInstance = container.GetInstance<Lazy<IFoo>>();
            var instance = lazyInstance.Value;
            Assert.IsInstanceOfType(instance,typeof(IFoo));
        }

        [TestMethod]
        public void GetInstance_LazySingletonService_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IFoo), typeof(Foo));
            var lazyInstance1 = container.GetInstance<Lazy<IFoo>>();
            var lazyInstance2 = container.GetInstance<Lazy<IFoo>>();
            Assert.AreSame(lazyInstance1.Value, lazyInstance2.Value);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InvalidService_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.GetInstance<IFoo>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InvalidServiceName_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo),"SomeFoo");
            var instance = container.GetInstance<IFoo>("SomeInvalidServiceName");
            Assert.IsInstanceOfType(instance,typeof(Foo));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InvalidServiceName_ThrowsInvalidOperationExceptionWhenDefaultServiceIfAvailable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var instance = container.GetInstance<IFoo>("SomeInvalidServiceName");
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InvalidServiceNameWithTwoNamedServices_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo),"SomeFoo");
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.GetInstance<IFoo>("SomeInvalidServiceName");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_DefaultServiceWithTwoNamedServices_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.GetInstance<IFoo>();
        }

        [TestMethod]
        public void GetInstance_IEnumerableOfT_ReturnsIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo),"AnotherFoo");
            var services = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreEqual(2,services.Count());
        }

        #region Constructor Dependency Injection

        [TestMethod]
        public void GetInstance_ServiceWithTransientDependency_ReturnsInstanceWithInjectedDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }


        [TestMethod]
        public void GetInstance_ServiceWithIEnumerableDependency_ReturnsInstanceWithIEnumerableDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IBar), typeof(AnotherBar),"AnotherBar");
            container.Register(typeof(IFoo), typeof(FooWithEnumerableDependency));
            var instance = (FooWithEnumerableDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Bars,typeof(IEnumerable<IBar>));
        }

        [TestMethod]
        public void GetInstance_ServiceWithUnknownDependency_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();            
            container.Register(typeof(IFactory), typeof(BarFactory));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Bar, typeof(Bar));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_ServiceWithUnknownDependency_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();            
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            container.GetInstance<IFoo>();
        }

        [TestMethod]
        public void GetInstance_InjectSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();            
            container.RegisterAsSingleton(typeof(IBar),typeof(Bar));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.AreSame(instance1.Bar,instance2.Bar);
        }


        [TestMethod]
        public void GetInstance_InjectLazyService_ReturnsInstanceWithInjectedLazyService()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo), typeof(FooWithLazyDependency));
            var instance = (FooWithLazyDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.LazyService,typeof(Lazy<IBar>));
        }
       
        [TestMethod]
        public void GetInstance_InjectCustomFactoryOneImplementingType_DoesNotPassParameterNameAsServiceName()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));            
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var factory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance<IFoo>();
            Assert.AreEqual(string.Empty,factory.ServiceName);
        }

        [TestMethod]
        public void GetInstance_InjectCustomFactoryTwoImplementingTypes_PassesParameterNameAsServiceName()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IBar), typeof(AnotherBar), "AnotherBar");            
            container.Register(typeof(IFactory), typeof(BarFactory));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var factory = (BarFactory)container.GetInstance<IFactory>();
            container.GetInstance<IFoo>();
            Assert.AreEqual("bar", factory.ServiceName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_RecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));            
            container.GetInstance<IFoo>();            
        }

        [TestMethod]
        public void GetAllInstances_TwoServices_ReturnsIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo),typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo),"AnotherFoo");
            var instances = container.GetAllInstances(typeof (IFoo));
            Assert.IsInstanceOfType(instances,typeof(IEnumerable<IFoo>));            
        }

        [TestMethod]
        public void GetAllInstances_TwoServices_ReturnsBothServices()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.AreEqual(2,instances.Count());

        }

        [TestMethod]
        public void GenericGetAllInstances_TwoServices_ReturnsIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GenericGetAllInstances_UnknownService_ReturnsEmptyIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));            
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GenericGetAllInstances_TwoServices_ReturnsBothServices()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsSingleIEnumerableInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.RegisterAsSingleton(typeof(IEnumerable<IFoo>),null);
            var instance1 = container.GetInstance<IEnumerable<IFoo>>();
            var instance2 = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreSame(instance1,instance2);
        }


        [TestMethod]
        public void GetInstance_OpenGenericTypeWithSingletonDependency_ReturnsSameDependencyInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo<>),typeof(FooWithGenericDependency<>));
            var instance1 = (FooWithGenericDependency<IBar>)container.GetInstance(typeof(IFoo<IBar>));
            var instance2 = (FooWithGenericDependency<IBar>)container.GetInstance(typeof(IFoo<IBar>));
            Assert.AreEqual(instance1.Dependency,instance2.Dependency);
        }

        [TestMethod]
        public void GetInstance_SingletonWithCustomFactory_CanProceed()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.RegisterAsSingleton(typeof(IFoo),typeof(Foo));
            var factory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance(typeof(IFoo));
            Assert.IsTrue(factory.ServiceRequest.CanProceed);            
        }

        [TestMethod]
        public void GetInstance_SingletonWithCustomFactory_CallsFactoryTwice()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFactory), typeof(FooFactory));
            var singletonFactory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance(typeof(IFoo));
            container.GetInstance(typeof(IFoo));
            Assert.AreEqual(2, singletonFactory.CallCount);
        }


        [TestMethod]
        public void GetInstance_OpenGenericTypeWithCustomFactory_CanProceed()
        {
            var container = CreateContainer();
            container.Register(typeof(IFactory), typeof(GenericFooFactory));
            var factory = (GenericFooFactory)container.GetInstance<IFactory>();
            container.Register(typeof(IFoo<>),typeof(Foo<>));
            container.GetInstance<IFoo<int>>();
            Assert.IsTrue(factory.ServiceRequest.CanProceed);
        }

        [TestMethod]
        public void GetInstance_MultipleContructors_UsesConstructorWithMostParameters()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo),typeof(FooWithMultipleConstructors));
            container.Register(typeof(IBar),typeof(Bar));
            var foo = (FooWithMultipleConstructors)container.GetInstance<IFoo>();
            Assert.IsNotNull(foo.Bar);
        }

        [TestMethod]
        public void GetInstance_ServiceWithPropertyDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithProperyDependency));
            container.Register(typeof(IBar), typeof(Bar));
            var foo = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsNotNull(foo.Bar);
        }

        [TestMethod]
        public void GetInstance_ServicesRegisteredFromAssembly_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(Foo).Assembly, t => true);
            var foo = container.GetInstance<IFoo>();
            Assert.IsNotNull(foo);
        }




        #endregion
        private static ServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    
    
        
    
    }

    
}
