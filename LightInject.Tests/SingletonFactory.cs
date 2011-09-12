using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightInject;
using LightInject.SampleLibrary;

namespace DependencyInjector.Tests
{
    public class SingletonFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            CallCount++;
            return new Service();

        }
       
        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof (IService);
        }

        public int CallCount { get; private set; }
    }
}
