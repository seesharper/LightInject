/*****************************************************************************   
   Copyright 2012 bernhard.richter@gmail.com

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
   LightInject version 2.0.0.1 
   https://github.com/seesharper/LightInject/wiki/Getting-started
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject
{
    using System;
#if NET
    using System.Collections.Concurrent;
#endif
#if SILVERLIGHT
    using System.Collections;
#endif
    using System.Collections.Generic;
#if NET    
    using System.IO;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;    

    /// <summary>
    /// Defines a set of methods used to register services into the service container.
    /// </summary>
    internal interface IServiceRegistry
    {
        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        void Register(Type serviceType, Type implementingType);
        
        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register(Type serviceType, Type implementingType, ILifetime lifetime);

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="serviceName">The name of the service.</param>
        void Register(Type serviceType, Type implementingType, string serviceName);
        
        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register(Type serviceType, Type implementingType, string serviceName, ILifetime lifetime);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        void Register<TService, TImplementation>() where TImplementation : TService;

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register<TService, TImplementation>(ILifetime lifetime) where TImplementation : TService;

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        /// <param name="serviceName">The name of the service.</param>
        void Register<TService, TImplementation>(string serviceName) where TImplementation : TService;
        
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register<TService, TImplementation>(string serviceName, ILifetime lifetime) where TImplementation : TService;

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        void Register<TService>(TService instance);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        void Register<TService>(TService instance, string serviceName);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The lambdaExpression that describes the dependencies of the service.</param>
        /// <example>
        /// The following example shows how to register a new IFoo service.
        /// <code>
        /// <![CDATA[
        /// container.Register<IFoo>(r => new FooWithDependency(r.GetInstance<IBar>()))
        /// ]]>
        /// </code>
        /// </example>
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression);
        
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, ILifetime lifetime);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="serviceName">The name of the service.</param>        
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, string serviceName);
        
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="serviceName">The name of the service.</param>        
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, string serviceName, ILifetime lifetime);

        void Register(Func<Type, string, bool> servicePredicate, Func<Type, string, IServiceFactory, object> factory);


        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>        
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        void RegisterAssembly(Assembly assembly);

        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        void RegisterAssembly(Assembly assembly, Func<Type, bool> shouldRegister);
        
        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetime);
        
        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>
        /// <param name="lifetimeFactory">The <see cref="ILifetime"/> factory that controls the lifetime of the registered service.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, bool> shouldRegister);

#if NET

        /// <summary>
        /// Registers services from assemblies in the base directory that matches the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern used to filter the assembly files.</param>
        void RegisterAssembly(string searchPattern);
#endif

        void Decorate(Type serviceType, Type decoratorType, Func<ServiceInfo, bool> shouldDecorate);

        void Decorate(Type serviceType, Type decoratorType);

        void Decorate<TService>(Expression<Func<IServiceFactory, TService, TService>> factory);
    }

    /// <summary>
    /// Defines a set of methods used to retrieve service instances.
    /// </summary>
    internal interface IServiceFactory
    {
        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType);

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType, string serviceName);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <returns>The requested service instance.</returns>
        TService GetInstance<TService>();

        /// <summary>
        /// Gets a named instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<TService>(string serviceName);

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>A list that contains all implementations of the <paramref name="serviceType"/>.</returns>
        IEnumerable<object> GetAllInstances(Type serviceType);

        /// <summary>
        /// Gets all instances of type <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of services to resolve.</typeparam>
        /// <returns>A list that contains all implementations of the <typeparamref name="TService"/> type.</returns>
        IEnumerable<TService> GetAllInstances<TService>();
    }
  
    internal interface ILifetime
    {
        object GetInstance(Func<object> createInstance, ResolutionContext resolutionContext);        
    }
    
    /// <summary>
    /// Represents a class that acts as a composition root for an <see cref="IServiceRegistry"/> instance.
    /// </summary>
    internal interface ICompositionRoot
    {
        /// <summary>
        /// Composes services by adding services to the <paramref name="serviceRegistry"/>.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        void Compose(IServiceRegistry serviceRegistry);
    }

    /// <summary>
    /// Represents a class that is responsible for selecting properties that represents a dependency to the target <see cref="Type"/>.
    /// </summary>
    internal interface IPropertySelector
    {
        /// <summary>
        /// Selects properties that represents a dependency from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the properties.</param>
        /// <returns>A list of properties that represents a dependency to the target <paramref name="type"/></returns>
        IEnumerable<PropertyInfo> Select(Type type);
    }

    /// <summary>
    /// Represents a class that is responsible loading a set of assemblies based on the given search pattern.
    /// </summary>
    internal interface IAssemblyLoader
    {
        /// <summary>
        /// Loads a set of assemblies based on the given <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use.</param>
        /// <returns>A list of assemblies based on the given <paramref name="searchPattern"/>.</returns>
        IEnumerable<Assembly> Load(string searchPattern);
    }

    /// <summary>
    /// Represents a class that is capable of scanning an assembly and register services into an <see cref="IServiceContainer"/> instance.
    /// </summary>
    internal interface IAssemblyScanner
    {
        /// <summary>
        /// Scans the target <paramref name="assembly"/> and registers services found within the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, bool> shouldRegister);
    }

    /// <summary>
    /// Represents an inversion of control container.
    /// </summary>
    internal interface IServiceContainer : IServiceRegistry, IServiceFactory
    {        
        /// <summary>
        /// Gets a list of <see cref="ServiceInfo"/> instances that represents the 
        /// registered services.          
        /// </summary>
        IEnumerable<ServiceInfo> AvailableServices { get; }

        /// <summary>
        /// Returns <b>true</b> if the container can create the requested service, otherwise <b>false</b>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <returns><b>true</b> if the container can create the requested service, otherwise <b>false</b>.</returns>
        bool CanGetInstance(Type serviceType, string serviceName);

        ResolutionScope BeginResolutionScope();
    }

    /// <summary>
    /// An ultra lightweight service container.
    /// </summary>
    internal class ServiceContainer : IServiceContainer
    {        
        private const string UnresolvedDependencyError = "Unresolved dependency {0}";                
        private static readonly MethodInfo GetCurrentMethod;        

        private readonly ServiceRegistry<Action<DynamicMethodInfo>> emitters = new ServiceRegistry<Action<DynamicMethodInfo>>();        
        private readonly ServiceRegistry<Action<DynamicMethodInfo, Type>> openGenericEmitters = new ServiceRegistry<Action<DynamicMethodInfo, Type>>(); 
        private readonly DelegateRegistry<Type> delegates = new DelegateRegistry<Type>();
        private readonly DelegateRegistry<Tuple<Type, string>> namedDelegates = new DelegateRegistry<Tuple<Type, string>>();
        private readonly ThreadSafeDictionary<ServiceInfo, ConstructionInfo> implementations = new ThreadSafeDictionary<ServiceInfo, ConstructionInfo>();                
        private readonly Storage<object> constants = new Storage<object>();
        private readonly Stack<Action<DynamicMethodInfo>> dependencyStack = new Stack<Action<DynamicMethodInfo>>();
        private readonly ThreadSafeDictionary<Tuple<Type, string>, ServiceInfo> availableServices =
            new ThreadSafeDictionary<Tuple<Type, string>, ServiceInfo>();
        private readonly ThreadSafeDictionary<Type, List<DecoratorRegistration>> decorators = new ThreadSafeDictionary<Type, List<DecoratorRegistration>>();        
        private bool firstServiceRequest = true;
        private readonly ScopeManager scopeManager = new ScopeManager();
        


        static ServiceContainer()
        {            
            GetCurrentMethod = typeof(ScopeManager).GetProperty("Current").GetGetMethod();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        public ServiceContainer()
        {            
            AssemblyScanner = new AssemblyScanner();
            PropertySelector = new PropertySelector();
#if NET
            AssemblyLoader = new AssemblyLoader();
#endif
        }

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyScanner"/> instance that is responsible for scanning assemblies.
        /// </summary>
        public IAssemblyScanner AssemblyScanner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPropertySelector"/> instance that is responsible selecting the properties
        /// that represents a dependency for a given <see cref="Type"/>.
        /// </summary>
        public IPropertySelector PropertySelector { get; set; }
#if NET
        
        /// <summary>
        /// Gets or sets the <see cref="IAssemblyLoader"/> instance that is responsible for loading assemblies during assembly scanning. 
        /// </summary>
        public IAssemblyLoader AssemblyLoader { get; set; }
#endif

        /// <summary>
        /// Gets a list of <see cref="ServiceInfo"/> instances that represents the registered services.           
        /// </summary>
        public IEnumerable<ServiceInfo> AvailableServices
        {
            get
            {
                return availableServices.Values;
            }
        }

        /// <summary>
        /// Returns <b>true</b> if the container can create the requested service, otherwise <b>false</b>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <returns><b>true</b> if the container can create the requested service, otherwise <b>false</b>.</returns>
        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return GetEmitMethod(serviceType, serviceName) != null;
        }

        public ResolutionScope BeginResolutionScope()
        {
            return scopeManager.BeginScope(ScopeOption.Required);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="serviceName">The name of the service.</param>        
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, string serviceName, ILifetime lifetime)
        {
            RegisterServiceFromLambdaExpression(expression, lifetime, serviceName);
        }

        public void Register(Func<Type, string, bool> servicePredicate, Func<Type, string, IServiceFactory, object> factory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>        
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>             
        public void RegisterAssembly(Assembly assembly)
        {
            RegisterAssembly(assembly, t => true);
        }

        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        public void RegisterAssembly(Assembly assembly, Func<Type, bool> shouldRegister)
        {
            AssemblyScanner.Scan(assembly, this, () => null, shouldRegister);
        }
        
        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>
        /// <param name="lifetimeFactory">The <see cref="ILifetime"/> factory that controls the lifetime of the registered service.</param>
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        public void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory)
        {
            AssemblyScanner.Scan(assembly, this, lifetimeFactory, t => true);
        }
        
        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>
        /// <param name="lifetimeFactory">The <see cref="ILifetime"/> factory that controls the lifetime of the registered service.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        public void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, bool> shouldRegister)
        {
            AssemblyScanner.Scan(assembly, this, lifetimeFactory, shouldRegister);
        }

#if NET
       
        /// <summary>
        /// Registers services from assemblies in the base directory that matches the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern used to filter the assembly files.</param>
        public void RegisterAssembly(string searchPattern)
        {
            foreach (Assembly assembly in AssemblyLoader.Load(searchPattern))
            {
                RegisterAssembly(assembly);
            }
        }       
#endif        
        public void Decorate(Type serviceType, Type decoratorType, Func<ServiceInfo, bool> shouldDecorate)
        {
            var decoratorInfo = new DecoratorRegistration { ServiceType = serviceType, ImplementingType = decoratorType, CanDecorate = shouldDecorate };                
            GetRegisteredDecorators(serviceType).Add(decoratorInfo);
        }

        public void Decorate(Type serviceType, Type decoratorType)
        {
            Decorate(serviceType, decoratorType, si => true);
        }

        public void Decorate<TService>(Expression<Func<IServiceFactory, TService, TService>> factory)
        {
            var decoratorInfo = new DecoratorRegistration { FactoryExpression = factory, ServiceType = typeof(TService), CanDecorate = si => true};            
            GetRegisteredDecorators(typeof(TService)).Add(decoratorInfo);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register(Type serviceType, Type implementingType, ILifetime lifetime)
        {
            Register(serviceType, implementingType, string.Empty, lifetime);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register(Type serviceType, Type implementingType, string serviceName, ILifetime lifetime)
        {
            RegisterService(serviceType, implementingType, lifetime, serviceName);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation));
        }
        
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register<TService, TImplementation>(ILifetime lifetime) where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), lifetime);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        /// <param name="serviceName">The name of the service.</param>
        public void Register<TService, TImplementation>(string serviceName) where TImplementation : TService
        {
            Register<TService, TImplementation>(serviceName, lifetime: null);
        }
        
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <typeparamref name="TImplementation"/>.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <typeparam name="TImplementation">The implementing type.</typeparam>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register<TService, TImplementation>(string serviceName, ILifetime lifetime) where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), serviceName, lifetime);
        }
        
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="factory">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory, ILifetime lifetime)
        {
            RegisterServiceFromLambdaExpression(factory, lifetime, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="factory">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="serviceName">The name of the service.</param>        
        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory, string serviceName)
        {
            RegisterServiceFromLambdaExpression(factory, null, serviceName);
        }
      
        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        public void Register<TService>(TService instance)
        {
            Register(instance, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        public void Register<TService>(TService instance, string serviceName)
        {
            RegisterValue(typeof(TService), instance, serviceName);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="factory">The lambdaExpression that describes the dependencies of the service.</param>
        /// <example>
        /// The following example shows how to register a new IFoo service.
        /// <code>
        /// <![CDATA[
        /// container.Register<IFoo>(r => new FooWithDependency(r.GetInstance<IBar>()))
        /// ]]>
        /// </code>
        /// </example>
        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory)
        {
            RegisterServiceFromLambdaExpression(factory, null, string.Empty);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="serviceName">The name of the service.</param>
        public void Register(Type serviceType, Type implementingType, string serviceName)
        {
            RegisterService(serviceType, implementingType, null, serviceName);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        public void Register(Type serviceType, Type implementingType)
        {
            RegisterService(serviceType, implementingType, null, string.Empty);
        }

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType)
        {
            Func<object> del;

            if (!delegates.TryGetValue(serviceType, out del))
            {
                del = delegates.GetOrAdd(serviceType, t => CreateDelegate(t, string.Empty));
            }

            return del();                       
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <returns>The requested service instance.</returns>
        public TService GetInstance<TService>()
        {
            return (TService)GetInstance(typeof(TService));
        }

        /// <summary>
        /// Gets a named instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<TService>(string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName);
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, string serviceName)
        {
            Func<object> del;

            if (!namedDelegates.TryGetValue(Tuple.Create(serviceType, serviceName), out del))
            {
                del = namedDelegates.GetOrAdd(Tuple.Create(serviceType, serviceName), t => CreateDelegate(t.Item1, serviceName));
            }

            return del();                              
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>A list that contains all implementations of the <paramref name="serviceType"/>.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (IEnumerable<object>)GetInstance(typeof(IEnumerable<>).MakeGenericType(serviceType));
        }

        /// <summary>
        /// Gets all instances of type <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of services to resolve.</typeparam>
        /// <returns>A list that contains all implementations of the <typeparamref name="TService"/> type.</returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return GetInstance<IEnumerable<TService>>();
        }

        private static void EmitLoadConstant(DynamicMethodInfo dynamicMethodInfo, int index, Type type)
        {           
            var generator = dynamicMethodInfo.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, index);
            generator.Emit(OpCodes.Ldelem_Ref);
            generator.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        private static void EmitEnumerable(IList<Action<DynamicMethodInfo>> serviceEmitters, Type elementType, DynamicMethodInfo dynamicMethodInfo)
        {
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            LocalBuilder array = generator.DeclareLocal(elementType.MakeArrayType());
            generator.Emit(OpCodes.Ldc_I4, serviceEmitters.Count);
            generator.Emit(OpCodes.Newarr, elementType);
            generator.Emit(OpCodes.Stloc, array);

            for (int index = 0; index < serviceEmitters.Count; index++)
            {
                generator.Emit(OpCodes.Ldloc, array);
                generator.Emit(OpCodes.Ldc_I4, index);
                var serviceEmitter = serviceEmitters[index];
                serviceEmitter(dynamicMethodInfo);
                generator.Emit(OpCodes.Stelem, elementType);
            }

            generator.Emit(OpCodes.Ldloc, array);
        }
       
        private static bool IsEnumerableOfT(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsFunc(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Func<>);
        }

        private static bool IsClosedGeneric(Type serviceType)
        {
            return serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition;
        }

        private static bool IsFuncWithStringArgument(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Func<,>)
                && serviceType.GetGenericArguments()[0] == typeof(string);
        }

        private static ConstructorInfo GetConstructorWithTheMostParameters(Type implementingType)
        {
            return implementingType.GetConstructors().OrderBy(c => c.GetParameters().Count()).LastOrDefault();
        }
        
        private static IEnumerable<ConstructorDependency> GetConstructorDependencies(ConstructorInfo constructorInfo)
        {
            return
                constructorInfo.GetParameters().OrderBy(p => p.Position).Select(
                    p => new ConstructorDependency { ServiceName = string.Empty, ServiceType = p.ParameterType, Parameter = p });
        }

        private static ILifetime CloneLifeTime(ILifetime lifetime)
        {
            return lifetime == null ? null : (ILifetime)Activator.CreateInstance(lifetime.GetType());
        }

        private Func<object> CreateDynamicMethodDelegate(Action<DynamicMethodInfo> serviceEmitter, Type serviceType)
        {
            var dynamicMethodInfo = new DynamicMethodInfo();
            serviceEmitter(dynamicMethodInfo);
            if (serviceType.IsValueType)
            {
                dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Box, serviceType);
            }

            var del = dynamicMethodInfo.CreateDelegate();
            return () => del(constants.Items);
        }

        private ConstructionInfo CreateConstructionInfo(Registration registration)
        {
            if (registration.FactoryExpression != null)
            {
                return CreateConstructionInfoFromLambdaExpression(registration.FactoryExpression);                
            }
            
            return CreateConstructionInfoFromImplementingType(registration.ImplementingType);
        }

        private ConstructionInfo CreateConstructionInfoFromImplementingType(Type implementingType)
        {
            var constructionInfo = new ConstructionInfo();
            ConstructorInfo constructorInfo = GetConstructorWithTheMostParameters(implementingType);
            constructionInfo.ImplementingType = implementingType;
            constructionInfo.Constructor = constructorInfo;
            constructionInfo.ConstructorDependencies.AddRange(GetConstructorDependencies(constructorInfo));
            constructionInfo.PropertyDependencies.AddRange(GetPropertyDependencies(implementingType));
            return constructionInfo;
        }

        private IEnumerable<PropertyDependecy> GetPropertyDependencies(Type implementingType)
        {
            return GetInjectableProperties(implementingType).Select(
                p => new PropertyDependecy { Property = p, ServiceName = string.Empty, ServiceType = p.PropertyType });
        }

        private IEnumerable<PropertyInfo> GetInjectableProperties(Type implementingType)
        {
            return PropertySelector.Select(implementingType);
        }

        private ConstructionInfo CreateConstructionInfoFromLambdaExpression(LambdaExpression lambdaExpression)
        {
            var lambdaExpressionParser = new LambdaExpressionParser();
            ConstructionInfo constructionInfo = lambdaExpressionParser.Parse(lambdaExpression);
            return constructionInfo;
        }

        private Action<DynamicMethodInfo> GetEmitMethod(Type serviceType, string serviceName)
        {
            if (FirstServiceRequest())
            {
                EnsureThatServiceRegistryIsConfigured(serviceType);                
            }

            Action<DynamicMethodInfo> emitter = GetRegisteredEmitMethod(serviceType, serviceName);
                        
            return CreateEmitMethodWrapper(emitter, serviceType, serviceName);
        }

        private Action<DynamicMethodInfo> CreateEmitMethodWrapper(Action<DynamicMethodInfo> emitter, Type serviceType, string serviceName)
        {
            if (emitter == null)
            {
                return null;
            }

            return dmi =>
                {
                    if (dependencyStack.Contains(emitter))
                    {                        
                        throw new InvalidOperationException(
                            string.Format("Recursive dependency detected: ServiceType:{0}, ServiceName:{1}]", serviceType, serviceName));                                                
                    }

                    dependencyStack.Push(emitter);
                    emitter(dmi);
                    dependencyStack.Pop();
                };
        }

        private Action<DynamicMethodInfo> GetRegisteredEmitMethod(Type serviceType, string serviceName)
        {
            Action<DynamicMethodInfo> emitter;
            var registrations = GetServiceEmitters(serviceType);
            registrations.TryGetValue(serviceName, out emitter);
            return emitter ?? ResolveUnknownServiceEmitter(serviceType, serviceName);
        }

        private void UpdateServiceEmitter(Type serviceType, string serviceName, Action<DynamicMethodInfo> emitter)
        {
            if (emitter != null)
            {
                GetServiceEmitters(serviceType).AddOrUpdate(serviceName, s => emitter, (s, m) => emitter);
            }
        }
        
        private void UpdateServiceRegistration(ServiceInfo serviceInfo)
        {
            var key = Tuple.Create(serviceInfo.ServiceType, serviceInfo.ServiceName);
            availableServices.AddOrUpdate(key, k => serviceInfo, (k, s) => serviceInfo);
        }

        private void EmitNewInstance(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            
            var serviceDecorators = GetDecorators(serviceInfo.ServiceType);
            if (serviceDecorators.Length > 0)
            {
                EmitDecorators(serviceInfo, serviceDecorators, dynamicMethodInfo, () => DoEmitNewInstance(GetConstructionInfo(serviceInfo), dynamicMethodInfo));
            }
            else
            {
                DoEmitNewInstance(GetConstructionInfo(serviceInfo), dynamicMethodInfo);    
            }
        }
        
        private DecoratorRegistration[] GetDecorators(Type serviceType)
        {
            var registeredDecorators = GetRegisteredDecorators(serviceType);
            if (registeredDecorators.Count == 0 && serviceType.IsGenericType)
            {
                var openGenericServiceType = serviceType.GetGenericTypeDefinition();
                var openGenericDecorators = GetRegisteredDecorators(openGenericServiceType);
                if (openGenericDecorators.Count >= 0)
                {                    
                    foreach (DecoratorRegistration openGenericDecorator in openGenericDecorators)
                    {
                        var closedGenericDecoratorType = openGenericDecorator.ImplementingType.MakeGenericType(serviceType.GetGenericArguments());
                        var decoratorInfo = new DecoratorRegistration() { ServiceType = serviceType, ImplementingType = closedGenericDecoratorType, CanDecorate = openGenericDecorator.CanDecorate};                        
                        registeredDecorators.Add(decoratorInfo);
                    }
                }
            }

            return registeredDecorators.ToArray();
        }

        private void DoEmitDecoratorInstance(DecoratorRegistration decoratorRegistration, DynamicMethodInfo dynamicMethodInfo, Action pushInstance)
        {

            ConstructionInfo constructionInfo = GetConstructionInfo(decoratorRegistration);
            var constructorDependency =
                constructionInfo.ConstructorDependencies.FirstOrDefault(cd => cd.ServiceType == decoratorRegistration.ServiceType);
            if (constructorDependency != null)
            {
                constructorDependency.IsDecoratorTarget = true;
            }
            
            if (constructionInfo.FactoryDelegate != null)
            {                
                EmitNewDecoratorUsingFactoryDelegate(constructionInfo.FactoryDelegate, dynamicMethodInfo, pushInstance);
            }
            else
            {                
                EmitNewInstanceUsingImplementingType(dynamicMethodInfo, constructionInfo, pushInstance);
            }
        }

        private void EmitNewDecoratorUsingFactoryDelegate(Delegate factoryDelegate, DynamicMethodInfo dynamicMethodInfo, Action pushInstance)
        {
            var factoryDelegateIndex = constants.Add(factoryDelegate);
            var serviceFactoryIndex = constants.Add(this);
            Type funcType = factoryDelegate.GetType();
            EmitLoadConstant(dynamicMethodInfo, factoryDelegateIndex, funcType);
            EmitLoadConstant(dynamicMethodInfo, serviceFactoryIndex, typeof(IServiceFactory));
            pushInstance();
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            MethodInfo invokeMethod = funcType.GetMethod("Invoke");
            generator.Emit(OpCodes.Callvirt, invokeMethod);
        }

        private void DoEmitNewInstance(ConstructionInfo constructionInfo, DynamicMethodInfo dynamicMethodInfo)
        {        
            if (constructionInfo.FactoryDelegate != null)
            {
                EmitNewInstanceUsingFactoryDelegate(constructionInfo.FactoryDelegate, dynamicMethodInfo);
            }
            else
            {
                EmitNewInstanceUsingImplementingType(dynamicMethodInfo, constructionInfo, null);
            }
        }
     
        private void EmitDecorators(ServiceInfo serviceInfo, IEnumerable<DecoratorRegistration> serviceDecorators, DynamicMethodInfo dynamicMethodInfo, Action decoratorTargetEmitter)
        {
            Action decoratorChainEmitter = decoratorTargetEmitter;            
            foreach (DecoratorRegistration decorator in serviceDecorators)
            {
                if (!decorator.CanDecorate(serviceInfo))
                {
                    continue;
                }
                Action currentDecoratorTargetEmitter = decoratorTargetEmitter;
                DecoratorRegistration currentDecorator = decorator;
                decoratorTargetEmitter = () => DoEmitDecoratorInstance(currentDecorator, dynamicMethodInfo, currentDecoratorTargetEmitter);
            }

            decoratorTargetEmitter();
        }

        private void EmitNewInstanceUsingImplementingType(DynamicMethodInfo dynamicMethodInfo, ConstructionInfo constructionInfo, Action decoratorTargetEmitter)
        {
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            EmitConstructorDependencies(constructionInfo, dynamicMethodInfo, decoratorTargetEmitter);
            generator.Emit(OpCodes.Newobj, constructionInfo.Constructor);
            EmitPropertyDependencies(constructionInfo, dynamicMethodInfo);
        }

        private void EmitNewInstanceUsingFactoryDelegate(Delegate factoryDelegate, DynamicMethodInfo dynamicMethodInfo)
        {            
            var factoryDelegateIndex = constants.Add(factoryDelegate);
            var serviceFactoryIndex = constants.Add(this);
            Type funcType = factoryDelegate.GetType();            
            EmitLoadConstant(dynamicMethodInfo, factoryDelegateIndex, funcType);
            EmitLoadConstant(dynamicMethodInfo, serviceFactoryIndex, typeof(IServiceFactory));
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            MethodInfo invokeMethod = funcType.GetMethod("Invoke");
            generator.Emit(OpCodes.Callvirt, invokeMethod);
        }

        private void EmitConstructorDependencies(ConstructionInfo constructionInfo, DynamicMethodInfo dynamicMethodInfo, Action decoratorTargetEmitter)
        {
            foreach (ConstructorDependency dependency in constructionInfo.ConstructorDependencies)
            {
                if (!dependency.IsDecoratorTarget)
                {
                    EmitDependency(dynamicMethodInfo, dependency);                    
                }
                else
                {
                    decoratorTargetEmitter();
                }
            }
        }

        private void EmitDependency(DynamicMethodInfo dynamicMethodInfo, Dependency dependency)
        {
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            if (dependency.FactoryExpression != null)
            {
                var lambda = Expression.Lambda(dependency.FactoryExpression, new ParameterExpression[] { }).Compile();
                MethodInfo methodInfo = lambda.GetType().GetMethod("Invoke");
                EmitLoadConstant(dynamicMethodInfo, constants.Add(lambda), lambda.GetType());
                generator.Emit(OpCodes.Callvirt, methodInfo);
            }
            else
            {
                Action<DynamicMethodInfo> emitter = GetEmitMethod(dependency.ServiceType, dependency.ServiceName);                                                
                if (emitter == null)
                {
                    emitter = GetEmitMethod(dependency.ServiceType, dependency.Name);
                    if (emitter == null)
                    {
                        throw new InvalidOperationException(string.Format(UnresolvedDependencyError, dependency));    
                    }                    
                }

                try
                {
                    emitter(dynamicMethodInfo);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(string.Format(UnresolvedDependencyError, dependency), ex);
                }                
            }
        }

        private void EmitPropertyDependencies(ConstructionInfo constructionInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            if (constructionInfo.PropertyDependencies.Count == 0)
            {
                return;
            }

            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            LocalBuilder instance = generator.DeclareLocal(constructionInfo.ImplementingType);
            generator.Emit(OpCodes.Stloc, instance);
            foreach (var propertyDependency in constructionInfo.PropertyDependencies)
            {
                generator.Emit(OpCodes.Ldloc, instance);
                EmitDependency(dynamicMethodInfo, propertyDependency);
                dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Callvirt, propertyDependency.Property.GetSetMethod());
            }

            generator.Emit(OpCodes.Ldloc, instance);
        }

        private Action<DynamicMethodInfo> ResolveUnknownServiceEmitter(Type serviceType, string serviceName)
        {
            Action<DynamicMethodInfo> emitter = null;
            if (IsFunc(serviceType))
            {
                emitter = CreateServiceEmitterBasedOnFuncServiceRequest(serviceType, false);
            }
            else if (IsEnumerableOfT(serviceType))
            {
                emitter = CreateEnumerableServiceEmitter(serviceType);
            }
            else if (IsFuncWithStringArgument(serviceType))
            {
                emitter = CreateServiceEmitterBasedOnFuncServiceRequest(serviceType, true);
            }
            else if (CanRedirectRequestForDefaultServiceToSingleNamedService(serviceType, serviceName))
            {
                emitter = CreateServiceEmitterBasedOnSingleNamedInstance(serviceType);
            }
            else if (IsClosedGeneric(serviceType))
            {
                emitter = CreateServiceEmitterBasedOnClosedGenericServiceRequest(serviceType, serviceName);
            }
                        
            UpdateServiceEmitter(serviceType, serviceName, emitter);
            
            return emitter;
        }

        private Action<DynamicMethodInfo> CreateEnumerableServiceEmitter(Type serviceType)
        {
            Type actualServiceType = serviceType.GetGenericArguments()[0];
            if (actualServiceType.IsGenericType)
            {
                EnsureEmitMethodsForOpenGenericTypesAreCreated(actualServiceType);
            }

            IList<Action<DynamicMethodInfo>> serviceEmitters = GetServiceEmitters(actualServiceType).Values.ToList();
            
            if (dependencyStack.Count > 0 && serviceEmitters.Contains(dependencyStack.Peek()))
            {
                serviceEmitters.Remove(dependencyStack.Peek());
            }

            return dmi => EmitEnumerable(serviceEmitters, actualServiceType, dmi);
        }

        private void EnsureEmitMethodsForOpenGenericTypesAreCreated(Type actualServiceType)
        {
            var openGenericServiceType = actualServiceType.GetGenericTypeDefinition();
            var openGenericServiceEmitters = GetOpenGenericRegistrations(openGenericServiceType);
            foreach (var openGenericEmitterEntry in openGenericServiceEmitters.Keys)
            {
                GetRegisteredEmitMethod(actualServiceType, openGenericEmitterEntry);
            }
        }

        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnFuncServiceRequest(Type serviceType, bool namedService)
        {
            var actualServiceType = serviceType.GetGenericArguments().Last();
            var methodInfo = typeof(ServiceContainer).GetMethod("CreateFuncGetInstanceDelegate", BindingFlags.Instance | BindingFlags.NonPublic);
            var del = methodInfo.MakeGenericMethod(actualServiceType).Invoke(this, new object[] { namedService });
            var constantIndex = constants.Add(del);
            return dmi => EmitLoadConstant(dmi, constantIndex, serviceType);
        }

        private Delegate CreateFuncGetInstanceDelegate<TServiceType>(bool namedService)
        {
            if (namedService)
            {
                Func<string, TServiceType> func = GetInstance<TServiceType>;
                return func;
            }
            else
            {
                Func<TServiceType> func = GetInstance<TServiceType>;
                return func;
            }
        }
        
        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnClosedGenericServiceRequest(Type closedGenericServiceType, string serviceName)
        {
            Type openGenericServiceType = closedGenericServiceType.GetGenericTypeDefinition();

            Action<DynamicMethodInfo, Type> openGenericEmitter = GetOpenGenericTypeInfo(openGenericServiceType, serviceName);
            if (openGenericEmitter == null)
            {
                return null;
            }
            
            return dmi => openGenericEmitter(dmi, closedGenericServiceType);
        }
  
        private Action<DynamicMethodInfo, Type> GetOpenGenericTypeInfo(Type openGenericServiceType, string serviceName)
        {
            var openGenericRegistrations = GetOpenGenericRegistrations(openGenericServiceType);
            if (CanRedirectRequestForDefaultOpenGenericServiceToSingleNamedService(openGenericServiceType, serviceName))
            {
                return openGenericRegistrations.First().Value;
            }

            Action<DynamicMethodInfo, Type> openGenericEmitter;
            openGenericRegistrations.TryGetValue(serviceName, out openGenericEmitter);
            return openGenericEmitter;
        }

        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnSingleNamedInstance(Type serviceType)
        {
            return GetEmitMethod(serviceType, GetServiceEmitters(serviceType).First().Key);
        }

        private bool CanRedirectRequestForDefaultServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetServiceEmitters(serviceType).Count == 1;
        }

        private bool CanRedirectRequestForDefaultOpenGenericServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetOpenGenericRegistrations(serviceType).Count == 1;
        }

        private ConstructionInfo GetConstructionInfo(Registration registration)
        {
            return CreateConstructionInfo(registration);            
        }

        private ThreadSafeDictionary<string, Action<DynamicMethodInfo>> GetServiceEmitters(Type serviceType)
        {
            return emitters.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, Action<DynamicMethodInfo>>(StringComparer.InvariantCultureIgnoreCase));
        }

        private List<DecoratorRegistration> GetRegisteredDecorators(Type serviceType)
        {
            return decorators.GetOrAdd(serviceType, s => new List<DecoratorRegistration>());
        }

        private ThreadSafeDictionary<string, Action<DynamicMethodInfo, Type>> GetOpenGenericRegistrations(Type serviceType)
        {
            return openGenericEmitters.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, Action<DynamicMethodInfo, Type>>(StringComparer.InvariantCultureIgnoreCase));
        }

        private void RegisterService(Type serviceType, Type implementingType, ILifetime lifetime, string serviceName)
        {
            if (serviceType.IsGenericTypeDefinition)
            {
                RegisterOpenGenericService(serviceType, implementingType, lifetime, serviceName);
            }
            else
            {                
                var serviceInfo = new ServiceInfo { ServiceType = serviceType, ImplementingType = implementingType, ServiceName = serviceName, Lifetime = lifetime };                
                UpdateServiceEmitter(serviceType, serviceName, GetEmitDelegate(serviceInfo));    
                UpdateServiceRegistration(serviceInfo);
            }
        }

        private void RegisterOpenGenericService(Type openGenericServiceType, Type openGenericImplementingType, ILifetime lifetime, string serviceName)
        {
            Action<DynamicMethodInfo, Type> emitter = (dmi, closedGenericServiceType) =>
                { 
                    Type closedGenericImplementingType = openGenericImplementingType.MakeGenericType(closedGenericServiceType.GetGenericArguments());
                    var serviceInfo = new ServiceInfo { ServiceType = closedGenericServiceType, ImplementingType = closedGenericImplementingType, ServiceName = serviceName, Lifetime = CloneLifeTime(lifetime) };
                    var closedGenericEmitter = GetEmitDelegate(serviceInfo);
                    UpdateServiceEmitter(closedGenericServiceType, serviceName, closedGenericEmitter);
                    UpdateServiceRegistration(serviceInfo);
                    closedGenericEmitter(dmi);
                };
            GetOpenGenericRegistrations(openGenericServiceType).AddOrUpdate(serviceName, s => emitter, (s, e) => emitter);            
        }
        
        private Action<DynamicMethodInfo> GetEmitDelegate(ServiceInfo serviceInfo)
        {
            if (serviceInfo.Lifetime == null)
            {
                return dynamicMethodInfo => EmitNewInstance(serviceInfo, dynamicMethodInfo);
            }
            
            return dynamicMethodInfo => EmitLifetime(serviceInfo, dmi => EmitNewInstance(serviceInfo, dmi), dynamicMethodInfo);
        }
                
        private void EmitLifetime(ServiceInfo serviceInfo, Action<DynamicMethodInfo> instanceEmitter, DynamicMethodInfo dynamicMethodInfo)
        {                        
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            int instanceDelegateIndex = CreateInstanceDelegateIndex(instanceEmitter);
            int lifetimeIndex = CreateLifetimeIndex(serviceInfo.Lifetime);
            int scopeManagerIndex = CreateScopeManagerIndex();
            var getInstanceMethod = typeof(ILifetime).GetMethod("GetInstance");
            EmitLoadConstant(dynamicMethodInfo, lifetimeIndex, typeof(ILifetime));
            EmitLoadConstant(dynamicMethodInfo, instanceDelegateIndex, typeof(Func<object>));            
            EmitLoadConstant(dynamicMethodInfo, scopeManagerIndex, typeof(ScopeManager));
            generator.Emit(OpCodes.Callvirt, GetCurrentMethod);
            generator.Emit(OpCodes.Callvirt, getInstanceMethod);
            generator.Emit(serviceInfo.ServiceType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, serviceInfo.ServiceType);
        }

        private int CreateScopeManagerIndex()
        {
            return constants.Add(scopeManager);
        }

        private int CreateInstanceDelegateIndex(Action<DynamicMethodInfo> instanceEmitter)
        {
            return constants.Add(CreateInstanceDelegate(instanceEmitter));
        }

        private int CreateLifetimeIndex(ILifetime lifetime)
        {
            return constants.Add(lifetime);
        }

        private Func<object> CreateInstanceDelegate(Action<DynamicMethodInfo> instanceEmitter)
        {
            var dynamicMethodInfo = new DynamicMethodInfo();
            instanceEmitter(dynamicMethodInfo);
            Func<object> del = () => dynamicMethodInfo.CreateDelegate()(constants.Items);
            return del;            
        }

        private Func<object> CreateDelegate(Type serviceType, string serviceName)
        {                        
            var serviceEmitter = GetEmitMethod(serviceType, serviceName);
            if (serviceEmitter == null)
            {                
                throw new InvalidOperationException(string.Format("Unable to resolve type: {0}, service name: {1}", serviceType, serviceName));
            }

            try
            {
                return CreateDynamicMethodDelegate(serviceEmitter, serviceType);
            }
            catch (InvalidOperationException ex)
            {
                dependencyStack.Clear();
                throw new InvalidOperationException(string.Format("Unable to resolve type: {0}, service name: {1}", serviceType, serviceName), ex);
            }            
        }

        private bool FirstServiceRequest()
        {
            if (firstServiceRequest)
            {                
                firstServiceRequest = false;
                return true;
            }

            return false;
        }

        private void EnsureThatServiceRegistryIsConfigured(Type serviceType)
        {
            if (ServiceRegistryIsEmpty())
            {
                RegisterAssembly(serviceType.Assembly, () => null);
            }
        }
         
        private bool ServiceRegistryIsEmpty()
        {
            return emitters.Count == 0 && openGenericEmitters.Count == 0;
        }

        private void RegisterValue(Type serviceType, object value, string serviceName)
        {
            int index = constants.Add(value);
            UpdateServiceEmitter(serviceType, serviceName, dmi => EmitLoadConstant(dmi, index, serviceType));            
        }

        private void RegisterServiceFromLambdaExpression<TService>(
            Expression<Func<IServiceFactory, TService>> factory, ILifetime lifetime, string serviceName)
        {            
            var serviceInfo = new ServiceInfo { ServiceType = typeof(TService), FactoryExpression = factory, ServiceName = serviceName, Lifetime = lifetime };                        
            UpdateServiceEmitter(typeof(TService), serviceName, GetEmitDelegate(serviceInfo));
            UpdateServiceRegistration(serviceInfo);
        }
               
        /// <summary>
        /// Parses a <see cref="LambdaExpression"/> into a <see cref="ConstructionInfo"/> instance.
        /// </summary>
        public class LambdaExpressionParser
        {                                                            
            /// <summary>
            /// Parses the <paramref name="lambdaExpression"/> and returns a <see cref="ConstructionInfo"/> instance.
            /// </summary>
            /// <param name="lambdaExpression">The <see cref="LambdaExpression"/> to parse.</param>
            /// <returns>A <see cref="ConstructionInfo"/> instance.</returns>
            public ConstructionInfo Parse(LambdaExpression lambdaExpression)
            {                                                                
                var lambdaExpressionValidator = new LambdaExpressionValidator();
                
                if (!lambdaExpressionValidator.CanParse(lambdaExpression))
                {
                    return CreateServiceInfoBasedOnLambdaExpression(lambdaExpression);
                }
                
                switch (lambdaExpression.Body.NodeType)
                {
                    case ExpressionType.New:
                        return CreateServiceInfoBasedOnNewExpression((NewExpression)lambdaExpression.Body);
                    case ExpressionType.MemberInit:
                        return CreateServiceInfoBasedOnHandleMemberInitExpression((MemberInitExpression)lambdaExpression.Body);                                      
                    default:
                        return CreateServiceInfoBasedOnLambdaExpression(lambdaExpression);
                }                
            }

            private static ConstructionInfo CreateServiceInfoBasedOnLambdaExpression(LambdaExpression lambdaExpression)
            {
                return new ConstructionInfo { FactoryDelegate = lambdaExpression.Compile() };
            }

            private static ConstructionInfo CreateServiceInfoBasedOnNewExpression(NewExpression newExpression)
            {
                var serviceInfo = CreateServiceInfo(newExpression);
                ParameterInfo[] parameters = newExpression.Constructor.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    ConstructorDependency constructorDependency = CreateConstructorDependency(parameters[i]);
                    ApplyDependencyDetails(newExpression.Arguments[i], constructorDependency);
                    serviceInfo.ConstructorDependencies.Add(constructorDependency);
                }

                return serviceInfo;
            }

            private static ConstructionInfo CreateServiceInfo(NewExpression newExpression)
            {
                var serviceInfo = new ConstructionInfo { Constructor = newExpression.Constructor, ImplementingType = newExpression.Constructor.DeclaringType };
                return serviceInfo;
            }

            private static ConstructionInfo CreateServiceInfoBasedOnHandleMemberInitExpression(MemberInitExpression memberInitExpression)
            {
                var serviceInfo = CreateServiceInfoBasedOnNewExpression(memberInitExpression.NewExpression);
                foreach (MemberBinding memberBinding in memberInitExpression.Bindings)
                {
                    HandleMemberAssignment((MemberAssignment)memberBinding, serviceInfo);
                }

                return serviceInfo;
            }
           
            private static void HandleMemberAssignment(MemberAssignment memberAssignment, ConstructionInfo constructionInfo)
            {
                var propertyDependency = CreatePropertyDependency(memberAssignment);
                ApplyDependencyDetails(memberAssignment.Expression, propertyDependency);
                constructionInfo.PropertyDependencies.Add(propertyDependency);
            }

            private static ConstructorDependency CreateConstructorDependency(ParameterInfo parameterInfo)
            {
                var constructorDependency = new ConstructorDependency
                {
                    Parameter = parameterInfo,
                    ServiceType = parameterInfo.ParameterType
                };
                return constructorDependency;
            }

            private static PropertyDependecy CreatePropertyDependency(MemberAssignment memberAssignment)
            {
                var propertyDependecy = new PropertyDependecy
                {
                    Property = (PropertyInfo)memberAssignment.Member,
                    ServiceType = ((PropertyInfo)memberAssignment.Member).PropertyType
                };
                return propertyDependecy;
            }

            private static void ApplyDependencyDetails(Expression expression, Dependency dependency)
            {                
                if (RepresentsServiceFactoryMethod(expression))
                {
                    ApplyDependencyDetailsFromMethodCall((MethodCallExpression)expression, dependency);
                }
                else
                {
                    ApplyDependecyDetailsFromExpression(expression, dependency);
                }
            }

            private static bool RepresentsServiceFactoryMethod(Expression expression)
            {
                return IsMethodCall(expression) &&
                    IsServiceFactoryMethod(((MethodCallExpression)expression).Method);
            }

            private static bool IsMethodCall(Expression expression)
            {
                return expression.NodeType == ExpressionType.Call;
            }

            private static bool IsServiceFactoryMethod(MethodInfo methodInfo)
            {
                return methodInfo.DeclaringType == typeof(IServiceFactory);
            }

            private static void ApplyDependecyDetailsFromExpression(Expression expression, Dependency dependency)
            {                
                dependency.FactoryExpression = expression;
                dependency.ServiceName = string.Empty;
            }

            private static void ApplyDependencyDetailsFromMethodCall(MethodCallExpression methodCallExpression, Dependency dependency)
            {
                dependency.ServiceType = methodCallExpression.Method.ReturnType;
                if (RepresentsGetNamedInstanceMethod(methodCallExpression))
                {
                    dependency.ServiceName = (string)((ConstantExpression)methodCallExpression.Arguments[0]).Value;
                }
                else
                {
                    dependency.ServiceName = string.Empty;
                }
            }

            private static bool RepresentsGetNamedInstanceMethod(MethodCallExpression node)
            {
                return IsGetInstanceMethod(node.Method) && HasOneArgumentRepresentingServiceName(node);
            }

            private static bool IsGetInstanceMethod(MethodInfo methodInfo)
            {
                return methodInfo.Name == "GetInstance";
            }

            private static bool HasOneArgumentRepresentingServiceName(MethodCallExpression node)
            {
                return HasOneArgument(node) && IsConstantExpression(node.Arguments[0]);
            }

            private static bool HasOneArgument(MethodCallExpression node)
            {
                return node.Arguments.Count == 1;
            }

            private static bool IsConstantExpression(Expression argument)
            {                
                return argument.NodeType == ExpressionType.Constant;
            }
        }
 
        public class LambdaExpressionValidator : ExpressionVisitor
        {
            private bool canParse = true;

            public bool CanParse(LambdaExpression lambdaExpression)
            {
                Visit(lambdaExpression.Body);                
                return canParse;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                canParse = false;
                return base.VisitLambda(node);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Convert)
                {
                    canParse = false;
                }

                return base.VisitUnary(node);
            }
        }

        /// <summary>
        /// Contains information about how to create a service instance.
        /// </summary>
        public class ConstructionInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructionInfo"/> class.
            /// </summary>
            public ConstructionInfo()
            {
                PropertyDependencies = new List<PropertyDependecy>();
                ConstructorDependencies = new List<ConstructorDependency>();
            }

            /// <summary>
            /// Gets or sets the implementing type that represents the concrete class to create.
            /// </summary>
            public Type ImplementingType { get; set; }
            
            /// <summary>
            /// Gets or sets the <see cref="ConstructorInfo"/> that is used to create a service instance.
            /// </summary>
            public ConstructorInfo Constructor { get; set; }

            /// <summary>
            /// Gets a list of <see cref="PropertyDependecy"/> instances that represent 
            /// the property dependencies for the target service instance. 
            /// </summary>
            public List<PropertyDependecy> PropertyDependencies { get; private set; }

            /// <summary>
            /// Gets a list of <see cref="ConstructorDependency"/> instances that represent 
            /// the property dependencies for the target service instance. 
            /// </summary>
            public List<ConstructorDependency> ConstructorDependencies { get; private set; }

            /// <summary>
            /// Gets or sets the function delegate to be used to create the service instance.
            /// </summary>
            public Delegate FactoryDelegate { get; set; }
        }

        /// <summary>
        /// Represents a class dependency.
        /// </summary>
        public abstract class Dependency
        {
            /// <summary>
            /// Gets or sets the service <see cref="Type"/> of the <see cref="Dependency"/>.
            /// </summary>
            public Type ServiceType { get; set; }

            /// <summary>
            /// Gets or sets the service name of the <see cref="Dependency"/>.
            /// </summary>
            public string ServiceName { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="FactoryExpression"/> that represent getting the value of the <see cref="Dependency"/>.
            /// </summary>            
            public Expression FactoryExpression { get; set; }

            /// <summary>
            /// Gets the name of the dependency accessor.
            /// </summary>
            public abstract string Name { get; }

            /// <summary>
            /// Returns textual information about the dependency.
            /// </summary>
            /// <returns>A string that describes the dependency.</returns>
            public override string ToString()
            {
                var sb = new StringBuilder();
                return sb.AppendFormat("[Requested dependency: ServiceType:{0}, ServiceName:{1}]", ServiceType, ServiceName).ToString();                                
            }
        }

        /// <summary>
        /// Represents a property dependency.
        /// </summary>
        public class PropertyDependecy : Dependency
        {
            /// <summary>
            /// Gets or sets the <see cref="MethodInfo"/> that is used to set the property value.
            /// </summary>
            public PropertyInfo Property { get; set; }

            /// <summary>
            /// Gets the name of the dependency accessor.
            /// </summary>
            public override string Name
            {
                get
                {
                    return Property.Name;
                }
            }

            /// <summary>
            /// Returns textual information about the dependency.
            /// </summary>
            /// <returns>A string that describes the dependency.</returns>
            public override string ToString()
            {
                return string.Format("[Target Type: {0}], [Property: {1}({2})]", Property.DeclaringType, Property.Name, Property.PropertyType) + ", " + base.ToString();
            }
        }

        /// <summary>
        /// Represents a constructor dependency.
        /// </summary>
        public class ConstructorDependency : Dependency
        {
            /// <summary>
            /// Gets or sets the <see cref="ParameterInfo"/> for this <see cref="ConstructorDependency"/>.
            /// </summary>
            public ParameterInfo Parameter { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether that this parameter represents  
            /// the decoration target passed into a decorator instance. 
            /// </summary>
            public bool IsDecoratorTarget { get; set; }

            /// <summary>
            /// Gets the name of the dependency accessor.
            /// </summary>
            public override string Name
            {
                get
                {
                    return Parameter.Name;
                }
            }

            /// <summary>
            /// Returns textual information about the dependency.
            /// </summary>
            /// <returns>A string that describes the dependency.</returns>
            public override string ToString()
            {
                return string.Format("[Target Type: {0}], [Parameter: {1}({2})]", Parameter.Member.DeclaringType, Parameter.Name, Parameter.ParameterType) + ", " + base.ToString();
            }
        }

        private class Storage<T>
        {
            private readonly object lockObject = new object();
            private T[] items = new T[0];
                              
            public T[] Items
            {
                get
                {
                    return items;
                }
            }

            public int Add(T value)
            {
                int index = Array.IndexOf(items, value);
                if (index == -1)
                {
                    return TryAddValue(value);
                }

                return index;
            }

            private int TryAddValue(T value)
            {
                lock (lockObject)
                {
                    int index = Array.IndexOf(items, value);
                    if (index == -1)
                    {
                        index = AddValue(value);
                    }

                    return index;
                }
            }

            private int AddValue(T value)
            {
                int index = items.Length;
                T[] snapshot = CreateSnapshot();
                snapshot[index] = value;
                items = snapshot;
                return index;
            }

            private T[] CreateSnapshot()
            {
                var snapshot = new T[items.Length + 1];
                Array.Copy(items, snapshot, items.Length);
                return snapshot;
            }
        }

        private class KeyValueStorage<TKey, TValue>
        {
            private readonly object lockObject = new object();
            private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

            public bool TryGetValue(TKey key, out TValue value)
            {
                return dictionary.TryGetValue(key, out value);                
            }

            public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
            {
                TValue value;
                if (!dictionary.TryGetValue(key, out value))
                {
                    lock (lockObject)
                    {
                        value = TryAddValue(key, valueFactory);
                    }
                }

               return value;
            }

            private TValue TryAddValue(TKey key, Func<TKey, TValue> valueFactory)
            {
                TValue value;
                if (!dictionary.TryGetValue(key, out value))
                {
                    var snapshot = new Dictionary<TKey, TValue>(dictionary);
                    value = valueFactory(key);
                    snapshot.Add(key, value);
                    dictionary = snapshot;
                }

                return value;
            }
        }

        private class DynamicMethodInfo
        {
            private readonly IDictionary<ServiceInfo, LocalBuilder> localVariables = new Dictionary<ServiceInfo, LocalBuilder>();

            private DynamicMethod dynamicMethod;

            public DynamicMethodInfo()
            {
                CreateDynamicMethod();
            }

            public ILGenerator GetILGenerator()
            {
                return dynamicMethod.GetILGenerator();
            }

            public Func<object[], object> CreateDelegate()
            {
                dynamicMethod.GetILGenerator().Emit(OpCodes.Ret);
                return (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));
            }

            private void CreateDynamicMethod()
            {
#if NET                
                dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(object[]) }, typeof(ServiceContainer).Module, false);
#endif
#if SILVERLIGHT
                dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(List<object>) });
#endif
            }
        }

        private class ServiceRegistry<T> : ThreadSafeDictionary<Type, ThreadSafeDictionary<string, T>>
        {
        }

        private class DelegateRegistry<TKey> : KeyValueStorage<TKey, Func<object>>
        {
        }
    }

#if NET

    internal class ThreadSafeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public ThreadSafeDictionary()
        {
        }

        public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }
    }
#endif
#if SILVERLIGHT
        
        internal class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            private readonly Dictionary<TKey, TValue> dictionary;
            private readonly object syncObject = new object();

            public ThreadSafeDictionary()
            {
                dictionary = new Dictionary<TKey, TValue>();
            }            

            public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
            {
                dictionary = new Dictionary<TKey, TValue>(comparer);
            }

            public int Count
            {
                get { return dictionary.Count; }
            }

            public ICollection<TValue> Values
            {
                get
                {
                    lock (syncObject)
                    {
                        return dictionary.Values;
                    }
                }
            }

            public ICollection<TKey> Keys
            {
                get
                {
                    lock (syncObject)
                    {
                        return dictionary.Keys;
                    }
                }
            }

            public TValue GetOrAdd(TKey key, Func<TKey, TValue> valuefactory)
            {
                lock (syncObject)
                {
                    TValue value;
                    if (!dictionary.TryGetValue(key, out value))
                    {
                        value = valuefactory(key);
                        dictionary.Add(key, value);
                    }

                    return value;
                }
            }    
        
            public void AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
            {
                lock (syncObject)
                {
                    TValue value;
                    if (!dictionary.TryGetValue(key, out value))
                    {
                        dictionary.Add(key, addValueFactory(key));
                    }
                    else
                    {
                        dictionary[key] = updateValueFactory(key, value);
                    }
                }
            }

            public void TryGetValue(TKey key, out TValue value)
            {
                lock (syncObject)
                {
                    dictionary.TryGetValue(key, out value);
                }
            }

            public bool TryRemove(TKey key, out TValue value)
            {
                lock (syncObject)
                {                    
                    if (dictionary.TryGetValue(key, out value))
                    {                 
                        return true;
                    }
                    else
                    {
                        value = default(TValue);
                        return false;
                    }
                }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                lock (syncObject)
                {
                    return dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }        
#endif

    internal class Registration
    {
        /// <summary>
        /// Gets or sets the service <see cref="Type"/>.
        /// </summary>
        public Type ServiceType { get; internal set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> that implements the <see cref="Registration.ServiceType"/>.
        /// </summary>
        public Type ImplementingType { get; internal set; }

        /// <summary>
        /// Gets or sets the <see cref="LambdaExpression"/> used to create an instance of the decorator.
        /// </summary>
        public LambdaExpression FactoryExpression { get; set; }
    }
    
    internal class DecoratorRegistration : Registration
    {       
        public Func<ServiceInfo, bool> CanDecorate { get; set; }
    }

    /// <summary>
    /// Contains information about a registered service.
    /// </summary>
    internal class ServiceInfo : Registration
    {                                
     
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string ServiceName { get; internal set; }
         
        /// <summary>
        /// Gets or sets the <see cref="ILifetime"/> instance that controls the lifetime of the service.
        /// </summary>
        public ILifetime Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the value that represents the instance of the service.
        /// </summary>
        public object Value { get; set; }

        public bool IsDecorator { get; set; }

        public override int GetHashCode()
        {
            return ServiceType.GetHashCode() ^ ServiceName.GetHashCode() ^ IsDecorator.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            var other = obj as ServiceInfo;
            if (other == null)
            {
                return false;
            }

            var result = ServiceName == other.ServiceName && ServiceType == other.ServiceType && other.IsDecorator == IsDecorator;
            return result;                
        }            
    }

    internal class SingletonLifetime : ILifetime, IDisposable
    {
        private readonly object syncRoot = new object();
        private object singleton;

        public object GetInstance(Func<object> createInstance, ResolutionContext resolutionContext)
        {
            if (singleton != null)
            {
                return singleton;
            }

            lock (syncRoot)
            {
                if (singleton == null)
                {
                    singleton = createInstance();
                }
            }

            return singleton;
        }

        public void Dispose()
        {
            var disposable = singleton as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }

    internal class PerGraphLifetime : ILifetime
    {
        private readonly ThreadSafeDictionary<ResolutionContext, object> instances = new ThreadSafeDictionary<ResolutionContext, object>();
       
        public object GetInstance(Func<object> createInstance, ResolutionContext resolutionContext)
        {            
            if (resolutionContext == null)
            {
                return createInstance();
            }

            return instances.GetOrAdd(resolutionContext, s => CreateScopedInstance(s, createInstance));
        }

        private static void RegisterForDisposal(ResolutionContext context, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                context.RegisterForDisposal(disposable);
            }
        }

        private object CreateScopedInstance(ResolutionContext context, Func<object> createInstance)
        {
            context.Completed += OnContextCompleted;
            var instance = createInstance();
            RegisterForDisposal(context, instance);
            return instance;
        }
       
        private void OnContextCompleted(object sender, EventArgs e)
        {
            object removedInstance;
            instances.TryRemove((ResolutionContext)sender, out removedInstance);
        }      
    }

    internal enum ScopeOption
    {
        Required,
        RequiresNew,
        Suppress
    }

    internal class ScopeHolder
    {
        internal ResolutionScope Value { get; set; }

        internal bool IsRootScope { get; set; }
    }


    internal class ScopeManager
    {
        private ResolutionScope currentScope { get; set; }


        public ResolutionContext Current
        {
            get
            {
                return currentScope.Context;
            }
        }

        public ScopeManager()
        {
            CreateSuppressScope();
        }

        public ResolutionScope BeginScope(ScopeOption scopeOption)
        {                        
            if (scopeOption == ScopeOption.Suppress)
            {
                CreateSuppressScope();
            }

            if (scopeOption == ScopeOption.Required)
            {
                CreateRequiredScope();
            }

            if (scopeOption == ScopeOption.RequiresNew)
            {
                CreateRequiresNewScope();
            }
                        
            return currentScope;
        }

        private void CreateRequiresNewScope()
        {            
            currentScope = new ResolutionScope(this, currentScope, new ResolutionContext());
        }

        private void CreateRequiredScope()
        {
            if (currentScope.Context == null)
            {
                CreateRequiresNewScope();
            }
            else
            {
                currentScope = new ResolutionScope(this, currentScope, currentScope.Context);
            }
        }

        private void CreateSuppressScope()
        {
            currentScope = new ResolutionScope(this, currentScope, null);
        }

        public void EndScope(ResolutionScope scope)
        {
            if (scope.Context != null)
            {
                scope.Context.SetComplete();
            }

            currentScope = scope.ParentScope;
        }

    }

    internal class ResolutionScope : IDisposable
    {
        private readonly ScopeManager scopeManager;

        private readonly ResolutionScope parentScope;

        public ResolutionScope(ScopeManager scopeManager, ResolutionScope parentScope, ResolutionContext resolutionContext)
        {
            this.scopeManager = scopeManager;
            this.parentScope = parentScope;
            Context = resolutionContext;
            
        }

        public ResolutionScope ParentScope
        {
            get
            {
                return parentScope;
            }
        }

        public ResolutionContext Context { get; private set; }
        
        public void Dispose()
        {
            if (parentScope.Context == null)
            {
                scopeManager.EndScope(this);
            }
        }
    }

    /// <summary>
    /// Represents a context in which services are resolved.
    /// </summary>
    internal interface IResolutionContext
    {
        /// <summary>
        /// Fired when the current <see cref="IResolutionContext"/> is completed.
        /// </summary>
        event EventHandler<EventArgs> Completed;

        /// <summary>
        /// 
        /// </summary>
        void SetComplete();

        void RegisterForDisposal(IDisposable disposable);
    }

    internal class ResolutionContext : IResolutionContext
    {
        private readonly IList<IDisposable> disposableObjects = new List<IDisposable>();
        
        public event EventHandler<EventArgs> Completed;

        public void SetComplete()
        {
            foreach (var disposableObject in disposableObjects)
            {
                disposableObject.Dispose();
            }

            disposableObjects.Clear();
            OnCompleted();
        }

        public void RegisterForDisposal(IDisposable disposable)
        {
            disposableObjects.Add(disposable);
        }

        private void OnCompleted()
        {
            var completedHandler = Completed;
            if (completedHandler != null)
            {
                completedHandler(this, new EventArgs());
            }
        }
        
    }

    /// <summary>
    /// An assembly scanner that registers services based on the types contained within an <see cref="Assembly"/>.
    /// </summary>    
    internal class AssemblyScanner : IAssemblyScanner
    {
        private static readonly List<Type> InternalInterfaces = new List<Type>();
        private static readonly List<Type> InternalTypes = new List<Type>();
        private Assembly currentAssembly;

        static AssemblyScanner()
        {
            InternalInterfaces.Add(typeof(IServiceContainer));
            InternalInterfaces.Add(typeof(IServiceFactory));
            InternalInterfaces.Add(typeof(IServiceRegistry));
            InternalInterfaces.Add(typeof(IPropertySelector));
            InternalInterfaces.Add(typeof(IAssemblyLoader));
            InternalInterfaces.Add(typeof(IAssemblyScanner));
            InternalInterfaces.Add(typeof(ILifetime));
            InternalInterfaces.Add(typeof(IResolutionContext));
            InternalTypes.Add(typeof(ServiceContainer.LambdaExpressionParser));
            InternalTypes.Add(typeof(ServiceContainer.LambdaExpressionValidator));
            InternalTypes.Add(typeof(ServiceContainer.ConstructorDependency));
            InternalTypes.Add(typeof(ServiceContainer.PropertyDependecy));
            InternalTypes.Add(typeof(ThreadSafeDictionary<,>));
            InternalTypes.Add(typeof(ResolutionScope));
            InternalTypes.Add(typeof(SingletonLifetime));
            InternalTypes.Add(typeof(PerGraphLifetime));
            InternalTypes.Add(typeof(ResolutionScope));
            InternalTypes.Add(typeof(ScopeManager));    
            InternalTypes.Add(typeof(ServiceInfo));
            InternalTypes.Add(typeof(DecoratorRegistration));
        }

        /// <summary>
        /// Scans the target <paramref name="assembly"/> and registers services found within the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="lifetimeFactory">The <see cref="ILifetime"/> factory that controls the lifetime of the registered service.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetimeFactory, Func<Type, bool> shouldRegister)
        {            
            IEnumerable<Type> concreteTypes = GetConcreteTypes(assembly).ToList();
            var compositionRoots = concreteTypes.Where(t => typeof(ICompositionRoot).IsAssignableFrom(t)).ToList();
            if (compositionRoots.Count > 0 && currentAssembly != assembly)
            {
                currentAssembly = assembly;
                ExecuteCompositionRoots(compositionRoots, serviceRegistry);
            }
            else
            {
                foreach (Type type in concreteTypes.Where(shouldRegister))
                {
                    BuildImplementationMap(type, serviceRegistry, lifetimeFactory);
                }
            }
        }

        private static void ExecuteCompositionRoots(IEnumerable<Type> compositionRoots, IServiceRegistry serviceRegistry)
        {
            foreach (var compositionRoot in compositionRoots)
            {
                ((ICompositionRoot)Activator.CreateInstance(compositionRoot)).Compose(serviceRegistry);
            }
        }

        private static string GetServiceName(Type serviceType, Type implementingType)
        {
            string implementingTypeName = implementingType.Name;
            string serviceTypeName = serviceType.Name;
            if (implementingType.IsGenericTypeDefinition)
            {
                var regex = new Regex("((?:[a-z][a-z]+))", RegexOptions.IgnoreCase);
                implementingTypeName = regex.Match(implementingTypeName).Groups[1].Value;
                serviceTypeName = regex.Match(serviceTypeName).Groups[1].Value;
            }

            if (serviceTypeName.Substring(1) == implementingTypeName)
            {
                implementingTypeName = string.Empty;
            }

            return implementingTypeName;
        }

        private static IEnumerable<Type> GetBaseTypes(Type concreteType)
        {
            Type baseType = concreteType.BaseType;
            while (baseType != typeof(object) && baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        private static IEnumerable<Type> GetConcreteTypes(Assembly assembly)
        {            
            return assembly.GetTypes().Where(t => t.IsClass && !t.IsNestedPrivate && !t.IsAbstract && !(t.Namespace ?? string.Empty).StartsWith("System") && !IsCompilerGenerated(t) && InternalTypes.All(it => it != t));
        }

        private static bool IsCompilerGenerated(Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false);
        }

        private void BuildImplementationMap(Type implementingType, IServiceRegistry serviceRegistry, Func<ILifetime> lifetimeFactory)
        {
            Type[] interfaces = implementingType.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                if (InternalInterfaces.All(i => i != interfaceType))
                {
                    RegisterInternal(interfaceType, implementingType, serviceRegistry, lifetimeFactory());
                }
            }

            foreach (Type baseType in GetBaseTypes(implementingType))
            {
                RegisterInternal(baseType, implementingType, serviceRegistry, lifetimeFactory());
            }
        }

        private void RegisterInternal(Type serviceType, Type implementingType, IServiceRegistry serviceRegistry, ILifetime lifetime)
        {
            if (serviceType.IsGenericType && serviceType.ContainsGenericParameters)
            {
                serviceType = serviceType.GetGenericTypeDefinition();
            }

            serviceRegistry.Register(serviceType, implementingType, GetServiceName(serviceType, implementingType), lifetime);
        }
    }

    /// <summary>
    /// Selects the properties that represents a dependency to the target <see cref="Type"/>.
    /// </summary>
    internal class PropertySelector : IPropertySelector
    {
        /// <summary>
        /// Selects properties that represents a dependency from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the properties.</param>
        /// <returns>A list of properties that represents a dependency to the target <paramref name="type"/></returns>
        public IEnumerable<PropertyInfo> Select(Type type)
        {
            return type.GetProperties().Where(IsInjectable).ToList();
        }

        /// <summary>
        /// Determines if the <paramref name="propertyInfo"/> represents an injectable property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> that describes the target property.</param>
        /// <returns><b>true</b> if the property is injectable, otherwise <b>false</b>.</returns>
        protected virtual bool IsInjectable(PropertyInfo propertyInfo)
        {
            return !IsReadOnly(propertyInfo);
        }

        private static bool IsReadOnly(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod(false) == null || propertyInfo.GetSetMethod(false).IsStatic;
        }
    }
#if NET
    
    /// <summary>
    /// Loads all assemblies from the application base directory that matches the given search pattern.
    /// </summary>
    internal class AssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Loads a set of assemblies based on the given <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use.</param>
        /// <returns>A list of assemblies based on the given <paramref name="searchPattern"/>.</returns>
        public IEnumerable<Assembly> Load(string searchPattern)
        {
            string directory = Path.GetDirectoryName(new Uri(typeof(ServiceContainer).Assembly.CodeBase).LocalPath);
            if (directory != null)
            {
                string[] searchPatterns = searchPattern.Split('|');
                foreach (string file in searchPatterns.SelectMany(sp => Directory.GetFiles(directory, sp)).Where(CanLoad))
                {
                    yield return Assembly.LoadFrom(file);
                }
            }
        }

        /// <summary>
        /// Indicates if the current <paramref name="fileName"/> represent a file that can be loaded.
        /// </summary>
        /// <param name="fileName">The name of the target file.</param>
        /// <returns><b>true</b> if the file can be loaded, otherwise <b>false</b>.</returns>
        protected virtual bool CanLoad(string fileName)
        {
            return true;
        }
    }
#endif
}