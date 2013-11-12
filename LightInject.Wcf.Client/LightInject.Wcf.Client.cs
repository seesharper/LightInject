using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.Wcf.Client
{
    using System.ServiceModel;

    internal static class WcfContainerExtensions
    {
        internal static void EnableWcf(this IServiceContainer serviceContainer)
        {
            serviceContainer.RegisterFallback(
                (serviceType, serviceName) => serviceType.IsDefined(typeof(ServiceContractAttribute), true),
                CreateWcfProxy);
        }

        private static object CreateWcfProxy(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }
    }
}
