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
   LightInject version 3.0.0.9 
   http://www.lightinject.net/
   http://twitter.com/bernhardrichter
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Defines a set of methods used to register services into the service container.
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// Gets a list of <see cref="ServiceRegistration"/> instances that represents the 
        /// registered services.          
        /// </summary>
        IEnumerable<ServiceRegistration> AvailableServices { get; }

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
        /// Registers a concrete type as a service.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        void Register<TService>();

        /// <summary>
        /// Registers a concrete type as a service.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register<TService>(ILifetime lifetime);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        void Register<TService>(TService instance, string serviceName);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        void Register<TService>(Expression<Func<IServiceFactory, TService>> factory);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        void Register<T, TService>(Expression<Func<IServiceFactory, T, TService>> factory);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param> 
        /// <param name="serviceName">The name of the service.</param>        
        void Register<T, TService>(Expression<Func<IServiceFactory, T, TService>> factory, string serviceName);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        void Register<T1, T2, TService>(Expression<Func<IServiceFactory, T1, T2, TService>> factory);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        /// <param name="serviceName">The name of the service.</param>
        void Register<T1, T2, TService>(Expression<Func<IServiceFactory, T1, T2, TService>> factory, string serviceName);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        void Register<T1, T2, T3, TService>(Expression<Func<IServiceFactory, T1, T2, T3, TService>> factory);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        /// <param name="serviceName">The name of the service.</param>
        void Register<T1, T2, T3, TService>(Expression<Func<IServiceFactory, T1, T2, T3, TService>> factory, string serviceName);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        void Register<T1, T2, T3, T4, TService>(Expression<Func<IServiceFactory, T1, T2, T3, T4, TService>> factory);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        /// <param name="serviceName">The name of the service.</param>
        void Register<T1, T2, T3, T4, TService>(Expression<Func<IServiceFactory, T1, T2, T3, T4, TService>> factory, string serviceName);

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

        /// <summary>
        /// Registers a custom factory delegate used to create services that is otherwise unknown to the service container.
        /// </summary>
        /// <param name="predicate">Determines if the service can be created by the <paramref name="factory"/> delegate.</param>
        /// <param name="factory">Creates a service instance according to the <paramref name="predicate"/> predicate.</param>
        void Register(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory);

        /// <summary>
        /// Registers a custom factory delegate used to create services that is otherwise unknown to the service container.
        /// </summary>
        /// <param name="predicate">Determines if the service can be created by the <paramref name="factory"/> delegate.</param>
        /// <param name="factory">Creates a service instance according to the <paramref name="predicate"/> predicate.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime);

        /// <summary>
        /// Registers a service based on a <see cref="ServiceRegistration"/> instance.
        /// </summary>
        /// <param name="serviceRegistration">The <see cref="ServiceRegistration"/> instance that contains service metadata.</param>
        void Register(ServiceRegistration serviceRegistration);
        
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
        void RegisterAssembly(Assembly assembly, Func<Type, Type, bool> shouldRegister);

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
        void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister);

        /// <summary>
        /// Decorates the <paramref name="serviceType"/> with the given <paramref name="decoratorType"/>.
        /// </summary>
        /// <param name="serviceType">The target service type.</param>
        /// <param name="decoratorType">The decorator type used to decorate the <paramref name="serviceType"/>.</param>
        /// <param name="predicate">A function delegate that determines if the <paramref name="decoratorType"/>
        /// should be applied to the target <paramref name="serviceType"/>.</param>
        void Decorate(Type serviceType, Type decoratorType, Func<ServiceRegistration, bool> predicate);

        /// <summary>
        /// Decorates the <paramref name="serviceType"/> with the given <paramref name="decoratorType"/>.
        /// </summary>
        /// <param name="serviceType">The target service type.</param>
        /// <param name="decoratorType">The decorator type used to decorate the <paramref name="serviceType"/>.</param>        
        void Decorate(Type serviceType, Type decoratorType);

        /// <summary>
        /// Decorates the <typeparamref name="TService"/> with the given <typeparamref name="TDecorator"/>.
        /// </summary>
        /// <typeparam name="TService">The target service type.</typeparam>
        /// <typeparam name="TDecorator">The decorator type used to decorate the <typeparamref name="TService"/>.</typeparam>
        void Decorate<TService, TDecorator>() where TDecorator : TService;

        /// <summary>
        /// Decorates the <typeparamref name="TService"/> using the given decorator <paramref name="factory"/>.
        /// </summary>
        /// <typeparam name="TService">The target service type.</typeparam>
        /// <param name="factory">A factory delegate used to create a decorator instance.</param>
        void Decorate<TService>(Expression<Func<IServiceFactory, TService, TService>> factory);

        /// <summary>
        /// Registers a decorator based on a <see cref="DecoratorRegistration"/> instance.
        /// </summary>
        /// <param name="decoratorRegistration">The <see cref="DecoratorRegistration"/> instance that contains the decorator metadata.</param>
        void Decorate(DecoratorRegistration decoratorRegistration);
    }

    /// <summary>
    /// Defines a set of methods used to retrieve service instances.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType);

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="arguments">The arguments to be passed to the target instance.</param>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType, object[] arguments);

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <param name="arguments">The arguments to be passed to the target instance.</param>        
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType, string serviceName, object[] arguments);

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
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="value">The argument value.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T, TService>(T value);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="value">The argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T, TService>(T value, string serviceName);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2, string serviceName);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T1, T2, T3, TService>(T1 arg1, T2 arg2, T3 arg3);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T1, T2, T3, TService>(T1 arg1, T2 arg2, T3 arg3, string serviceName);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <param name="arg4">The fourth argument value.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T1, T2, T3, T4, TService>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <param name="arg4">The fourth argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        TService GetInstance<T1, T2, T3, T4, TService>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, string serviceName);

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise null.</returns>
        object TryGetInstance(Type serviceType);

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise null.</returns>
        object TryGetInstance(Type serviceType, string serviceName);

        /// <summary>
        /// Tries to get an instance of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <returns>The requested service instance if available, otherwise default(T).</returns>
        TService TryGetInstance<TService>();

        /// <summary>
        /// Tries to get an instance of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise default(T).</returns>
        TService TryGetInstance<TService>(string serviceName);

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

    /// <summary>
    /// Represents an inversion of control container.
    /// </summary>
    public interface IServiceContainer : IServiceRegistry, IServiceFactory, IDisposable
    {
        /// <summary>
        /// Returns <b>true</b> if the container can create the requested service, otherwise <b>false</b>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the service.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <returns><b>true</b> if the container can create the requested service, otherwise <b>false</b>.</returns>
        bool CanGetInstance(Type serviceType, string serviceName);

        /// <summary>
        /// Starts a new <see cref="Scope"/>.
        /// </summary>
        /// <returns><see cref="Scope"/></returns>
        Scope BeginScope();

        /// <summary>
        /// Injects the property dependencies for a given <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The target instance for which to inject its property dependencies.</param>
        /// <returns>The <paramref name="instance"/> with its property dependencies injected.</returns>
        object InjectProperties(object instance);
    }

    /// <summary>
    /// Represents a class that manages the lifetime of a service instance.
    /// </summary>
    public interface ILifetime
    {
        /// <summary>
        /// Returns a service instance according to the specific lifetime characteristics.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <param name="scope">The <see cref="Scope"/> of the current service request.</param>
        /// <returns>The requested services instance.</returns>
        object GetInstance(Func<object> createInstance, Scope scope);
    }

    /// <summary>
    /// Represents a class that acts as a composition root for an <see cref="IServiceRegistry"/> instance.
    /// </summary>
    public interface ICompositionRoot
    {
        /// <summary>
        /// Composes services by adding services to the <paramref name="serviceRegistry"/>.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        void Compose(IServiceRegistry serviceRegistry);
    }

    /// <summary>
    /// Represents a class that extracts a set of types from an <see cref="Assembly"/>.
    /// </summary>
    public interface ITypeExtractor
    {
        /// <summary>
        /// Extracts types found in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to extract types.</param>
        /// <returns>A set of types found in the given <paramref name="assembly"/>.</returns>
        IEnumerable<Type> Execute(Assembly assembly);
    }

    /// <summary>
    /// Represents a class that is responsible for selecting injectable properties.
    /// </summary>
    public interface IPropertySelector
    {
        /// <summary>
        /// Selects properties that represents a dependency from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the properties.</param>
        /// <returns>A list of injectable properties.</returns>
        IEnumerable<PropertyInfo> Execute(Type type);
    }

    /// <summary>
    /// Represents a class that is responsible for selecting the property dependencies for a given <see cref="Type"/>.
    /// </summary>
    public interface IPropertyDependencySelector
    {
        /// <summary>
        /// Selects the property dependencies for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the property dependencies.</param>
        /// <returns>A list of <see cref="PropertyDependency"/> instances that represents the property
        /// dependencies for the given <paramref name="type"/>.</returns>
        IEnumerable<PropertyDependency> Execute(Type type);
    }

    /// <summary>
    /// Represents a class that is responsible for selecting the constructor dependencies for a given <see cref="ConstructorInfo"/>.
    /// </summary>
    public interface IConstructorDependencySelector
    {
        /// <summary>
        /// Selects the constructor dependencies for the given <paramref name="constructor"/>.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructionInfo"/> for which to select the constructor dependencies.</param>
        /// <returns>A list of <see cref="ConstructorDependency"/> instances that represents the constructor
        /// dependencies for the given <paramref name="constructor"/>.</returns>
        IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor);
    }

    /// <summary>
    /// Represents a class that is capable of building a <see cref="ConstructorInfo"/> instance 
    /// based on a <see cref="Registration"/>.
    /// </summary>
    public interface IConstructionInfoBuilder
    {
        /// <summary>
        /// Returns a <see cref="ConstructionInfo"/> instance based on the given <see cref="Registration"/>.
        /// </summary>
        /// <param name="registration">The <see cref="Registration"/> for which to return a <see cref="ConstructionInfo"/> instance.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance that describes how to create a service instance.</returns>
        ConstructionInfo Execute(Registration registration);
    }

    /// <summary>
    /// Represents a class that keeps track of a <see cref="ConstructionInfo"/> instance for each <see cref="Registration"/>.
    /// </summary>
    public interface IConstructionInfoProvider
    {
        /// <summary>
        /// Gets a <see cref="ConstructionInfo"/> instance for the given <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">The <see cref="Registration"/> for which to get a <see cref="ConstructionInfo"/> instance.</param>
        /// <returns>The <see cref="ConstructionInfo"/> instance that describes how to create an instance of the given <paramref name="registration"/>.</returns>
        ConstructionInfo GetConstructionInfo(Registration registration);

        /// <summary>
        /// Invalidates the <see cref="IConstructionInfoProvider"/> and causes new <see cref="ConstructionInfo"/> instances 
        /// to be created when the <see cref="GetConstructionInfo"/> method is called.
        /// </summary>
        void Invalidate();
    }

    /// <summary>
    /// Represents a class that builds a <see cref="ConstructionInfo"/> instance based on a <see cref="LambdaExpression"/>.
    /// </summary>
    public interface ILambdaConstructionInfoBuilder
    {
        /// <summary>
        /// Parses the <paramref name="lambdaExpression"/> and returns a <see cref="ConstructionInfo"/> instance.
        /// </summary>
        /// <param name="lambdaExpression">The <see cref="LambdaExpression"/> to parse.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance.</returns>
        ConstructionInfo Execute(LambdaExpression lambdaExpression);
    }

    /// <summary>
    /// Represents a class that builds a <see cref="ConstructionInfo"/> instance based on the implementing <see cref="Type"/>.
    /// </summary>
    public interface ITypeConstructionInfoBuilder
    {
        /// <summary>
        /// Analyzes the <paramref name="implementingType"/> and returns a <see cref="ConstructionInfo"/> instance.
        /// </summary>
        /// <param name="implementingType">The <see cref="Type"/> to analyze.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance.</returns>
        ConstructionInfo Execute(Type implementingType);
    }

    /// <summary>
    /// Represents a class that selects the constructor to be used for creating a new service instance. 
    /// </summary>
    public interface IConstructorSelector
    {
        /// <summary>
        /// Selects the constructor to be used when creating a new instance of the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="implementingType">The <see cref="Type"/> for which to return a <see cref="ConstructionInfo"/>.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance that represents the constructor to be used
        /// when creating a new instance of the <paramref name="implementingType"/>.</returns>
        ConstructorInfo Execute(Type implementingType);
    }

    /// <summary>
    /// Represents a class that is responsible loading a set of assemblies based on the given search pattern.
    /// </summary>
    public interface IAssemblyLoader
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
    public interface IAssemblyScanner
    {
        /// <summary>
        /// Scans the target <paramref name="assembly"/> and registers services found within the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister);
    }

    /// <summary>
    /// Represents a dynamic method skeleton for emitting the code needed to resolve a service instance.
    /// </summary>
    public interface IMethodSkeleton
    {
        /// <summary>
        /// Gets the <see cref="ILGenerator"/> for the this dynamic method.
        /// </summary>
        /// <returns>The <see cref="ILGenerator"/> for the this dynamic method.</returns>
        ILGenerator GetILGenerator();

        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it.
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method.</param>
        /// <returns>A delegate of the specified type, which can be used to execute the dynamic method.</returns>
        Delegate CreateDelegate(Type delegateType);

        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it, 
        /// specifying the delegate type and an object the delegate is bound to.
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method, minus the first parameter.</param>
        /// <param name="target">An object the delegate is bound to. Must be of the same type as the first parameter of the dynamic method.</param>
        /// <returns>A delegate of the specified type, which can be used to execute the dynamic method with the specified target object.</returns>
        Delegate CreateDelegate(Type delegateType, object target);
    }

    internal static class ReflectionHelper
    {
        private static readonly Lazy<MethodInfo> LifetimeGetInstanceMethodInfo;
        private static readonly Lazy<MethodInfo> OpenGenericGetInstanceMethodInfo;
        private static readonly Lazy<MethodInfo> OpenGenericGetNamedInstanceMethodInfo;        
        private static readonly Lazy<MethodInfo[]> OpenGenericGetInstanceMethods;
        private static readonly Lazy<MethodInfo> GetCurrentScopeMethodInfo;
        private static readonly Lazy<MethodInfo> GetCurrentScopeManagerMethodInfo;
        private static readonly Lazy<ThreadSafeDictionary<Type, Type>> LazyTypes;
        private static readonly Lazy<ThreadSafeDictionary<Type, Type>> FuncTypes;        
        private static readonly Lazy<ThreadSafeDictionary<Type, ConstructorInfo>> LazyConstructors;
        private static readonly Lazy<ThreadSafeDictionary<Type, ConstructorInfo>> LazyCollectionConstructors;
        private static readonly Lazy<ThreadSafeDictionary<Type, Type[]>> GenericArgumentTypes;

        private static readonly Lazy<ThreadSafeDictionary<Type, MethodInfo>> GetInstanceMethods;
        private static readonly Lazy<ThreadSafeDictionary<Type, MethodInfo>> GetNamedInstanceMethods;

        private static readonly Lazy<ThreadSafeDictionary<Type, MethodInfo>>
            GetInstanceWithParametersMethods;

        private static readonly Lazy<ThreadSafeDictionary<Type, MethodInfo>>
           NamedGetInstanceWithParametersMethods;

        private static readonly Lazy<ThreadSafeDictionary<Type, Type>> EnumerableTypes;

        static ReflectionHelper()
        {
            LifetimeGetInstanceMethodInfo = new Lazy<MethodInfo>(() => typeof(ILifetime).GetMethod("GetInstance"));
            OpenGenericGetInstanceMethods = new Lazy<MethodInfo[]>(
                () => typeof(IServiceFactory).GetMethods().Where(TypeHelper.IsGenericGetInstanceMethod).ToArray());
            OpenGenericGetInstanceMethodInfo = new Lazy<MethodInfo>(
                () => OpenGenericGetInstanceMethods.Value.FirstOrDefault(m => !m.GetParameters().Any()));
            OpenGenericGetNamedInstanceMethodInfo = new Lazy<MethodInfo>(
                () => OpenGenericGetInstanceMethods.Value.FirstOrDefault(m => m.GetParameters().Any()));
            GetCurrentScopeMethodInfo = new Lazy<MethodInfo>(
                () => typeof(ScopeManager).GetProperty("CurrentScope").GetGetMethod());
            GetCurrentScopeManagerMethodInfo = new Lazy<MethodInfo>(
                () => typeof(ThreadLocal<ScopeManager>).GetProperty("Value").GetGetMethod());
            LazyTypes = new Lazy<ThreadSafeDictionary<Type, Type>>(() => new ThreadSafeDictionary<Type, Type>());
            FuncTypes = new Lazy<ThreadSafeDictionary<Type, Type>>(() => new ThreadSafeDictionary<Type, Type>());            
            LazyConstructors = new Lazy<ThreadSafeDictionary<Type, ConstructorInfo>>(() => new ThreadSafeDictionary<Type, ConstructorInfo>());
            LazyCollectionConstructors = new Lazy<ThreadSafeDictionary<Type, ConstructorInfo>>(() => new ThreadSafeDictionary<Type, ConstructorInfo>());
            GetInstanceMethods = new Lazy<ThreadSafeDictionary<Type, MethodInfo>>(() => new ThreadSafeDictionary<Type, MethodInfo>());
            GetNamedInstanceMethods = new Lazy<ThreadSafeDictionary<Type, MethodInfo>>(() => new ThreadSafeDictionary<Type, MethodInfo>());

            GenericArgumentTypes = new Lazy<ThreadSafeDictionary<Type, Type[]>>();

            EnumerableTypes = new Lazy<ThreadSafeDictionary<Type, Type>>(() => new ThreadSafeDictionary<Type, Type>());
            
            GetInstanceWithParametersMethods =
                new Lazy<ThreadSafeDictionary<Type, MethodInfo>>(
                    () => new ThreadSafeDictionary<Type, MethodInfo>());
            NamedGetInstanceWithParametersMethods = new Lazy<ThreadSafeDictionary<Type, MethodInfo>>(() => new ThreadSafeDictionary<Type, MethodInfo>());
        }

        public static MethodInfo LifetimeGetInstanceMethod
        {
            get
            {
                return LifetimeGetInstanceMethodInfo.Value;
            }
        }

        public static MethodInfo GetCurrentScopeMethod
        {
            get
            {
                return GetCurrentScopeMethodInfo.Value;
            }
        }

        public static MethodInfo GetCurrentScopeManagerMethod
        {
            get
            {
                return GetCurrentScopeManagerMethodInfo.Value;
            }
        }
       
        public static Delegate CreateGetInstanceDelegate(Type serviceType, IServiceFactory serviceFactory)
        {
            Type delegateType = GetFuncType(serviceType);
            MethodInfo getInstanceMethod = GetGetInstanceMethod(serviceType);
            return getInstanceMethod.CreateDelegate(delegateType, serviceFactory);
        }
       
        public static MethodInfo GetGetInstanceWithParametersMethod(Type serviceType)
        {
            return GetInstanceWithParametersMethods.Value.GetOrAdd(serviceType, CreateGetInstanceWithParametersMethod);            
        }

        public static MethodInfo GetNamedGetInstanceWithParametersMethod(Type serviceType)
        {
            return NamedGetInstanceWithParametersMethods.Value.GetOrAdd(serviceType, CreateNamedGetInstanceWithParametersMethod);
        }

        public static Type[] GetGenericArguments(Type type)
        {
            return GenericArgumentTypes.Value.GetOrAdd(type, ResolveGenericArguments);
        }

        public static Type GetEnumerableType(Type type)
        {
            return EnumerableTypes.Value.GetOrAdd(type, CreateEnumerableType);
        }

        public static Type GetFuncType(Type type)
        {
            return FuncTypes.Value.GetOrAdd(type, CreateFuncType);
        }
 
        public static Type GetLazyType(Type type)
        {
            return LazyTypes.Value.GetOrAdd(type, CreateLazyType);
        }

        public static ConstructorInfo GetLazyConstructor(Type type)
        {
            return LazyConstructors.Value.GetOrAdd(type, ResolveLazyConstructor);
        }

        public static ConstructorInfo GetLazyCollectionConstructor(Type elementType)
        {
            return LazyCollectionConstructors.Value.GetOrAdd(elementType, ResolveLazyCollectionConstructor);
        }

        public static MethodInfo GetGetInstanceMethod(Type type)
        {
            return GetInstanceMethods.Value.GetOrAdd(type, CreateClosedGenericGetInstanceMethod);
        }

        public static MethodInfo GetGetNamedInstanceMethod(Type type)
        {
            return GetNamedInstanceMethods.Value.GetOrAdd(type, CreateClosedGenericGetNamedInstanceMethod);
        }

        private static MethodInfo CreateGetInstanceWithParametersMethod(Type serviceType)
        {
            Type[] genericArguments = serviceType.GetGenericArguments();
            MethodInfo openGenericMethod =
                typeof(IServiceFactory).GetMethods().Single(m => m.Name == "GetInstance"
                    && m.GetGenericArguments().Length == genericArguments.Length && m.GetParameters().All(p => p.Name != "serviceName"));

            MethodInfo closedGenericMethod = openGenericMethod.MakeGenericMethod(genericArguments);

            return closedGenericMethod;
        }

        private static MethodInfo CreateNamedGetInstanceWithParametersMethod(Type serviceType)
        {
            Type[] genericArguments = serviceType.GetGenericArguments();
            MethodInfo openGenericMethod =
                typeof(IServiceFactory).GetMethods().Single(m => m.Name == "GetInstance"
                    && m.GetGenericArguments().Length == genericArguments.Length && m.GetParameters().Any(p => p.Name == "serviceName"));

            MethodInfo closedGenericMethod = openGenericMethod.MakeGenericMethod(genericArguments);

            return closedGenericMethod;
        }

        private static Type CreateLazyType(Type type)
        {
            return typeof(Lazy<>).MakeGenericType(type);
        }

        private static Type CreateEnumerableType(Type type)
        {
            return typeof(IEnumerable<>).MakeGenericType(type);
        }

        private static Type[] ResolveGenericArguments(Type type)
        {
            return type.GetGenericArguments();
        }

        private static ConstructorInfo ResolveLazyConstructor(Type type)
        {
            return GetLazyType(type).GetConstructor(new[] { GetFuncType(type) });
        }

        private static ConstructorInfo ResolveLazyCollectionConstructor(Type elementType)
        {
            Type lazyType = GetLazyType(GetEnumerableType(elementType));
            var collectionType = typeof(LazyReadOnlyCollection<>).MakeGenericType(elementType);
            return collectionType.GetConstructor(new[] { lazyType, typeof(int) });
        }

        private static Type CreateFuncType(Type type)
        {
            return typeof(Func<>).MakeGenericType(type);
        }
               
        private static MethodInfo CreateClosedGenericGetInstanceMethod(Type type)
        {
            return OpenGenericGetInstanceMethodInfo.Value.MakeGenericMethod(type);
        }
     
        private static MethodInfo CreateClosedGenericGetNamedInstanceMethod(Type type)
        {
            return OpenGenericGetNamedInstanceMethodInfo.Value.MakeGenericMethod(type);
        }
    }

    internal static class TypeHelper
    {
        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

        public static bool IsClass(this Type type)
        {
            return type.IsClass;
        }

        public static bool IsAbstract(this Type type)
        {
            return type.IsAbstract;
        }

        public static bool IsNestedPrivate(this Type type)
        {
            return type.IsNestedPrivate;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static bool ContainsGenericParameters(this Type type)
        {
            return type.ContainsGenericParameters;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.BaseType;
        }

        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.IsGenericTypeDefinition;
        }
   
        public static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }        

        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            return del.Method;
        }

        public static MethodInfo GetPrivateMethod(this Type type, string name)
        {            
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static bool IsEnumerableOfT(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsLazy(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        public static bool IsFunc(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public static bool IsFuncWithParameters(this Type serviceType)
        {
            if (!serviceType.IsGenericType())
            {
                return false;
            }

            Type genericTypeDefinition = serviceType.GetGenericTypeDefinition();

            return genericTypeDefinition == typeof(Func<,>) || genericTypeDefinition == typeof(Func<,,>)
                || genericTypeDefinition == typeof(Func<,,,>) || genericTypeDefinition == typeof(Func<,,,,>);
        }

        public static bool IsClosedGeneric(this Type serviceType)
        {
            return serviceType.IsGenericType() && !serviceType.IsGenericTypeDefinition();
        }

        public static bool IsGenericGetInstanceMethod(MethodInfo m)
        {
            return m.Name == "GetInstance" && m.IsGenericMethodDefinition;
        }
    }

    /// <summary>
    /// An ultra lightweight service container.
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private const string UnresolvedDependencyError = "Unresolved dependency {0}";
        private readonly Func<Type, Type[], IMethodSkeleton> methodSkeletonFactory;
        private readonly ServiceRegistry<Action<IMethodSkeleton>> emitters = new ServiceRegistry<Action<IMethodSkeleton>>();        
        private readonly DelegateRegistry<Type> delegates = new DelegateRegistry<Type>();
        private readonly DelegateRegistry<Tuple<Type, string>> namedDelegates = new DelegateRegistry<Tuple<Type, string>>();
        private readonly DelegateRegistry<Type> propertyInjectionDelegates = new DelegateRegistry<Type>();
        private readonly Storage<object> constants = new Storage<object>();
        private readonly Storage<FactoryRule> factoryRules = new Storage<FactoryRule>();
        private readonly Stack<Action<IMethodSkeleton>> dependencyStack = new Stack<Action<IMethodSkeleton>>();
        
        private readonly ServiceRegistry<ServiceRegistration> availableServices = new ServiceRegistry<ServiceRegistration>();

        private readonly Storage<DecoratorRegistration> decorators = new Storage<DecoratorRegistration>();
        private readonly ThreadLocal<ScopeManager> scopeManagers =
            new ThreadLocal<ScopeManager>(() => new ScopeManager());

        private readonly Lazy<IConstructionInfoProvider> constructionInfoProvider;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        public ServiceContainer()
        {
            AssemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor());
            PropertyDependencySelector = new PropertyDependencySelector(new PropertySelector());
            ConstructorDependencySelector = new ConstructorDependencySelector();
            constructionInfoProvider = new Lazy<IConstructionInfoProvider>(CreateConstructionInfoProvider);
            methodSkeletonFactory = (returnType, parameterTypes) => new DynamicMethodSkeleton(returnType, parameterTypes);
        }

        /// <summary>
        /// Gets or sets the <see cref="IPropertyDependencySelector"/> instance that 
        /// is responsible for selecting the property dependencies for a given type.
        /// </summary>
        public IPropertyDependencySelector PropertyDependencySelector { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IConstructorDependencySelector"/> instance that 
        /// is responsible for selecting the constructor dependencies for a given constructor.
        /// </summary>
        public IConstructorDependencySelector ConstructorDependencySelector { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyScanner"/> instance that is responsible for scanning assemblies.
        /// </summary>
        public IAssemblyScanner AssemblyScanner { get; set; }

        /// <summary>
        /// Gets a list of <see cref="ServiceRegistration"/> instances that represents the registered services.           
        /// </summary>                  
        public IEnumerable<ServiceRegistration> AvailableServices
        {
            get
            {
                return availableServices.Values.SelectMany(t => t.Values);                
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

        /// <summary>
        /// Starts a new <see cref="Scope"/>.
        /// </summary>
        /// <returns><see cref="Scope"/></returns>
        public Scope BeginScope()
        {
            return scopeManagers.Value.BeginScope();
        }

        /// <summary>
        /// Injects the property dependencies for a given <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The target instance for which to inject its property dependencies.</param>
        /// <returns>The <paramref name="instance"/> with its property dependencies injected.</returns>
        public object InjectProperties(object instance)
        {
            var type = instance.GetType();
            return propertyInjectionDelegates.GetOrAdd(type, t => CreatePropertyInjectionDelegate(type))(new[] { instance });
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
            RegisterServiceFromLambdaExpression<TService>(expression, lifetime, serviceName);
        }

        /// <summary>
        /// Registers a custom factory delegate used to create services that is otherwise unknown to the service container.
        /// </summary>
        /// <param name="predicate">Determines if the service can be created by the <paramref name="factory"/> delegate.</param>
        /// <param name="factory">Creates a service instance according to the <paramref name="predicate"/> predicate.</param>
        public void Register(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory)
        {
            factoryRules.Add(new FactoryRule { CanCreateInstance = predicate, Factory = factory });
        }

        /// <summary>
        /// Registers a custom factory delegate used to create services that is otherwise unknown to the service container.
        /// </summary>
        /// <param name="predicate">Determines if the service can be created by the <paramref name="factory"/> delegate.</param>
        /// <param name="factory">Creates a service instance according to the <paramref name="predicate"/> predicate.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime)
        {
            factoryRules.Add(new FactoryRule { CanCreateInstance = predicate, Factory = factory, LifeTime = lifetime });
        }

        /// <summary>
        /// Registers a service based on a <see cref="ServiceRegistration"/> instance.
        /// </summary>
        /// <param name="serviceRegistration">The <see cref="ServiceRegistration"/> instance that contains service metadata.</param>
        public void Register(ServiceRegistration serviceRegistration)
        {
            var services = GetAvailableServices(serviceRegistration.ServiceType);            
            var sr = serviceRegistration;
            services.AddOrUpdate(
                serviceRegistration.ServiceName,
                s => AddServiceRegistration(sr),
                (k, existing) => UpdateServiceRegistration(existing, sr));            
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
            RegisterAssembly(assembly, (serviceType, implementingType) => true);
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
        public void RegisterAssembly(Assembly assembly, Func<Type, Type, bool> shouldRegister)
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
            AssemblyScanner.Scan(assembly, this, lifetimeFactory, (serviceType, implementingType) => true);
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
        public void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            AssemblyScanner.Scan(assembly, this, lifetimeFactory, shouldRegister);
        }

        /// <summary>
        /// Decorates the <paramref name="serviceType"/> with the given <paramref name="decoratorType"/>.
        /// </summary>
        /// <param name="serviceType">The target service type.</param>
        /// <param name="decoratorType">The decorator type used to decorate the <paramref name="serviceType"/>.</param>
        /// <param name="predicate">A function delegate that determines if the <paramref name="decoratorType"/>
        /// should be applied to the target <paramref name="serviceType"/>.</param>
        public void Decorate(Type serviceType, Type decoratorType, Func<ServiceRegistration, bool> predicate)
        {
            var decoratorRegistration = new DecoratorRegistration { ServiceType = serviceType, ImplementingType = decoratorType, CanDecorate = predicate };
            Decorate(decoratorRegistration);
        }

        /// <summary>
        /// Decorates the <paramref name="serviceType"/> with the given <paramref name="decoratorType"/>.
        /// </summary>
        /// <param name="serviceType">The target service type.</param>
        /// <param name="decoratorType">The decorator type used to decorate the <paramref name="serviceType"/>.</param>        
        public void Decorate(Type serviceType, Type decoratorType)
        {
            Decorate(serviceType, decoratorType, si => true);
        }

        /// <summary>
        /// Decorates the <typeparamref name="TService"/> with the given <typeparamref name="TDecorator"/>.
        /// </summary>
        /// <typeparam name="TService">The target service type.</typeparam>
        /// <typeparam name="TDecorator">The decorator type used to decorate the <typeparamref name="TService"/>.</typeparam>
        public void Decorate<TService, TDecorator>() where TDecorator : TService
        {
            Decorate(typeof(TService), typeof(TDecorator));
        }

        /// <summary>
        /// Decorates the <typeparamref name="TService"/> using the given decorator <paramref name="factory"/>.
        /// </summary>
        /// <typeparam name="TService">The target service type.</typeparam>
        /// <param name="factory">A factory delegate used to create a decorator instance.</param>
        public void Decorate<TService>(Expression<Func<IServiceFactory, TService, TService>> factory)
        {
            var decoratorRegistration = new DecoratorRegistration { FactoryExpression = factory, ServiceType = typeof(TService), CanDecorate = si => true };
            Decorate(decoratorRegistration);            
        }

        /// <summary>
        /// Registers a decorator based on a <see cref="DecoratorRegistration"/> instance.
        /// </summary>
        /// <param name="decoratorRegistration">The <see cref="DecoratorRegistration"/> instance that contains the decorator metadata.</param>
        public void Decorate(DecoratorRegistration decoratorRegistration)
        {
            int index = decorators.Add(decoratorRegistration);
            decoratorRegistration.Index = index;            
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
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="factory">The lambdaExpression that describes the dependencies of the service.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory, ILifetime lifetime)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, lifetime, string.Empty);
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
            RegisterServiceFromLambdaExpression<TService>(factory, null, serviceName);
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
        /// Registers a concrete type as a service.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        public void Register<TService>()
        {
            Register<TService, TService>();
        }

        /// <summary>
        /// Registers a concrete type as a service.
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register<TService>(ILifetime lifetime)
        {
            Register<TService, TService>(lifetime);
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
        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        public void Register<T, TService>(Expression<Func<IServiceFactory, T, TService>> factory)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param> 
        /// <param name="serviceName">The name of the service.</param>        
        public void Register<T, TService>(Expression<Func<IServiceFactory, T, TService>> factory, string serviceName)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, serviceName);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        public void Register<T1, T2, TService>(Expression<Func<IServiceFactory, T1, T2, TService>> factory)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        /// <param name="serviceName">The name of the service.</param>
        public void Register<T1, T2, TService>(Expression<Func<IServiceFactory, T1, T2, TService>> factory, string serviceName)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, serviceName);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        public void Register<T1, T2, T3, TService>(Expression<Func<IServiceFactory, T1, T2, T3, TService>> factory)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        /// <param name="serviceName">The name of the service.</param>
        public void Register<T1, T2, T3, TService>(Expression<Func<IServiceFactory, T1, T2, T3, TService>> factory, string serviceName)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, serviceName);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        public void Register<T1, T2, T3, T4, TService>(Expression<Func<IServiceFactory, T1, T2, T3, T4, TService>> factory)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, string.Empty);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="factory"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The service type to register.</typeparam>        
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TService"/> instance.</param>    
        /// <param name="serviceName">The name of the service.</param>
        public void Register<T1, T2, T3, T4, TService>(Expression<Func<IServiceFactory, T1, T2, T3, T4, TService>> factory, string serviceName)
        {
            RegisterServiceFromLambdaExpression<TService>(factory, null, serviceName);
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
            return GetDefaultDelegate(serviceType, true)(constants.Items);
        }

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="arguments">The arguments to be passed to the target instance.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, object[] arguments)
        {
            var del = GetDefaultDelegate(serviceType, true);
             
            object[] constantsWithArguments = constants.Items.Concat(new object[] { arguments }).ToArray();

            return del(constantsWithArguments);
        }

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <param name="arguments">The arguments to be passed to the target instance.</param>        
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, string serviceName, object[] arguments)
        {
            var del = GetNamedDelegate(serviceType, serviceName, true);

            object[] constantsWithArguments = constants.Items.Concat(new object[] { arguments }).ToArray();

            return del(constantsWithArguments);
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
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="value">The argument value.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T, TService>(T value)
        {
            return (TService)GetInstance(typeof(TService), new object[] { value });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="value">The argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T, TService>(T value, string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName, new object[] { value });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2)
        {
            return (TService)GetInstance(typeof(TService), new object[] { arg1, arg2 });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2, string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName, new object[] { arg1, arg2 });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T1, T2, T3, TService>(T1 arg1, T2 arg2, T3 arg3)
        {
            return (TService)GetInstance(typeof(TService), new object[] { arg1, arg2, arg3 });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T1, T2, T3, TService>(T1 arg1, T2 arg2, T3 arg3, string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName, new object[] { arg1, arg2, arg3 });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <param name="arg4">The fourth argument value.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T1, T2, T3, T4, TService>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (TService)GetInstance(typeof(TService), new object[] { arg1, arg2, arg3, arg4 });
        }

        /// <summary>
        /// Gets an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="TService">The type of the requested service.</typeparam>        
        /// <param name="arg1">The first argument value.</param>
        /// <param name="arg2">The second argument value.</param>
        /// <param name="arg3">The third argument value.</param>
        /// <param name="arg4">The fourth argument value.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<T1, T2, T3, T4, TService>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName, new object[] { arg1, arg2, arg3, arg4 });
        }

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise null.</returns>
        public object TryGetInstance(Type serviceType)
        {
            return GetDefaultDelegate(serviceType, false)(constants.Items);
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise null.</returns>
        public object TryGetInstance(Type serviceType, string serviceName)
        {
            return GetNamedDelegate(serviceType, serviceName, false)(constants.Items);
        }

        /// <summary>
        /// Tries to get an instance of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <returns>The requested service instance if available, otherwise default(T).</returns>
        public TService TryGetInstance<TService>()
        {
            return (TService)TryGetInstance(typeof(TService));
        }

        /// <summary>
        /// Tries to get an instance of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise default(T).</returns>
        public TService TryGetInstance<TService>(string serviceName)
        {
            return (TService)TryGetInstance(typeof(TService), serviceName);
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, string serviceName)
        {
            return GetNamedDelegate(serviceType, serviceName, true)(constants.Items);
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>A list that contains all implementations of the <paramref name="serviceType"/>.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (IEnumerable<object>)GetInstance(ReflectionHelper.GetEnumerableType(serviceType));
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

        /// <summary>
        /// Disposes any services registered using the <see cref="PerContainerLifetime"/>.
        /// </summary>
        public void Dispose()
        {
            var disposableLifetimeInstances = availableServices.Values.SelectMany(t => t.Values)
                .Where(sr => sr.Lifetime != null)
                .Select(sr => sr.Lifetime)
                .Where(lt => lt is IDisposable).Cast<IDisposable>();
            foreach (var disposableLifetimeInstance in disposableLifetimeInstances)
            {
                disposableLifetimeInstance.Dispose();
            }

            scopeManagers.Dispose();
        }

        private static void EmitLoadConstant(IMethodSkeleton dynamicMethodSkeleton, int index, Type type)
        {
            var generator = dynamicMethodSkeleton.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, index);
            generator.Emit(OpCodes.Ldelem_Ref);
            generator.Emit(type.IsValueType() ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        private static void EmitNewArray(IList<Action<IMethodSkeleton>> serviceEmitters, Type elementType, IMethodSkeleton dynamicMethodSkeleton)
        {
            ILGenerator generator = dynamicMethodSkeleton.GetILGenerator();
            LocalBuilder array = generator.DeclareLocal(elementType.MakeArrayType());
            generator.Emit(OpCodes.Ldc_I4, serviceEmitters.Count);
            generator.Emit(OpCodes.Newarr, elementType);
            generator.Emit(OpCodes.Stloc, array);

            for (int index = 0; index < serviceEmitters.Count; index++)
            {
                generator.Emit(OpCodes.Ldloc, array);
                generator.Emit(OpCodes.Ldc_I4, index);
                var serviceEmitter = serviceEmitters[index];
                serviceEmitter(dynamicMethodSkeleton);
                generator.Emit(OpCodes.Stelem, elementType);
            }

            generator.Emit(OpCodes.Ldloc, array);
        }
        
        private static ILifetime CloneLifeTime(ILifetime lifetime)
        {
            return lifetime == null ? null : (ILifetime)Activator.CreateInstance(lifetime.GetType());
        }

        private static ConstructorDependency GetConstructorDependencyThatRepresentsDecoratorTarget(
            DecoratorRegistration decoratorRegistration, ConstructionInfo constructionInfo)
        {
            var constructorDependency =
                constructionInfo.ConstructorDependencies.FirstOrDefault(
                    cd =>
                    cd.ServiceType == decoratorRegistration.ServiceType
                    || (cd.ServiceType.IsLazy()
                        && ReflectionHelper.GetGenericArguments(cd.ServiceType)[0] == decoratorRegistration.ServiceType));
            return constructorDependency;
        }

        private static DecoratorRegistration CreateClosedGenericDecoratorRegistration(
            ServiceRegistration serviceRegistration, DecoratorRegistration openGenericDecorator)
        {
            var closedGenericDecoratorType =
                openGenericDecorator.ImplementingType.MakeGenericType(
                    ReflectionHelper.GetGenericArguments(serviceRegistration.ServiceType));
            var decoratorInfo = new DecoratorRegistration
            {
                ServiceType = serviceRegistration.ServiceType,
                ImplementingType = closedGenericDecoratorType,
                CanDecorate = openGenericDecorator.CanDecorate,
                Index = openGenericDecorator.Index
            };
            return decoratorInfo;
        }

        private void EmitEnumerable(IList<Action<IMethodSkeleton>> serviceEmitters, Type elementType, IMethodSkeleton dynamicMethodSkeleton)
        {
            Type enumerableType = ReflectionHelper.GetEnumerableType(elementType);
            var factoryDelegate = CreateTypedInstanceDelegate(skeleton => EmitNewArray(serviceEmitters, elementType, skeleton), enumerableType);

            ConstructorInfo lazyConstructor = ReflectionHelper.GetLazyConstructor(enumerableType);
            ConstructorInfo lazyCollectionConstructor = ReflectionHelper.GetLazyCollectionConstructor(elementType);

            var constantIndex = constants.Add(factoryDelegate);
            EmitLoadConstant(dynamicMethodSkeleton, constantIndex, ReflectionHelper.GetFuncType(enumerableType));
            dynamicMethodSkeleton.GetILGenerator().Emit(OpCodes.Newobj, lazyConstructor);
            dynamicMethodSkeleton.GetILGenerator().Emit(OpCodes.Ldc_I4, serviceEmitters.Count);
            dynamicMethodSkeleton.GetILGenerator().Emit(OpCodes.Newobj, lazyCollectionConstructor);
        }

        private Func<object[], object> CreatePropertyInjectionDelegate(Type concreteType)
        {
            IMethodSkeleton methodSkeleton = methodSkeletonFactory(typeof(object), new[] { typeof(object[]) });
            ConstructionInfo constructionInfo = GetContructionInfoForConcreteType(concreteType);
            EmitLoadConstant(methodSkeleton, 0, concreteType);
            try
            {
                EmitPropertyDependencies(constructionInfo, methodSkeleton);
            }
            catch (Exception)
            {
                dependencyStack.Clear();
                throw;
            }

            return (Func<object[], object>)methodSkeleton.CreateDelegate(typeof(Func<object[], object>));                        
        }

        private ConstructionInfo GetContructionInfoForConcreteType(Type concreteType)
        {
            var serviceRegistration = GetServiceRegistrationForConcreteType(concreteType);
            return GetConstructionInfo(serviceRegistration);
        }

        private ServiceRegistration GetServiceRegistrationForConcreteType(Type concreteType)
        {
            var services = GetAvailableServices(concreteType);            
            return services.GetOrAdd(string.Empty, s => CreateServiceRegistrationBasedOnConcreteType(concreteType));            
        }

        private ServiceRegistration CreateServiceRegistrationBasedOnConcreteType(Type type)
        {
            var serviceRegistration = new ServiceRegistration
            {
                ServiceType = type,
                ImplementingType = type,
                ServiceName = string.Empty
            };
            return serviceRegistration;
        }

        private ConstructionInfoProvider CreateConstructionInfoProvider()
        {
            return new ConstructionInfoProvider(CreateConstructionInfoBuilder());
        }

        private ConstructionInfoBuilder CreateConstructionInfoBuilder()
        {
            return new ConstructionInfoBuilder(() => new LambdaConstructionInfoBuilder(), CreateTypeConstructionInfoBuilder);
        }

        private TypeConstructionInfoBuilder CreateTypeConstructionInfoBuilder()
        {
            return new TypeConstructionInfoBuilder(new ConstructorSelector(), ConstructorDependencySelector, PropertyDependencySelector);
        }

        private Func<object[], object> GetDefaultDelegate(Type serviceType, bool throwError)
        {
            Func<object[], object> del;

            if (!delegates.TryGetValue(serviceType, out del))
            {
                del = delegates.GetOrAdd(serviceType, t => CreateDelegate(t, string.Empty, throwError));
            }

            return del;
        }

        private Func<object[], object> GetNamedDelegate(Type serviceType, string serviceName, bool throwError)
        {
            Func<object[], object> del;

            if (!namedDelegates.TryGetValue(Tuple.Create(serviceType, serviceName), out del))
            {
                del = namedDelegates.GetOrAdd(
                    Tuple.Create(serviceType, serviceName), t => CreateDelegate(t.Item1, serviceName, throwError));
            }

            return del;
        }

        private Func<object[], object> CreateDynamicMethodDelegate(Action<IMethodSkeleton> serviceEmitter, Type serviceType)
        {
            var methodSkeleton = methodSkeletonFactory(typeof(object), new[] { typeof(object[]) });
            serviceEmitter(methodSkeleton);
            if (serviceType.IsValueType())
            {
                methodSkeleton.GetILGenerator().Emit(OpCodes.Box, serviceType);
            }

            return (Func<object[], object>)methodSkeleton.CreateDelegate(typeof(Func<object[], object>));                                    
        }

        private Func<object> WrapAsFuncDelegate(Func<object[], object> instanceDelegate)
        {
            return () => instanceDelegate(constants.Items);
        }
       
        private Action<IMethodSkeleton> GetEmitMethod(Type serviceType, string serviceName)
        {           
            Action<IMethodSkeleton> emitter = GetRegisteredEmitMethod(serviceType, serviceName);

            if (emitter == null)
            {
                if (!HasProcessedTypesFromThisAssembly(serviceType.GetAssembly()))
                {
                    RegisterAssembly(serviceType.GetAssembly());
                    emitter = GetRegisteredEmitMethod(serviceType, serviceName);
                }
            }
        
            return CreateEmitMethodWrapper(emitter, serviceType, serviceName);
        }

        private bool HasProcessedTypesFromThisAssembly(Assembly assembly)
        {
            return availableServices.Keys.Select(TypeHelper.GetAssembly).Any(a => Equals(a, assembly));
        }

        private Action<IMethodSkeleton> CreateEmitMethodWrapper(Action<IMethodSkeleton> emitter, Type serviceType, string serviceName)
        {
            if (emitter == null)
            {
                return null;
            }

            return ms =>
            {
                if (dependencyStack.Contains(emitter))
                {
                    throw new InvalidOperationException(
                        string.Format("Recursive dependency detected: ServiceType:{0}, ServiceName:{1}]", serviceType, serviceName));
                }

                dependencyStack.Push(emitter);
                emitter(ms);
                dependencyStack.Pop();
            };
        }

        private Action<IMethodSkeleton> GetRegisteredEmitMethod(Type serviceType, string serviceName)
        {
            Action<IMethodSkeleton> emitter;
            var registrations = GetServiceEmitters(serviceType);
            registrations.TryGetValue(serviceName, out emitter);
            return emitter ?? ResolveUnknownServiceEmitter(serviceType, serviceName);
        }

        private void UpdateServiceEmitter(Type serviceType, string serviceName, Action<IMethodSkeleton> emitter)
        {
            if (emitter != null)
            {
                GetServiceEmitters(serviceType).AddOrUpdate(serviceName, s => emitter, (s, m) => emitter);
            }
        }
        
        private ServiceRegistration AddServiceRegistration(ServiceRegistration serviceRegistration)
        {
            var emitDelegate = ResolveEmitDelegate(serviceRegistration);
            GetServiceEmitters(serviceRegistration.ServiceType).TryAdd(serviceRegistration.ServiceName, emitDelegate);                
            return serviceRegistration;
        }

        private ServiceRegistration UpdateServiceRegistration(ServiceRegistration existingRegistration, ServiceRegistration newRegistration)
        {
            if (existingRegistration.IsReadOnly)
            {
                return existingRegistration;
            }

            Invalidate();
            Action<IMethodSkeleton> emitDelegate = ResolveEmitDelegate(newRegistration);            
            
            var serviceEmitters = GetServiceEmitters(newRegistration.ServiceType);
            serviceEmitters[newRegistration.ServiceName] = emitDelegate;                                               
            return newRegistration;
        }

        private void EmitNewInstance(ServiceRegistration serviceRegistration, IMethodSkeleton dynamicMethodSkeleton)
        {
            var serviceDecorators = GetDecorators(serviceRegistration);
            if (serviceDecorators.Length > 0)
            {
                EmitDecorators(serviceRegistration, serviceDecorators, dynamicMethodSkeleton, dm => DoEmitNewInstance(serviceRegistration, dm));
            }
            else
            {
                DoEmitNewInstance(serviceRegistration, dynamicMethodSkeleton);
            }
        }

        private DecoratorRegistration[] GetDecorators(ServiceRegistration serviceRegistration)
        {
            var registeredDecorators = decorators.Items.Where(d => d.ServiceType == serviceRegistration.ServiceType).ToList();            
            
            registeredDecorators.AddRange(GetOpenGenericDecoratorRegistrations(serviceRegistration));            
            return registeredDecorators.OrderBy(d => d.Index).ToArray();
        }

        private IEnumerable<DecoratorRegistration> GetOpenGenericDecoratorRegistrations(
            ServiceRegistration serviceRegistration)
        {
            var registrations = new List<DecoratorRegistration>();
            if (serviceRegistration.ServiceType.IsGenericType())
            {
                var openGenericServiceType = serviceRegistration.ServiceType.GetGenericTypeDefinition();
                var openGenericDecorators = decorators.Items.Where(d => d.ServiceType == openGenericServiceType);
                registrations.AddRange(
                    openGenericDecorators.Select(
                        openGenericDecorator =>
                        CreateClosedGenericDecoratorRegistration(serviceRegistration, openGenericDecorator)));
            }

            return registrations;
        }

        private void DoEmitDecoratorInstance(DecoratorRegistration decoratorRegistration, IMethodSkeleton dynamicMethodSkeleton, Action<IMethodSkeleton> pushInstance)
        {
            ConstructionInfo constructionInfo = GetConstructionInfo(decoratorRegistration);
            var constructorDependency = GetConstructorDependencyThatRepresentsDecoratorTarget(
                decoratorRegistration, constructionInfo);
                
            if (constructorDependency != null)
            {
                constructorDependency.IsDecoratorTarget = true;
            }

            if (constructionInfo.FactoryDelegate != null)
            {
                EmitNewDecoratorUsingFactoryDelegate(constructionInfo.FactoryDelegate, dynamicMethodSkeleton, pushInstance);
            }
            else
            {
                EmitNewInstanceUsingImplementingType(dynamicMethodSkeleton, constructionInfo, pushInstance);
            }
        }

        private void EmitNewDecoratorUsingFactoryDelegate(Delegate factoryDelegate, IMethodSkeleton dynamicMethodSkeleton, Action<IMethodSkeleton> pushInstance)
        {
            var factoryDelegateIndex = constants.Add(factoryDelegate);
            var serviceFactoryIndex = constants.Add(this);
            Type funcType = factoryDelegate.GetType();
            EmitLoadConstant(dynamicMethodSkeleton, factoryDelegateIndex, funcType);
            EmitLoadConstant(dynamicMethodSkeleton, serviceFactoryIndex, typeof(IServiceFactory));
            pushInstance(dynamicMethodSkeleton);
            ILGenerator generator = dynamicMethodSkeleton.GetILGenerator();
            MethodInfo invokeMethod = funcType.GetMethod("Invoke");
            generator.Emit(OpCodes.Callvirt, invokeMethod);
        }

        private void DoEmitNewInstance(ServiceRegistration serviceRegistration, IMethodSkeleton dynamicMethodSkeleton)
        {
            if (serviceRegistration.Value != null)
            {
                int index = constants.Add(serviceRegistration.Value);
                Type serviceType = serviceRegistration.ServiceType;
                EmitLoadConstant(dynamicMethodSkeleton, index, serviceType);                
            }
            else
            {
                var constructionInfo = GetConstructionInfo(serviceRegistration);

                if (constructionInfo.FactoryDelegate != null)
                {
                    EmitNewInstanceUsingFactoryDelegate(constructionInfo.FactoryDelegate, dynamicMethodSkeleton);
                }
                else
                {
                    EmitNewInstanceUsingImplementingType(dynamicMethodSkeleton, constructionInfo, null);
                }    
            }                        
        }

        private void EmitDecorators(ServiceRegistration serviceRegistration, IEnumerable<DecoratorRegistration> serviceDecorators, IMethodSkeleton dynamicMethodSkeleton, Action<IMethodSkeleton> decoratorTargetEmitter)
        {
            foreach (DecoratorRegistration decorator in serviceDecorators)
            {
                if (!decorator.CanDecorate(serviceRegistration))
                {
                    continue;
                }
                
                Action<IMethodSkeleton> currentDecoratorTargetEmitter = decoratorTargetEmitter;
                DecoratorRegistration currentDecorator = decorator;                
                decoratorTargetEmitter = dm => DoEmitDecoratorInstance(currentDecorator, dm, currentDecoratorTargetEmitter);
            }

            decoratorTargetEmitter(dynamicMethodSkeleton);
        }

        private void EmitNewInstanceUsingImplementingType(IMethodSkeleton dynamicMethodSkeleton, ConstructionInfo constructionInfo, Action<IMethodSkeleton> decoratorTargetEmitter)
        {
            ILGenerator generator = dynamicMethodSkeleton.GetILGenerator();
            EmitConstructorDependencies(constructionInfo, dynamicMethodSkeleton, decoratorTargetEmitter);
            generator.Emit(OpCodes.Newobj, constructionInfo.Constructor);
            EmitPropertyDependencies(constructionInfo, dynamicMethodSkeleton);
        }

        private void EmitNewInstanceUsingFactoryDelegate(Delegate factoryDelegate, IMethodSkeleton dynamicMethodSkeleton)
        {                        
            var factoryDelegateIndex = constants.Add(factoryDelegate);
            var serviceFactoryIndex = constants.Add(this);
            Type funcType = factoryDelegate.GetType();
            ILGenerator generator = dynamicMethodSkeleton.GetILGenerator();
            EmitLoadConstant(dynamicMethodSkeleton, factoryDelegateIndex, funcType);            
            EmitLoadConstant(dynamicMethodSkeleton, serviceFactoryIndex, typeof(IServiceFactory));            
            if (factoryDelegate.GetMethodInfo().GetParameters().Length > 2)
            {
                var parameters = factoryDelegate.GetMethodInfo().GetParameters().Skip(2).ToArray();
                PushArguments(generator, parameters);                
            }
                                   
            MethodInfo invokeMethod = funcType.GetMethod("Invoke");            
            generator.Emit(OpCodes.Callvirt, invokeMethod);
        }

        private void PushArguments(ILGenerator generator, ParameterInfo[] parameters)
        {
            var argumentArray = generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldlen);
            generator.Emit(OpCodes.Conv_I4);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Sub);
            generator.Emit(OpCodes.Ldelem_Ref);
            generator.Emit(OpCodes.Castclass, typeof(object[]));
            generator.Emit(OpCodes.Stloc, argumentArray);

            for (int i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldloc, argumentArray);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldelem_Ref);
                generator.Emit(                
                    parameters[i].ParameterType.IsValueType() ? OpCodes.Unbox_Any : OpCodes.Castclass,
                    parameters[i].ParameterType);
            }
        }

        private void EmitConstructorDependencies(ConstructionInfo constructionInfo, IMethodSkeleton dynamicMethodSkeleton, Action<IMethodSkeleton> decoratorTargetEmitter)
        {
            foreach (ConstructorDependency dependency in constructionInfo.ConstructorDependencies)
            {
                if (!dependency.IsDecoratorTarget)
                {
                    EmitDependency(dynamicMethodSkeleton, dependency);
                }
                else
                {                    
                    if (dependency.ServiceType.IsLazy())
                    {
                        Action<IMethodSkeleton> instanceEmitter = decoratorTargetEmitter;                        
                        decoratorTargetEmitter = CreateServiceEmitterBasedOnLazyServiceRequest(
                            dependency.ServiceType, t => CreateTypedInstanceDelegate(instanceEmitter, t));
                    }

                    decoratorTargetEmitter(dynamicMethodSkeleton);
                }
            }
        }

        private Delegate CreateTypedInstanceDelegate(Action<IMethodSkeleton> serviceEmitter, Type serviceType)
        {                        
            var openGenericMethod = GetType().GetPrivateMethod("CreateGenericDynamicMethodDelegate");
            var closedGenericMethod = openGenericMethod.MakeGenericMethod(serviceType);
            var del = WrapAsFuncDelegate(CreateDynamicMethodDelegate(serviceEmitter, serviceType));
            return (Delegate)closedGenericMethod.Invoke(this, new object[] { del });
        }

        // ReSharper disable UnusedMember.Local
        private Func<T> CreateGenericDynamicMethodDelegate<T>(Func<object> del)
        // ReSharper restore UnusedMember.Local
        {            
            return () => (T)del();
        }
        
        private void EmitDependency(IMethodSkeleton dynamicMethodSkeleton, Dependency dependency)
        {
            var emitter = GetEmitMethodForDependency(dependency);

            try
            {
                emitter(dynamicMethodSkeleton);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(string.Format(UnresolvedDependencyError, dependency), ex);
            }
        }

        private void EmitPropertyDependency(IMethodSkeleton dynamicMethodSkeleton, PropertyDependency propertyDependency, LocalBuilder instance)
        {
            var emitter = GetEmitMethodForDependency(propertyDependency);
            var generator = dynamicMethodSkeleton.GetILGenerator();

            if (emitter != null)
            {
                generator.Emit(OpCodes.Ldloc, instance);
                emitter(dynamicMethodSkeleton);
                dynamicMethodSkeleton.GetILGenerator().Emit(OpCodes.Callvirt, propertyDependency.Property.GetSetMethod());
            }
        }

        private Action<IMethodSkeleton> GetEmitMethodForDependency(Dependency dependency)
        {
            if (dependency.FactoryExpression != null)
            {
                return skeleton => EmitDependencyUsingFactoryExpression(skeleton, dependency);
            }

            Action<IMethodSkeleton> emitter = GetEmitMethod(dependency.ServiceType, dependency.ServiceName);
            if (emitter == null)
            {
                emitter = GetEmitMethod(dependency.ServiceType, dependency.Name);
                if (emitter == null && dependency.IsRequired)
                {
                    throw new InvalidOperationException(string.Format(UnresolvedDependencyError, dependency));
                }
            }

            return emitter;
        }

        private void EmitDependencyUsingFactoryExpression(IMethodSkeleton dynamicMethodSkeleton, Dependency dependency)
        {
            var generator = dynamicMethodSkeleton.GetILGenerator();
            var lambda = Expression.Lambda(dependency.FactoryExpression, new ParameterExpression[] { }).Compile();
            MethodInfo methodInfo = lambda.GetType().GetMethod("Invoke");
            EmitLoadConstant(dynamicMethodSkeleton, constants.Add(lambda), lambda.GetType());
            generator.Emit(OpCodes.Callvirt, methodInfo);
        }

        private void EmitPropertyDependencies(ConstructionInfo constructionInfo, IMethodSkeleton dynamicMethodSkeleton)
        {
            if (constructionInfo.PropertyDependencies.Count == 0)
            {
                return;
            }

            ILGenerator generator = dynamicMethodSkeleton.GetILGenerator();
            LocalBuilder instance = generator.DeclareLocal(constructionInfo.ImplementingType);
            generator.Emit(OpCodes.Stloc, instance);
            foreach (var propertyDependency in constructionInfo.PropertyDependencies)
            {
                EmitPropertyDependency(dynamicMethodSkeleton, propertyDependency, instance);
            }

            generator.Emit(OpCodes.Ldloc, instance);
        }

        private Action<IMethodSkeleton> ResolveUnknownServiceEmitter(Type serviceType, string serviceName)
        {
            Action<IMethodSkeleton> emitter = null;

            var rule = factoryRules.Items.FirstOrDefault(r => r.CanCreateInstance(serviceType, serviceName));
            if (rule != null)
            {
                emitter = CreateServiceEmitterBasedOnFactoryRule(rule, serviceType, serviceName);
            }
            else if (serviceType.IsLazy())
            {
                emitter = CreateServiceEmitterBasedOnLazyServiceRequest(serviceType, t => ReflectionHelper.CreateGetInstanceDelegate(t, this));
            }
            else if (serviceType.IsFuncWithParameters())
            {
                emitter = CreateServiceEmitterBasedParameterizedFuncRequest(serviceType, serviceName);
            }
            else if (serviceType.IsFunc())
            {
                emitter = CreateServiceEmitterBasedOnFuncServiceRequest(serviceType, serviceName);
            }
            else if (serviceType.IsEnumerableOfT())
            {
                emitter = CreateEnumerableServiceEmitter(serviceType);
            }            
            else if (CanRedirectRequestForDefaultServiceToSingleNamedService(serviceType, serviceName))
            {
                emitter = CreateServiceEmitterBasedOnSingleNamedInstance(serviceType);
            }
            else if (serviceType.IsClosedGeneric())
            {
                emitter = CreateServiceEmitterBasedOnClosedGenericServiceRequest(serviceType, serviceName);
            }

            UpdateServiceEmitter(serviceType, serviceName, emitter);

            return emitter;
        }
        
        private Action<IMethodSkeleton> CreateServiceEmitterBasedParameterizedFuncRequest(Type serviceType, string serviceName)
        {
            Type[] genericArguments = serviceType.GetGenericArguments();
            Type returnType = genericArguments[genericArguments.Length - 1];
            Type[] parameterTypes = genericArguments.Take(genericArguments.Length - 1).ToArray();
            Type[] methodParameterTypes = new[] { typeof(IServiceFactory) }.Concat(parameterTypes).ToArray();
            
            var methodSkeleton = methodSkeletonFactory(returnType, methodParameterTypes);
            var generator = methodSkeleton.GetILGenerator();

            MethodInfo getInstanceMethod;

            generator.Emit(OpCodes.Ldarg_0);
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg, i + 1);
            }
            
            if (string.IsNullOrEmpty(serviceName))
            {
                getInstanceMethod = ReflectionHelper.GetGetInstanceWithParametersMethod(serviceType);
            }
            else
            {
                getInstanceMethod = ReflectionHelper.GetNamedGetInstanceWithParametersMethod(serviceType);
                generator.Emit(OpCodes.Ldstr, serviceName);
            }

            generator.Emit(OpCodes.Callvirt, getInstanceMethod);

            var getInstanceDelegate = methodSkeleton.CreateDelegate(serviceType, this);            
            var constantIndex = constants.Add(getInstanceDelegate);
            return ms => EmitLoadConstant(ms, constantIndex, serviceType);
        }

        private Action<IMethodSkeleton> CreateServiceEmitterBasedOnFuncServiceRequest(Type serviceType, string serviceName)
        {
            var returnType = serviceType.GetGenericArguments().Single();
            var methodSkeleton = methodSkeletonFactory(returnType, new[] { typeof(IServiceFactory) });
            
            var generator = methodSkeleton.GetILGenerator();
            MethodInfo getInstanceMethod;
            generator.Emit(OpCodes.Ldarg_0);            
            if (string.IsNullOrEmpty(serviceName))
            {
                getInstanceMethod = ReflectionHelper.GetGetInstanceMethod(returnType);
            }
            else
            {
                getInstanceMethod = ReflectionHelper.GetGetNamedInstanceMethod(returnType);
                generator.Emit(OpCodes.Ldstr, serviceName);
            }

            generator.Emit(OpCodes.Callvirt, getInstanceMethod);
            
            var getInstanceDelegate = methodSkeleton.CreateDelegate(serviceType, this);            
            var constantIndex = constants.Add(getInstanceDelegate);
            return ms => EmitLoadConstant(ms, constantIndex, serviceType);
        }

        private Action<IMethodSkeleton> CreateServiceEmitterBasedOnFactoryRule(FactoryRule rule, Type serviceType, string serviceName)
        {
            var serviceRegistration = new ServiceRegistration { ServiceType = serviceType, ServiceName = serviceName, Lifetime = rule.LifeTime };
            ParameterExpression serviceFactoryParameterExpression = Expression.Parameter(typeof(IServiceFactory));
            ConstantExpression serviceRequestConstantExpression = Expression.Constant(new ServiceRequest(serviceType, serviceName, this));
            ConstantExpression delegateConstantExpression = Expression.Constant(rule.Factory);
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(IServiceFactory), serviceType);
            UnaryExpression convertExpression = Expression.Convert(
                Expression.Invoke(delegateConstantExpression, serviceRequestConstantExpression), serviceType);

            LambdaExpression lambdaExpression = Expression.Lambda(delegateType, convertExpression, serviceFactoryParameterExpression);
            serviceRegistration.FactoryExpression = lambdaExpression;

            if (rule.LifeTime != null)
            {
                return methodSkeleton => EmitLifetime(serviceRegistration, ms => EmitNewInstance(serviceRegistration, ms), methodSkeleton);
            }

            return methodSkeleton => EmitNewInstance(serviceRegistration, methodSkeleton);
        }

        private Action<IMethodSkeleton> CreateEnumerableServiceEmitter(Type serviceType)
        {
            Type actualServiceType = ReflectionHelper.GetGenericArguments(serviceType)[0];
            if (actualServiceType.IsGenericType())
            {
                EnsureEmitMethodsForOpenGenericTypesAreCreated(actualServiceType);
            }

            IList<Action<IMethodSkeleton>> serviceEmitters = GetServiceEmitters(actualServiceType).Values.ToList();

            if (dependencyStack.Count > 0 && serviceEmitters.Contains(dependencyStack.Peek()))
            {
                serviceEmitters.Remove(dependencyStack.Peek());
            }

            return ms => EmitEnumerable(serviceEmitters, actualServiceType, ms);
        }

        private void EnsureEmitMethodsForOpenGenericTypesAreCreated(Type actualServiceType)
        {
            var openGenericServiceType = actualServiceType.GetGenericTypeDefinition();
            var openGenericServiceEmitters = GetAvailableServices(openGenericServiceType);
            foreach (var openGenericEmitterEntry in openGenericServiceEmitters.Keys)
            {
                GetRegisteredEmitMethod(actualServiceType, openGenericEmitterEntry);
            }
        }

        private Action<IMethodSkeleton> CreateServiceEmitterBasedOnLazyServiceRequest(Type serviceType, Func<Type, Delegate> valueFactoryDelegate)
        {
            Type actualServiceType = ReflectionHelper.GetGenericArguments(serviceType)[0];                        
            Type funcType = ReflectionHelper.GetFuncType(actualServiceType);            
            ConstructorInfo lazyConstructor = ReflectionHelper.GetLazyConstructor(actualServiceType);
            Delegate getInstanceDelegate = valueFactoryDelegate(actualServiceType);
            var constantIndex = constants.Add(getInstanceDelegate);

            return ms =>
                {
                    EmitLoadConstant(ms, constantIndex, funcType);
                    ms.GetILGenerator().Emit(OpCodes.Newobj, lazyConstructor);
                };
        }
             
        private ServiceRegistration GetOpenGenericServiceRegistration(Type openGenericServiceType, string serviceName)
        {
            var services = GetAvailableServices(openGenericServiceType);
            if (services.Count == 0)
            {
                return null;
            } 
           
            ServiceRegistration openGenericServiceRegistration;
            services.TryGetValue(serviceName, out openGenericServiceRegistration);
            if (openGenericServiceRegistration == null && string.IsNullOrEmpty(serviceName) && services.Count == 1)
            {
                return services.First().Value;
            }

            return openGenericServiceRegistration;
        }

        private Action<IMethodSkeleton> CreateServiceEmitterBasedOnClosedGenericServiceRequest(Type closedGenericServiceType, string serviceName)
        {
            Type openGenericServiceType = closedGenericServiceType.GetGenericTypeDefinition();
            ServiceRegistration openGenericServiceRegistration =
                GetOpenGenericServiceRegistration(openGenericServiceType, serviceName);
           
            if (openGenericServiceRegistration == null)
            {
                return null;
            }

            Type closedGenericImplementingType =
                openGenericServiceRegistration.ImplementingType.MakeGenericType(
                    ReflectionHelper.GetGenericArguments(closedGenericServiceType));

            var serviceRegistration = new ServiceRegistration
                                                          {
                                                              ServiceType = closedGenericServiceType,
                                                              ImplementingType =
                                                                  closedGenericImplementingType,
                                                              ServiceName = serviceName,
                                                              Lifetime =
                                                                  CloneLifeTime(
                                                                      openGenericServiceRegistration
                                                                  .Lifetime)
                                                          };            
            Register(serviceRegistration);
            return GetEmitMethod(serviceRegistration.ServiceType, serviceRegistration.ServiceName);            
        }

        private Action<IMethodSkeleton> CreateServiceEmitterBasedOnSingleNamedInstance(Type serviceType)
        {
            return GetEmitMethod(serviceType, GetServiceEmitters(serviceType).First().Key);
        }

        private bool CanRedirectRequestForDefaultServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetServiceEmitters(serviceType).Count == 1;
        }
        
        private ConstructionInfo GetConstructionInfo(Registration registration)
        {
            return constructionInfoProvider.Value.GetConstructionInfo(registration);
        }

        private ThreadSafeDictionary<string, Action<IMethodSkeleton>> GetServiceEmitters(Type serviceType)
        {
            return emitters.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, Action<IMethodSkeleton>>(StringComparer.CurrentCultureIgnoreCase));
        }
       
        private ThreadSafeDictionary<string, ServiceRegistration> GetAvailableServices(Type serviceType)
        {
            return availableServices.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, ServiceRegistration>(StringComparer.CurrentCultureIgnoreCase));
        }

        private void RegisterService(Type serviceType, Type implementingType, ILifetime lifetime, string serviceName)
        {
            var serviceRegistration = new ServiceRegistration { ServiceType = serviceType, ImplementingType = implementingType, ServiceName = serviceName, Lifetime = lifetime };
            Register(serviceRegistration);         
        }
        
        private Action<IMethodSkeleton> ResolveEmitDelegate(ServiceRegistration serviceRegistration)
        {                    
            if (serviceRegistration.Lifetime == null)
            {
                return methodSkeleton => EmitNewInstance(serviceRegistration, methodSkeleton);
            }

            return methodSkeleton => EmitLifetime(serviceRegistration, ms => EmitNewInstance(serviceRegistration, ms), methodSkeleton);
        }
        
        private void EmitLifetime(ServiceRegistration serviceRegistration, Action<IMethodSkeleton> instanceEmitter, IMethodSkeleton dynamicMethodSkeleton)
        {
            if (serviceRegistration.Lifetime is PerContainerLifetime)
            {
                var del =
                    WrapAsFuncDelegate(
                        CreateDynamicMethodDelegate(instanceEmitter, serviceRegistration.ServiceType));
                var instance = serviceRegistration.Lifetime.GetInstance(del, null);
                var instanceIndex = constants.Add(instance);
                EmitLoadConstant(dynamicMethodSkeleton, instanceIndex, serviceRegistration.ServiceType);
            }
            else
            {
                ILGenerator generator = dynamicMethodSkeleton.GetILGenerator();
                int instanceDelegateIndex = CreateInstanceDelegateIndex(instanceEmitter, serviceRegistration.ServiceType);
                int lifetimeIndex = CreateLifetimeIndex(serviceRegistration.Lifetime);
                int scopeManagerIndex = CreateScopeManagerIndex();
                var getInstanceMethod = ReflectionHelper.LifetimeGetInstanceMethod;
                EmitLoadConstant(dynamicMethodSkeleton, lifetimeIndex, typeof(ILifetime));
                EmitLoadConstant(dynamicMethodSkeleton, instanceDelegateIndex, typeof(Func<object>));
                EmitLoadConstant(dynamicMethodSkeleton, scopeManagerIndex, typeof(ThreadLocal<ScopeManager>));
                generator.Emit(OpCodes.Callvirt, ReflectionHelper.GetCurrentScopeManagerMethod);
                generator.Emit(OpCodes.Callvirt, ReflectionHelper.GetCurrentScopeMethod);
                generator.Emit(OpCodes.Callvirt, getInstanceMethod);
                generator.Emit(serviceRegistration.ServiceType.IsValueType() ? OpCodes.Unbox_Any : OpCodes.Castclass, serviceRegistration.ServiceType);
            }
        }

        private int CreateScopeManagerIndex()
        {
            return constants.Add(scopeManagers);
        }

        private int CreateInstanceDelegateIndex(Action<IMethodSkeleton> instanceEmitter, Type serviceType)
        {
            return constants.Add(WrapAsFuncDelegate(CreateDynamicMethodDelegate(instanceEmitter, serviceType)));
        }                

        private int CreateLifetimeIndex(ILifetime lifetime)
        {
            return constants.Add(lifetime);
        }

        private Func<object[], object> CreateDelegate(Type serviceType, string serviceName, bool throwError)
        {
            var serviceEmitter = GetEmitMethod(serviceType, serviceName);
            if (serviceEmitter == null && throwError)
            {
                throw new InvalidOperationException(string.Format("Unable to resolve type: {0}, service name: {1}", serviceType, serviceName));
            }

            if (serviceEmitter != null)
            {
                try
                {                   
                    return CreateDynamicMethodDelegate(serviceEmitter, serviceType);
                }
                catch (InvalidOperationException ex)
                {
                    dependencyStack.Clear();
                    throw new InvalidOperationException(
                        string.Format("Unable to resolve type: {0}, service name: {1}", serviceType, serviceName), ex);
                }
            }

            return c => null;
        }
    
        private void Invalidate()
        {
            delegates.Clear();
            namedDelegates.Clear();
            constants.Clear();
            constructionInfoProvider.Value.Invalidate();
        }
        
        private void RegisterValue(Type serviceType, object value, string serviceName)
        {
            var serviceRegistration = new ServiceRegistration { ServiceType = serviceType, ServiceName = serviceName, Value = value, Lifetime = new PerContainerLifetime() };
            Register(serviceRegistration);            
        }

        private void RegisterServiceFromLambdaExpression<TService>(
            LambdaExpression factory, ILifetime lifetime, string serviceName)
        {
            var serviceRegistration = new ServiceRegistration { ServiceType = typeof(TService), FactoryExpression = factory, ServiceName = serviceName, Lifetime = lifetime };
            Register(serviceRegistration);            
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

            public void Clear()
            {
                lock (lockObject)
                {
                    items = new T[0];
                }
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

            public void Clear()
            {
                lock (lockObject)
                {
                    dictionary.Clear();
                }
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

        private class DynamicMethodSkeleton : IMethodSkeleton
        {            
            private DynamicMethod dynamicMethod;

            public DynamicMethodSkeleton(Type returnType, Type[] parameterTypes)
            {         
                CreateDynamicMethod(returnType, parameterTypes);
            }

            public ILGenerator GetILGenerator()
            {
                return dynamicMethod.GetILGenerator();
            }

            public Delegate CreateDelegate(Type delegateType)
            {                                                         
                dynamicMethod.GetILGenerator().Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(delegateType);
            }

            public Delegate CreateDelegate(Type delegateType, object target)
            {
                dynamicMethod.GetILGenerator().Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(delegateType, target);
            }
            
            private void CreateDynamicMethod(Type returnType, Type[] parameterTypes)
            {
                dynamicMethod = new DynamicMethod(returnType, parameterTypes);                    
            }
        }

        private class ServiceRegistry<T> : ThreadSafeDictionary<Type, ThreadSafeDictionary<string, T>>
        {
        }

        private class DelegateRegistry<TKey> : KeyValueStorage<TKey, Func<object[], object>>
        {
        }

        private class FactoryRule
        {
            public Func<Type, string, bool> CanCreateInstance { get; set; }

            public Func<ServiceRequest, object> Factory { get; set; }

            public ILifetime LifeTime { get; set; }
        }
    }
    
    /// <summary>
    /// A thread safe dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class ThreadSafeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeDictionary{TKey,TValue}"/> class.
        /// </summary>
        public ThreadSafeDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeDictionary{TKey,TValue}"/> class using the 
        /// given <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys</param>
        public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }
    }

    /// <summary>
    /// Provides field representations of the Microsoft Intermediate Language (MSIL) instructions.
    /// </summary>
    public class OpCodes
    {
        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack.
        /// </summary>
        public static readonly OpCode Ldarg_0;
        
        /// <summary>
        /// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly OpCode Ldc_I4;
        
        /// <summary>
        /// Loads the element containing an object reference at a specified array index onto the top of the evaluation stack as type O (object reference).
        /// </summary>
        public static readonly OpCode Ldelem_Ref;
        
        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class.
        /// </summary>
        public static readonly OpCode Castclass;

        /// <summary>
        /// Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.
        /// </summary>
        public static readonly OpCode Newarr;
        
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.
        /// </summary>
        public static readonly OpCode Stloc;

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack.
        /// </summary>        
        public static readonly OpCode Ldloc;
        
        /// <summary>
        /// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
        /// </summary>
        public static readonly OpCode Newobj;
        
        /// <summary>
        /// Converts the boxed representation of a type specified in the instruction to its unboxed form.
        /// </summary>
        public static readonly OpCode Unbox_Any;

        /// <summary>
        /// Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.
        /// </summary>
        public static readonly OpCode Stelem;
        
        /// <summary>
        /// Converts a value type to an object reference (type O).
        /// </summary>
        public static readonly OpCode Box;

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        public static readonly OpCode Callvirt;

        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
        /// </summary>
        public static readonly OpCode Ldlen;

        /// <summary>
        /// Converts the value on top of the evaluation stack to int32.
        /// </summary>
        public static readonly OpCode Conv_I4;

        /// <summary>
        /// Subtracts one value from another and pushes the result onto the evaluation stack.
        /// </summary>
        public static readonly OpCode Sub;

        /// <summary>
        /// Pushes the integer value of 1 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly OpCode Ldc_I4_1;

        /// <summary>
        /// Pushes a new object reference to a string literal stored in the metadata.
        /// </summary>
        public static readonly OpCode Ldstr;

        /// <summary>
        /// Loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        public static readonly OpCode Ldarg;

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
        /// </summary>
        public static readonly OpCode Ret;

        static OpCodes()
        {
            Ldarg_0 = new OpCode(1);
            Ldc_I4 = new OpCode(2);
            Ldelem_Ref = new OpCode(3);
            Castclass = new OpCode(4);
            Newarr = new OpCode(5);
            Stloc = new OpCode(6);
            Ldloc = new OpCode(7);
            Newobj = new OpCode(8);
            Unbox_Any = new OpCode(9);
            Stelem = new OpCode(10);
            Box = new OpCode(11);
            Callvirt = new OpCode(12);
            Ldlen = new OpCode(13);
            Conv_I4 = new OpCode(14);
            Sub = new OpCode(15);
            Ldc_I4_1 = new OpCode(16);
            Ldstr = new OpCode(17);
            Ldarg = new OpCode(18);
            Ret = new OpCode(19);
        }
    }

    /// <summary>
    /// Describes a Microsoft intermediate language (MSIL) instruction.
    /// </summary>
    public struct OpCode
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OpCode"/> struct.
        /// </summary>
        /// <param name="value">The value that identifies this <see cref="OpCode"/>.</param>
        public OpCode(short value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// Gets a <see cref="short"/> value that identifies this <see cref="OpCode"/>.
        /// </summary>
        public short Value { get; private set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return ((OpCode)obj).Value == Value;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Overloads the equals operator.
        /// </summary>
        /// <param name="a">The left-hand <see cref="OpCode"/>.</param>
        /// <param name="b">The right-hand <see cref="OpCode"/>.</param>
        /// <returns>true if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, false.</returns>
        public static bool operator ==(OpCode a, OpCode b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Overloads the not equals operator.
        /// </summary>
        /// <param name="a">The left-hand <see cref="OpCode"/>.</param>
        /// <param name="b">The right-hand <see cref="OpCode"/>.</param>
        /// <returns>true if <paramref name="a"/> is not equal to <paramref name="b"/>; otherwise, false.</returns>
        public static bool operator !=(OpCode a, OpCode b)
        {
            return !(a == b);
        }
    }

    /// <summary>
    /// Defines and represents a dynamic method that can be compiled and executed.
    /// </summary>    
    internal class DynamicMethod
    {
        private readonly Type returnType;

        private readonly Type[] parameterTypes;

        private readonly ParameterExpression[] parameters;

        private readonly ILGenerator ilGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMethod"/> class.
        /// </summary>
        /// <param name="returnType">A <see cref="Type"/> object that specifies the return type of the dynamic method.</param>
        /// <param name="parameterTypes">An array of <see cref="Type"/> objects specifying the types of the parameters of the dynamic method, or null if the method has no parameters.</param>
        public DynamicMethod(Type returnType, Type[] parameterTypes)
        {
            this.returnType = returnType;
            this.parameterTypes = parameterTypes;
            parameters = parameterTypes.Select(Expression.Parameter).ToArray();
            ilGenerator = new ILGenerator(parameters);
        }

        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method.</param>
        /// <returns>A delegate of the specified type, which can be used to execute the dynamic method.</returns>
        public Delegate CreateDelegate(Type delegateType)
        {
            var lambda = Expression.Lambda(delegateType, ilGenerator.CurrentExpression, parameters);
            return lambda.Compile();
        }

        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it, specifying the delegate type and an object the delegate is bound to.
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method, minus the first parameter.</param>
        /// <param name="target">An object the delegate is bound to. Must be of the same type as the first parameter of the dynamic method.</param>
        /// <returns>A delegate of the specified type, which can be used to execute the dynamic method with the specified target object.</returns>
        public Delegate CreateDelegate(Type delegateType, object target)
        {
            Type delegateTypeWithTargetParameter =
                Expression.GetDelegateType(parameterTypes.Concat(new[] { returnType }).ToArray());
            var lambdaWithTargetParameter = Expression.Lambda(
                delegateTypeWithTargetParameter, ilGenerator.CurrentExpression, true, parameters);

            Expression[] arguments = new Expression[] { Expression.Constant(target) }.Concat(parameters.Cast<Expression>().Skip(1)).ToArray();
            var invokeExpression = Expression.Invoke(lambdaWithTargetParameter, arguments);
            
            var lambda = Expression.Lambda(delegateType, invokeExpression, parameters.Skip(1));
            return lambda.Compile();            
        }

        /// <summary>
        /// Returns a <see cref="ILGenerator"/> for the method
        /// </summary>
        /// <returns>An <see cref="ILGenerator"/> object for the method.</returns>
        public ILGenerator GetILGenerator()
        {
            return ilGenerator;
        }
    }

    /// <summary>
    /// A generator that transforms <see cref="OpCodes"/> into an expression tree.
    /// </summary>
    public class ILGenerator
    {
        private readonly ParameterExpression[] parameters;
        private readonly Stack<Expression> stack = new Stack<Expression>();
        private readonly List<LocalBuilder> locals = new List<LocalBuilder>();
        private readonly List<Expression> expressions = new List<Expression>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ILGenerator"/> class.
        /// </summary>
        /// <param name="parameters">An array of parameters used by the target <see cref="DynamicMethod"/>.</param>
        public ILGenerator(ParameterExpression[] parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Gets the current expression based the emitted <see cref="OpCodes"/>. 
        /// </summary>
        public Expression CurrentExpression
        {
            get
            {
                var variables = locals.Select(l => l.Variable).ToList();
                var ex = new List<Expression>(expressions) { stack.Peek() };
                return Expression.Block(variables, ex);
            }
        }

        /// <summary>
        /// Puts the specified instruction and metadata token for the specified constructor onto the Microsoft intermediate language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="code">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="constructor">A <see cref="ConstructorInfo"/> representing a constructor.</param>
        public void Emit(OpCode code, ConstructorInfo constructor)
        {
            if (code == OpCodes.Newobj)
            {
                var parameterCount = constructor.GetParameters().Length;
                var expression = Expression.New(constructor, Pop(parameterCount));
                stack.Push(expression);
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }
        }
 
        /// <summary>
        /// Puts the specified instruction onto the stream of instructions.
        /// </summary>
        /// <param name="code">The Microsoft Intermediate Language (MSIL) instruction to be put onto the stream.</param>
        public void Emit(OpCode code)
        {
            if (code == OpCodes.Ldarg_0)
            {
                stack.Push(parameters[0]);
            }
            else if (code == OpCodes.Ldelem_Ref)
            {
                Expression[] indexes = new[] { stack.Pop() };
                Expression array = stack.Pop();
                stack.Push(Expression.ArrayAccess(array, indexes));
            }
            else if (code == OpCodes.Ldlen)
            {
                Expression array = stack.Pop();
                stack.Push(Expression.ArrayLength(array));
            }        
            else if (code == OpCodes.Conv_I4)
            {
                stack.Push(Expression.Convert(stack.Pop(), typeof(int)));
            }
            else if (code == OpCodes.Ldc_I4_1)
            {
                stack.Push(Expression.Constant(1, typeof(int)));
            }
            else if (code == OpCodes.Sub)
            {
                var right = stack.Pop();
                var left = stack.Pop();                
                stack.Push(Expression.Subtract(left, right));                
            }
            else if (code == OpCodes.Ret)
            {                
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the index of the given local variable.
        /// </summary>
        /// <param name="code">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="localBuilder">A local variable.</param>
        public void Emit(OpCode code, LocalBuilder localBuilder)
        {
            if (code == OpCodes.Stloc)
            {
                Expression valueExpression = stack.Pop();
                var assignExpression = Expression.Assign(localBuilder.Variable, valueExpression);
                expressions.Add(assignExpression);
            }            
            else if (code == OpCodes.Ldloc)
            {
                stack.Push(localBuilder.Variable);
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }
        }

        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="code">The MSIL instruction to be put onto the stream.</param>
        /// <param name="arg">The numerical argument pushed onto the stream immediately after the instruction.</param>
        public void Emit(OpCode code, int arg)
        {
            if (code == OpCodes.Ldc_I4)
            {
                stack.Push(Expression.Constant(arg, typeof(int)));
            }
            else if (code == OpCodes.Ldarg)
            {
                stack.Push(parameters[arg]);
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the metadata token for the given string.
        /// </summary>
        /// <param name="code">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="arg">The String to be emitted.</param>
        public void Emit(OpCode code, string arg)
        {
            if (code == OpCodes.Ldstr)
            {
                stack.Push(Expression.Constant(arg, typeof(string)));
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }
        }

        /// <summary>
        /// Declares a local variable of the specified type.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> object that represents the type of the local variable.</param>
        /// <returns>The declared local variable.</returns>
        public LocalBuilder DeclareLocal(Type type)
        {
            var localBuilder = new LocalBuilder(type);
            locals.Add(localBuilder);
            return localBuilder;
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the metadata token for the given type.
        /// </summary>
        /// <param name="code">The MSIL instruction to be put onto the stream.</param>
        /// <param name="type">A <see cref="Type"/>.</param>
        public void Emit(OpCode code, Type type)
        {
            if (code == OpCodes.Newarr)
            {
                stack.Push(Expression.NewArrayBounds(type, Pop(1)));
            }            
            else if (code == OpCodes.Stelem)
            {
                var value = stack.Pop();
                var index = stack.Pop();
                var array = stack.Pop();                
                var arrayAccess = Expression.ArrayAccess(array, index);
                
                var assignExpression = Expression.Assign(arrayAccess, value);
                expressions.Add(assignExpression);
            }
            else if (code == OpCodes.Castclass)
            {
                stack.Push(Expression.Convert(stack.Pop(), type));
            }
            else if (code == OpCodes.Box)
            {
                stack.Push(Expression.Convert(stack.Pop(), typeof(object)));
            }
            else if (code == OpCodes.Unbox_Any)
            {
                stack.Push(Expression.Convert(stack.Pop(), type));
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }            
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed by the metadata token for the given method.
        /// </summary>
        /// <param name="code">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="methodInfo">A <see cref="MethodInfo"/> representing a method.</param>
        public void Emit(OpCode code, MethodInfo methodInfo)
        {
            if (code == OpCodes.Callvirt)
            {
                var parameterCount = methodInfo.GetParameters().Length;
                Expression[] arguments = Pop(parameterCount);
                var instance = stack.Pop();
                var methodCallExpression = Expression.Call(instance, methodInfo, arguments);
                if (methodInfo.ReturnType == typeof(void))
                {
                    expressions.Add(methodCallExpression);
                }
                else
                {
                    stack.Push(Expression.Call(instance, methodInfo, arguments));    
                }                
            }
            else
            {
                throw new NotSupportedException(code.ToString());
            }
        }

        private Expression[] Pop(int numberOfElements)
        {
            var expressionsToPop = new Expression[numberOfElements];

            for (int i = 0; i < numberOfElements; i++)
            {
                expressionsToPop[i] = stack.Pop();
            }

            return expressionsToPop.Reverse().ToArray();
        }
    }

    /// <summary>
    /// Represents a local variable within a method or constructor.
    /// </summary>
    public class LocalBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalBuilder"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the variable that this <see cref="LocalBuilder"/> represents.</param>
        public LocalBuilder(Type type)
        {
            Variable = Expression.Parameter(type);
        }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> that represents the variable.
        /// </summary>
        public ParameterExpression Variable { get; private set; }         
    } 
  
    /// <summary>
    /// Selects the <see cref="ConstructionInfo"/> from a given type that has the highest number of parameters.
    /// </summary>
    public class ConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Selects the constructor to be used when creating a new instance of the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="implementingType">The <see cref="Type"/> for which to return a <see cref="ConstructionInfo"/>.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance that represents the constructor to be used
        /// when creating a new instance of the <paramref name="implementingType"/>.</returns>
        public ConstructorInfo Execute(Type implementingType)
        {
            ConstructorInfo constructorInfo = implementingType.GetConstructors().OrderBy(c => c.GetParameters().Count()).LastOrDefault();
            if (constructorInfo == null)
            {
                throw new InvalidOperationException("Missing public constructor for Type: " + implementingType.FullName);
            }

            return constructorInfo;
        }
    }

    /// <summary>
    /// Selects the constructor dependencies for a given <see cref="ConstructorInfo"/>.
    /// </summary>
    public class ConstructorDependencySelector : IConstructorDependencySelector
    {
        /// <summary>
        /// Selects the constructor dependencies for the given <paramref name="constructor"/>.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructionInfo"/> for which to select the constructor dependencies.</param>
        /// <returns>A list of <see cref="ConstructorDependency"/> instances that represents the constructor
        /// dependencies for the given <paramref name="constructor"/>.</returns>
        public virtual IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor)
        {
            return
                constructor.GetParameters()
                           .OrderBy(p => p.Position)
                           .Select(
                               p =>
                               new ConstructorDependency
                                   {
                                       ServiceName = string.Empty,
                                       ServiceType = p.ParameterType,
                                       Parameter = p,
                                       IsRequired = true
                                   });
        }
    }

    /// <summary>
    /// Selects the property dependencies for a given <see cref="Type"/>.
    /// </summary>
    public class PropertyDependencySelector : IPropertyDependencySelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDependencySelector"/> class.
        /// </summary>
        /// <param name="propertySelector">The <see cref="IPropertySelector"/> that is 
        /// responsible for selecting a list of injectable properties.</param>
        public PropertyDependencySelector(IPropertySelector propertySelector)
        {
            PropertySelector = propertySelector;
        }

        /// <summary>
        /// Gets the <see cref="IPropertySelector"/> that is responsible for selecting a 
        /// list of injectable properties.
        /// </summary>
        protected IPropertySelector PropertySelector { get; private set; }

        /// <summary>
        /// Selects the property dependencies for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the property dependencies.</param>
        /// <returns>A list of <see cref="PropertyDependency"/> instances that represents the property
        /// dependencies for the given <paramref name="type"/>.</returns>
        public virtual IEnumerable<PropertyDependency> Execute(Type type)
        {
            return PropertySelector.Execute(type).Select(
                p => new PropertyDependency { Property = p, ServiceName = string.Empty, ServiceType = p.PropertyType });
        }
    }

    /// <summary>
    /// Builds a <see cref="ConstructionInfo"/> instance based on the implementing <see cref="Type"/>.
    /// </summary>
    public class TypeConstructionInfoBuilder : ITypeConstructionInfoBuilder
    {
        private readonly IConstructorSelector constructorSelector;
        private readonly IConstructorDependencySelector constructorDependencySelector;
        private readonly IPropertyDependencySelector propertyDependencySelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConstructionInfoBuilder"/> class.
        /// </summary>
        /// <param name="constructorSelector">The <see cref="IConstructorSelector"/> that is responsible
        /// for selecting the constructor to be used for constructor injection.</param>
        /// <param name="constructorDependencySelector">The <see cref="IConstructorDependencySelector"/> that is 
        /// responsible for selecting the constructor dependencies for a given <see cref="ConstructionInfo"/>.</param>
        /// <param name="propertyDependencySelector">The <see cref="IPropertyDependencySelector"/> that is responsible
        /// for selecting the property dependencies for a given <see cref="Type"/>.</param>
        public TypeConstructionInfoBuilder(
            IConstructorSelector constructorSelector,
            IConstructorDependencySelector constructorDependencySelector,
            IPropertyDependencySelector propertyDependencySelector)
        {
            this.constructorSelector = constructorSelector;
            this.constructorDependencySelector = constructorDependencySelector;
            this.propertyDependencySelector = propertyDependencySelector;
        }

        /// <summary>
        /// Analyzes the <paramref name="implementingType"/> and returns a <see cref="ConstructionInfo"/> instance.
        /// </summary>
        /// <param name="implementingType">The <see cref="Type"/> to analyze.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance.</returns>
        public ConstructionInfo Execute(Type implementingType)
        {
            var constructionInfo = new ConstructionInfo();
            constructionInfo.Constructor = constructorSelector.Execute(implementingType);
            constructionInfo.ImplementingType = implementingType;
            constructionInfo.PropertyDependencies.AddRange(propertyDependencySelector.Execute(implementingType));
            constructionInfo.ConstructorDependencies.AddRange(constructorDependencySelector.Execute(constructionInfo.Constructor));
            return constructionInfo;
        }
    }

    /// <summary>
    /// Keeps track of a <see cref="ConstructionInfo"/> instance for each <see cref="Registration"/>.
    /// </summary>
    public class ConstructionInfoProvider : IConstructionInfoProvider
    {
        private readonly IConstructionInfoBuilder constructionInfoBuilder;
        private readonly ThreadSafeDictionary<Registration, ConstructionInfo> cache = new ThreadSafeDictionary<Registration, ConstructionInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionInfoProvider"/> class.
        /// </summary>
        /// <param name="constructionInfoBuilder">The <see cref="IConstructionInfoBuilder"/> that 
        /// is responsible for building a <see cref="ConstructionInfo"/> instance based on a given <see cref="Registration"/>.</param>
        public ConstructionInfoProvider(IConstructionInfoBuilder constructionInfoBuilder)
        {
            this.constructionInfoBuilder = constructionInfoBuilder;
        }

        /// <summary>
        /// Gets a <see cref="ConstructionInfo"/> instance for the given <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">The <see cref="Registration"/> for which to get a <see cref="ConstructionInfo"/> instance.</param>
        /// <returns>The <see cref="ConstructionInfo"/> instance that describes how to create an instance of the given <paramref name="registration"/>.</returns>
        public ConstructionInfo GetConstructionInfo(Registration registration)
        {
            return cache.GetOrAdd(registration, constructionInfoBuilder.Execute);
        }

        /// <summary>
        /// Invalidates the <see cref="IConstructionInfoProvider"/> and causes new <see cref="ConstructionInfo"/> instances 
        /// to be created when the <see cref="IConstructionInfoProvider.GetConstructionInfo"/> method is called.
        /// </summary>
        public void Invalidate()
        {
            cache.Clear();
        }
    }

    /// <summary>
    /// Provides a <see cref="ConstructorInfo"/> instance 
    /// that describes how to create a service instance.
    /// </summary>
    public class ConstructionInfoBuilder : IConstructionInfoBuilder
    {
        private readonly Lazy<ILambdaConstructionInfoBuilder> lambdaConstructionInfoBuilder;
        private readonly Lazy<ITypeConstructionInfoBuilder> typeConstructionInfoBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionInfoBuilder"/> class.             
        /// </summary>
        /// <param name="lambdaConstructionInfoBuilderFactory">
        /// A function delegate used to provide a <see cref="ILambdaConstructionInfoBuilder"/> instance.
        /// </param>
        /// <param name="typeConstructionInfoBuilderFactory">
        /// A function delegate used to provide a <see cref="ITypeConstructionInfoBuilder"/> instance.
        /// </param>
        public ConstructionInfoBuilder(
            Func<ILambdaConstructionInfoBuilder> lambdaConstructionInfoBuilderFactory,
            Func<ITypeConstructionInfoBuilder> typeConstructionInfoBuilderFactory)
        {
            typeConstructionInfoBuilder = new Lazy<ITypeConstructionInfoBuilder>(typeConstructionInfoBuilderFactory);
            lambdaConstructionInfoBuilder = new Lazy<ILambdaConstructionInfoBuilder>(lambdaConstructionInfoBuilderFactory);
        }

        /// <summary>
        /// Returns a <see cref="ConstructionInfo"/> instance based on the given <see cref="Registration"/>.
        /// </summary>
        /// <param name="registration">The <see cref="Registration"/> for which to return a <see cref="ConstructionInfo"/> instance.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance that describes how to create a service instance.</returns>
        public ConstructionInfo Execute(Registration registration)
        {
            return registration.FactoryExpression != null
                ? CreateConstructionInfoFromLambdaExpression(registration.FactoryExpression)
                : CreateConstructionInfoFromImplementingType(registration.ImplementingType);
        }

        private ConstructionInfo CreateConstructionInfoFromLambdaExpression(LambdaExpression lambdaExpression)
        {
            return lambdaConstructionInfoBuilder.Value.Execute(lambdaExpression);
        }

        private ConstructionInfo CreateConstructionInfoFromImplementingType(Type implementingType)
        {
            return typeConstructionInfoBuilder.Value.Execute(implementingType);
        }
    }

    /// <summary>
    /// Parses a <see cref="LambdaExpression"/> into a <see cref="ConstructionInfo"/> instance.
    /// </summary>
    public class LambdaConstructionInfoBuilder : ILambdaConstructionInfoBuilder
    {
        /// <summary>
        /// Parses the <paramref name="lambdaExpression"/> and returns a <see cref="ConstructionInfo"/> instance.
        /// </summary>
        /// <param name="lambdaExpression">The <see cref="LambdaExpression"/> to parse.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance.</returns>
        public ConstructionInfo Execute(LambdaExpression lambdaExpression)
        {
            var lambdaExpressionValidator = new LambdaExpressionValidator();

            if (!lambdaExpressionValidator.CanParse(lambdaExpression))
            {
                return CreateConstructionInfoBasedOnLambdaExpression(lambdaExpression);
            }

            switch (lambdaExpression.Body.NodeType)
            {
                case ExpressionType.New:
                    return CreateConstructionInfoBasedOnNewExpression((NewExpression)lambdaExpression.Body);
                case ExpressionType.MemberInit:
                    return CreateConstructionInfoBasedOnHandleMemberInitExpression((MemberInitExpression)lambdaExpression.Body);
                default:
                    return CreateConstructionInfoBasedOnLambdaExpression(lambdaExpression);
            }
        }

        private static ConstructionInfo CreateConstructionInfoBasedOnLambdaExpression(LambdaExpression lambdaExpression)
        {
            return new ConstructionInfo { FactoryDelegate = lambdaExpression.Compile() };
        }

        private static ConstructionInfo CreateConstructionInfoBasedOnNewExpression(NewExpression newExpression)
        {
            var constructionInfo = CreateConstructionInfo(newExpression);
            ParameterInfo[] parameters = newExpression.Constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ConstructorDependency constructorDependency = CreateConstructorDependency(parameters[i]);
                ApplyDependencyDetails(newExpression.Arguments[i], constructorDependency);
                constructionInfo.ConstructorDependencies.Add(constructorDependency);
            }

            return constructionInfo;
        }

        private static ConstructionInfo CreateConstructionInfo(NewExpression newExpression)
        {
            var constructionInfo = new ConstructionInfo { Constructor = newExpression.Constructor, ImplementingType = newExpression.Constructor.DeclaringType };
            return constructionInfo;
        }

        private static ConstructionInfo CreateConstructionInfoBasedOnHandleMemberInitExpression(MemberInitExpression memberInitExpression)
        {
            var constructionInfo = CreateConstructionInfoBasedOnNewExpression(memberInitExpression.NewExpression);
            foreach (MemberBinding memberBinding in memberInitExpression.Bindings)
            {
                HandleMemberAssignment((MemberAssignment)memberBinding, constructionInfo);
            }

            return constructionInfo;
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

        private static PropertyDependency CreatePropertyDependency(MemberAssignment memberAssignment)
        {
            var propertyDependecy = new PropertyDependency
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

    /// <summary>
    /// Inspects the body of a <see cref="LambdaExpression"/> and determines if the expression can be parsed.
    /// </summary>
    public class LambdaExpressionValidator : ExpressionVisitor
    {
        private bool canParse = true;

        /// <summary>
        /// Determines if the <paramref name="lambdaExpression"/> can be parsed.
        /// </summary>
        /// <param name="lambdaExpression">The <see cref="LambdaExpression"/> to validate.</param>
        /// <returns><b>true</b>, if the expression can be parsed, otherwise <b>false</b>.</returns>
        public bool CanParse(LambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Parameters.Count > 1)
            {
                return false;
            }

            Visit(lambdaExpression.Body);
            return canParse;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-expression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param><typeparam name="T">The type of the delegate.</typeparam>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            canParse = false;
            return base.VisitLambda(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-expression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
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
    /// Contains information about a service request that originates from a rule based service registration.
    /// </summary>    
    public class ServiceRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRequest"/> class.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <param name="serviceFactory">The <see cref="IServiceFactory"/> to be associated with this <see cref="ServiceRequest"/>.</param>
        public ServiceRequest(Type serviceType, string serviceName, IServiceFactory serviceFactory)
        {
            ServiceType = serviceType;
            ServiceName = serviceName;
            ServiceFactory = serviceFactory;
        }

        /// <summary>
        /// Gets the service type.
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Gets the service name.
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// Gets the <see cref="IServiceFactory"/> that is associated with this <see cref="ServiceRequest"/>.
        /// </summary>
        public IServiceFactory ServiceFactory { get; private set; }
    }

    /// <summary>
    /// Base class for concrete registrations within the service container.
    /// </summary>
    public abstract class Registration
    {
        /// <summary>
        /// Gets or sets the service <see cref="Type"/>.
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> that implements the <see cref="Registration.ServiceType"/>.
        /// </summary>
        public virtual Type ImplementingType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LambdaExpression"/> used to create a service instance.
        /// </summary>
        public LambdaExpression FactoryExpression { get; set; }
    }

    /// <summary>
    /// Contains information about a registered decorator.
    /// </summary>
    public class DecoratorRegistration : Registration
    {
        /// <summary>
        /// Gets or sets a function delegate that determines if the decorator can decorate the service 
        /// represented by the supplied <see cref="ServiceRegistration"/>.
        /// </summary>
        public Func<ServiceRegistration, bool> CanDecorate { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Lazy{T}"/> that defers resolving of the decorators implementing type.
        /// </summary>
        public Func<IServiceFactory, ServiceRegistration, Type> ImplementingTypeFactory { get; set; }

        /// <summary>
        /// Gets or sets the index of this <see cref="DecoratorRegistration"/>.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Contains information about a registered service.
    /// </summary>
    public class ServiceRegistration : Registration
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ILifetime"/> instance that controls the lifetime of the service.
        /// </summary>
        public ILifetime Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the value that represents the instance of the service.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ServiceRegistration"/> can be overridden 
        /// by another registration.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return ServiceType.GetHashCode() ^ ServiceName.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// True if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var other = obj as ServiceRegistration;
            if (other == null)
            {
                return false;
            }

            var result = ServiceName == other.ServiceName && ServiceType == other.ServiceType;
            return result;
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
            PropertyDependencies = new List<PropertyDependency>();
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
        /// Gets a list of <see cref="PropertyDependency"/> instances that represent 
        /// the property dependencies for the target service instance. 
        /// </summary>
        public List<PropertyDependency> PropertyDependencies { get; private set; }

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
        /// Gets or sets a value indicating whether this dependency is required.
        /// </summary>
        public bool IsRequired { get; set; }

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
    public class PropertyDependency : Dependency
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

    /// <summary>
    /// Ensures that only one instance of a given service can exist within the current <see cref="IServiceContainer"/>.
    /// </summary>
    public class PerContainerLifetime : ILifetime, IDisposable
    {
        private readonly object syncRoot = new object();
        private object singleton;

        /// <summary>
        /// Returns a service instance according to the specific lifetime characteristics.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <param name="scope">The <see cref="Scope"/> of the current service request.</param>
        /// <returns>The requested services instance.</returns>
        public object GetInstance(Func<object> createInstance, Scope scope)
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

        /// <summary>
        /// Disposes the service instances managed by this <see cref="PerContainerLifetime"/> instance.
        /// </summary>
        public void Dispose()
        {
            var disposable = singleton as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Ensures that a new instance is created for each request in addition to tracking disposable instances.
    /// </summary>
    public class PerRequestLifeTime : ILifetime
    {
        /// <summary>
        /// Returns a service instance according to the specific lifetime characteristics.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <param name="scope">The <see cref="Scope"/> of the current service request.</param>
        /// <returns>The requested services instance.</returns>
        public object GetInstance(Func<object> createInstance, Scope scope)
        {
            var instance = createInstance();
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                TrackInstance(scope, disposable);
            }

            return instance;
        }

        private static void TrackInstance(Scope scope, IDisposable disposable)
        {
            if (scope == null)
            {
                throw new InvalidOperationException("Attempt to create a disposable instance without a current scope.");
            }

            scope.TrackInstance(disposable);
        }
    }

    /// <summary>
    /// Ensures that only one service instance can exist within a given <see cref="Scope"/>.
    /// </summary>
    /// <remarks>
    /// If the service instance implements <see cref="IDisposable"/>, 
    /// it will be disposed when the <see cref="Scope"/> ends.
    /// </remarks>
    public class PerScopeLifetime : ILifetime
    {
        private readonly ThreadSafeDictionary<Scope, object> instances = new ThreadSafeDictionary<Scope, object>();

        /// <summary>
        /// Returns the same service instance within the current <see cref="Scope"/>.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <param name="scope">The <see cref="Scope"/> of the current service request.</param>
        /// <returns>The requested services instance.</returns>
        public object GetInstance(Func<object> createInstance, Scope scope)
        {
            if (scope == null)
            {
                throw new InvalidOperationException(
                    "Attempt to create a scoped instance without a current scope.");
            }

            return instances.GetOrAdd(scope, s => CreateScopedInstance(s, createInstance));
        }

        private static void RegisterForDisposal(Scope scope, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                scope.TrackInstance(disposable);
            }
        }

        private object CreateScopedInstance(Scope scope, Func<object> createInstance)
        {
            scope.Completed += OnScopeCompleted;
            var instance = createInstance();

            RegisterForDisposal(scope, instance);
            return instance;
        }

        private void OnScopeCompleted(object sender, EventArgs e)
        {
            var scope = (Scope)sender;
            scope.Completed -= OnScopeCompleted;
            object removedInstance;
            instances.TryRemove(scope, out removedInstance);
        }
    }

    /// <summary>
    /// Manages a set of <see cref="Scope"/> instances.
    /// </summary>
    public class ScopeManager
    {
        private readonly object syncRoot = new object();

        private Scope currentScope;

        /// <summary>
        /// Gets the current <see cref="Scope"/>.
        /// </summary>
        public Scope CurrentScope
        {
            get
            {
                lock (syncRoot)
                {
                    return currentScope;
                }
            }
        }

        /// <summary>
        /// Starts a new <see cref="Scope"/>. 
        /// </summary>
        /// <returns>A new <see cref="Scope"/>.</returns>
        public Scope BeginScope()
        {
            lock (syncRoot)
            {
                var scope = new Scope(this, currentScope);
                if (currentScope != null)
                {
                    currentScope.ChildScope = scope;
                }

                currentScope = scope;
                return scope;
            }
        }

        /// <summary>
        /// Ends the given <paramref name="scope"/> and updates the <see cref="CurrentScope"/> property.
        /// </summary>
        /// <param name="scope">The scope that is completed.</param>
        public void EndScope(Scope scope)
        {
            lock (syncRoot)
            {
                if (scope.ChildScope != null)
                {
                    throw new InvalidOperationException("Attempt to end a scope before all child scopes are completed.");
                }

                currentScope = scope.ParentScope;
                if (currentScope != null)
                {
                    currentScope.ChildScope = null;
                }
            }
        }
    }

    /// <summary>
    /// Represents a scope. 
    /// </summary>
    public class Scope : IDisposable
    {
        private readonly IList<IDisposable> disposableObjects = new List<IDisposable>();

        private readonly ScopeManager scopeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        /// <param name="scopeManager">The <see cref="scopeManager"/> that manages this <see cref="Scope"/>.</param>
        /// <param name="parentScope">The parent <see cref="Scope"/>.</param>
        public Scope(ScopeManager scopeManager, Scope parentScope)
        {
            this.scopeManager = scopeManager;
            ParentScope = parentScope;
        }

        /// <summary>
        /// Raised when the <see cref="Scope"/> is completed.
        /// </summary>
        public event EventHandler<EventArgs> Completed;

        /// <summary>
        /// Gets or sets the parent <see cref="Scope"/>.
        /// </summary>
        public Scope ParentScope { get; internal set; }

        /// <summary>
        /// Gets or sets the child <see cref="Scope"/>.
        /// </summary>
        public Scope ChildScope { get; internal set; }

        /// <summary>
        /// Registers the <paramref name="disposable"/> so that it is disposed when the scope is completed.
        /// </summary>
        /// <param name="disposable">The <see cref="IDisposable"/> object to register.</param>
        public void TrackInstance(IDisposable disposable)
        {
            disposableObjects.Add(disposable);
        }

        /// <summary>
        /// Disposes all instances tracked by this scope.
        /// </summary>
        public void Dispose()
        {
            DisposeTrackedInstances();
            OnCompleted();
        }

        private void DisposeTrackedInstances()
        {
            foreach (var disposableObject in disposableObjects)
            {
                disposableObject.Dispose();
            }
        }

        private void OnCompleted()
        {
            scopeManager.EndScope(this);
            var completedHandler = Completed;
            if (completedHandler != null)
            {
                completedHandler(this, new EventArgs());
            }
        }
    }

    /// <summary>
    /// Used at the assembly level to describe the composition root(s) for the target assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class CompositionRootTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRootTypeAttribute"/> class.
        /// </summary>
        /// <param name="compositionRootType">A <see cref="Type"/> that implements the <see cref="ICompositionRoot"/> interface.</param>
        public CompositionRootTypeAttribute(Type compositionRootType)
        {
            CompositionRootType = compositionRootType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> that implements the <see cref="ICompositionRoot"/> interface.
        /// </summary>
        public Type CompositionRootType { get; private set; }
    }

    /// <summary>
    /// Extracts concrete types from an <see cref="Assembly"/>.
    /// </summary>
    public class ConcreteTypeExtractor : ITypeExtractor
    {
        private static readonly List<Type> InternalTypes = new List<Type>();
       
        static ConcreteTypeExtractor()
        {        
            InternalTypes.Add(typeof(LambdaConstructionInfoBuilder));
            InternalTypes.Add(typeof(LambdaExpressionValidator));
            InternalTypes.Add(typeof(ConstructorDependency));
            InternalTypes.Add(typeof(PropertyDependency));
            InternalTypes.Add(typeof(ThreadSafeDictionary<,>));
            InternalTypes.Add(typeof(Scope));
            InternalTypes.Add(typeof(PerContainerLifetime));
            InternalTypes.Add(typeof(PerScopeLifetime));
            InternalTypes.Add(typeof(ScopeManager));
            InternalTypes.Add(typeof(ServiceRegistration));
            InternalTypes.Add(typeof(DecoratorRegistration));
            InternalTypes.Add(typeof(ServiceRequest));
            InternalTypes.Add(typeof(Registration));
            InternalTypes.Add(typeof(ServiceContainer));
            InternalTypes.Add(typeof(ConstructionInfo));
            InternalTypes.Add(typeof(LazyReadOnlyCollection<>));
            InternalTypes.Add(typeof(TypeConstructionInfoBuilder));
            InternalTypes.Add(typeof(ConstructionInfoProvider));
            InternalTypes.Add(typeof(ConstructionInfoBuilder));
            InternalTypes.Add(typeof(ConstructorSelector));
            InternalTypes.Add(typeof(PerContainerLifetime));
            InternalTypes.Add(typeof(PerContainerLifetime));
            InternalTypes.Add(typeof(PerRequestLifeTime));
            InternalTypes.Add(typeof(PropertySelector));
            InternalTypes.Add(typeof(AssemblyScanner));
            InternalTypes.Add(typeof(ConstructorDependencySelector));
            InternalTypes.Add(typeof(PropertyDependencySelector));
            InternalTypes.Add(typeof(CompositionRootTypeAttribute));
            InternalTypes.Add(typeof(ConcreteTypeExtractor));
            InternalTypes.Add(typeof(OpCodes));
            InternalTypes.Add(typeof(DynamicMethod));
            InternalTypes.Add(typeof(ILGenerator));
            InternalTypes.Add(typeof(LocalBuilder));
        }

        /// <summary>
        /// Extracts concrete types found in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to extract types.</param>
        /// <returns>A set of concrete types found in the given <paramref name="assembly"/>.</returns>
        public IEnumerable<Type> Execute(Assembly assembly)
        {            
            return assembly.GetTypes().Where(t => t.IsClass()
                                               && !t.IsNestedPrivate()
                                               && !t.IsAbstract() 
                                               && t.GetAssembly() != typeof(string).GetAssembly()                                          
                                               && !IsCompilerGenerated(t)).Except(InternalTypes);
        }

        private static bool IsCompilerGenerated(Type type)
        {
            return type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }
    }

    /// <summary>
    /// An assembly scanner that registers services based on the types contained within an <see cref="Assembly"/>.
    /// </summary>    
    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly ITypeExtractor typeExtractor;        
        private Assembly currentAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyScanner"/> class.
        /// </summary>
        /// <param name="typeExtractor">The <see cref="ITypeExtractor"/> that is responsible for 
        /// extracting types from the assembly being scanned.</param>
        public AssemblyScanner(ITypeExtractor typeExtractor)
        {
            this.typeExtractor = typeExtractor;
        }
                
        /// <summary>
        /// Scans the target <paramref name="assembly"/> and registers services found within the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="lifetimeFactory">The <see cref="ILifetime"/> factory that controls the lifetime of the registered service.</param>
        /// <param name="shouldRegister">A function delegate that determines if a service implementation should be registered.</param>
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            IEnumerable<Type> concreteTypes = GetConcreteTypes(assembly).ToList();
            var compositionRoots = concreteTypes.Where(t => typeof(ICompositionRoot).IsAssignableFrom(t)).ToList();
            if (compositionRoots.Count > 0 && !Equals(currentAssembly, assembly))
            {
                currentAssembly = assembly;
                ExecuteCompositionRoots(compositionRoots, serviceRegistry);
            }
            else
            {
                foreach (Type type in concreteTypes)
                {
                    BuildImplementationMap(type, serviceRegistry, lifetimeFactory, shouldRegister);
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
            if (implementingType.IsGenericTypeDefinition())
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
            Type baseType = concreteType;
            while (baseType != typeof(object) && baseType != null)
            {
                yield return baseType;
                baseType = baseType.GetBaseType();
            }
        }

        private IEnumerable<Type> GetConcreteTypes(Assembly assembly)
        {
            return typeExtractor.Execute(assembly);            
        }

        private void BuildImplementationMap(Type implementingType, IServiceRegistry serviceRegistry, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            Type[] interfaces = implementingType.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                if (shouldRegister(interfaceType, implementingType))
                {
                    RegisterInternal(interfaceType, implementingType, serviceRegistry, lifetimeFactory());
                }
            }

            foreach (Type baseType in GetBaseTypes(implementingType))
            {
                if (shouldRegister(baseType, implementingType))
                {
                    RegisterInternal(baseType, implementingType, serviceRegistry, lifetimeFactory());
                }
            }
        }

        private void RegisterInternal(Type serviceType, Type implementingType, IServiceRegistry serviceRegistry, ILifetime lifetime)
        {
            if (serviceType.IsGenericType() && serviceType.ContainsGenericParameters())
            {
                serviceType = serviceType.GetGenericTypeDefinition();
            }

            serviceRegistry.Register(serviceType, implementingType, GetServiceName(serviceType, implementingType), lifetime);
        }
    }

    /// <summary>
    /// Selects the properties that represents a dependency to the target <see cref="Type"/>.
    /// </summary>
    public class PropertySelector : IPropertySelector
    {
        /// <summary>
        /// Selects properties that represents a dependency from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to select the properties.</param>
        /// <returns>A list of properties that represents a dependency to the target <paramref name="type"/></returns>
        public IEnumerable<PropertyInfo> Execute(Type type)
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
            return propertyInfo.GetSetMethod() == null || propertyInfo.GetSetMethod().IsStatic || propertyInfo.GetSetMethod().IsPrivate;
        }
    }

    /// <summary>
    /// Represents a read-only collection that is initialized with a lazy <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of values in the collection.</typeparam>
    public class LazyReadOnlyCollection<T> : ICollection<T>
    {
        private readonly Lazy<IEnumerable<T>> lazyEnumerable;
        private readonly int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyReadOnlyCollection{T}"/> class.
        /// </summary>
        /// <param name="lazyEnumerable">A lazy <see cref="IEnumerable{T}"/> that represents the elements in the collection.</param>
        /// <param name="count">The number of elements in the <paramref name="lazyEnumerable"/>.</param>
        public LazyReadOnlyCollection(Lazy<IEnumerable<T>> lazyEnumerable, int count)
        {
            this.lazyEnumerable = lazyEnumerable;
            this.count = count;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return lazyEnumerable.Value.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// True if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(T item)
        {
            return lazyEnumerable.Value.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lazyEnumerable.Value.ToArray().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }        
    }    
}
