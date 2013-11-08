using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using LightInject.Interception;

namespace LightInject.Wcf
{
    public class LightInjectServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            var attribute = serviceType.GetCustomAttributes(typeof(ServiceContractAttribute), true)
                                       .Cast<ServiceContractAttribute>()
                                       .FirstOrDefault();

            if (!serviceType.IsInterface || attribute == null)
                throw new NotSupportedException("Only interfaces with [ServiceContract] attribute are supported with LightInjectServiceHostFactory.");

            var container = new ServiceContainer();
            var proxyBuilder = new ProxyBuilder();
            var proxyDefinition = new ProxyDefinition(serviceType, () => container.GetInstance(serviceType));
            var proxyType = proxyBuilder.GetProxyType(proxyDefinition);

            return base.CreateServiceHost(proxyType, baseAddresses);
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var type = Type.GetType(constructorString);
            if (type == null)
                throw new ArgumentException("Could not get Type for {0}", constructorString);

            return CreateServiceHost(type, baseAddresses);
        }
    }
}