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
   LightInject.Wcf.Client version 1.0.0.1
   http://www.lightinject.net/
   http://twitter.com/bernhardrichter
******************************************************************************/

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
    using LightInject.Wcf.Client;

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> class with a method that enables services 
    /// to resolved as WCF service proxies.
    /// </summary>
    internal static class WcfContainerExtensions
    {        
        /// <summary>
        /// Enables services to be resolved a WCF service proxies.
        /// </summary>
        /// <param name="serviceRegistery">The target <see cref="IServiceRegistry"/>.</param>
        /// <param name="uriProvider">The <see cref="IUriProvider"/> that is responsible 
        /// for providing an <see cref="Uri"/> that represents the service endpoint address.</param>
        internal static void EnableWcf(this IServiceRegistry serviceRegistery, IUriProvider uriProvider)
        {
            serviceRegistery.RegisterFallback(
                (serviceType, serviceName) => serviceType.IsDefined(typeof(ServiceContractAttribute), true),
                CreateWcfProxy);
            serviceRegistery.Register<IBindingProvider, BindingProvider>(new PerContainerLifetime());
            serviceRegistery.RegisterInstance(uriProvider);
            serviceRegistery.Register<IChannelProvider, ChannelProvider>(new PerContainerLifetime());
            serviceRegistery.Register<IChannelFactoryProvider, ChannelFactoryProvider>(new PerContainerLifetime());
#if NET
            serviceRegistery.Register<IServiceProxyFactory, ServiceProxyFactory>(new PerContainerLifetime());
            serviceRegistery.Register<IServiceProxyTypeFactory, ServiceProxyTypeFactory>(new PerContainerLifetime());
#endif
            serviceRegistery.Register<IServiceClient, ServiceClient>();
            serviceRegistery.Decorate<IChannelFactoryProvider, ChannelFactoryProviderCacheDecorator>();
        }

        private static object CreateWcfProxy(ServiceRequest serviceRequest)
        {
            var serviceProxyFactory = serviceRequest.ServiceFactory.GetInstance<IServiceProxyFactory>();
            return serviceProxyFactory.CreateProxy(serviceRequest.ServiceType);
        }
    }
}

namespace LightInject.Wcf.Client
{
    using System;
    using System.Collections.Generic;    
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;    
#if NET 
    using LightInject.Interception;
#endif

    /// <summary>
    /// Represents a class that is capable of providing a
    /// <see cref="Binding"/> that is used to invoke the target service.
    /// </summary>
    internal interface IBindingProvider
    {
        /// <summary>
        /// Gets an instance of a class that inherits from the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for which to return a <see cref="Binding"/></param>        
        /// <returns>An instance of a class that inherits from the <see cref="Binding"/> class.</returns>
        Binding GetBinding(Type serviceType);
    }

    /// <summary>
    /// Represents a class that is capable of providing a valid <see cref="Uri"/>
    /// that represents the endpoint address for a given service type.
    /// </summary>
    internal interface IUriProvider
    {
        /// <summary>
        /// Gets an <see cref="Uri"/> that represents 
        /// the endpoint address for the <param name="serviceType"/>.        
        /// </summary>
        /// <param name="serviceType">The service type for which to return an <see cref="Uri"/></param>
        /// <returns><see cref="Uri"/></returns>
        Uri GetUri(Type serviceType);        
    }

    /// <summary>
    /// Represents a class that is capable of providing a <see cref="ChannelFactory{TChannel}"/>-
    /// </summary>
    internal interface IChannelFactoryProvider
    {
        /// <summary>
        /// Gets a <see cref="ChannelFactory{TChannel}"/> that is used to 
        /// create a <typeparamref name="TService"/> channel.
        /// </summary>
        /// <typeparam name="TService">The service type for which to get a <see cref="ChannelFactory{TChannel}"/>.</typeparam>
        /// <returns>a <see cref="ChannelFactory{TChannel}"/> that is used to create a <typeparamref name="TService"/> channel.</returns>
        ChannelFactory<TService> GetChannelFactory<TService>();
    }

    /// <summary>
    /// Represents a class that is capable of providing a channel 
    /// that is used to invoke the service.
    /// </summary>
    internal interface IChannelProvider
    {
        /// <summary>
        /// Gets a channel used to invoke the target <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service for which to get a channel.</typeparam>
        /// <returns>A channel used to invoke the target <typeparamref name="TService"/>.</returns>
        TService GetChannel<TService>();        
    }

