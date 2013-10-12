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
   LightInject.Mvc version 1.0.0.0
   http://seesharper.github.io/LightInject/
   http://twitter.com/bernhardrichter    
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Mvc
{
    using System;    
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    
    /// <summary>
    /// Extends the <see cref="IServiceContainer"/> interface with a method that 
    /// enables dependency injection in an ASP.NET MVC application.
    /// </summary>
    internal static class MvcContainerExtensions
    {
        /// <summary>
        /// Enables dependency injection in an ASP.NET MVC application.
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="IServiceContainer"/>.</param>
        public static void EnableMvc(this IServiceContainer serviceContainer)
        {
            serviceContainer.EnablePerWebRequestScope();
            SetDependencyResolver(serviceContainer);
            InitializeFilterAttributeProvider(serviceContainer);
        }

        private static void SetDependencyResolver(IServiceContainer serviceContainer)
        {
            DependencyResolver.SetResolver(new LightInjectMvcDependencyResolver(serviceContainer));
        }

        private static void InitializeFilterAttributeProvider(IServiceContainer serviceContainer)
        {
            RemoveExistingFilterAttributeFilterProviders();
            FilterProviders.Providers.Add(new LightInjectFilterProvider(serviceContainer));
        }

        private static void RemoveExistingFilterAttributeFilterProviders()
        {
            var existingFilterAttributeProviders =
                FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().ToArray();
            foreach (FilterAttributeFilterProvider filterAttributeFilterProvider in existingFilterAttributeProviders)
            {
                FilterProviders.Providers.Remove(filterAttributeFilterProvider);
            }
        }
    }

    /// <summary>
    /// An <see cref="IDependencyResolver"/> adapter for the LightInject service container.
    /// </summary>
    internal class LightInjectMvcDependencyResolver : IDependencyResolver
    {
        private readonly IServiceContainer serviceContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectMvcDependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceContainer">The <see cref="IServiceContainer"/> instance to 
        /// be used for resolving service instances.</param>
        internal LightInjectMvcDependencyResolver(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        /// <param name="serviceType">The type of the requested service or object.</param>
        public object GetService(Type serviceType)
        {
            return serviceContainer.TryGetInstance(serviceType);
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <returns>
        /// The requested services.
        /// </returns>
        /// <param name="serviceType">The type of the requested services.</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceContainer.GetAllInstances(serviceType);
        }
    }

    /// <summary>
    /// A <see cref="FilterAttributeFilterProvider"/> that uses an <see cref="IServiceContainer"/>    
    /// to inject property dependencies into <see cref="Filter"/> instances.
    /// </summary>
    internal class LightInjectFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IServiceContainer serviceContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectFilterProvider"/> class.
        /// </summary>
        /// <param name="serviceContainer">The <see cref="IServiceContainer"/> instance 
        /// used to inject property dependencies.</param>
        public LightInjectFilterProvider(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
        }

        /// <summary>
        /// Aggregates the filters from all of the filter providers into one collection.
        /// </summary>
        /// <returns>
        /// The collection filters from all of the filter providers.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);
            foreach (var filter in filters)
            {
                serviceContainer.InjectProperties(filter.Instance);
                yield return filter;
            }                       
        }                
    }
}
