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
    LightInject.Nancy version 1.0.0.1
    http://seesharper.github.io/LightInject/
    http://twitter.com/bernhardrichter    
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Extension methods must be visible")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Nancy
{
    using System;
    using System.Collections.Generic;
    using global::Nancy;
    using global::Nancy.Bootstrapper;
    using global::Nancy.Diagnostics;

    /// <summary>
    /// Enables LightInject to be used as the IoC container with the Nancy web framework.
    /// </summary>
    internal abstract class LightInjectNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<Scope>
    {
        private IServiceContainer container;

        /// <summary>
        /// Gets the diagnostics for initialization.
        /// </summary>
        /// <returns>An <see cref="IDiagnostics"/> instance.</returns>                
        protected override IDiagnostics GetDiagnostics()
        {
            return container.GetInstance<IDiagnostics>();
        }

        /// <summary>
        /// Gets all registered application startup tasks
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances.
        /// </returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            var result = container.GetAllInstances<IApplicationStartup>();
            return result;
        }

        /// <summary>
        /// Registers and resolves all request startup tasks
        /// </summary>
        /// <param name="scope">Not used in this method. The value is always null.</param>
        /// <param name="requestStartupTypes">Types to register</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> instance containing <see cref="IRequestStartup"/> instances.
        /// </returns>
        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(Scope scope, Type[] requestStartupTypes)
        {
            foreach (var requestStartupType in requestStartupTypes)
            {
                container.Register(typeof(IRequestStartup), requestStartupType, requestStartupType.FullName);
            }

            return container.GetAllInstances<IRequestStartup>();
        }

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            var result = container.GetAllInstances<IRegistrations>();
            return result;
        }

        /// <summary>
        /// Gets the <see cref="INancyEngine"/> instance.
        /// </summary>
        /// <returns><see cref="INancyEngine"/></returns>
        protected override INancyEngine GetEngineInternal()
        {
            return container.GetInstance<INancyEngine>();
        }

        /// <summary>
        /// Initializes the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <returns>The return value from this method is always null.</returns>
        protected override Scope GetApplicationContainer()
        {
            container = GetServiceContainer();
            Configure(container);
            return null;
        }

        /// <summary>
        /// Returns the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <returns><see cref="IServiceContainer"/>.</returns>
        protected virtual IServiceContainer GetServiceContainer()
        {
            return new ServiceContainer();
        }

        /// <summary>
        /// Registers the <see cref="INancyModuleCatalog"/> into the underlying <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="scope">This value is always null.</param>
        protected override void RegisterBootstrapperTypes(Scope scope)
        {
            container.RegisterInstance<INancyModuleCatalog>(this);
        }

        /// <summary>
        /// Registers the <paramref name="typeRegistrations"/> into the underlying <see cref="IServiceContainer"/>.
        /// </summary>
        /// <param name="scope">Not used in this method. The value is always null.</param>
        /// <param name="typeRegistrations">Each <see cref="TypeRegistration"/> represents a service 
        /// to be registered.</param>
        protected override void RegisterTypes(Scope scope, IEnumerable<TypeRegistration> typeRegistrations)
        {
            foreach (var typeRegistration in typeRegistrations)
            {
                switch (typeRegistration.Lifetime)
                {
                    case Lifetime.Transient:
                        RegisterTransient(typeRegistration.RegistrationType, typeRegistration.ImplementationType);
                        break;
                    case Lifetime.Singleton:
                        RegisterSingleton(typeRegistration.RegistrationType, typeRegistration.ImplementationType);
                        break;
                    case Lifetime.PerRequest:
                        RegisterPerRequest(typeRegistration.RegistrationType, typeRegistration.ImplementationType);
                        break;
                }
            }
        }

        /// <summary>
        /// Register the various collections into the container as singletons to later be resolved
        /// by IEnumerable{Type} constructor dependencies.
        /// </summary>
        /// <param name="scope">Not used in this method. The value is always null.</param>
        /// <param name="collectionTypeRegistrations">Collection type registrations to register</param>
        protected override void RegisterCollectionTypes(Scope scope, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
        {
            foreach (var collectionTypeRegistration in collectionTypeRegistrations)
            {
                foreach (Type implementingType in collectionTypeRegistration.ImplementationTypes)
                {
                    switch (collectionTypeRegistration.Lifetime)
                    {
                        case Lifetime.Transient:
                            RegisterTransient(collectionTypeRegistration.RegistrationType, implementingType);
                            break;
                        case Lifetime.Singleton:
                            RegisterSingleton(collectionTypeRegistration.RegistrationType, implementingType);
                            break;
                        case Lifetime.PerRequest:
                            RegisterPerRequest(collectionTypeRegistration.RegistrationType, implementingType);
                            break;
                    }                    
                }
            }
        }
        
        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="scope">Not used in this method.</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(Scope scope, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            foreach (var instanceRegistration in instanceRegistrations)
            {
                container.RegisterInstance(
                    instanceRegistration.RegistrationType,
                    instanceRegistration.Implementation);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Scope"/>.
        /// </summary>
        /// <returns><see cref="Scope"/>.</returns>
        protected override Scope CreateRequestContainer()
        {
            return container.BeginScope();
        }

        /// <summary>
        /// Register the given module types into the request container
        /// </summary>
        /// <param name="scope">Not used in this method.</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected override void RegisterRequestContainerModules(Scope scope, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            foreach (var moduleRegistrationType in moduleRegistrationTypes)
            {
                container.Register(
                    typeof(INancyModule),
                    moduleRegistrationType.ModuleType,
                    moduleRegistrationType.ModuleType.FullName,
                    new PerScopeLifetime());
            }
        }

        /// <summary>
        /// Retrieve all <see cref="INancyModule"/> instances from the container
        /// </summary>
        /// <param name="scope">Not used in this method</param>
        /// <returns>
        /// A list of <see cref="INancyModule"/> instances.
        /// </returns>
        protected override IEnumerable<INancyModule> GetAllModules(Scope scope)
        {
            return container.GetAllInstances<INancyModule>();
        }

        /// <summary>
        /// Retrieve a specific <see cref="INancyModule"/> instance from the container
        /// </summary>
        /// <param name="scope">Not used in this method</param>
        /// <param name="moduleType">Type of the module</param>
        /// <returns>A <see cref="INancyModule"/> instance.</returns>
        protected override INancyModule GetModule(Scope scope, Type moduleType)
        {
            return container.GetInstance<INancyModule>(moduleType.FullName);            
        }

        /// <summary>
        /// Allows the <paramref name="serviceContainer"/> to be configured.
        /// </summary>
        /// <param name="serviceContainer">The target <see cref="IServiceContainer"/> instance.</param>
        protected virtual void Configure(IServiceContainer serviceContainer)
        {
        }

        private void RegisterTransient(Type serviceType, Type implementingType)
        {
            if (typeof(IDisposable).IsAssignableFrom(implementingType))
            {
                container.Register(serviceType, implementingType, new PerRequestLifeTime());
            }
        }

        private void RegisterPerRequest(Type serviceType, Type implementingType)
        {
            container.Register(serviceType, implementingType, new PerScopeLifetime());
        }

        private void RegisterSingleton(Type serviceType, Type implementingType)
        {
            container.Register(serviceType, implementingType, new PerContainerLifetime());
        }
    }  
}