    /// <summary>
    /// Represents a class that is capable of creating a proxy type 
    /// that implements the target service interface.
    /// </summary>
    internal interface IServiceProxyTypeFactory
    {
        /// <summary>
        /// Gets a proxy type that implements the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of service for which to create a service proxy type.</param>
        /// <returns>A proxy type that implements the <paramref name="serviceType"/>.</returns>
        Type GetProxyType(Type serviceType);
    }

    /// <summary>
    /// Represents a class that is capable of creating 
    /// a proxy used to invoke the target service.
    /// </summary>
    internal interface IServiceProxyFactory
    {
        /// <summary>
        /// Creates a service proxy used to invoke the target service.
        /// </summary>
        /// <param name="serviceType">The type of service for which to create a service proxy.</param>
        /// <returns>A service proxy used to invoke the target service</returns>
        object CreateProxy(Type serviceType);
    }

    /// <summary>
    /// Represents a class that is capable of invoking a service method     
    /// </summary>
    internal interface IServiceClient
    {
        /// <summary>
        /// Invokes a method on the target <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to invoke.</typeparam>
        /// <typeparam name="TResult">The return type of the service invocation.</typeparam>
        /// <param name="function">A function delegate that represents the service method to be invoked.</param>
        /// <returns>The return value from the service invocation.</returns>
        TResult Invoke<TService, TResult>(Func<TService, TResult> function);
    }

#if NET    
    /// <summary>
    /// An <see cref="IInterceptor"/> that intercepts method calls made to 
    /// a service interface and forwards the method call to an <see cref="IServiceClient"/>.
    /// </summary>
    public class ServiceInterceptor : IInterceptor
    {
        private readonly IServiceClient serviceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInterceptor"/> class.
        /// </summary>
        /// <param name="serviceClient">The <see cref="IServiceClient"/> that is 
        /// responsible for invoking the target service.</param>
        internal ServiceInterceptor(IServiceClient serviceClient)
        {
            this.serviceClient = serviceClient;
        }

        /// <summary>
        /// Invoked when a service method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        public object Invoke(IInvocationInfo invocationInfo)
        {
            Type funcType = typeof(Func<,>).MakeGenericType(
                invocationInfo.Method.DeclaringType,
                invocationInfo.Method.ReturnType);

            var del = Delegate.CreateDelegate(funcType, invocationInfo.Method);

            var openGenericinvokeMethod = typeof(IServiceClient).GetMethod("Invoke");
            var closedGenericMethod = openGenericinvokeMethod.MakeGenericMethod(
                invocationInfo.Method.DeclaringType,
                invocationInfo.Method.ReturnType);

            var result = closedGenericMethod.Invoke(serviceClient, new object[] { del });

            return result;
        }
    }
#endif        
    /// <summary>
    /// Provides a <see cref="ChannelFactory{TChannel}"/>.
    /// </summary>
    internal class ChannelFactoryProvider : IChannelFactoryProvider
    {
        private readonly IBindingProvider bindingProvider;
        private readonly IUriProvider uriProvider;

        private readonly IEnumerable<IEndpointBehavior> endpointBehaviors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFactoryProvider"/> class.
        /// </summary>
        /// <param name="bindingProvider">The <see cref="IBindingProvider"/> that is responsible 
        /// for providing a <see cref="Binding"/> to the <see cref="ChannelFactory{TChannel}"/>.</param>
        /// <param name="uriProvider">The <see cref="IUriProvider"/> that is responsible for 
        /// providing an <see cref="Uri"/> that represents the service endpoint address.</param>
        /// <param name="endpointBehaviors">A list of <see cref="IEndpointBehavior"/> instances.</param>
        public ChannelFactoryProvider(IBindingProvider bindingProvider, IUriProvider uriProvider, IEnumerable<IEndpointBehavior> endpointBehaviors)
        {
            this.bindingProvider = bindingProvider;
            this.uriProvider = uriProvider;
            this.endpointBehaviors = endpointBehaviors;
        }

        /// <summary>
        /// Gets a <see cref="ChannelFactory{TChannel}"/> that is used to 
        /// create a <typeparamref name="TService"/> channel.
        /// </summary>
        /// <typeparam name="TService">The service type for which to get a <see cref="ChannelFactory{TChannel}"/>.</typeparam>
        /// <returns>a <see cref="ChannelFactory{TChannel}"/> that is used to create a <typeparamref name="TService"/> channel.</returns>
        public virtual ChannelFactory<TService> GetChannelFactory<TService>()
        {
            var binding = bindingProvider.GetBinding(typeof(TService));
            var uri = uriProvider.GetUri(typeof(TService));
            var channelFactory = new ChannelFactory<TService>(binding, new EndpointAddress(uri));
            foreach (var endpointBehavior in endpointBehaviors)
            {                                
                channelFactory.Endpoint.EndpointBehaviors.Add(endpointBehavior);
            }

            return channelFactory;
        }
    }

