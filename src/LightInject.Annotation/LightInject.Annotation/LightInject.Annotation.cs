/*****************************************************************************   
    The MIT License (MIT)

    Copyright (c) 2015 bernhard.richter@gmail.com

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
    LightInject.Annotation version 1.0.0.5
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
    using LightInject.Annotation;

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
    public static class AnnotationExtension
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
            serviceContainer.ConstructorSelector = new AnnotatedConstructorSelector(serviceContainer.CanGetInstance);
        }
    }
}

namespace LightInject.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    /// <summary>
    /// An <see cref="IPropertyDependencySelector"/> that uses the <see cref="InjectAttribute"/>
    /// to determine which properties to inject dependencies.
    /// </summary>
    public class AnnotatedPropertyDependencySelector : PropertyDependencySelector
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
            var properties =
                PropertySelector.Execute(type).Where(p => p.IsDefined(typeof(InjectAttribute), true)).ToArray();
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

    /// <summary>
    /// A <see cref="ConstructorDependencySelector"/> that looks for the <see cref="InjectAttribute"/> 
    /// to determine the name of service to be injected.
    /// </summary>
    public class AnnotatedConstructorDependencySelector : ConstructorDependencySelector
    {
        /// <summary>
        /// Selects the constructor dependencies for the given <paramref name="constructor"/>.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructionInfo"/> for which to select the constructor dependencies.</param>
        /// <returns>A list of <see cref="ConstructorDependency"/> instances that represents the constructor
        /// dependencies for the given <paramref name="constructor"/>.</returns>
        public override IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor)
        {
            var constructorDependencies = base.Execute(constructor).ToArray();
            foreach (var constructorDependency in constructorDependencies)
            {
                var injectAttribute =
                    (InjectAttribute)
                    constructorDependency.Parameter.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
                if (injectAttribute != null)
                {
                    constructorDependency.ServiceName = injectAttribute.ServiceName;
                }
            }

            return constructorDependencies;
        }
    }

    /// <summary>
    /// A <see cref="IConstructorSelector"/> implementation that uses information 
    /// from the <see cref="InjectAttribute"/> to determine if a given service can be resolved.
    /// </summary>
    public class AnnotatedConstructorSelector : MostResolvableConstructorSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotatedConstructorSelector"/> class.
        /// </summary>
        /// <param name="canGetInstance">A function delegate that determines if a service type can be resolved.</param>
        public AnnotatedConstructorSelector(Func<Type, string, bool> canGetInstance)
            : base(canGetInstance)
        {
        }

        /// <summary>
        /// Gets the service name based on the given <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterInfo"/> for which to get the service name.</param>
        /// <returns>The name of the service for the given <paramref name="parameter"/>.</returns>
        protected override string GetServiceName(ParameterInfo parameter)
        {
            var injectAttribute =
                      (InjectAttribute)
                      parameter.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
            
            return injectAttribute != null ? injectAttribute.ServiceName : base.GetServiceName(parameter);
        }
    }
}