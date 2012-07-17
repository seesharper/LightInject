using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjector.Tests
{
    [TestClass]
    public class ServiceRegistryTests
    {
        //[TestMethod]
        //public void RegisterNonGeneric_ImplementingType_CanGetInstance()
        //{
        //    var container = CreateContainer();
        //    container.Register(typeof(IFoo), typeof(Foo));
        //    Assert.IsInstanceOfType(container.GetInstance<IFoo>(), typeof(Foo));
        //}

        //[TestMethod]
        //public void RegisterNonGeneric_NamedImplementingType_CanGetInstance()
        //{
        //    var container = CreateContainer();
        //    container.Register(typeof(IFoo), typeof(Foo), "SomeFoo");
        //    Assert.IsInstanceOfType(container.GetInstance<IFoo>("SomeFoo"), typeof(Foo));
        //}

        //[TestMethod]
        //public void RegisterNonGeneric_NamedInstance_CanGetInstance()
        //{
        //    var container = CreateContainer();
        //    container.Register(typeof(IFoo), new Foo());
        //    Assert.IsInstanceOfType(container.GetInstance<IFoo>(), typeof(Foo));
        //}


        


        private static IServiceContainer CreateContainer()
        {
            return new EmitServiceContainer();
        }
    
    }


  
}