    /// <summary>
    /// An <see cref="IChannelFactoryProvider"/> cache decorator that ensures that 
    /// channel factories are cached by the service type.
    /// </summary>
    internal class ChannelFactoryProviderCacheDecorator : IChannelFactoryProvider
    {
        private readonly Lazy<IChannelFactoryProvider> channelFactoryProvider;
        private readonly ThreadSafeDictionary<Type, IChannelFactory> cache = new ThreadSafeDictionary<Type, IChannelFactory>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFactoryProviderCacheDecorator"/> class.
        /// </summary>
        /// <param name="channelFactoryProvider">The <see cref="IChannelFactoryProvider"/> that is 
        /// responsible for providing channel factories.</param>
        public ChannelFactoryProviderCacheDecorator(Lazy<IChannelFactoryProvider> channelFactoryProvider)
        {
            this.channelFactoryProvider = channelFactoryProvider;
        }

        /// <summary>
        /// Gets a <see cref="ChannelFactory{TChannel}"/> that is used to 
        /// create a <typeparamref name="TService"/> channel.
        /// </summary>
        /// <typeparam name="TService">The service type for which to get a <see cref="ChannelFactory{TChannel}"/>.</typeparam>
        /// <returns>a <see cref="ChannelFactory{TChannel}"/> that is used to create a <typeparamref name="TService"/> channel.</returns>
        public ChannelFactory<TService> GetChannelFactory<TService>()
        {
            return (ChannelFactory<TService>)cache.GetOrAdd(typeof(TService), type => CreateChannelFactory<TService>());
        }

        private IChannelFactory CreateChannelFactory<T>()
        {
            return channelFactoryProvider.Value.GetChannelFactory<T>();
        }
    }
#if NET
    /// <summary>
    /// Creates a proxy used to invoke the target service.
    /// </summary>
    internal class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly IServiceProxyTypeFactory serviceProxyTypeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProxyFactory"/> class.
        /// </summary>
        /// <param name="serviceProxyTypeFactory">The <see cref="IServiceProxyTypeFactory"/> that 
        /// is responsible for creating a service proxy type.</param>
        public ServiceProxyFactory(IServiceProxyTypeFactory serviceProxyTypeFactory)
        {
            this.serviceProxyTypeFactory = serviceProxyTypeFactory;
        }

        /// <summary>
        /// Creates a service proxy used to invoke the target service.
        /// </summary>
        /// <param name="serviceType">The type of service for which to create a service proxy.</param>
        /// <returns>A service proxy used to invoke the target service</returns>
        public object CreateProxy(Type serviceType)
        {
            var proxyType = serviceProxyTypeFactory.GetProxyType(serviceType);
            return Activator.CreateInstance(proxyType);
        }
    }

    /// <summary>
    /// Creates a proxy type that implements the target service interface.    
    /// </summary>
    internal class ServiceProxyTypeFactory : IServiceProxyTypeFactory
    {        
        private readonly IServiceClient serviceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProxyTypeFactory"/> class.
        /// </summary>
        /// <param name="serviceClient">The <see cref="IServiceClient"/> that is responsible 
        /// for invoking the target service.</param>
        public ServiceProxyTypeFactory(IServiceClient serviceClient)
        {     
            this.serviceClient = serviceClient;
        }

        /// <summary>
        /// Gets a proxy type that implements the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of service for which to create a service proxy type.</param>
        /// <returns>A proxy type that implements the <paramref name="serviceType"/>.</returns>
        public Type GetProxyType(Type serviceType)
        {
            var proxyDefinition = CreateProxyDefinition(serviceType);

            IProxyBuilder proxyBuilder = new ProxyBuilder();
            return proxyBuilder.GetProxyType(proxyDefinition);
        }

        private ProxyDefinition CreateProxyDefinition(Type serviceType)
        {
            var proxyDefinition = new ProxyDefinition(serviceType, () => null);
            proxyDefinition.Implement(
                () => new ServiceInterceptor(serviceClient),
                method => method.DeclaringType == serviceType);
            return proxyDefinition;
        }
    }
