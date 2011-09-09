using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
namespace DependencyInjector.Tests
{
    [TestClass]
    public class ServiceContainerTests
    {
        //private static ServiceContainer _serviceContainer;

        //[ClassInitialize]
        //public static void ClassInitialize(TestContext testContext)
        //{
        //    _serviceContainer = new ServiceContainer(); 
            
        //}
        
        
        //[TestMethod]
        //public void GetInstance_InterfaceImplementedByOneClass_ReturnsInstance()
        //{
        //    var instance = _serviceContainer.GetInstance<ISimpleService>();
        //    Assert.IsNotNull(instance);
        //}


        

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void GetInstance_InvalidService_ThrowsException()
        //{
        //    _serviceContainer.GetInstance<IUnknown>();
        //}


        //[TestMethod]
        //public void GetInstance_InterfaceWithMultipleImplementations_ReturnsDefaultInstance()
        //{           
        //    var service = _serviceContainer.GetInstance<ICar>();            
        //    Assert.IsInstanceOfType(service,typeof(Car));
        //}
        
        //[TestMethod]
        //public void GetInstance_NamedServiceWithMultipleImplementations_ReturnsConcreteClassCorrespondingToServiceName()
        //{
        //    var service = _serviceContainer.GetInstance<IFoo>("Foo1");            
        //    Assert.IsInstanceOfType(service, typeof(Foo1));
        //}

        //[TestMethod]
        //public void GetInstance_InterfaceThroughAbstractBaseClass_ReturnsInheritedClass()
        //{
        //    var service = _serviceContainer.GetInstance<IAbstractService>();
        //    Assert.IsInstanceOfType(service, typeof(ServiceWithAbstractBaseClass));
        //}



        //[TestMethod]
        //public void GetAllInstances_InterfaceWithMultipleImplementations_ReturnsIEnumerableContainingAllInstances()
        //{            
        //    IEnumerable<IFoo> services = _serviceContainer.GetAllInstances<IFoo>();
        //    Assert.AreEqual(4, services.Count());
        //}

     

        
        //[TestMethod]
        //public void GetInstance_AbstractClass_ReturnsInstanceOfInheritedClass()
        //{
        //    var service = _serviceContainer.GetInstance<AbstractService>();
        //    Assert.IsInstanceOfType(service,typeof(ServiceWithAbstractBaseClass));
        //}


        
        //[TestMethod]
        //public void GetAllInstances_UnknownInterface_ReturnsIEnumerableWithZeroInstances()
        //{
        //    var services = _serviceContainer.GetAllInstances<IUnknown>();
        //    Assert.AreEqual(0, services.Count());
        //}

        //[TestMethod]
        //public void MustBeAbleToInjectDependency()
        //{
        //    var instance = _serviceContainer.GetInstance<ICustomerManager>();
        //    Assert.IsInstanceOfType(((CustomerManager)instance).Customer,typeof(Customer));
        //}

        //[TestMethod]
        //public void MustBeAbleToInjectEnumerableDependencies()
        //{
        //    var instance = _serviceContainer.GetInstance<IParkingLot>();            
        //    Assert.IsNotNull(((ParkingLot)instance).Cars);
        //    Assert.AreEqual(2, ((ParkingLot)instance).Cars.Count());
        //}
        
        //[TestMethod]
        //public void GetInstance_SingletonService_ReturnsSameInstance()
        //{
        //    var firstResult = _serviceContainer.GetInstance<ISingleton>();
        //    var secondResult = _serviceContainer.GetInstance<ISingleton>();
        //    Assert.AreEqual(firstResult, secondResult);
        //}

        //[TestMethod]
        //public void GetInstance_NamedSingletonService_ReturnsSameInstance()
        //{
        //    var firstResult = _serviceContainer.GetInstance<ISingleton>("SampleSingleton");
        //    var secondResult = _serviceContainer.GetInstance<ISingleton>("SampleSingleton");
        //    Assert.AreEqual(firstResult,secondResult);
        //}
                
        //[TestMethod]
        //public void GetInstance_NamedAndDefaultSingletonService_ReturnsSameInstance()
        //{
        //    var firstResult = _serviceContainer.GetInstance<ISingleton>();
        //    var secondResult = _serviceContainer.GetInstance<ISingleton>();
        //    Assert.AreEqual(firstResult, secondResult);
        //}



