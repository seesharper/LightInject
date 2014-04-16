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

    internal class LightInjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IServiceContainer serviceContainer;

        public LightInjectDependencyResolver(IServiceContainer serviceContainer) 
        {           
            this.serviceContainer = serviceContainer;
            
        }

        public override object GetService(Type serviceType)
        {
            return serviceContainer.TryGetInstance(serviceType) ?? base.GetService(serviceType);            
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceContainer.GetAllInstances(serviceType).Concat(base.GetServices(serviceType));
        }

        protected override void Dispose(bool disposing)
        {
            serviceContainer.Dispose();
            base.Dispose(disposing);
        }        
    }

    internal class LightInjectHubActivator : IHubActivator
    {
        private readonly IServiceFactory serviceFactory;

        public LightInjectHubActivator(IServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

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
