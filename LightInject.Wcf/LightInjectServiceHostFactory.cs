/*****************************************************************************   
   Copyright 2013 bernhard.richter@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
******************************************************************************
   LightInject.Wcf version 1.0.0.1
   http://www.lightinject.net/
   http://twitter.com/bernhardrichter
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Wcf
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using LightInject.Interception;
    
    /// <summary>
    /// A <see cref="ServiceHostFactory"/> that uses the LightInject <see cref="ServiceContainer"/>
    /// to create WCF services.
    /// </summary>
    public class LightInjectServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var type = Type.GetType(constructorString);
            if (type == null)
            {
                throw new ArgumentException("Could not get Type for {0}", constructorString);
            }

            return CreateServiceHost(type, baseAddresses);
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ValidateServiceType(serviceType);

            var container = new ServiceContainer();
            var proxyBuilder = new ProxyBuilder();
            var proxyDefinition = new ProxyDefinition(serviceType, () => container.GetInstance(serviceType));
            var proxyType = proxyBuilder.GetProxyType(proxyDefinition);

            return base.CreateServiceHost(proxyType, baseAddresses);
        }

        private static void ValidateServiceType(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            var attribute =
                serviceType.GetCustomAttributes(typeof(ServiceContractAttribute), true)
                           .Cast<ServiceContractAttribute>()
                           .FirstOrDefault();

            if (!serviceType.IsInterface || attribute == null)
            {
                throw new NotSupportedException(
                    "Only interfaces with [ServiceContract] attribute are supported with LightInjectServiceHostFactory.");
            }
        }
    }
}