        //[TestMethod]
        //public void MustBeAbleToInjectSingletonService()
        //{
        //    var result = (SampleClassWithSingletonDepedency)_serviceContainer.GetInstance<ISampleService>("SampleClassWithSingletonDepedency");            
        //    Assert.AreEqual(result.First,result.Second);
        //}

        
        
        //[TestMethod]
        //public void GetInstance_OnRequestingGenericServiceWithClosedGenericImplementation_ReturnsInstance()
        //{
        //    _serviceContainer.Register(typeof(IGenericInterface<string>), typeof(ClosedGenericClass));
        //    var instance = _serviceContainer.GetInstance<IGenericInterface<string>>();
        //    Assert.IsInstanceOfType(instance,typeof(ClosedGenericClass));
        //}

        //[TestMethod]
        //public void GetInstance_OnRequestingGenericServiceWithOpenGenericImplementation_ReturnsInstance()
        //{
        //    var i = typeof (OpenGenericClass<>).GetInterfaces().First().GetGenericTypeDefinition(); 
        //    Assert.AreEqual(typeof(IGenericInterface<>),i.GetGenericTypeDefinition());
        //    //_serviceContainer.Register(typeof(IGenericInterface<string>), typeof(ClosedGenericClass));
        //    //_serviceContainer.Register(typeof(IGenericInterface<>),typeof(OpenGenericClass<>));
        //    var instance = _serviceContainer.GetInstance<IGenericInterface<int>>("OpenGenericClass1");
        //    Assert.IsInstanceOfType(instance, typeof(OpenGenericClass<int>));
        //}

        //public void GetInstance_NamedOpenGenericService()
        //{
        //    //_serviceContainer.Register(typeof(IGenericInterface<>),typeof(OpenGenericClass1<>));
        //    var instance = _serviceContainer.GetInstance<IGenericInterface<int>>("OpenGenericClass1");
        //    Assert.IsInstanceOfType(instance, typeof(OpenGenericClass<int>));
        //}



        //[TestMethod]
        //public void GetInstance_OnRequestingNamedGenericServiceWithOpenGenericImplementation_ReturnsOpenGenericInstanceCorrespondingToServiceName()
        //{
        //    var instance = _serviceContainer.GetInstance<IGenericInterface<int>>("OpenGenericClass1");
        //    Assert.IsInstanceOfType(instance, typeof(OpenGenericClass<int>));
        //}
              
        
        //[TestMethod]
        //public void GetInstance_OnRequestingSameFactoryTwice_ReturnsSameInstance()
        //{
        //    var result1 = _serviceContainer.GetInstance<IFactory>("SqlCommandFactory");
        //    var result2 = _serviceContainer.GetInstance<IFactory>("SqlCommandFactory");
        //    Assert.AreEqual(result1,result2);
        //}


        
        //[TestMethod]
        //public void GetInstance_WhenRequestingUnknownServiceThatHasFactory_ReturnsInstance()
        //{
        //    var service = _serviceContainer.GetInstance<ICloneable>();
        //    Assert.IsNotNull(service); 
        //    Assert.IsInstanceOfType(service,typeof(string));
        //}


        
        //[TestMethod]
        //public void GetInstance_WhenRequestingUnknownServiceThatHasFactory_ServiceNameIsPassedToFactoryInstance()
        //{
                                    
        //    var service = _serviceContainer.GetInstance<ICloneable>();
        //    var factory = (ClonableFactory)_serviceContainer.GetInstance<IFactory>("ClonableFactory");
        //    _serviceContainer.GetInstance<ICloneable>("SomeServiceName");
        //    Assert.AreEqual("SomeServiceName",factory.ServiceName);
        //}
      
        //[TestMethod]
        //public void MustBeAbleToResolveLazyService()
        //{
        //    var result = _serviceContainer.GetInstance<Lazy<ISimpleService>>();
        //    Assert.IsNotNull(result);    
        //    Assert.IsNotNull(result.Value);            
        //}

