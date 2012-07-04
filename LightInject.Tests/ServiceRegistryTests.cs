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
        public void Register_Service()
        {
            var container = CreateContainer();
            
        }



        private static IServiceContainer CreateContainer()
        {
            return new EmitServiceContainer();
        }
    
    }


  
}