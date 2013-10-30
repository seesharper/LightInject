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
   LightInject.Web version 1.0.0.2
   http://seesharper.github.io/LightInject/
   http://twitter.com/bernhardrichter    
******************************************************************************/
[assembly: System.Web.PreApplicationStartMethod(typeof(LightInject.Web.LightInjectHttpModuleInitializer), "Initialize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Extension methods must be visible within the root namespace")]
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
            ((Scope)HttpContext.Current.Items["Scope"]).Dispose();            
        }

        private static void BeginScope()
        {            
            HttpContext.Current.Items["Scope"] = serviceContainer.BeginScope();
        }         
    }   
}