        //[TestMethod]
        //public void MustBeAbleToInjectLazyService()
        //{
        //    var result = (SampleClassWithLazyDependency)_serviceContainer.GetInstance<ISampleService>("SampleClassWithLazyDependency");
        //    Assert.IsNotNull(result.LazyCustomer);
        //    Assert.IsNotNull(result.LazyCustomer.Value);
        //}

        
        //[TestMethod]
        //public void GetInstance_WhenRequestingConcreteService_ReturnsInstanceOfRequestedType()
        //{
        //    var result = _serviceContainer.GetInstance<SampleService>();
        //    Assert.IsNotNull(result);
        //}
        
        //[TestMethod]
        //public void GetInstance_BaseClassImplementedByConcreteClass_ReturnsInstanceOfInheritedClass()
        //{
        //    var result = _serviceContainer.GetInstance<AbstractService>();            
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOfType(result, typeof(ServiceWithAbstractBaseClass));
        //}
            
        
        
        //[TestMethod]
        //public void GetInstance_FunctionDelegate_ReturnsFunctionDelegateCapableOfCreatingServiceInstance()
        //{
        //    var result = _serviceContainer.GetInstance<Func<ISampleService>>();
        //    Assert.IsNotNull(result);
        //    var instance = result();
        //    Assert.IsNotNull(instance);
        //}
        
        
        //[TestMethod]
        //public void GetInstance_FunctionDelegate_ReturnsFunctionDelegateAsSingleton()
        //{
        //    var result1 = _serviceContainer.GetInstance<Func<ISampleService>>();
        //    var result2 = _serviceContainer.GetInstance<Func<ISampleService>>();
        //    Assert.AreSame(result1,result2);
        //}

        //[TestMethod]
        //public void GetInstance_FuncFactory_ReturnsInstance()
        //{
        //    Mock<IFactoryCreatedService> mock = new Mock<IFactoryCreatedService>();
            
        //    _serviceContainer.Register<IFactoryCreatedService>(() => mock.Object);

        //    var instance = _serviceContainer.GetInstance<IFactoryCreatedService>();
        //    Assert.AreSame(instance,mock.Object);

        //}


        //[TestMethod]
        //public void ABC()
        //{
        //    NewExpression newExpression = Expression.New(typeof(Service).GetConstructor(Type.EmptyTypes));
        //    LambdaExpression lambdaExpression = Expression.Lambda<Func<object>>(newExpression);
        //    Func<object> compiledLambda = (Func<object>)lambdaExpression.Compile();
        //    IService instance = (IService)compiledLambda();
        //}

        ////[TestMethod]
        ////public void ABC2()
        ////{
        ////    NewExpression newExpressionA = Expression.New(typeof(Service).GetConstructor(Type.EmptyTypes));            
        ////    NewExpression newExpressionB = Expression.New(typeof(ServiceB).GetConstructors().Single(),newExpressionA);                        
        ////    LambdaExpression lambdaExpression = Expression.Lambda<Func<object>>(newExpressionB);
        ////    Func<object> compiledLambda = (Func<object>)lambdaExpression.Compile();
        ////    IServiceB instance = (IServiceB)compiledLambda();
        ////}






        //public void NewOperator()
        //{
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    for (int i = 0; i < 1000000; i++)
        //    {
        //        Game game = new Game();
        //        Player sampleService = new Player(game, new Gun(game, () => new Bullet(game)));
        //    }
        //    stopwatch.Stop();
        //    Console.WriteLine(string.Format("New Operator : {0}", stopwatch.ElapsedMilliseconds));
        //}

        //public void GetInstance()
        //{
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    _serviceContainer.GetInstance<Player>();
        //    for (int i = 0; i < 1000000; i++)
        //    {
        //        _serviceContainer.GetInstance<Player>();
        //    }
        //    stopwatch.Stop();
        //    Console.WriteLine(string.Format("GetInstance<ISampleService> : {0}", stopwatch.ElapsedMilliseconds));
        //}



      
   
      

        


        


        //[TestMethod]
        //public void PerformanceTest10()
        //{
        //    _serviceContainer.GetInstance<Player>();
        //    NewOperator();
        //    //LambdaUsingTypedArument();
        //    //LambdaUsingObject();
        //    GetInstance();
        //    //GetInstanceObject();
        //    //ResolveUsingLambda();


                
        //}

       
    }
}
