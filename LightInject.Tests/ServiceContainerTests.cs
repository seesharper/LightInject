using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependencyInjector.Tests
{    
    [TestClass]
    public class ServiceContainerTests
    {                                    
        [TestMethod]
        public void GetInstance_OneService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof (IService), typeof (Service));
            object instance = container.GetInstance(typeof (IService));        
            Assert.IsInstanceOfType(instance,typeof(Service));
        }
                
        [TestMethod]
        public void GetInstance_TwoServices_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            container.Register(typeof(IService), typeof(AnotherService),"AnotherService");
            object instance = container.GetInstance(typeof(IService));
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            container.Register(typeof(IService), typeof(AnotherService),"AnotherService");
            object instance = container.GetInstance(typeof(IService),"AnotherService");
            Assert.IsInstanceOfType(instance, typeof(AnotherService));
        }

        [TestMethod]
        public void GetInstance_OneNamedService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service),"SomeService");
            object instance = container.GetInstance(typeof(IService));
            Assert.IsInstanceOfType(instance, typeof(Service));
        }
       
        [TestMethod]
        public void GetInstance_NamedService_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService),typeof(Service),"SomeService");
            object instance = container.GetInstance<IService>("SomeService");
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService<>), typeof(Service<>));
            var instance = container.GetInstance(typeof(IService<int>));            
            Assert.IsInstanceOfType(instance, typeof(Service<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericType_ReturnsTransientInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IService<>), typeof(Service<>));
            var instance1 = container.GetInstance(typeof(IService<int>));
            var instance2 = container.GetInstance(typeof(IService<int>));
            Assert.AreNotSame(instance1,instance2);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_UnknownOpenGenericType_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IService),typeof(Service));
            container.GetInstance<IService<int>>();
        }



        [TestMethod]
        public void GetInstance_DefaultAndNamedOpenGenericType_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService<>), typeof(Service<>));
            container.Register(typeof(IService<>), typeof(AnotherOpenGenericClass<>),"AnotherOpenGenericClass");
            var instance = container.GetInstance(typeof(IService<int>), "AnotherOpenGenericClass");
            Assert.IsInstanceOfType(instance, typeof(AnotherOpenGenericClass<int>));
        }

        [TestMethod]
        public void GetInstance_TwoNamedOpenGenericTypes_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService<>), typeof(Service<>),"SomeService");
            container.Register(typeof(IService<>), typeof(AnotherOpenGenericClass<>), "AnotherOpenGenericClass");
            var instance = container.GetInstance(typeof(IService<int>), "AnotherOpenGenericClass");
            Assert.IsInstanceOfType(instance, typeof(AnotherOpenGenericClass<int>));
        }


        [TestMethod]
        public void GetInstance_OpenGenericTypeWithDependency_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            container.Register(typeof(IService<>), typeof(ServiceWithGenericDependency<>));
            var instance = (ServiceWithGenericDependency<IService>)container.GetInstance(typeof(IService<IService>));
            Assert.IsInstanceOfType(instance.Dependency, typeof(Service));
        }


        [TestMethod]
        public void GetInstance_OpenGenericSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IService<>), typeof(Service<>));
            var instance1 = container.GetInstance(typeof (IService<int>));
            var instance2 = container.GetInstance(typeof(IService<int>));
            Assert.AreSame(instance1,instance2);
        }

        [TestMethod]
        public void GetInstance_ClosedGenericType_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService<string>), typeof(ClosedGenericClass));
            var instance = container.GetInstance(typeof(IService<string>));
            Assert.IsInstanceOfType(instance, typeof(ClosedGenericClass));
        }
        
        [TestMethod]
        public void GetInstance_Singleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(ISingleton), typeof(Singleton));
            var instance1 = container.GetInstance(typeof(ISingleton));
            var instance2 = container.GetInstance(typeof(ISingleton));
            Assert.AreSame(instance1,instance2);
        }

        [TestMethod]
        public void GetInstance_ManualSingletonRegistration_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IService), typeof(Service));
            var instance1 = container.GetInstance(typeof(IService));
            var instance2 = container.GetInstance(typeof(IService));
            Assert.AreEqual(instance1, instance2);
        }
        
        [TestMethod]
        public void GetInstance_SingletonWithCustomFactory_CallsFactoryOnlyOnce()
        {
                                    
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IService), typeof(Service));
            container.Register(typeof(IFactory),typeof(SingletonFactory));
            var singletonFactory = (SingletonFactory)container.GetInstance<IFactory>();
            container.GetInstance(typeof (IService));
            container.GetInstance(typeof(IService));
            Assert.AreEqual(1,singletonFactory.CallCount);
        }


        [TestMethod]
        public void GetInstance_Func_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            var factory = container.GetInstance(typeof(Func<IService>));
            Assert.IsInstanceOfType(factory, typeof(Func<IService>));
        }

        [TestMethod]
        public void GetInstance_Func_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            var factory = (Func<IService>)container.GetInstance(typeof(Func<IService>));
            var instance = factory();
            Assert.IsInstanceOfType(instance,typeof(Service));
        }

        [TestMethod]
        public void GetInstance_Func_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            var factory1 = (Func<IService>)container.GetInstance(typeof(Func<IService>));
            var factory2 = (Func<IService>)container.GetInstance(typeof(Func<IService>));
            Assert.AreSame(factory1,factory2);
        }

        [TestMethod]
        public void GetInstance_FuncWithSingletonTarget_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IService), typeof(Service));
            var factory = (Func<IService>)container.GetInstance(typeof(Func<IService>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreEqual(instance1,instance2);
        }

        [TestMethod]
        public void GetInstance_FuncWithTransientTarget_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            var factory = (Func<IService>)container.GetInstance(typeof(Func<IService>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.AreNotEqual(instance1, instance2);
        }



        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IService>(() => new Service());
            var instance = container.GetInstance(typeof(IService));
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod]
        public void GetInstance_InjectServiceUsingFunc_DoesNotCallGenericGetInstanceMethod()
        {
            var container = CreateContainer();
            var containerMock = new Mock<IServiceContainer>();
            container.Register<IFoo>(() => new Foo());
            containerMock.Setup(c => c.GetInstance<IFoo>()).Returns(container.GetInstance<IFoo>());
            container.Register<IService>(() => new ServiceWithDependency(containerMock.Object.GetInstance<IFoo>()));
            container.GetInstance(typeof(IService));
            containerMock.Verify(c => c.GetInstance<IFoo>(),Times.Never());
        }

        [TestMethod]
        public void GetInstance_InjectNamedServiceUsingFunc_DoesNotCallGenericGetInstanceMethod()
        {
            var container = CreateContainer();
            var containerMock = new Mock<IServiceContainer>();
            container.Register<IFoo>(() => new Foo(),"SomeFoo");
            containerMock.Setup(c => c.GetInstance<IFoo>("SomeFoo")).Returns(container.GetInstance<IFoo>("SomeFoo"));
            container.Register<IService>(() => new ServiceWithDependency(containerMock.Object.GetInstance<IFoo>("SomeFoo")));
            container.GetInstance(typeof(IService));
            containerMock.Verify(c => c.GetInstance<IFoo>("SomeFoo"),Times.Never());
        }


        [TestMethod]
        public void GetInstance_Funcfactory_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(() => new Foo());       
            container.Register<IService>(() => new ServiceWithDependency(container.GetInstance<IFoo>()));
            var instance = container.GetInstance(typeof(IService));
            Assert.IsInstanceOfType(instance, typeof(ServiceWithDependency));
        }

        [TestMethod]
        public void GetInstance_SingletonFuncFactory_ReturnsSingleInstance()
        {                        
            var container = CreateContainer();
            container.RegisterAsSingleton<IService>(() => new Service());
            var instance1 = container.GetInstance(typeof(IService));
            var instance2 = container.GetInstance(typeof(IService));
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
            container.Register(typeof(IFoo), typeof(Foo1));            
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(Foo1));

        }

        [TestMethod]
        public void GetInstance_LazyService_ReturnsLazyInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService),typeof(Service));
            var lazyInstance = container.GetInstance<Lazy<IService>>();
            Assert.IsInstanceOfType(lazyInstance, typeof(Lazy<IService>));
        }

        [TestMethod]
        public void GetInstance_LazyService_InstanceCreatedWhenValueIsAccessed()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            var lazyInstance = container.GetInstance<Lazy<IService>>();
            var instance = lazyInstance.Value;
            Assert.IsInstanceOfType(instance,typeof(IService));
        }

        [TestMethod]
        public void GetInstance_LazySingletonService_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(ISingleton), typeof(Singleton));
            var lazyInstance1 = container.GetInstance<Lazy<ISingleton>>();
            var lazyInstance2 = container.GetInstance<Lazy<ISingleton>>();
            Assert.AreEqual(lazyInstance1.Value, lazyInstance2.Value);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InvalidService_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            container.GetInstance<IFoo>();
        }

        [TestMethod]        
        public void GetInstance_InvalidServiceName_ReturnsResolvedDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service),"Service");
            var instance = container.GetInstance<IService>("SomeInvalidServiceName");
            Assert.IsInstanceOfType(instance,typeof(Service));
        }

        [TestMethod]
        public void GetInstance_InvalidServiceName_ReturnsDefaultServiceIfAvailable()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            var instance = container.GetInstance<IService>("SomeInvalidServiceName");
            Assert.IsInstanceOfType(instance, typeof(Service));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_InvalidServiceNameWithTwoNamedServices_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service),"SomeService");
            container.Register(typeof(IService), typeof(Service), "AnotherService");
            container.GetInstance<IService>("SomeInvalidServiceName");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_DefaultServiceWithTwoNamedServices_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service), "SomeService");
            container.Register(typeof(IService), typeof(Service), "AnotherService");
            container.GetInstance<IService>();
        }


        [TestMethod]
        public void GetInstance_IEnumerableOfT_ReturnsIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IService), typeof(Service));
            container.Register(typeof(IService), typeof(AnotherService),"AnotherService");
            var services = container.GetInstance<IEnumerable<IService>>();
            Assert.AreEqual(2,services.Count());
        }

        #region Constructor Dependency Injection

        [TestMethod]
        public void GetInstance_ServiceWithTransientDependency_ReturnsInstanceWithInjectedDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IService), typeof(ServiceWithDependency));
            var instance = (ServiceWithDependency)container.GetInstance<IService>();
            Assert.IsInstanceOfType(instance.Foo, typeof(Foo));
        }


        [TestMethod]
        public void GetInstance_ServiceWithIEnumerableDependency_ReturnsInstanceWithIEnumerableDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2),"Foo2");
            container.Register(typeof(IService), typeof(ServiceWithEnumerableDependency));
            var instance = (ServiceWithEnumerableDependency)container.GetInstance<IService>();
            Assert.IsInstanceOfType(instance.Foos,typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GetInstance_ServiceWithUnknownDependency_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();            
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.Register(typeof(IService), typeof(ServiceWithDependency));
            var instance = (ServiceWithDependency)container.GetInstance<IService>();
            Assert.IsInstanceOfType(instance.Foo, typeof(Foo));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_ServiceWithUnknownDependency_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();            
            container.Register(typeof(IService), typeof(ServiceWithDependency));
            container.GetInstance<IService>();
        }

        [TestMethod]
        public void GetInstance_InjectSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();            
            container.RegisterAsSingleton(typeof(ISingleton),typeof(Singleton));
            container.Register(typeof(IService), typeof(ServiceWithSingletonDependency));
            var instance1 = (ServiceWithSingletonDependency)container.GetInstance<IService>();
            var instance2 = (ServiceWithSingletonDependency)container.GetInstance<IService>();
            Assert.AreEqual(instance1.Singleton,instance2.Singleton);
        }


        [TestMethod]
        public void GetInstance_InjectLazyService_ReturnsInstanceWithInjectedLazyService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IService), typeof(ServiceWithLazyDependency));
            var instance = (ServiceWithLazyDependency)container.GetInstance<IService>();
            Assert.IsInstanceOfType(instance.LazyService,typeof(Lazy<IFoo>));
        }

        [TestMethod]
        public void GetInstance_FuncFactory_ReturnsFactoryCreatedDependency()
        {                        
            var container = CreateContainer();
            var foo = new Foo();
            container.Register<IFoo>(() => foo);
            container.Register(typeof (IFoo), typeof (Foo2),"Foo2");
            container.Register(typeof(IService),typeof(ServiceWithDependency));
            var instance = (ServiceWithDependency)container.GetInstance<IService>();
            Assert.AreEqual(foo,instance.Foo);
        }


        [TestMethod]
        public void GetInstance_InjectCustomFactoryOneImplementingType_DoesNotPassParameterNameAsServiceName()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));            
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.Register(typeof(IService), typeof(ServiceWithDependency));
            var factory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance<IService>();
            Assert.AreEqual(string.Empty,factory.ServiceName);
        }

        [TestMethod]
        public void GetInstance_InjectCustomFactoryTwoImplementingTypes_PassesParameterNameAsServiceName()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2), "AnotherFoo");            
            container.Register(typeof(IFactory), typeof(FooFactory));
            container.Register(typeof(IService), typeof(ServiceWithDependency));
            var factory = (FooFactory)container.GetInstance<IFactory>();
            container.GetInstance<IService>();
            Assert.AreEqual("foo", factory.ServiceName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_RecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            container.Register(typeof(IService), typeof(ServiceWithRecursiveDependency));
            container.GetInstance<IService>();            
        }


        [TestMethod]
        public void GetAllInstances_TwoServices_ReturnsIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo),typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2),"Foo2");
            var instances = container.GetAllInstances(typeof (IFoo));
            Assert.IsInstanceOfType(instances,typeof(IEnumerable<IFoo>));            
        }

        [TestMethod]
        public void GetAllInstances_TwoServices_ReturnsBothServices()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2), "Foo2");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.AreEqual(2,instances.Count());

        }

        [TestMethod]
        public void GenericGetAllInstances_TwoServices_ReturnsIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2), "Foo2");
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IFoo>));
        }

        [TestMethod]
        public void GenericGetAllInstances_UnknownService_ReturnsEmptyIEnumerable()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2), "Foo2");
            var instances = container.GetAllInstances<IService>();
            Assert.IsInstanceOfType(instances, typeof(IEnumerable<IService>));
        }

        [TestMethod]
        public void GenericGetAllInstances_TwoServices_ReturnsBothServices()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2), "Foo2");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetInstance_TwoServices_ReturnsSingleIEnumerableInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(Foo2), "Foo2");
            container.RegisterAsSingleton(typeof(IEnumerable<IFoo>),null);
            var instance1 = container.GetInstance<IEnumerable<IFoo>>();
            var instance2 = container.GetInstance<IEnumerable<IFoo>>();
            Assert.AreSame(instance1,instance2);
        }


        [TestMethod]
        public void GetInstance_OpenGenericTypeWithSingletonDependency_ReturnsSameDependencyInstance()
        {
            var container = CreateContainer();
            container.RegisterAsSingleton(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IService<>),typeof(ServiceWithGenericDependency<>));
            var instance1 = (ServiceWithGenericDependency<IFoo>)container.GetInstance(typeof(IService<IFoo>));
            var instance2 = (ServiceWithGenericDependency<IFoo>)container.GetInstance(typeof(IService<IFoo>));
            Assert.AreEqual(instance1.Dependency,instance2.Dependency);
        }


        #endregion
        private static ServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    
    
        public void GetInstance_NoConfig_ReturnsInstance()
        {
            var container = new ServiceContainer();
            IService service = container.GetInstance<IService>();
        }
    
    }

    
}
