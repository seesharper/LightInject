using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightInject;
using LightInject.SampleLibrary;

namespace DependencyInjector.Tests
{
    public class ServiceFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            if (serviceRequest.CanProceed)
                //Alternatively return something else like a decorator or a proxy 
                //based on the instance resolved by the container 
                return serviceRequest.Proceed();
            
            //Manually resolved service 
            return new Service();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof (IService) == serviceType;
        }
    }
}
