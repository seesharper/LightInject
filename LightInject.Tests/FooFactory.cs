using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightInject;
using LightInject.SampleLibrary;
namespace DependencyInjector.Tests
{    
    public class FooFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            ServiceName = serviceRequest.ServiceName;
            return new Foo();

        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof (IFoo).IsAssignableFrom(serviceType);
        }

        public string ServiceName { get; set; }
    }
}
