namespace LightInject.Tests
{
    using System;
    using System.Text;    
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PropertyInjectionTests : TestBase
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
        public void GetInstance_UnKnownDependency_ReturnsInstanceWithoutDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();
            Assert.IsNull(instance.Bar);
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
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithProperyDependency>();            
            FooWithProperyDependency instance1;
            FooWithProperyDependency instance2;
            using (container.BeginScope())
            {
                instance1 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            }
            using (container.BeginScope())
            {
                instance2 = (FooWithProperyDependency)container.GetInstance<IFoo>();
            }            
            Assert.AreNotEqual(instance1.Bar, instance2.Bar);
        }

        [TestMethod]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingleonDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
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
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.AreEqual(instance.Bar1, instance.Bar2);
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsSameDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            using (container.BeginScope())
            {
                var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
                Assert.AreEqual(instance.Bar1, instance.Bar2);
            }            
        }

        [TestMethod]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependenciesForMultipleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();           

            FooWithSamePropertyDependencyTwice instance1;
            FooWithSamePropertyDependencyTwice instance2;
            using (container.BeginScope())
            {
                instance1 = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            }
            using (container.BeginScope())
            {
                instance2 = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            }            
            Assert.AreNotEqual(instance1.Bar1, instance2.Bar2);
        }

        [TestMethod]
        public void GetInstance_ValueTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(42);           
            container.Register<IFoo, FooWithValueTypePropertyDependency>();
            var instance = (FooWithValueTypePropertyDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_EnumDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(Encoding.UTF8);
            container.Register<IFoo, FooWithEnumPropertyDependency>();
            var instance = (FooWithEnumPropertyDependency)container.GetInstance<IFoo>();
            Assert.AreEqual(Encoding.UTF8, instance.Value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
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
            container.Register(typeof(IBar), typeof(Bar), new PerScopeLifetime());
            container.Register(typeof(IFoo), typeof(FooWithSamePropertyDependencyTwice));
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
                Assert.AreEqual(1, Bar.InitializeCount);
            }
        }

        [TestMethod]
        public void GetInstance_StaticDependency_DoesNotInjectDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithStaticDependency>();
            container.GetInstance<IFoo>();
            Assert.IsNull(FooWithStaticDependency.Bar);
        }    

        [TestMethod]
        public void InjectProperties_KnownClassWithPropertyDependency_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.Register<FooWithProperyDependency>();
            container.Register<IBar, Bar>();
            var fooWithProperyDependency = new FooWithProperyDependency();

            var result = (FooWithProperyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsInstanceOfType(result.Bar, typeof(Bar));
        }

        [TestMethod]
        public void InjectProperties_FuncDependency_InjectsDependency()
        {
            var container = CreateContainer();            
            container.Register<IBar, Bar>((factory, bar) => new Bar());
            var fooWithFuncPropertyDependency = new FooWithFuncPropertyDependency();

            var result = (FooWithFuncPropertyDependency)container.InjectProperties(fooWithFuncPropertyDependency);            
            Assert.IsNotNull(result.BarFunc);
        }



        [TestMethod]
        public void InjectProperties_UnknownClassWithPropertyDependency_InjectsPropertyDependencies()
        {
            var container = CreateContainer();            
            container.Register<IBar, Bar>();
            var fooWithProperyDependency = new FooWithProperyDependency();

            var result = (FooWithProperyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsInstanceOfType(result.Bar, typeof(Bar));
        }

        [TestMethod]
        public void InjectProperties_UnknownClassWithPropertyDependencyRegisteredAsInstance_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.RegisterInstance<IBar>(new Bar());
            var fooWithProperyDependency = new FooWithProperyDependency();

            var result = (FooWithProperyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsInstanceOfType(result.Bar, typeof(Bar));
        }
#if NET || NET45 || NETFX_CORE || WINDOWS_PHONE       
        [TestMethod]
        public void InjectProperties_FuncFactory_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IBar, AnotherBar>("AnotherBar");
            container.Register(f => new FooWithProperyDependency(){ Bar = f.GetInstance<IBar>("AnotherBar") });
            var fooWithProperyDependency = new FooWithProperyDependency();

            var result = (FooWithProperyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsInstanceOfType(result.Bar, typeof(AnotherBar));
        }
#endif
        [TestMethod]
        public void InjectProperties_RecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithRecursiveDependency>();

            var barWithPropertyDependency = new BarWithPropertyDependency();

            ExceptionAssert.Throws<InvalidOperationException>(
                () => container.InjectProperties(barWithPropertyDependency), ErrorMessages.RecursivePropertyDependency);

        }

        [TestMethod]
        public void InjectProperties_InstanceWithUnknownConstructorDependencyAndMultipleConstructors_ReturnsInstance()
        {
            var container = new ServiceContainer();
            var fooWithDependency = new FooWithTwoConstructors(42);
            var result = container.InjectProperties(fooWithDependency);
        }

        [TestMethod]
        public void GetInstance_ClassWithIndexer_CanGetInstance()
        {
            var container = new ServiceContainer();
            container.Register<object, Foo>();
            container.Register<FooWithIndexer>();
            var instance = container.GetInstance<FooWithIndexer>();
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void GetInstance_ClassWithObjectPropertyCanGetInstance()
        {
            var container = new ServiceContainer();
            container.Register<object, Foo>();
            container.Register<FooWithObjectProperty>();
            var instance = container.GetInstance<FooWithObjectProperty>();
            Assert.IsNotNull(instance.Property);
        }
    }
}