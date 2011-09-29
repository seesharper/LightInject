using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dynamo.Ioc;
namespace DependencyInjector.Tests
{
    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public void GetInstance_ServiceWithDependency()
        {
            IServiceContainer serviceContainer = new ServiceContainer();
            serviceContainer.Register(typeof(IFoo), typeof(Foo));
            serviceContainer.Register(typeof(IService),typeof(ServiceWithDependency));
            serviceContainer.GetInstance(typeof(IService));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                serviceContainer.GetInstance(typeof (IService));
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [TestMethod]
        public void GetInstance_ServiceWithDependency_Dynamo()
        {
            Container container = new Container();            
            container.Register<IService>(c => new ServiceWithDependency(c.Resolve<IFoo>()));
            container.Register<IFoo>(c => new Foo());
            container.Compile();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                container.Resolve(typeof(IService));
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

    }
}
