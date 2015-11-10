using LightMock;

namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using System.Security;
    
    using System.Text;
    using System.Threading.Tasks;

    using LightInject;
    using LightInject.SampleLibrary;
    using Xunit;

    using Foo = LightInject.SampleLibrary.Foo;
    using IFoo = LightInject.SampleLibrary.IFoo;
    using IBar = LightInject.SampleLibrary.IBar;
    using Bar = LightInject.SampleLibrary.Bar;
    
    public class ServiceContainerTests : TestBase
    {
        #region InputValidation

        //[Fact]
        //public void FailingTest()
        //{
        //    Assert.True(1 == 0);
        //}

        //[Fact]
        //public void AnotherFailingTest()
        //{
        //    Assert.True(1 == 0);
        //}

        [Fact]
        public void Register_NullServiceType_ThrowsArgumentNullException()
        {
            var container = CreateContainer();
            ExceptionAssert.Throws<ArgumentNullException>(
                () => container.Register(null, typeof(Foo)),
                e => e.ParamName == "serviceType");
        }

        [Fact]
        public void Register_NullImplementingType_ThrowsArgumentNullException()
        {
            var container = CreateContainer();
            ExceptionAssert.Throws<ArgumentNullException>(
                () => container.Register(typeof(IFoo), (Type)null),
                e => e.ParamName == "implementingType");
        }

        [Fact]
        public void Register_NullServiceName_ThrowsArgumentNullException()
        {
            var container = CreateContainer();
            ExceptionAssert.Throws<ArgumentNullException>(
                () => container.Register(typeof(IFoo), typeof(Foo), (string)null),
                e => e.ParamName == "serviceName");
        }

        //[Fact]
        //public void Register_NonCompatibleImplementingType_ThrowsArgumentException()
        //{
        //    var container = CreateContainer();
        //    ExceptionAssert.Throws<ArgumentException>(
        //        () => container.Register(typeof(IBar), typeof(Foo)),
        //        e => e.ParamName == "implementingType");
        //}

        [Fact]
        public void RegisterInstance_NullServiceType_ThrowsArgumentNullException()
        {
            var container = CreateContainer();
            ExceptionAssert.Throws<ArgumentNullException>(
                () => container.RegisterInstance(null, new object()),
                e => e.ParamName == "serviceType");
        }

        [Fact]
        public void RegisterInstance_NullInstance_ThrowsArgumentNullException()
        {
            var container = CreateContainer();
            ExceptionAssert.Throws<ArgumentNullException>(
                () => container.RegisterInstance((string)null),
                e => e.ParamName == "instance");
        }

        [Fact]
        public void RegisterInstance_NullServiceName_ThrowsArgumentNullException()
        {
            var container = CreateContainer();
            ExceptionAssert.Throws<ArgumentNullException>(
                () => container.RegisterInstance(typeof(object), new object(), null),
                e => e.ParamName == "serviceName");
        }
        #endregion


        #region Values

        [Fact]
        public void GetInstance_ReferenceTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.Equal("SomeValue", value);
        }

        [Fact]
        public void GetInstance_ReferenceTypeValue_ReturnsLastRegisteredValue()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.RegisterInstance("AnotherValue");
            var value = (string)container.GetInstance(typeof(string));
            Assert.Equal("AnotherValue", value);
        }

        [Fact]
        public void GetInstance_ReferenceTypeValue_ReturnSameValue()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            var value1 = (string)container.GetInstance(typeof(string));
            var value2 = (string)container.GetInstance(typeof(string));
            Assert.Same(value1, value2);
        }

        [Fact]
        public void GetInstance_ValueTypeValue_ReturnsValue()
        {
            var container = CreateContainer();
            container.RegisterInstance(42);
            var value = (int)container.GetInstance(typeof(int));
            Assert.Equal(42, value);
        }
        

        [Fact]
        public void GetInstance_NamedValue_ReturnsNamedValue()
        {
            var container = CreateContainer();
            container.RegisterInstance(42, "Answer");
            var value = (int)container.GetInstance(typeof(int), "Answer");
            Assert.Equal(42, value);
        }

        [Fact]
        public void GetInstance_ValueTypeAsSingeton_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register<int>(factory => 42, new PerContainerLifetime());
            var value = container.GetInstance<int>();
            Assert.Equal(42, value);
        }

        [Fact]
        public void GetInstance_ValueTypeAsPerScope_ReturnsValue()
        {
            var container = CreateContainer();
            container.Register<int>(factory => 42, new PerScopeLifetime());
            using (container.BeginScope())
            {
                var value = container.GetInstance<int>();
                Assert.Equal(42, value);    
            }
        }

        #endregion

        [Fact]
        public void GetInstance_ConcreteService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<Foo>();

            var instance = container.GetInstance<Foo>();

            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_ConcreteServiceUsingNonGenericMethod_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(Foo));

            var instance = container.GetInstance<Foo>();

            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_ConcreteSingletonServiceUsingNonGenericMethod_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(Foo), new PerContainerLifetime());

            var first = container.GetInstance<Foo>();
            var second = container.GetInstance<Foo>();
            Assert.Same(first, second);
        }



        [Fact]
        public void GetInstance_ConcreteServiceAsSingleton_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register<Foo>(new PerContainerLifetime());

            var firstInstance = container.GetInstance<Foo>();
            var secondInstance = container.GetInstance<Foo>();

            Assert.Same(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_UnknownService_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();

            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IBar>());
        }
        
        
        [Fact]
        public void GetInstance_OneService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_TwoServices_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, Foo>("AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_TwoNamedServices_ThrowsExceptionWhenRequestingDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo)), ErrorMessages.UnableToResolveType);
        }

        [Fact]
        public void GetInstance_DuplicateRegistration_ReturnsLastRegisteredService()
        {
            var container = CreateContainer();            
            container.Register<IFoo, Foo>();
            container.RegisterInstance<IFoo>(new AnotherFoo());
            var instance = container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(AnotherFoo), instance);
        }

        [Fact]
        public void GetInstance_UnknownGenericType_ThrowsExceptionWhenRequestingDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar<>), typeof(Bar<>));
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo<int>)), ErrorMessages.UnknownGenericDependency);
        }

        [Fact]
        public void GetInstance_TwoServices_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            object instance = container.GetInstance(typeof(IFoo), "AnotherFoo");
            Assert.IsAssignableFrom(typeof(AnotherFoo), instance);
        }

        [Fact]
        public void GetInstance_TwoServices_ReturnsNamedInstanceAfterGettingDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.GetInstance(typeof(IFoo), "AnotherFoo");
            object defaultInstance = container.GetInstance(typeof(IFoo));            
            Assert.IsAssignableFrom(typeof(Foo), defaultInstance);
        }

        [Fact]
        public void GetInstance_OneNamedService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            object instance = container.GetInstance(typeof(IFoo));
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_OneNamedClosedGenericService_ReturnsDefaultService()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>), "SomeFoo");
            object instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsAssignableFrom(typeof(Foo<int>), instance);
        }

        [Fact]
        public void GetInstance_NamedService_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            object instance = container.GetInstance<IFoo>("SomeFoo");
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_NamedInstanceTwice_CompilesOnlyOnce()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
            container.GetInstance<IFoo>("SomeFoo");
            container.GetInstance<IFoo>("SomeFoo");


        }


        [Fact]
        public void GetInstance_OpenGenericType_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsAssignableFrom(typeof(Foo<int>), instance);
        }

        [Fact]
        public void GetInstance_OpenGenericType_ReturnsInstanceOfLastRegisteredType()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>));
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsAssignableFrom(typeof(AnotherFoo<int>), instance);
        }

        [Fact]
        public void GetInstance_NamedOpenGenericType_ReturnsDefaultInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            var instance = container.GetInstance(typeof(IFoo<int>));
            Assert.IsAssignableFrom(typeof(Foo<int>), instance);
        }

        [Fact]
        public void GetInstance_OpenGenericType_ReturnsTransientInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void GetInstance_OpenGenericType_ReturnsClosedGenericInstancesIfPresent()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<string>), typeof(FooWithStringTypeParameter));
            var instance = container.GetInstance(typeof(IFoo<string>));
            Assert.IsAssignableFrom(typeof(FooWithStringTypeParameter), instance);
        }

        [Fact]
        public void GetInstance_DefaultAndNamedOpenGenericType_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsAssignableFrom(typeof(AnotherFoo<int>), instance);
        }

        [Fact]
        public void GetInstance_TwoNamedOpenGenericTypes_ReturnsNamedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instance = container.GetInstance(typeof(IFoo<int>), "AnotherFoo");
            Assert.IsAssignableFrom(typeof(AnotherFoo<int>), instance);
        }

        [Fact]
        public void GetInstance_OpenGenericTypeWithDependency_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo<>), typeof(FooWithGenericDependency<>));
            var instance = (FooWithGenericDependency<IBar>)container.GetInstance(typeof(IFoo<IBar>));
            Assert.IsAssignableFrom(typeof(Bar), instance.Dependency);
        }

        [Fact]
        public void GetInstance_OpenGenericSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo<int>));
            var instance2 = container.GetInstance(typeof(IFoo<int>));
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetInstance_Singleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetInstance_Singleton_CallsConstructorOnlyOnce()
        {
            var container = CreateContainer();
            Foo.Instances = 0;
            container.Register(typeof(IFoo), typeof(Foo), new PerContainerLifetime());
            container.GetInstance(typeof(IFoo));
            container.GetInstance(typeof(IFoo));
            Assert.Equal(1, Foo.Instances);
        }

        [Fact]
        public void GetInstance_NamedSingleton_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo", new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo), "SomeFoo");
            var instance2 = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetInstance_PerScopeService_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<IFoo>();
                var instance2 = container.GetInstance<IFoo>();
                Assert.Same(instance1, instance2);
            }            
        }

        [Fact]
        public void GetInstance_PerScopeServiceOutSideOfScope_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());

            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetInstance<IFoo>(), e => e.Message == ErrorMessages.GetInstanceOutSideScope);
        }

        [Fact]
        public void GetInstance_GenericServiceWithPerScopeLifetime_DoesNotShareLifetimeInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerScopeLifetime());
            using(container.BeginScope())
            {
                var intInstance = container.GetInstance<IFoo<int>>();
                var stringInstance = container.GetInstance<IFoo<string>>();
                Assert.IsAssignableFrom(typeof(IFoo<int>), intInstance);
                Assert.IsAssignableFrom(typeof(IFoo<string>), stringInstance);
            }            
        }

        [Fact]
        public void GetInstance_PerRequestService_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerRequestLifeTime());
            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<IFoo>();
                var instance2 = container.GetInstance<IFoo>();
                Assert.NotSame(instance1, instance2);
            }
        }

        [Fact]
        public void GetInstance_DisposablePerRequestServiceOutsideScope_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, DisposableFoo>(new PerRequestLifeTime());

            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetInstance<IFoo>(),
                exception => exception.Message == ErrorMessages.DisposableOutsideScope);

        }

        #region Array
        
        [Fact]
        public void GetInstance_Array_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IFoo[]>();
            Assert.Equal(2, services.Length);
        }

        #endregion

        #region List

        [Fact]
        public void GetInstance_List_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IList<IFoo>>();
            Assert.Equal(2, services.Count);
        }

        #endregion

        #region Collection

        [Fact]
        public void GetInstance_Collection_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<ICollection<IFoo>>();
            Assert.Equal(2, services.Count);
        }

        #endregion

