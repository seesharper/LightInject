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
   LightInject.Annotation version 1.0.0.0
   https://github.com/seesharper/LightInject/wiki/Getting-started
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Indicates a required property dependency or a named constructor dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        public InjectAttribute()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the service to be injected.</param>
        public InjectAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the name of the service to be injected.
        /// </summary>
        public string ServiceName { get; private set; }
    }

    /// <summary>
    /// Extends the <see cref="ServiceContainer"/> class with methods for enabling
    /// annotated property/constructor injection.
    /// </summary>
    internal static class AnnotationExtension
    {
        /// <summary>
        /// Enables annotated property injection.
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="ServiceContainer"/>
        /// for which to enable annotated property injection.</param>
        public static void EnableAnnotatedPropertyInjection(this ServiceContainer serviceContainer)
        {
            var propertySelector = new PropertySelector();
            serviceContainer.PropertyDependencySelector = new AnnotatedPropertyDependencySelector(propertySelector);
        }

        /// <summary>
        /// Enables annotated constructor injection.
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="ServiceContainer"/>
        /// for which to enable annotated constructor injection.</param>
        public static void EnableAnnotatedConstructorInjection(this ServiceContainer serviceContainer)
        {            
            serviceContainer.ConstructorDependencySelector = new AnnotatedConstructorDependencySelector();
        }
    }
    
    /// <summary>
    /// An <see cref="IPropertyDependencySelector"/> that uses the <see cref="InjectAttribute"/>
    /// to determine which properties to inject dependencies.
    /// </summary>
    internal class AnnotatedPropertyDependencySelector : PropertyDependencySelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotatedPropertyDependencySelector"/> class.
        /// </summary>
        /// <param name="propertySelector">The <see cref="IPropertySelector"/> that is 
        /// responsible for selecting a list of injectable properties.</param>
        public AnnotatedPropertyDependencySelector(IPropertySelector propertySelector)
            : base(propertySelector)
        {
        }

        /// <summary>
        /// Selects the property dependencies for the given <paramref name="type"/> 
        /// that is annotated with the <see cref="InjectAttribute"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the property dependencies.</param>
        /// <returns>A list of <see cref="PropertyDependency"/> instances that represents the property
        /// dependencies for the given <paramref name="type"/>.</returns>
        public override IEnumerable<PropertyDependency> Execute(Type type)
        {
            var properties = PropertySelector.Execute(type).Where(p => p.IsDefined(typeof(InjectAttribute), true)).ToArray();
            foreach (var propertyInfo in properties)
            {
                var injectAttribute =
                    (InjectAttribute)propertyInfo.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
                if (injectAttribute != null)
                {
                    yield return
                        new PropertyDependency
                            {
                                Property = propertyInfo,
                                ServiceName = injectAttribute.ServiceName,
                                ServiceType = propertyInfo.PropertyType,
                                IsRequired = true
                            };
                }
            }            
        }
    }

    internal class AnnotatedConstructorDependencySelector : ConstructorDependencySelector
    {
        public override IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor)
        {
            var constructorDependencies = base.Execute(constructor).ToArray();
            foreach (var constructorDependency in constructorDependencies)
            {
                var injectAttribute =
                    (InjectAttribute)constructorDependency.Parameter.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
                if (injectAttribute != null)
                {
                    constructorDependency.ServiceName = injectAttribute.ServiceName;
                }
            }

            return constructorDependencies;
        } 
    }
}
