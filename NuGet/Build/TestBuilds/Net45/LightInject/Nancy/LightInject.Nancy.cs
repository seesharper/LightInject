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

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class LightInjectNancyBootstrapper : NancyBootstrapperBase<IServiceContainer>
    {
        private IServiceContainer serviceContainer;

        /// <summary>
        /// Gets an <see cref="INancyModule"/> instance.
        /// </summary>
        /// <param name="moduleType">The type of <see cref="INancyModule"/> to get.</param>
        /// <param name="context">The current <see cref="NancyContext"/>.</param>
        /// <returns>An <see cref="INancyModule"/> instance.</returns>
        public override INancyModule GetModule(Type moduleType, NancyContext context)
        {
            EnsureScopeIsStarted(context);
            return serviceContainer.GetInstance<INancyModule>(moduleType.FullName);
        }

        /// <summary>
        /// Gets all <see cref="INancyModule"/> instances.
        /// </summary>
        /// <param name="context">The current <see cref="NancyContext"/>.</param>
        /// <returns>All <see cref="INancyModule"/> instances.</returns>
        public override IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            EnsureScopeIsStarted(context);
            return serviceContainer.GetAllInstances<INancyModule>();
        }

        /// <summary>
        /// Gets the diagnostics for initialization.
        /// </summary>
        /// <returns>An <see cref="IDiagnostics"/> instance.</returns>      
        protected override IDiagnostics GetDiagnostics()
        {
            return serviceContainer.GetInstance<IDiagnostics>();
        }

        /// <summary>
        /// Gets the <see cref="INancyEngine"/> instance.
        /// </summary>
        /// <returns><see cref="INancyEngine"/></returns>
        protected override INancyEngine GetEngineInternal()
        {
            return serviceContainer.GetInstance<INancyEngine>();
        }

        /// <summary>
        /// Initializes the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <returns><see cref="IServiceContainer"/>.</returns>
        protected override IServiceContainer GetApplicationContainer()
        {
            serviceContainer = GetServiceContainer();
            foreach (var requestStartupType in RequestStartupTasks)
            {
                serviceContainer.Register(typeof(IRequestStartup), requestStartupType, requestStartupType.FullName);
            }

            return serviceContainer;
        }

        /// <summary>
        /// Registers the <see cref="INancyModuleCatalog"/> into the underlying <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        protected override void RegisterBootstrapperTypes(IServiceContainer container)
        {
            container.RegisterInstance<INancyModuleCatalog>(this);
        }

        /// <summary>
        /// Registers the <paramref name="typeRegistrations"/> into the underlying <see cref="IServiceContainer"/>.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="typeRegistrations">Each <see cref="TypeRegistration"/> represents a service 
        /// to be registered.</param>
        protected override void RegisterTypes(IServiceContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            foreach (var typeRegistration in typeRegistrations)
            {
                switch (typeRegistration.Lifetime)
                {
                    case Lifetime.Transient:
                        RegisterTransient(typeRegistration.RegistrationType, typeRegistration.ImplementationType, string.Empty);
                        break;
                    case Lifetime.Singleton:
                        RegisterSingleton(typeRegistration.RegistrationType, typeRegistration.ImplementationType, string.Empty);
                        break;
                    case Lifetime.PerRequest:
                        RegisterPerRequest(typeRegistration.RegistrationType, typeRegistration.ImplementationType, string.Empty);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets all registered application startup tasks
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances.
        /// </returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return serviceContainer.GetAllInstances<IApplicationStartup>();
        }

        /// <summary>
        /// Gets all <see cref="IRequestStartup"/> instances.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/>.</param>
        /// <param name="requestStartupTypes">Not used in this method.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRequestStartup"/> instances.</returns>
        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(IServiceContainer container, Type[] requestStartupTypes)
        {
            return container.GetAllInstances<IRequestStartup>();
        }

        /// <summary>
        /// Gets all <see cref="IRegistrations"/> instances.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            return serviceContainer.GetAllInstances<IRegistrations>();
        }

        /// <summary>
        /// Registers multiple implementations of a given interface.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="collectionTypeRegistrations">A list of <see cref="CollectionTypeRegistration"/> instances
        /// where each instance represents an abstraction and its implementations.</param>
        protected override void RegisterCollectionTypes(IServiceContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
        {
            foreach (var collectionTypeRegistration in collectionTypeRegistrations)
            {
                foreach (Type implementingType in collectionTypeRegistration.ImplementationTypes)
                {
                    switch (collectionTypeRegistration.Lifetime)
                    {
                        case Lifetime.Transient:
                            RegisterTransient(collectionTypeRegistration.RegistrationType, implementingType, implementingType.FullName);
                            break;
                        case Lifetime.Singleton:
                            RegisterSingleton(collectionTypeRegistration.RegistrationType, implementingType, implementingType.FullName);
                            break;
                        case Lifetime.PerRequest:
                            RegisterPerRequest(collectionTypeRegistration.RegistrationType, implementingType, implementingType.FullName);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Register the given <paramref name="moduleRegistrationTypes"/> into the <param name="container">.</param>
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="moduleRegistrationTypes">The list of <see cref="ModuleRegistration"/> that 
        /// represents an <see cref="INancyModule"/> registration.</param>
        protected override void RegisterModules(IServiceContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
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
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(IServiceContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            foreach (var instanceRegistration in instanceRegistrations)
            {
                container.RegisterInstance(
                    instanceRegistration.RegistrationType,
                    instanceRegistration.Implementation);
            }
        }

        protected override IPipelines InitializeRequestPipelines(NancyContext context)
        {
            var pipelines = new Pipelines(ApplicationPipelines);
            EnsureScopeIsStarted(context);

            var requestStartupTasks = serviceContainer.GetAllInstances<IRequestStartup>();
            foreach (var requestStartupTask in requestStartupTasks)
            {
                requestStartupTask.Initialize(pipelines, context);
            }

            return pipelines;
        }

        /// <summary>
        /// Returns the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <returns><see cref="IServiceContainer"/>.</returns>
        protected virtual IServiceContainer GetServiceContainer()
        {
            var container = new ServiceContainer();
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            return container;
        }

        private void EnsureScopeIsStarted(NancyContext context)
        {
            object contextObject;
            context.Items.TryGetValue("LightInjectScope", out contextObject);
            var scope = contextObject as Scope;

            if (scope == null)
            {
                scope = serviceContainer.BeginScope();
                context.Items["LightInjectScope"] = scope;
            }
        }

        private void RegisterTransient(Type serviceType, Type implementingType, string serviceName)
        {
            if (typeof(IDisposable).IsAssignableFrom(implementingType))
            {
                serviceContainer.Register(serviceType, implementingType, serviceName, new PerRequestLifeTime());
            }
            else
            {
                serviceContainer.Register(serviceType, implementingType, serviceName);
            }
        }

        private void RegisterPerRequest(Type serviceType, Type implementingType, string serviceName)
        {
            serviceContainer.Register(serviceType, implementingType, serviceName, new PerScopeLifetime());
        }

        private void RegisterSingleton(Type serviceType, Type implementingType, string serviceName)
        {
            serviceContainer.Register(serviceType, implementingType, serviceName, new PerContainerLifetime());
        }
    }
}