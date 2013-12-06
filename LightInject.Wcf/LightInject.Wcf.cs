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
[assembly: System.Web.PreApplicationStartMethod(typeof(LightInject.Wcf.LightInjectWcfInitializer), "Initialize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.Web;
    using System.Web.Hosting;

    using LightInject.Interception;

    /// <summary>
    /// Registers the <see cref="VirtualSvcPathProvider"/> with the current <see cref="HostingEnvironment"/>.
    /// </summary>
    public static class LightInjectWcfInitializer
    {
        private static bool isInitialized;

        /// <summary>
        /// Executed before the <see cref="HttpApplication"/> is started and registers
        /// the <see cref="VirtualSvcPathProvider"/> with the current <see cref="HostingEnvironment"/>.
        /// </summary>
        public static void Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                HostingEnvironment.RegisterVirtualPathProvider(new VirtualSvcPathProvider(string.Empty));
            }
        }
    }
    
    /// <summary>
    /// A <see cref="ServiceHostFactory"/> that uses the LightInject <see cref="ServiceContainer"/>
    /// to create WCF services.
    /// </summary>
    public class LightInjectServiceHostFactory : ServiceHostFactory
    {        
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
            container.RegisterAssembly("LightInject.Wcf.Configuration.dll");
            container.Register<IServiceBehavior>(factory => new ServiceMetadataBehavior() {HttpGetEnabled = true});
            var proxyType = CreateServiceProxyType(serviceType, container);
            
            ServiceHost serviceHost = base.CreateServiceHost(proxyType, baseAddresses);            
            serviceHost.AddDefaultEndpoints();
            ApplyServiceBehaviors(container, serviceHost);
            ApplyEndpointBehaviors(container, serviceHost);
                        
            return serviceHost;
        }

        private void ApplyEndpointBehaviors(ServiceContainer container, ServiceHost serviceHost)
        {
            IEnumerable<IEndpointBehavior> endpointBehaviors = container.GetAllInstances<IEndpointBehavior>().ToArray();
            foreach (var endpoint in serviceHost.Description.Endpoints)
            {
                foreach (var endpointBehavior in endpointBehaviors)
                {
                    endpoint.Behaviors.Add(endpointBehavior);
                }
            }
        }

        private void ApplyServiceBehaviors(ServiceContainer container, ServiceHost serviceHost)
        {
            var serviceBehaviors = container.GetAllInstances<IServiceBehavior>();
            var description = serviceHost.Description;
            foreach (var serviceBehavior in serviceBehaviors)
            {
                description.Behaviors.Add(serviceBehavior);
            }            
        }
 
        private static Type CreateServiceProxyType(Type serviceType, IServiceContainer container)
        {            
            var proxyBuilder = new ProxyBuilder();
            var proxyDefinition = CreateProxyDefinition(serviceType, container);            
            ImplementServiceInterface(serviceType, container, proxyDefinition);
            return proxyBuilder.GetProxyType(proxyDefinition);
        }

        private static void ImplementServiceInterface(
            Type serviceType, IServiceContainer container, ProxyDefinition proxyDefinition)
        {
            proxyDefinition.Implement(() => new ServiceInterceptor(container), m => m.IsDeclaredBy(serviceType));
        }

        private static ProxyDefinition CreateProxyDefinition(Type serviceType, IServiceContainer container)
        {
            var proxyDefinition = new ProxyDefinition(serviceType, () => container.GetInstance(serviceType));
            if (container.CanGetInstance(serviceType, string.Empty))
            {
                ServiceRegistration serviceRegistration = container.AvailableServices.FirstOrDefault(sr => sr.ServiceType == serviceType);
                if (serviceRegistration != null && serviceRegistration.ImplementingType != null)
                {
                    proxyDefinition.AddCustomAttributes(
                        serviceRegistration.ImplementingType.GetCustomAttributesData().ToArray());
                }
            }
            return proxyDefinition;
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

    /// <summary>
    /// An <see cref="IInterceptor"/> that ensures that a service operation is 
    /// executed within a <see cref="Scope"/>.    
    /// </summary>
    public class ServiceInterceptor : IInterceptor
    {
        private readonly IServiceContainer serviceContainer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInterceptor"/> class.
        /// </summary>
        /// <param name="serviceContainer">The <see cref="IServiceContainer"/> that is used to create the <see cref="Scope"/>.</param>
        internal ServiceInterceptor(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
        }
               
        /// <summary>
        /// Wraps the execution of a service operation inside a <see cref="Scope"/>.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        public object Invoke(IInvocationInfo invocationInfo)
        {
            using (serviceContainer.BeginScope())
            {
                return invocationInfo.Proceed();
            }            
        }
    }

    /// <summary>
    /// Represents a virtual .svc file. 
    /// </summary>
    public class VirtualSvcFile : VirtualFile
    {
        private readonly string content;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualSvcFile"/> class.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <param name="content">The content of the virtual file.</param>
        public VirtualSvcFile(string virtualPath, string content)
            : base(virtualPath)
        {
            this.content = content;
        }

        /// <summary>
        /// When overridden in a derived class, returns a read-only stream to the virtual resource.
        /// </summary>
        /// <returns>
        /// A read-only stream to the virtual file.
        /// </returns>
        public override Stream Open()
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(content);
            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }

    /// <summary>
    /// A <see cref="VirtualPathProvider"/> that enables WCF services to be hosted without creating .svc files.
    /// </summary>
    public class VirtualSvcPathProvider : VirtualPathProvider
    {
        private const string FileTemplate =
            "<%@ ServiceHost Service=\"{0}\" Factory = \"LightInject.Wcf.LightInjectServiceHostFactory, LightInject.Wcf\" %>";

        private readonly string servicePath;        

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualSvcPathProvider"/> class.
        /// </summary>
        /// <param name="servicePath">The virtual path to register.</param>        
        public VirtualSvcPathProvider(string servicePath)
        {            
            this.servicePath = servicePath;            
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
        /// </returns>
        /// <param name="virtualPath">The path to the primary virtual resource.</param><param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param><param name="utcStart">The UTC time at which the virtual resources were read.</param>
        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return IsPathVirtual(virtualPath) ? null : base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        /// <summary>
        /// Gets a value that indicates whether a directory exists in the virtual file system.
        /// </summary>
        /// <returns>
        /// true if the directory exists in the virtual file system; otherwise, false.
        /// </returns>
        /// <param name="virtualDir">The path to the virtual directory.</param>
        public override bool DirectoryExists(string virtualDir)
        {           
            return IsPathVirtual(virtualDir) || base.DirectoryExists(virtualDir);
        }

        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <returns>
        /// true if the file exists in the virtual file system; otherwise, false.
        /// </returns>
        /// <param name="virtualPath">The path to the virtual file.</param>
        public override bool FileExists(string virtualPath)
        {         
            return IsPathVirtual(virtualPath) || base.FileExists(virtualPath);
        }

        /// <summary>
        /// Gets a virtual file from the virtual file system.
        /// </summary>
        /// <returns>
        /// A descendent of the <see cref="T:System.Web.Hosting.VirtualFile"/> class that represents a file in the virtual file system.
        /// </returns>
        /// <param name="virtualPath">The path to the virtual file.</param>
        public override VirtualFile GetFile(string virtualPath)
        {
            return new VirtualSvcFile(virtualPath, CreateFileContent(virtualPath));
        }

        private static string GetServiceName(string virtualPath)
        {
            return Path.GetFileNameWithoutExtension(virtualPath);
        }

        private string CreateFileContent(string virtualPath)
        {
            return string.Format(FileTemplate, GetServiceName(virtualPath));
        }

        private bool IsPathVirtual(string virtualPath)
        {
            string checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            return checkPath.StartsWith(
                string.Format("~/{0}", servicePath), StringComparison.InvariantCultureIgnoreCase)
                   && checkPath.EndsWith("svc", StringComparison.InvariantCulture);
        }
    }    
}