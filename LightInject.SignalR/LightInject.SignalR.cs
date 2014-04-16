/*****************************************************************************   
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
    LightInject.SignalR version 1.0.0.1
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter    
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Extension methods must be visible")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.SignalR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using LightInject;
    using LightInject.Interception;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    /// <summary>
    /// Extends the <see cref="IServiceContainer"/> interface with a set of methods 
    /// that enables dependency injection in an ASP.Net SignalR application.
    /// </summary>
    internal static class SignalRContainerExtensions
    {
        /// <summary>
        /// Enables dependency injection in an ASP.NET SignalR application.
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="IServiceContainer"/>.</param>
        /// <param name="hubConfiguration">The <see cref="HubConfiguration"/> that represents the configuration of this ASP.Net SignalR application.</param>
        public static void EnableSignalR(this IServiceContainer serviceContainer, HubConfiguration hubConfiguration)
        {                        
            serviceContainer.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            serviceContainer.Intercept(
                sr => sr.ServiceType == typeof(IHub), ImplementDisposeMethod);               
            hubConfiguration.Resolver = new LightInjectDependencyResolver(serviceContainer);
        }

        private static void ImplementDisposeMethod(IServiceFactory serviceFactory, ProxyDefinition proxyDefinition)
        {
            proxyDefinition.Implement(
                () => new HubDisposeInterceptor(serviceFactory),
                m => m.IsDeclaredBy<IDisposable>());
        }
    }

    /// <summary>
    /// An <see cref="IDependencyResolver"/> adapter for the LightInject service container 
    /// that enables <see cref="Hub"/> instances and their dependencies to be resolved 
    /// through the service container.
    /// </summary>
    internal class LightInjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IServiceContainer serviceContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectDependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceContainer">The <see cref="IServiceContainer"/> instance to 
        /// be used for resolving service instances.</param>
        public LightInjectDependencyResolver(IServiceContainer serviceContainer) 
        {           
            this.serviceContainer = serviceContainer;            
        }

        /// <summary>
        /// Gets the requested service from the underlying <see cref="IServiceContainer"/>.
        /// If the service is not found, the request is routed to the <see cref="DefaultDependencyResolver"/>.
        /// </summary>
        /// <param name="serviceType">The requested service type.</param>
        /// <returns>An instance of the requested <paramref name="serviceType"/>.</returns>
        public override object GetService(Type serviceType)
        {
            return serviceContainer.TryGetInstance(serviceType) ?? base.GetService(serviceType);            
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>.        
        /// </summary>
        /// <param name="serviceType">The requested service type.</param>
        /// <returns>A list that contains all implementations of the <paramref name="serviceType"/>.</returns>
        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceContainer.GetAllInstances(serviceType).Concat(base.GetServices(serviceType));
        }

        /// <summary>
        /// Disposes the underlying <see cref="IServiceContainer"/>.
        /// </summary>
        /// <param name="disposing"><b>true</b> to release both managed and unmanaged resources; 
        /// <b>false</b> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                serviceContainer.Dispose();
            }
            
            base.Dispose(disposing);
        }        
    }

    /// <summary>
    /// An <see cref="IHubActivator"/> implementation that uses 
    /// an <see cref="IServiceFactory"/> instance to create <see cref="Hub"/> instances.
    /// </summary>
    internal class LightInjectHubActivator : IHubActivator
    {
        private readonly IServiceFactory serviceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectHubActivator"/> class.
        /// </summary>
        /// <param name="serviceFactory">The <see cref="IServiceFactory"/> instance that 
        /// is responsible for creating <see cref="Hub"/> instances.</param>
        public LightInjectHubActivator(IServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

        /// <summary>
        /// Creates a new hub instance.
        /// </summary>
        /// <param name="descriptor">The hub descriptor that contains information about the <see cref="Hub"/> to create.</param>
        /// <returns>A <see cref="Hub"/> instance.</returns>
        public IHub Create(HubDescriptor descriptor)
        {
            serviceFactory.BeginScope();
            var hub = (IHub)serviceFactory.GetInstance(descriptor.HubType);
            return hub;
        }
    }

    /// <summary>
    /// An <see cref="IInterceptor"/> that ends the current <see cref="Scope"/>
    /// when an <see cref="IHub"/> instance is disposed.
    /// </summary>
    internal class HubDisposeInterceptor : IInterceptor
    {
        private readonly IServiceFactory serviceContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubDisposeInterceptor"/> class.
        /// </summary>
        /// <param name="serviceContainer">The <see cref="IServiceContainer"/> that represents 
        /// the owner of the <see cref="Scope"/> to be ended.</param>
        public HubDisposeInterceptor(IServiceFactory serviceContainer)
        {
            this.serviceContainer = serviceContainer;
        }

        /// <summary>
        /// Invoked when a method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        public object Invoke(IInvocationInfo invocationInfo)
        {
            serviceContainer.EndCurrentScope();
            return invocationInfo.Proceed();
        }
    }
}