#if NET45 || PCL_111 || NET46
        #region ReadOnly Collection

        [Fact]
        public void GetInstance_ReadOnlyCollection_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IReadOnlyCollection<IFoo>>();
            Assert.Equal(2, services.Count);
        }

        [Fact]
        public void GetInstance_GenericFooWithReadOnlyCollection_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo<IReadOnlyCollection<IBar>>, FooWithGenericDependency<IReadOnlyCollection<IBar>>>();

            var instance = (FooWithGenericDependency<IReadOnlyCollection<IBar>>)container.GetInstance<IFoo<IReadOnlyCollection<IBar>>>();

            Assert.Equal(1, instance.Dependency.Count);
        }


        #endregion

        #region ReadOnly List

        [Fact]
        public void GetInstance_ReadOnlyList_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IReadOnlyList<IFoo>>();
            Assert.Equal(2, services.Count);
        }


        #endregion
#endif
        #region Func Services
                
        [Fact]
        public void GetInstance_Func_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            var factory = container.GetInstance(typeof(Func<IFoo>));
            Assert.IsAssignableFrom(typeof(Func<IFoo>), factory);
        }
    

        [Fact]
        public void GetInstance_FuncWithStringArgument_ReturnsFuncInstance()
        {
            var container = CreateContainer();
            var factory = container.GetInstance(typeof(Func<string, IFoo>));
            Assert.IsAssignableFrom(typeof(Func<string, IFoo>), factory);
        }

        [Fact]
        public void GetInstance_Func_ReturnsSameInstance()
        {
            var container = CreateContainer();
            var factory1 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var factory2 = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            Assert.Same(factory1, factory2);
        }

        [Fact]
        public void GetInstance_FuncWithStringArgument_ReturnsSameInstance()
        {
            var container = CreateContainer();
            var factory1 = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            var factory2 = (Func<string, IFoo>)container.GetInstance(typeof(Func<string, IFoo>));
            Assert.Same(factory1, factory2);
        }

        [Fact]
        public void GetInstance_Func_IsAbleToCreateInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance = factory();
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }
        
        [Fact]
        public void GetInstance_FuncWithSingletonTarget_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetInstance_FuncWithTransientTarget_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var factory = (Func<IFoo>)container.GetInstance(typeof(Func<IFoo>));
            var instance1 = factory();
            var instance2 = factory();
            Assert.NotSame(instance1, instance2);
        }
  
        

        #endregion
        #region Func Factory

        [Fact]
        public void GetInstance_FuncFactoryValueType_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<int>(c => 42);
            var instance = container.GetInstance(typeof(int));
            Assert.Equal(42, instance);
        }

        [Fact]
        public void GetInstance_FuncFactoryValueTypeWithLifetime_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            using (container.BeginScope())
            {
                container.Register<int>(c => 42, new PerContainerLifetime());
                var instance = container.GetInstance(typeof(int));
                Assert.Equal(42, instance);    
            }            
        }

        [Fact]
        public void GetInstance_FuncFactoryReferenceTypeWithLifetime_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            using (container.BeginScope())
            {
                container.Register<string>(c => "SomeValue", new PerContainerLifetime());
                var instance = container.GetInstance(typeof(string));
                Assert.Equal("SomeValue", instance);
            }
        }


        [Fact]
        public void GetInstance_FuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo());
            var instance = container.GetInstance(typeof(IFoo));
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }
    
        [Fact]
        public void GetInstance_FuncFactory_ReturnsLastRegisteredFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithMultipleConstructors());
            container.Register<IFoo>(c => new FooWithMultipleConstructors(new Bar()));
            var instance = (FooWithMultipleConstructors)container.GetInstance(typeof(IFoo));            
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_NamedFuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), "SomeFoo");
            var instance = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void GetInstance_NamedSingletonFuncFactory_ReturnsFactoryCreatedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), "SomeFoo", new PerContainerLifetime());
            var firstInstance = container.GetInstance(typeof(IFoo), "SomeFoo");
            var secondInstance = container.GetInstance(typeof(IFoo), "SomeFoo");
            Assert.Same(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_Funcfactory_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar>(c => new Bar());
            container.Register<IFoo>(c => new FooWithDependency(c.GetInstance<IBar>()));
            var instance = (FooWithDependency)container.GetInstance(typeof(IFoo));
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithReferenceTypeDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithReferenceTypeDependency("SomeStringValue"));
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo));
            Assert.Equal("SomeStringValue", instance.Value);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithValueTypeDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithValueTypeDependency(42));
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo));
            Assert.Equal(42, instance.Value);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithEnumDepenedency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new FooWithEnumDependency(Encoding.UTF8));
            var instance = (FooWithEnumDependency)container.GetInstance(typeof(IFoo));
            Assert.Equal(Encoding.UTF8, instance.Value);
        }

        [Fact]
        public void GetInstance_SingletonFuncFactory_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(c => new Foo(), new PerContainerLifetime());
            var instance1 = container.GetInstance(typeof(IFoo));
            var instance2 = container.GetInstance(typeof(IFoo));
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithMethodCall_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(factory => GetFoo());
            var foo = container.GetInstance<IFoo>();
            Assert.NotNull(foo);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithArrayDependency_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IBar, AnotherBar>("AnotherBar");
            container.Register<IFoo>(factory => new FooWithArrayDependency(container.GetAllInstances<IBar>().ToArray()));
            var instance = (FooWithArrayDependency)container.GetInstance<IFoo>();
            Assert.Equal(2, instance.Bars.Count());
        }

        [Fact]
        public void GetInstance_FuncFactoryWithArrayInitializer_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
           
            container.Register<IFoo>(factory => new FooWithArrayDependency(new[] { factory.GetInstance<IBar>() }));
            var instance = (FooWithArrayDependency)container.GetInstance<IFoo>();
            Assert.Equal(1, instance.Bars.Count());
        }

        [Fact]
        public void GetInstance_FuncFactoryWithParamsArrayInitializer_ReturnsInstanceWithDependency()
            {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            container.Register<IFoo>(factory => new FooWithParamsArrayDependency(new[] { factory.GetInstance<IBar>() }));
            var instance = (FooWithParamsArrayDependency)container.GetInstance<IFoo>();
            Assert.Equal(1, instance.Bars.Count());
        }

        [Fact]
        public void GetInstance_SingletonFuncFactoryWithMethodCall_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.Register(factory => GetFoo(), new PerContainerLifetime());
            var instance1 = container.GetInstance<IFoo>();
            var instance2 = container.GetInstance<IFoo>();
            Assert.Same(instance1, instance2);
        }

        private IFoo GetFoo()
        {
            return new Foo();
        }

        [Fact]
        public void GetInstance_FuncWithClosure_ReturnsInstance()
        {
            var container = new ServiceContainer();
            Register(container,"SomeName");
            var foo = container.GetInstance<IFoo>("SomeName");
            Assert.IsType<FooWithDependency>(foo);
        }

        private void Register(IServiceContainer container, string serviceName)
        {
            container.Register<IBar, Bar>(serviceName);
            container.Register<IFoo>(f => new FooWithDependency(f.GetInstance<IBar>(serviceName)), serviceName);
        }


        #endregion

        #region IEnumerable

        [Fact]
        public void GetInstance_IEnumerable_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var services = container.GetInstance<IEnumerable<IFoo>>();
            Assert.Equal(2, services.Count());
        }

        [Fact]
        public void GenericGetAllInstances_UnknownService_ReturnsEmptyIEnumerable()
        {
            var container = CreateContainer();
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsAssignableFrom(typeof(IEnumerable<IFoo>), instances);
        }

        [Fact]
        public void GetInstance_IEnumerableWithReferenceTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.RegisterInstance("AnotherValue", "AnotherStringValue");
            var services = container.GetInstance<IEnumerable<string>>();
            Assert.Equal(2, services.Count());
        }

        [Fact]
        public void GetInstance_IEnumerableWithValueTypes_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.RegisterInstance(1024);
            container.RegisterInstance(2048, "AnotherInt");
            var services = container.GetInstance<IEnumerable<int>>();
            Assert.Equal(2, services.Count());
        }

        [Fact]
        public void GetInstance_IEnumerable_ReturnsTransientInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instance1 = container.GetInstance<IEnumerable<IFoo>>();
            var instance2 = container.GetInstance<IEnumerable<IFoo>>();
            Assert.NotSame(instance1, instance2);
        }
               
        [Fact]
        public void GetAllInstances_NonGeneric_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances(typeof(IFoo));
            Assert.IsAssignableFrom(typeof(IEnumerable<IFoo>), instances);
        }

        [Fact]
        public void GetAllInstances_Generic_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo));
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsAssignableFrom(typeof(IEnumerable<IFoo>), instances);
        }

        [Fact]
        public void GetAllInstances_TwoOpenGenericServices_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.Equal(2, instances.Count());
        }

        [Fact]
        public void GetInstance_KnownOpenGenericCollection_ReturnsKnownCollection()
        {
            var container = CreateContainer();
            container.Register(typeof(ICollection<>), typeof(FooCollection<>));
            var instance = container.GetInstance<ICollection<int>>();
            Assert.IsType<FooCollection<int>>(instance);
        }



        [Fact]
        public void GetAllInstances_ClosedAndOpenGenericService_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.Equal(2, instances.Count());
        }

        [Fact]
        public void GetAllInstances_EnumerableWithRecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.GetAllInstances<IFoo>(), ex => ex.InnerException.InnerException.InnerException.Message == ErrorMessages.RecursiveDependency);
        }

        [Fact]
        public void GetAllInstances_Inheritance_ReturnsDerivedInstance()
        {
            var container = CreateContainer();
            container.Register<Foo>();
            container.Register<DerivedFoo>();
            var instances = container.GetAllInstances<Foo>();
            Assert.Equal(2, instances.Count());
        }

        [Fact]
        public void GetAllInstance_WithDisabledVariance_DoesNotReturnDerivedInstances()
        {
            var container = CreateContainer(new ContainerOptions { EnableVariance = false });
            container.Register<Foo>();
            container.Register<DerivedFoo>();
            var instances = container.GetAllInstances<Foo>();
            Assert.Equal(1, instances.Count());
        }

        [Fact]
        public void GetInstance_UsingServicePredicate_ReturnsInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo());
            var instance = container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(IFoo), instance);
        }

        [Fact]
        public void GetInstance_PerContainerLifetimeUsingServicePredicate_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo(), new PerContainerLifetime());
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();
            Assert.Same(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_UsingFallBack_ProvidesServiceReuest()
        {
            var container = CreateContainer();
            ServiceRequest serviceRequest = null;
            container.RegisterFallback((type, s) => type == typeof(IFoo), r =>
            {
                serviceRequest = r;
                return new Foo();
            });
            container.GetInstance<IFoo>();
            Assert.NotNull(serviceRequest.ServiceType);
            Assert.NotNull(serviceRequest.ServiceFactory);
            Assert.NotNull(serviceRequest.ServiceName);
        }

        [Fact]
        public void GetInstance_UsingFallback_DoesNotReuseLifetimeAcrossServices()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => true, request => Activator.CreateInstance(request.ServiceType), new PerContainerLifetime());
            var foo = container.GetInstance(typeof(Foo));
            var bar = container.GetInstance(typeof(Bar));            
            Assert.IsAssignableFrom(typeof(Foo), foo);
            Assert.IsAssignableFrom(typeof(Bar), bar);
        }

        [Fact]
        public void GetInstance_UsingFallbackForDependency_ReturnsSingleInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback((type, s) => true, request => new Bar(), new PerContainerLifetime());
            container.Register<IFoo, FooWithDependency>();
            var bar = container.GetInstance<IBar>();
            var foo = (FooWithDependency)container.GetInstance<IFoo>();
            var foo2 = (FooWithDependency)container.GetInstance<IFoo>();
            //Assert.Same(foo.Bar, foo2.Bar);
            Assert.Same(bar, foo.Bar);

        }

        [Fact]
        public void CanGetInstance_KnownService_ReturnsTrue()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var canCreateInstance = container.CanGetInstance(typeof(IFoo), string.Empty);
            Assert.True(canCreateInstance);
        }
        [Fact]
        public void CanGetInstance_UnknownService_ReturnFalse()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var canCreateInstance = container.CanGetInstance(typeof(IBar), string.Empty);
            Assert.False(canCreateInstance);
        }      

        [Fact]
        public void GetInstance_RegisterAfterGetInstance_ReturnsDependencyOfSecondRegistration()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IBar, Bar>();
            container.GetInstance<IFoo>();            
            //container.Register<IBar, AnotherBar>();

            //var instance = (FooWithDependency)container.GetInstance<IFoo>();

            //Assert.IsAssignableFrom(typeof(AnotherBar), instance.Bar);
        }

        [Fact]
        public void Run()
        {
            for (int i = 0; i < 1; i++)
            {
                GetInstance_SingletonUsingMultipleThreads_ReturnsSameInstance();
            }
        }
 
        [Fact]
        public void GetInstance_SingletonUsingMultipleThreads_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(SingletonFoo), new PerContainerLifetime());
            SingletonFoo.Instances = 0;
            
            for (int i = 0; i < 100; i++)
            {
                RunParallel(container);
            }
                      
            Assert.Equal(1, SingletonFoo.Instances);
        }

        [Fact]
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

        [Fact]
        public void BeginResolutionScope_TwoContainers_ResolutionContextIsScopedPerContainer()
        {
            var firstContainer = CreateContainer();
            var secondContainer = CreateContainer();
            
            using (Scope firstResolutionScope = firstContainer.BeginScope())
            {
                using (Scope secondResolutionScope = secondContainer.BeginScope())
                {
                    Assert.NotSame(firstResolutionScope, secondResolutionScope);
                }
            }
        }

        [Fact]
        public void Dispose_ServiceContainer_DisposesDisposableLifeTimeInstances()
        {
            var lifetime = new DisposableLifetime();
            
            using (var container = new ServiceContainer())
            {               
                container.Register<IFoo, Foo>(lifetime);                
            }
            Assert.True(lifetime.IsDisposed);
        }


        [Fact]
        public void Dispose_ServiceContainerWithDisposablePerContainerLifetimeService_DisposesInstance()
        {
            DisposableFoo foo;
            using (var container = new ServiceContainer())
            {
                container.Register<IFoo, DisposableFoo>(new PerContainerLifetime());                
                foo = (DisposableFoo)container.GetInstance<IFoo>();                
            }

            Assert.True(foo.IsDisposed);
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


        [Fact]
        public void TryGetInstance_UnknownService_ReturnsNull()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            var instance = container.TryGetInstance<IFoo>();

            Assert.Null(instance);
        }

        [Fact]
        public void TryGetInstance_UnknownService_IsAvailableAfterRegistration()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            container.TryGetInstance<IFoo>();
            container.Register<IFoo, Foo>();

            var instance = container.TryGetInstance<IFoo>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void TryGetInstance_UnknownNamedService_IsAvailableAfterRegistration()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            container.TryGetInstance<IFoo>("Foo");
            container.Register<IFoo, Foo>("Foo");

            var instance = container.TryGetInstance<IFoo>("Foo");

            Assert.NotNull(instance);
        }


        [Fact]
        public void TryGetInstance_UnknownNamedService_ReturnsNull()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();

            var instance = container.TryGetInstance<IFoo>("SomeFoo");

            Assert.Null(instance);
        }

        [Fact]
        public void TryGetInstance_KnownService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();

            var instance = container.TryGetInstance<IFoo>();

            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void TryGetInstance_KnownServiceWithUnknownDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
           
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());            
        }

        [Fact]
        public void GetInstance_LazyService_ReturnsInstance()
        {            
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();

            var lazyInstance = container.GetInstance<Lazy<IFoo>>();

            Assert.IsAssignableFrom(typeof(Lazy<IFoo>), lazyInstance);
        }

        [Fact]
        public void GetInstance_LazyService_DoesNotCreateTarget()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, LazyFoo>();
            LazyFoo.Instances = 0;
            container.GetInstance<Lazy<IFoo>>();
            Assert.Equal(0, LazyFoo.Instances);
        }

        [Fact]
        public void GetInstance_LazyService_CreatesTargetWhenValuePropertyIsAccessed()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            
            var instance = container.GetInstance<Lazy<IFoo>>();

            Assert.IsAssignableFrom(typeof(Foo), instance.Value);
        }

        [Fact]
        public void GetInstance_TypedFactory_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFooFactory, FooFactory>();

            var factory = container.GetInstance<IFooFactory>();
            var instance = factory.CreateFoo();

            Assert.IsAssignableFrom(typeof(Foo), instance);
        }
