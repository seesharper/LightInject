using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LightInject;

namespace DependencyInjector.Tests
{
    public class ClonableFactory : IFactory
    {
        
        public object GetInstance(ServiceRequest serviceRequest)
        {
            ServiceName = serviceRequest.ServiceName;
            if (serviceRequest.CanProceed)
            {
                Instance = serviceRequest.Proceed();
                return Instance;
            }
                

            return "string";
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof (ICloneable);                        
        }

        public string ServiceName { get; set; }


        public object Instance { get; private set; }
    }
}
