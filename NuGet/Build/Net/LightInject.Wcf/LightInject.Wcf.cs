/*********************************************************************************   
    The MIT License (MIT)

    Copyright (c) 2014 bernhard.richter@gmail.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
   LightInject.Wcf version 1.0.0.1
   http://www.lightinject.net/
   http://twitter.com/bernhardrichter
******************************************************************************/
[assembly: System.Web.PreApplicationStartMethod(typeof(LightInject.Wcf.LightInjectWcfInitializer), "Initialize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Extension methods must be visible")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject
{
    using System;
    using System.ServiceModel;
    using LightInject.Wcf;

    /// <summary>
    /// Extends the <see cref="IServiceContainer"/> interface with a method
    /// to enable services that are scoped per <see cref="OperationContext"/>.
    /// </summary>    
    public static class WcfContainerExtensions
    {
        /// <summary>
        /// Ensures that services registered with the <see cref="PerScopeLifetime"/> or <see cref="PerRequestLifeTime"/> 
        /// is properly disposed at the end of an <see cref="OperationContext"/>.        
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="IServiceContainer"/>.</param>
        public static void EnableWcf(this IServiceContainer serviceContainer)
        {
            LightInjectServiceHostFactory.Container = serviceContainer;
        }
    }

    /// <summary>
    /// Extends the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if the <paramref name="type"/> represents a service contract.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> represents a service type, otherwise <b>false</b>.</returns>
        public static bool IsServiceContract(this Type type)
        {
            return type.IsDefined(typeof(ServiceContractAttribute), true);
        }
    }
}

namespace LightInject.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
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
                HostingEnvironment.RegisterVirtualPathProvider(new VirtualSvcPathProvider());
            }
        }
    }

    /// <summary>
    /// A subclass of the <see cref="ServiceHost"/> class that exposes the 
    /// <see cref="ServiceHost.ApplyConfiguration"/> method through the <see cref="LoadConfiguration"/> method.   
    /// </summary>
    public class LightInjectServiceHost : ServiceHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectServiceHost"/> class with the type of service and its base addresses specified. 
        /// </summary>
        /// <param name="serviceType">The type of hosted service.</param>
        /// <param name="baseAddresses">An array of type <see cref="Uri"/> that contains the base addresses for the hosted service.</param>
        public LightInjectServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        /// <summary>
        /// Loads the service description from the configuration file and applies it to the runtime being constructed.
        /// </summary>
        public void LoadConfiguration()
        {
            ApplyConfiguration();
        }
    }

    /// <summary>
    /// A <see cref="ServiceHostFactory"/> that uses the LightInject <see cref="ServiceContainer"/>
    /// to create WCF services.
    /// </summary>    
    public class LightInjectServiceHostFactory : ServiceHostFactory
    {
        private static IServiceContainer container;

        /// <summary>
        /// Sets the <see cref="IServiceContainer"/> instance that is 
        /// used to resolve services.
        /// </summary>
        internal static IServiceContainer Container
        {
            set
            {              
                container = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> with specific base addresses and initializes it with specified data.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost"/> with specific base addresses.
        /// </returns>
        /// <param name="constructorString">The initialization data passed to the <see cref="T:System.ServiceModel.ServiceHostBase"/> instance being constructed by the factory. </param><param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param><exception cref="T:System.ArgumentNullException"><paramref name="baseAddresses"/> is null.</exception><exception cref="T:System.InvalidOperationException">There is no hosting context provided or <paramref name="constructorString"/> is null or empty.</exception>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            EnsureValidServiceContainer();
            
            ServiceRegistration registration = GetServiceRegistrationByName(constructorString);
            if (registration != null)
            {
                return CreateServiceHost(registration.ServiceType, registration.ServiceName, baseAddresses);
            }

            return base.CreateServiceHost(constructorString, baseAddresses);
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
            return CreateServiceHost(serviceType, serviceType.FullName, baseAddresses);
        }

        private static void EnsureValidServiceContainer()
        {
            if (container == null)
            {
                container = new ServiceContainer();
            }
        }

        private ServiceHost CreateServiceHost(Type serviceType, string constructorString, Uri[] baseAddresses)
        {
            ValidateServiceType(serviceType);

            var proxyType = CreateServiceProxyType(serviceType);

            var serviceHost = new LightInjectServiceHost(proxyType, baseAddresses);
            serviceHost.Description.ConfigurationName = constructorString;
            serviceHost.Description.Name = constructorString;
            serviceHost.AddDefaultEndpoints();
            serviceHost.LoadConfiguration();
            ApplyServiceBehaviors(serviceHost);
            ApplyEndpointBehaviors(serviceHost);

            return serviceHost;
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
        
        private static ServiceRegistration GetServiceRegistrationByName(string constructorString)
        {
            var registrations =
                container.AvailableServices.Where(
                    sr => sr.ServiceName.Equals(constructorString, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();

            if (registrations.Length == 0)
            {
                return null;
            }

            if (registrations.Length > 1)
            {
                throw new InvalidOperationException(
                    string.Format("Multiple services found under the same name '{0}'", constructorString));
            }

            return registrations[0];
        }
       
        private static void ApplyEndpointBehaviors(ServiceHost serviceHost)
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

        private static void ApplyServiceBehaviors(ServiceHostBase serviceHost)
        {
            var serviceBehaviors = container.GetAllInstances<IServiceBehavior>();
            var description = serviceHost.Description;
            foreach (var serviceBehavior in serviceBehaviors)
            {
                description.Behaviors.Add(serviceBehavior);
            }
        }

        private static Type CreateServiceProxyType(Type serviceType)
        {
            var proxyBuilder = new ProxyBuilder();
            var proxyDefinition = CreateProxyDefinition(serviceType);
            ImplementServiceInterface(serviceType, proxyDefinition);
            return proxyBuilder.GetProxyType(proxyDefinition);
        }

        private static ProxyDefinition CreateProxyDefinition(Type serviceType)
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

        private static void ImplementServiceInterface(
           Type serviceType, ProxyDefinition proxyDefinition)
        {
            proxyDefinition.Implement(() => new ServiceInterceptor(container), m => m.IsDeclaredBy(serviceType));
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
            if (IsPathVirtual(virtualPath))
            {
                return new VirtualSvcFile(virtualPath, CreateFileContent(virtualPath));
            }

            return base.GetFile(virtualPath);
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
                "~/", StringComparison.InvariantCultureIgnoreCase)
                   && checkPath.EndsWith("svc", StringComparison.InvariantCulture);
        }
    }
}