#if !PCL_111
        [Fact]
        public void GetInstance_ServiceWithGenericConstraint_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>));
            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo<int>)), ErrorMessages.UnknownGenericDependency);
        }

        [Fact]
        public void GetInstance_ServiceWithGenericInterfaceConstraint_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(
                typeof(IFooWithGenericInterfaceConstraint<,>),
                typeof(FooWithGenericInterfaceConstraint<,>));
            container.Register(typeof(IBar<>), typeof(Bar<>));

            container.GetInstance<IFooWithGenericInterfaceConstraint<IBar<string>, string>>();

        }

        [Fact]
        public void GetAllInstances_ServiceWithGenericConstraint_ReturnsOnlyMatchingInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>));
            container.Register(typeof(IFoo<>), typeof(Foo<>), "AnotherFoo");

            var instances = container.GetAllInstances<IFoo<int>>();
            
            Assert.Equal(1, instances.Count());
        }
       
        [Fact]
        public void GetAllInstances_ServiceWithGenericConstraint_ReturnsAllMatchingInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>));
            container.Register(typeof(IFoo<>), typeof(Foo<>), "AnotherFoo");

            var instances = container.GetAllInstances<IFoo<IBar>>();

            Assert.Equal(2, instances.Count());
        }
#endif
        [Fact]
        public void CreateGeneric_ConcreteClass_ReturnsInstance()
        {
            var container = CreateContainer();
            var instance = container.Create<Foo>();
            Assert.IsAssignableFrom(typeof(Foo), instance);
        }

        [Fact]
        public void Create_ConcreteClass_ReturnsInstance()
        {
            var container = CreateContainer();
            var instance = container.Create(typeof(Foo));
            Assert.IsAssignableFrom(typeof(Foo), instance);            
        }

        [Fact]
        public void Create_UsingFallback_ReturnsInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback(
                (type, s) =>
                    {
                        container.Register(type);
                        return false;
                    },
                null);

            var instance = container.GetInstance<Foo>();
            Assert.IsAssignableFrom(typeof(Foo), instance);   
        }

        [Fact]
        public void GetInstance_RegisteredConstructorDependency_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.RegisterConstructorDependency<IBar>((factory, info) => new Bar());
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisteredConstructorDependency_IgnoresRegistrationAfterFirstRequest()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.RegisterConstructorDependency<IBar>((factory, info) => new Bar());
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            container.RegisterConstructorDependency<IBar>((factory, info) => new AnotherBar());
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisteredConstructorDependency_CanUpdateRegistrationBeforeFirstRequest()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.RegisterConstructorDependency<IBar>((factory, info) => new Bar());            
            container.RegisterConstructorDependency<IBar>((factory, info) => new AnotherBar());
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(AnotherBar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisteredPropertyDependency_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.RegisterPropertyDependency<IBar>((factory, info) => new Bar());
            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisteredPropertyDependency_IgnoresRegistrationAfterFirstRequest()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.RegisterPropertyDependency<IBar>((factory, info) => new Bar());
            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();
            container.RegisterPropertyDependency<IBar>((factory, info) => new AnotherBar());
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisteredPropertyDependency_CanUpdateRegistrationBeforeFirstRequest()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.RegisterPropertyDependency<IBar>((factory, info) => new Bar());
            container.RegisterPropertyDependency<IBar>((factory, info) => new AnotherBar());
            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(AnotherBar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisterPropertyDependencyUsingFactoryParameter_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.Register<IBar, Bar>();
            container.RegisterPropertyDependency((factory, info) => factory.GetInstance<IBar>());
            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_RegisterConstructorDependencyUsingFactoryParameter_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IBar, Bar>();
            container.RegisterConstructorDependency((factory, info) => factory.GetInstance<IBar>());
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_UsingInitializer_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.Initialize(
                registration => true,
                (factory, instance) => ((FooWithProperyDependency)instance).Bar = new Bar());
            var foo = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), foo.Bar);
        }

        [Fact]
        public void GetInstance_ReadOnlyRegistration_ReturnsOriginalRegistration()
        {
            var container = CreateContainer();
            var registration = new ServiceRegistration();
            registration.ServiceType = typeof (IFoo);
            registration.ServiceName = "";
            registration.ImplementingType = typeof (Foo);
            registration.IsReadOnly = true;
            container.Register(registration);

            container.Register<IFoo, AnotherFoo>();

            var instance = container.GetInstance<IFoo>();

            Assert.IsType(typeof (Foo), instance);
        }

#if NET45 || NET40 || NET46
        [Fact]
        public void RegisterFrom_CompositionRoot_CallsCompositionRootExecutor()
        {
            var container = (ServiceContainer)CreateContainer();            
            var compositionRootExecutorMock = new CompositionRootExecutorMock();
            container.CompositionRootExecutor = compositionRootExecutorMock;
            
            container.RegisterFrom<CompositionRootMock>();

            compositionRootExecutorMock.Assert(c => c.Execute(typeof(CompositionRootMock)), Invoked.Once);
        }
#endif        
        [Fact]
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

        [Fact]
        public void GetInstance_InternalClassWithInternalConstructor_ThrowsInvalidOperationException()
        {
            var container = CreateContainer();
            container.Register<IFoo, InternalFooWithInternalConstructor>();
            var exception = ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
            //StringAssert.Contains(exception.InnerException.Message, "Missing public constructor for Type");            
        }

        [Fact]
        public void GetInstance_InternalClassWithPublicConstructor_ReturnInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, InternalFooWithPublicConstructor>();
            var instance = container.GetInstance<IFoo>();
            Assert.NotNull(instance);
        }
     
        #endregion
    }

    public interface IFooFactoryWithArgument
    {
        IFoo CreateFoo(int value);
    }

    
}