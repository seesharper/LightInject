namespace LightInject.Tests
{
    using System;
    using System.Linq;
    using System.Text;
    using LightInject;
    using LightInject.SampleLibrary;
    using Xunit;
        
    public class ConstructorInjectionTests : TestBase
    {
        [Fact]
        public void GetInstance_KnownDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo), typeof(FooWithDependency));
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_UnKnownDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithDependency>();
            var exception = Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
            Assert.Equal(ErrorMessages.UnknownConstructorDependency, exception.InnerException.Message);            
        }

        [Fact]
        public void GetInstance_GenericDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericDependency<>));
            var instance = (FooWithGenericDependency<IBar>)container.GetInstance<IFoo<IBar>>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Dependency);
        }

        [Fact]
        public void GetInstance_OpenGenericDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar<>), typeof(Bar<>));
            container.Register(typeof(IFoo<>), typeof(FooWithOpenGenericDependency<>));
            var instance = (FooWithOpenGenericDependency<int>)container.GetInstance<IFoo<int>>();
            Assert.IsAssignableFrom(typeof(Bar<int>), instance.Dependency);
        }

        [Fact]
        public void GetInstance_OpenGenericDependencyWithRequestLifeCycle_InjectsSameDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar<>), typeof(Bar<>), new PerScopeLifetime());
            container.Register(typeof(IFoo<>), typeof(FooWithSameOpenGenericDependencyTwice<>));
            using (container.BeginScope())
            {
                var instance = (FooWithSameOpenGenericDependencyTwice<int>)container.GetInstance<IFoo<int>>();
                Assert.Same(instance.Bar1, instance.Bar2);
            }
        }

        [Fact]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithDependency>();
            var instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.NotEqual(instance1.Bar, instance2.Bar);
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithDependency>();
            
            FooWithDependency instance1;
            FooWithDependency instance2;
            using (container.BeginScope())
            {
                instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            }
            using (container.BeginScope())
            {
                instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            }            
            Assert.NotEqual(instance1.Bar, instance2.Bar);
        }

        [Fact]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingleonDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Register<IFoo, FooWithDependency>();
            var instance1 = (FooWithDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.Equal(instance1.Bar, instance2.Bar);
        }

        [Fact]
        public void GetInstance_DependencyWithSingletonLifeCycle_CallsDependencyConstructorOnlyOnce()
        {
            var container = CreateContainer();
            Bar.InitializeCount = 0;
            container.Register<IBar>(c => new Bar(), new PerContainerLifetime());            
            container.Register<IFoo>(c => new FooWithDependency(c.GetInstance<IBar>()));
            container.GetInstance<IFoo>();
            container.GetInstance<IFoo>();
            Assert.Equal(1, Bar.InitializeCount);
        }
        
        [Fact]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithSameDependencyTwice>();
            var instance = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            Assert.NotEqual(instance.Bar1, instance.Bar2);
        }

        [Fact]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingletonDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Register<IFoo, FooWithSameDependencyTwice>();
            var instance = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            Assert.Equal(instance.Bar1, instance.Bar2);
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsSameDependencyForSingleClass()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSameDependencyTwice>();
            using (container.BeginScope())
            {
                var instance = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
                Assert.Equal(instance.Bar1, instance.Bar2);
            }
            
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsSameDependencyThroughoutObjectGraph()
        {
            var container = CreateContainer();
            container.Register<IBar, BarWithSampleServiceDependency>();
            container.Register<ISampleService, SampleService>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSampleServiceDependency>();
            using (container.BeginScope())
            {
                var instance = (FooWithSampleServiceDependency)container.GetInstance<IFoo>();
                Assert.Same(((BarWithSampleServiceDependency)instance.Bar).SampleService, instance.SampleService);
            }
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependenciesForMultipleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSameDependencyTwice>();
            FooWithSameDependencyTwice instance1;
            FooWithSameDependencyTwice instance2;
            using (container.BeginScope())
            {
                instance1 = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            }
            using (container.BeginScope())
            {
                instance2 = (FooWithSameDependencyTwice)container.GetInstance<IFoo>();
            }            
            Assert.NotEqual(instance1.Bar1, instance2.Bar2);
        }

        [Fact]
        public void GetInstance_ValueTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(42);
            container.Register<IFoo, FooWithValueTypeDependency>();
            var instance = (FooWithValueTypeDependency)container.GetInstance<IFoo>();
            Assert.Equal(42, instance.Value);
        }

        [Fact]
        public void GetInstance_EnumDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(Encoding.UTF8);
            container.Register<IFoo, FooWithEnumDependency>();
            var instance = (FooWithEnumDependency)container.GetInstance<IFoo>();
            Assert.Equal(Encoding.UTF8, instance.Value);
        }

        [Fact]
        public void GetInstance_ReferenceTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.Register<IFoo, FooWithReferenceTypeDependency>();
            var instance = (FooWithReferenceTypeDependency)container.GetInstance<IFoo>();
            Assert.Equal("SomeValue", instance.Value);
        }

        [Fact]
        public void GetInstance_RequestLifeCycle_CallConstructorsOnDependencyOnlyOnce()
        {
            var container = CreateContainer();
            Bar.InitializeCount = 0;
            container.Register(typeof(IBar), typeof(Bar), new PerScopeLifetime());
            container.Register(typeof(IFoo), typeof(FooWithSameDependencyTwice));
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
                Assert.Equal(1, Bar.InitializeCount);    
            }            
        }

        [Fact]
        public void GetInstance_MultipleConstructors_UsesConstructorWithMostParameters()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithMultipleConstructors));
            container.Register(typeof(IBar), typeof(Bar));
            var foo = (FooWithMultipleConstructors)container.GetInstance<IFoo>();
            Assert.NotNull(foo.Bar);
        }

        [Fact]
        public void GetInstance_FuncDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IFoo), typeof(FooWithFuncDependency));
            var instance = (FooWithFuncDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.GetBar());
        }

        [Fact]
        public void GetInstance_IEnumerableDependency_InjectsAllInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register(typeof(IBar), typeof(AnotherBar), "AnotherBar");
            container.Register(typeof(IFoo), typeof(FooWithEnumerableDependency));
            var instance = (FooWithEnumerableDependency)container.GetInstance<IFoo>();
            Assert.Equal(2, instance.Bars.Count());
        }

        [Fact]
        public void GetInstance_CompositeDependency_InjectsOnlyOtherImplementations()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(Foo), "Foo");
            container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
            container.Register(typeof(IFoo), typeof(FooWithEnumerableIFooDependency));            
            var instance = (FooWithEnumerableIFooDependency)container.GetInstance<IFoo>();

            Assert.True(instance.FooList.Any(f => f.GetType() == typeof(Foo)));
            Assert.True(instance.FooList.Any(f => f.GetType() == typeof(AnotherFoo)));
            Assert.Equal(2, instance.FooList.Count());
        }

        //[TestMethod, Ignore]
        //public void GetInstance_SecondLevelCompositeDependency_InjectsOnlyOtherImplementations()
        //{
        //    var container = CreateContainer();
        //    container.Register(typeof(IBar), typeof(BarWithFooDependency));
        //    container.Register(typeof(IFoo), typeof(Foo), "Foo");
        //    container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
        //    container.Register<IFoo>(f => new FooWithEnumerableIFooDependency(f.GetAllInstances<IFoo>()));

        //    var instance = (FooWithEnumerableIFooDependency)((BarWithFooDependency)container.GetInstance<IBar>()).Foo;

        //    Assert.True(instance.FooList.Any(f => f.GetType() == typeof(Foo)));
        //    Assert.True(instance.FooList.Any(f => f.GetType() == typeof(AnotherFoo)));
        //    Assert.Equal(2, instance.FooList.Count());            
        //}

        [Fact]
        public void GetInstance_RecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
            var exception = Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
            Assert.Equal(ErrorMessages.RecursiveDependency, exception.InnerException.InnerException.Message);            
        }

        [Fact]
        public void GetInstance_SecondLevelRecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(BarWithFooDependency));
            container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
            var exception = Assert.Throws<InvalidOperationException>(() => container.GetInstance<IBar>());
            Assert.Equal(ErrorMessages.RecursiveDependency, exception.InnerException.InnerException.InnerException.Message);
        }

        [Fact]
        public void GetInstance_PerContainerLifetimeWithRecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => CreateRecursive(factory), new PerContainerLifetime());
            var exception = Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
            Assert.True(exception.ToString().Contains("Recursive dependency detected"));            
        }

        private static FooWithRecursiveDependency CreateRecursive(IServiceFactory factory)
        {
            return new FooWithRecursiveDependency(factory.GetInstance<IFoo>());
        }

        private static BarWithFooDependency Create(IServiceFactory factory)
        {
            return new BarWithFooDependency(factory.GetInstance<IFoo>());
        }

        [Fact]
        public void GetInstance_RequestLifeCycle_FirstIEnumerableAndArgumentAreSame()
         {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar), new PerScopeLifetime());
            container.Register(typeof(IFoo), typeof(FooWithEnumerableAndRegularDependency));
            using (container.BeginScope())
            {
                var instance = (FooWithEnumerableAndRegularDependency)container.GetInstance<IFoo>();
                Assert.Same(instance.Bar, instance.Bars.First());
            }
         }

        [Fact]
        public void GetInstance_SingletonLifeCycle_FirstIEnumerableAndArgumentAreSame()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar), new PerContainerLifetime());
            container.Register(typeof(IFoo), typeof(FooWithEnumerableAndRegularDependency));
            var instance = (FooWithEnumerableAndRegularDependency)container.GetInstance<IFoo>();
            Assert.Same(instance.Bar, instance.Bars.First());
        }

        [Fact]
        public void GetInstance_FuncFactoryWithMethodCallAsDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new FooWithDependency(this.CreateBar()));
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_CustomFuncDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new FooWithCustomFuncDependency(() => "test"));
            var instance = (FooWithCustomFuncDependency)container.GetInstance<IFoo>();
            Assert.NotNull(instance.StringFunc);
        }

        [Fact]
        public void GetInstance_LambdaExpressionWithFactoryParameter_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.Register<IFoo>(factory => new FooWithCustomFuncDependency(() => factory.GetInstance<string>()));
            var instance = (FooWithCustomFuncDependency)container.GetInstance<IFoo>();
            Assert.NotNull(instance.StringFunc);
        }
      
        [Fact]
        public void GetInstance_MethodGroupWithFactoryParameter_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.Register<IFoo>(factory => new FooWithCustomFuncDependency(factory.GetInstance<string>));
            var instance = (FooWithCustomFuncDependency)container.GetInstance<IFoo>();
            Assert.NotNull(instance.StringFunc);
        }

        [Fact]
        public void GetInstance_UnknownService_UsesParameterNameAsServiceName()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>("Bar");
            container.Register<IBar, AnotherBar>("AnotherBar");
            container.Register<IFoo, FooWithDependency>();
            var instance = (FooWithDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom(typeof(Bar), instance.Bar);
        }

        [Fact]
        public void GetInstance_OpenGenericPerScopeService_ReturnsSameInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerScopeLifetime());
            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<IFoo<int>>();
                var instance2 = container.GetInstance<IFoo<int>>();
                Assert.Same(instance1, instance2);
            }
        }

        [Fact]
        public void GetInstance_TransientDependencyWithSingletonDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            container.Register<IBar, BarWithFooDependency>();
            container.Register<IFoo, FooWithDependency>("FooWithDependency");
            container.GetInstance<IFoo>("FooWithDependency");
        }


        private IBar CreateBar()
        {
            return new Bar();
        }       
    }
}