#endif
    /// <summary>
    /// A class that is capable of invoking a service method
    /// </summary>
    internal class ServiceClient : IServiceClient
    {
        private readonly IChannelProvider channelProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClient"/> class.
        /// </summary>
        /// <param name="channelProvider">The <see cref="IChannelProvider"/> that is responsible 
        /// for creating a channel that is used to invoke the target service.</param>
        public ServiceClient(IChannelProvider channelProvider)
        {
            this.channelProvider = channelProvider;
        }

        /// <summary>
        /// Invokes a method on the target <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to invoke.</typeparam>
        /// <typeparam name="TResult">The return type of the service invocation.</typeparam>
        /// <param name="function">A function delegate that represents the service method to be invoked.</param>
        /// <returns>The return value from the service invocation.</returns>
        public TResult Invoke<TService, TResult>(Func<TService, TResult> function)
        {
            var channel = channelProvider.GetChannel<TService>();            
            try
            {
                TResult result = function(channel);
                ((IClientChannel)channel).Close();
                return result;
            }
            catch 
            {
                ((IClientChannel)channel).Abort();
                throw;
            }            
        }
    }
   
    /// <summary>
    /// Provides a valid <see cref="Uri"/>
    /// that represents the endpoint address for a given service type.
    /// </summary>
    internal class UriProvider : IUriProvider
    {
        private readonly string baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriProvider"/> class.
        /// </summary>
        /// <param name="baseAddress">The base address from which the service url is constructed.</param>
        public UriProvider(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <summary>
        /// Gets an <see cref="Uri"/> that represents 
        /// the endpoint address for the <param name="serviceType"/>.        
        /// </summary>
        /// <param name="serviceType">The service type for which to return an <see cref="Uri"/></param>
        /// <returns><see cref="Uri"/></returns>
        public virtual Uri GetUri(Type serviceType)
        {            
            var url = baseAddress + serviceType.FullName + ".svc";
            return new Uri(url);
        }
    }

    /// <summary>
    /// Provides a <see cref="Binding"/> based on the given uri scheme 
    /// that is used to invoke the target service.    
    /// </summary>
    internal class BindingProvider : IBindingProvider
    {
        private readonly IUriProvider uriProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingProvider"/> class.
        /// </summary>
        /// <param name="uriProvider">The <see cref="IUriProvider"/> that is responsible 
        /// for providing the uri scheme.</param>    
        public BindingProvider(IUriProvider uriProvider)
        {
            this.uriProvider = uriProvider;
        }

        /// <summary>
        /// Gets an instance of a class that inherits from the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for which to return a <see cref="Binding"/></param>        
        /// <returns>An instance of a class that inherits from the <see cref="Binding"/> class.</returns>
        public virtual Binding GetBinding(Type serviceType)
        {
            Uri uri = uriProvider.GetUri(serviceType);

            Binding binding;
            switch (uri.Scheme)
            {
#if NET
                case "net.tcp":
                    binding = new NetTcpBinding(SecurityMode.None);
                    break;
                case "http":
                    binding = new BasicHttpBinding();
                    break;
                case "https":
                    binding = new WSHttpBinding(SecurityMode.None);
                    break;
                case "net.pipe":
                    binding = new NetNamedPipeBinding();
                    break;
#endif
#if WP_PCL || NETFX_CORE_PCL || WINDOWS_PHONE || NETFX_CORE
                case "net.tcp":
                    binding = new NetTcpBinding(SecurityMode.None);
                    break;
                case "http":
                case "https":
                    binding = new BasicHttpBinding();
                    break;

#endif
                default:
                    throw new InvalidOperationException(string.Format("Unable to resolve binding for Uri: {0}", uri));
            }

            return binding;
        }
    }

    /// <summary>
    /// Provides a channel that is used to invoke the service.    
    /// </summary>
    internal class ChannelProvider : IChannelProvider
    {
        private readonly IChannelFactoryProvider channelFactoryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelProvider"/> class.
        /// </summary>
        /// <param name="channelFactoryProvider">The <see cref="IChannelFactoryProvider"/> that 
        /// is responsible for creating a <see cref="ChannelFactory{TChannel}"/>.</param>    
        public ChannelProvider(IChannelFactoryProvider channelFactoryProvider)
        {
            this.channelFactoryProvider = channelFactoryProvider;
        }

        /// <summary>
        /// Gets a channel used to invoke the target <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service for which to get a channel.</typeparam>
        /// <returns>A channel used to invoke the target <typeparamref name="TService"/>.</returns>
        public TService GetChannel<TService>()
        {
            var channelFactory = channelFactoryProvider.GetChannelFactory<TService>();
            return channelFactory.CreateChannel();
        }       
    }    
}
