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
        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> with specific base addresses and initializes it with specified data.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost"/> with specific base addresses.
        /// </returns>
        /// <param name="constructorString">The initialization data passed to the <see cref="T:System.ServiceModel.ServiceHostBase"/> instance being constructed by the factory. </param>
        /// <param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param><exception cref="T:System.ArgumentNullException"><paramref name="baseAddresses"/> is null.</exception><exception cref="T:System.InvalidOperationException">There is no hosting context provided or <paramref name="constructorString"/> is null or empty.</exception>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var type = Type.GetType(constructorString);
            if (type == null)
            {
                throw new ArgumentException("Could not get Type for {0}", constructorString);
            }

            return CreateServiceHost(type, baseAddresses);
        }

        /// <summary>
        /// Creates a <see cref="ServiceHost"/> with the specified <paramref name="baseAddresses"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to be hosted by the <see cref="ServiceHost"/>.</typeparam>
        /// <param name="baseAddresses">The base addresses for the hosted service.</param>
        /// <returns>A <see cref="ServiceHost"/> for the specified <typeparamref name="TService"/>.</returns>
        public ServiceHost CreateServiceHost<TService>(params string[] baseAddresses)
        {
            var uriBaseAddresses = baseAddresses.Select(s => new Uri(s)).ToArray();
            return CreateServiceHost(typeof(TService), uriBaseAddresses);
        }

        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> for a specified type of service with a specific base address. 
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost"/> for the type of service specified with a specific base address.
        /// </returns>
        /// <param name="serviceType">Specifies the type of service to host. </param><param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param>
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
            
            if (!IsInterfaceWithServiceContractAttribute(serviceType))
            {
                throw new NotSupportedException(
                    "Only interfaces with [ServiceContract] attribute are supported with LightInjectServiceHostFactory.");
            }
        }       
 
        private static bool IsInterfaceWithServiceContractAttribute(Type serviceType)
        {
            return serviceType.IsInterface && serviceType.IsDefined(typeof(ServiceContractAttribute), true);
        }
    }
}
