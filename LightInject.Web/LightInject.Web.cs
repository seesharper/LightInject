/*****************************************************************************   
    The MIT License (MIT)

    Copyright (c) 2013 bernhard.richter@gmail.com

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
    LightInject.Web version 1.0.0.2
    http://seesharper.github.io/LightInject/
    http://twitter.com/bernhardrichter    
******************************************************************************/
[assembly: System.Web.PreApplicationStartMethod(typeof(LightInject.Web.LightInjectHttpModuleInitializer), "Initialize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Extension methods must be visible")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject
{
    using LightInject.Web;
    
    /// <summary>
    /// Extends the <see cref="IServiceContainer"/> interface with a method
    /// to enable services that are scoped per web request.
    /// </summary>
    internal static class WebContainerExtensions
    {
        /// <summary>
        /// Ensures that services registered with the <see cref="PerScopeLifetime"/> is properly 
        /// disposed when the web request ends.
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="IServiceContainer"/>.</param>
        public static void EnablePerWebRequestScope(this IServiceContainer serviceContainer)
        {            
            LightInjectHttpModule.SetServiceContainer(serviceContainer);
        }      
    }
}

namespace LightInject.Web
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    /// <summary>
    /// Registers the <see cref="LightInjectHttpModule"/> with the current <see cref="HttpApplication"/>.
    /// </summary>
    public static class LightInjectHttpModuleInitializer
    {
        private static bool isInitialized;

        /// <summary>
        /// Executed before the <see cref="HttpApplication"/> is started and registers
        /// the <see cref="LightInjectHttpModule"/> with the current <see cref="HttpApplication"/>.
        /// </summary>
        public static void Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                DynamicModuleUtility.RegisterModule(typeof(LightInjectHttpModule));                
            }
        }
    }

    /// <summary>
    /// A <see cref="IHttpModule"/> that ensures that services registered 
    /// with the <see cref="PerScopeLifetime"/> lifetime is scoped per web request.
    /// </summary>
    public class LightInjectHttpModule : IHttpModule
    {
        private static IServiceContainer serviceContainer;
              
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application </param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (s, a) => BeginScope();
            context.EndRequest += (s, a) => EndScope();   
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {            
        }

        /// <summary>
        /// Sets the <see cref="IServiceContainer"/> instance to be used by this <see cref="LightInjectHttpModule"/>.
        /// </summary>
        /// <param name="container">The container to be used by this <see cref="LightInjectHttpModule"/>.</param>
        internal static void SetServiceContainer(IServiceContainer container)
        {
            serviceContainer = container;
        }

        private static void EndScope()
        {            
            Console.WriteLine("EndScope");
            ((Scope)HttpContext.Current.Items["Scope"]).Dispose();            
        }

        private static void BeginScope()
        {
            Console.WriteLine("BeginScope");
            HttpContext.Current.Items["Scope"] = serviceContainer.BeginScope();
        }         
    }   
}
