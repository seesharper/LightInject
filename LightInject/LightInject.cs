/*********************************************************************************   
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
    LightInject version 3.0.1.5
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Performance")]

namespace LightInject
{
    using System;    
#if WINDOWS_PHONE
    using System.Collections;
#endif
#if NET || NET45 || NETFX_CORE
    using System.Collections.Concurrent;
#endif
    using System.Collections.Generic;
#if NET45 || NETFX_CORE || WINDOWS_PHONE
    using System.Collections.ObjectModel;
#endif    
#if NET || NET45    
    using System.IO;
#endif    
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Defines a set of methods used to register services into the service container.
    /// </summary>
    internal interface IServiceRegistry
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
        void RegisterInstance<TService>(TService instance);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        void RegisterInstance<TService>(TService instance, string serviceName);

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="instance">The instance returned when this service is requested.</param>
        void RegisterInstance(Type serviceType, object instance);

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        void RegisterInstance(Type serviceType, object instance, string serviceName);

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
        /// Registers a concrete type as a service.
        /// </summary>
        /// <param name="serviceType">The concrete type to register.</param>
        void Register(Type serviceType);

        /// <summary>
        /// Registers a concrete type as a service.
        /// </summary>
        /// <param name="serviceType">The concrete type to register.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void Register(Type serviceType, ILifetime lifetime);
       
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
        void RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory);

        /// <summary>
        /// Registers a custom factory delegate used to create services that is otherwise unknown to the service container.
        /// </summary>
        /// <param name="predicate">Determines if the service can be created by the <paramref name="factory"/> delegate.</param>
        /// <param name="factory">Creates a service instance according to the <paramref name="predicate"/> predicate.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        void RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime);

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
        /// Registers services from the given <typeparamref name="TCompositionRoot"/> type.
        /// </summary>
        /// <typeparam name="TCompositionRoot">The type of <see cref="ICompositionRoot"/> to register from.</typeparam>
        void RegisterFrom<TCompositionRoot>() where TCompositionRoot : ICompositionRoot, new();

#if NET || NET45
        /// <summary>
        /// Registers services from assemblies in the base directory that matches the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern used to filter the assembly files.</param>
        void RegisterAssembly(string searchPattern);    
   
#endif
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
    internal interface IServiceFactory
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
    internal interface IServiceContainer : IServiceRegistry, IServiceFactory, IDisposable
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
    internal interface ILifetime
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
    internal interface ICompositionRoot
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
    internal interface ITypeExtractor
    {
        /// <summary>
        /// Extracts types found in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to extract types.</param>
        /// <returns>A set of types found in the given <paramref name="assembly"/>.</returns>
        Type[] Execute(Assembly assembly);
    }

    /// <summary>
    /// Represents a class that is responsible for selecting injectable properties.
    /// </summary>
    internal interface IPropertySelector
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
    internal interface IPropertyDependencySelector
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
    internal interface IConstructorDependencySelector
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
    internal interface IConstructionInfoBuilder
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
    internal interface IConstructionInfoProvider
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
    internal interface ILambdaConstructionInfoBuilder
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
    internal interface ITypeConstructionInfoBuilder
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
    internal interface IConstructorSelector
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
        void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister);

        /// <summary>
        /// Scans the target <paramref name="assembly"/> and executes composition roots found within the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        void Scan(Assembly assembly, IServiceRegistry serviceRegistry);
    }

    /// <summary>
    /// Represents a class that is responsible for instantiating and executing an <see cref="ICompositionRoot"/>.
    /// </summary>
    internal interface ICompositionRootExecutor
    {
        /// <summary>
        /// Creates an instance of the <paramref name="compositionRootType"/> and executes the <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="compositionRootType">The concrete <see cref="ICompositionRoot"/> type to be instantiated and executed.</param>
        void Execute(Type compositionRootType);
    }

    /// <summary>
    /// Represents a dynamic method skeleton for emitting the code needed to resolve a service instance.
    /// </summary>
    internal interface IMethodSkeleton
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

    /// <summary>
    /// Represents a class that is capable of providing the current <see cref="ScopeManager"/>.
    /// </summary>
    internal interface IScopeManagerProvider
    {
        /// <summary>
        /// Returns the <see cref="ScopeManager"/> that is responsible for managing scopes.
        /// </summary>
        /// <returns>The <see cref="ScopeManager"/> that is responsible for managing scopes.</returns>
        ScopeManager GetScopeManager();
    }

    /// <summary>
    /// Extends the <see cref="ImmutableHashTree{TKey,TValue}"/> class.
    /// </summary>
    internal static class ImmutableHashTreeExtensions
    {
        /// <summary>
        /// Searches for a <typeparamref name="TValue"/> using the given <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="tree">The target <see cref="ImmutableHashTree{TKey,TValue}"/>.</param>
        /// <param name="key">The key of the <see cref="ImmutableHashTree{TKey,TValue}"/> to get.</param>
        /// <returns>If found, the <typeparamref name="TValue"/> with the given <paramref name="key"/>, otherwise the default <typeparamref name="TValue"/>.</returns>
        public static TValue Search<TKey, TValue>(this ImmutableHashTree<TKey, TValue> tree, TKey key)
        {
            int hashCode = key.GetHashCode();

            while (tree.Height != 0 && tree.HashCode != hashCode)
            {
                tree = hashCode < tree.HashCode ? tree.Left : tree.Right;
            }

            if (!tree.IsEmpty && (ReferenceEquals(tree.Key, key) || Equals(tree.Key, key)))
            {
                return tree.Value;
            }

            if (tree.Duplicates.Items.Length > 0)
            {
                foreach (var keyValue in tree.Duplicates.Items)
                {
                    if (ReferenceEquals(keyValue.Key, key) || Equals(keyValue.Key, key))
                    {
                        return keyValue.Value;
                    }
                }
            }
            
            return default(TValue);
        }

        /// <summary>
        /// Adds a new element to the <see cref="ImmutableHashTree{TKey,TValue}"/>. 
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="tree">The target <see cref="ImmutableHashTree{TKey,TValue}"/>.</param>
        /// <param name="key">The key to be associated with the value.</param>
        /// <param name="value">The value to be added to the tree.</param>
        /// <returns>A new <see cref="ImmutableHashTree{TKey,TValue}"/> that contains the new key/value pair.</returns>
        internal static ImmutableHashTree<TKey, TValue> Add<TKey, TValue>(this ImmutableHashTree<TKey, TValue> tree, TKey key, TValue value)
        {
            if (tree.IsEmpty)
            {
                return new ImmutableHashTree<TKey, TValue>(key, value, tree, tree);
            }

            int hashCode = key.GetHashCode();

            if (hashCode > tree.HashCode)
            {
                return AddToRightBranch(tree, key, value);
            }

            if (hashCode < tree.HashCode)
            {
                return AddToLeftBranch(tree, key, value);
            }

            return new ImmutableHashTree<TKey, TValue>(key, value, tree);
        }

        private static ImmutableHashTree<TKey, TValue> AddToLeftBranch<TKey, TValue>(ImmutableHashTree<TKey, TValue> tree, TKey key, TValue value)
        {
            return new ImmutableHashTree<TKey, TValue>(tree.Key, tree.Value, tree.Left.Add(key, value), tree.Right);
        }

        private static ImmutableHashTree<TKey, TValue> AddToRightBranch<TKey, TValue>(ImmutableHashTree<TKey, TValue> tree, TKey key, TValue value)
        {
            return new ImmutableHashTree<TKey, TValue>(tree.Key, tree.Value, tree.Left, tree.Right.Add(key, value));
        }
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
            GetCurrentScopeManagerMethodInfo =
                new Lazy<MethodInfo>(
                    () => typeof(IScopeManagerProvider).GetMethod("GetScopeManager"));
            LazyTypes = new Lazy<ThreadSafeDictionary<Type, Type>>(() => new ThreadSafeDictionary<Type, Type>());
            FuncTypes = new Lazy<ThreadSafeDictionary<Type, Type>>(() => new ThreadSafeDictionary<Type, Type>());            
            LazyConstructors = new Lazy<ThreadSafeDictionary<Type, ConstructorInfo>>(() => new ThreadSafeDictionary<Type, ConstructorInfo>());
            GetInstanceMethods = new Lazy<ThreadSafeDictionary<Type, MethodInfo>>(() => new ThreadSafeDictionary<Type, MethodInfo>());
            GetNamedInstanceMethods = new Lazy<ThreadSafeDictionary<Type, MethodInfo>>(() => new ThreadSafeDictionary<Type, MethodInfo>());
            
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
            Type[] genericTypeArguments = serviceType.GetGenericTypeArguments();
            MethodInfo openGenericMethod =
                typeof(IServiceFactory).GetMethods().Single(m => m.Name == "GetInstance"
                    && m.GetGenericArguments().Length == genericTypeArguments.Length && m.GetParameters().All(p => p.Name != "serviceName"));

            MethodInfo closedGenericMethod = openGenericMethod.MakeGenericMethod(genericTypeArguments);

            return closedGenericMethod;
        }

        private static MethodInfo CreateNamedGetInstanceWithParametersMethod(Type serviceType)
        {
            Type[] genericArguments = serviceType.GetGenericTypeArguments();
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
        
        private static ConstructorInfo ResolveLazyConstructor(Type type)
        {
            return GetLazyType(type).GetConstructor(new[] { GetFuncType(type) });
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
#if NETFX_CORE || WINDOWS_PHONE      
        public static Type[] GetGenericTypeArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }
       
        public static Type[] GetGenericTypeParameters(this Type type)
        {
            return type.GetTypeInfo().GenericTypeParameters;
        }

        public static Type[] GetGenericParameterConstraints(this Type type)
        {
            return type.GetTypeInfo().GetGenericParameterConstraints();
        }

        public static MethodInfo[] GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods.ToArray();
        }

        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetRuntimeProperties().ToArray();
        }

        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        public static ConstructorInfo[] GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic && !c.IsStatic).ToArray();
        }
        
        public static MethodInfo GetPrivateMethod(this Type type, string name)
        {            
            return GetMethod(type, name);
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethod(name);
        }

        public static bool IsAssignableFrom(this Type type, Type fromType)
        {
            return type.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo());
        }

        public static bool IsDefined(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().IsDefined(attributeType, inherit);
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static bool IsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }

        public static bool IsNestedPrivate(this Type type)
        {
            return type.GetTypeInfo().IsNestedPrivate;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool ContainsGenericParameters(this Type type)
        {
            return type.GetTypeInfo().ContainsGenericParameters;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {                        
            return propertyInfo.SetMethod;
        }

        public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod;
        }

        public static Type[] GetTypes(this Assembly assembly)
        {
            return assembly.DefinedTypes.Select(t => t.AsType()).ToArray();
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }
        
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return
                GetConstructors(type).FirstOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
        }        
#endif
#if NET        
        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

#endif
#if NET || NET45         
        public static Type[] GetGenericTypeArguments(this Type type)
        {
            return type.GetGenericArguments();
        }

        public static Type[] GetGenericTypeParameters(this Type type)
        {
            return type.GetGenericArguments();            
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

        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly assembly, Type attributeType)
        {
            return assembly.GetCustomAttributes(attributeType, false).Cast<Attribute>();
        }
#endif

        public static bool IsEnumerableOfT(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsListOfT(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(IList<>);
        }

        public static bool IsCollectionOfT(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(ICollection<>);
        }
#if NET45 || NETFX_CORE || WINDOWS_PHONE
        
        public static bool IsReadOnlyCollectionOfT(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>);
        }

        public static bool IsReadOnlyListOfT(this Type serviceType)
        {
            return serviceType.IsGenericType() && serviceType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>);
        }
#endif
        
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

        public static Type GetElementType(Type type)
        {
            if (type.IsGenericType() && type.GetGenericTypeArguments().Count() == 1)
            {
                return type.GetGenericTypeArguments()[0];
            }
           
            return type.GetElementType();
        }
    }

    /// <summary>
    /// An ultra lightweight service container.
    /// </summary>
    internal class ServiceContainer : IServiceContainer
    {
        private const string UnresolvedDependencyError = "Unresolved dependency {0}";
        private readonly Func<Type, Type[], IMethodSkeleton> methodSkeletonFactory;
        private readonly ServiceRegistry<Action<IMethodSkeleton>> emitters = new ServiceRegistry<Action<IMethodSkeleton>>();
        private readonly object lockObject = new object();

        private readonly Storage<object> constants = new Storage<object>();
        
        private readonly Storage<FactoryRule> factoryRules = new Storage<FactoryRule>();
        private readonly Stack<Action<IMethodSkeleton>> dependencyStack = new Stack<Action<IMethodSkeleton>>();

        private readonly ServiceRegistry<ServiceRegistration> availableServices = new ServiceRegistry<ServiceRegistration>();

        private readonly Storage<DecoratorRegistration> decorators = new Storage<DecoratorRegistration>();

        private readonly Lazy<IConstructionInfoProvider> constructionInfoProvider;
        private readonly ICompositionRootExecutor compositionRootExecutor;

        private ImmutableHashTree<Type, Func<object[], object>> delegates =
            ImmutableHashTree<Type, Func<object[], object>>.Empty;        
        
        private ImmutableHashTree<Tuple<Type, string>, Func<object[], object>> namedDelegates =
            ImmutableHashTree<Tuple<Type, string>, Func<object[], object>>.Empty;

        private ImmutableHashTree<Type, Func<object[], object, object>> propertyInjectionDelegates =
            ImmutableHashTree<Type, Func<object[], object, object>>.Empty;
                        
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        public ServiceContainer()
        {            
            var concreteTypeExtractor = new CachedTypeExtractor(new ConcreteTypeExtractor());
            var compositionRootTypeExtractor = new CachedTypeExtractor(new CompositionRootTypeExtractor());
            compositionRootExecutor = new CompositionRootExecutor(this);
            AssemblyScanner = new AssemblyScanner(concreteTypeExtractor, compositionRootTypeExtractor, compositionRootExecutor);
            PropertyDependencySelector = new PropertyDependencySelector(new PropertySelector());
            ConstructorDependencySelector = new ConstructorDependencySelector();
            ConstructorSelector = new MostResolvableConstructorSelector(CanGetInstance);
            constructionInfoProvider = new Lazy<IConstructionInfoProvider>(CreateConstructionInfoProvider);
            methodSkeletonFactory = (returnType, parameterTypes) => new DynamicMethodSkeleton(returnType, parameterTypes);
            ScopeManagerProvider = new PerThreadScopeManagerProvider();
#if NET || NET45      
            AssemblyLoader = new AssemblyLoader();            
#endif            
        }

        /// <summary>
        /// Gets or sets the <see cref="IScopeManagerProvider"/> that is responsible 
        /// for providing the <see cref="ScopeManager"/> used to manage scopes.
        /// </summary>
        public IScopeManagerProvider ScopeManagerProvider { get; set; }

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
        /// Gets or sets the <see cref="IConstructorSelector"/> instance that is responsible 
        /// for selecting the constructor to be used when creating new service instances.
        /// </summary>
        public IConstructorSelector ConstructorSelector { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyScanner"/> instance that is responsible for scanning assemblies.
        /// </summary>
        public IAssemblyScanner AssemblyScanner { get; set; }
#if NET || NET45 || NETFX_CORE 

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyLoader"/> instance that is responsible for loading assemblies during assembly scanning. 
        /// </summary>
        public IAssemblyLoader AssemblyLoader { get; set; }
#endif

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
            return ScopeManagerProvider.GetScopeManager().BeginScope();
        }

        /// <summary>
        /// Injects the property dependencies for a given <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The target instance for which to inject its property dependencies.</param>
        /// <returns>The <paramref name="instance"/> with its property dependencies injected.</returns>
        public object InjectProperties(object instance)
        {            
            var type = instance.GetType();

            var del = propertyInjectionDelegates.Search(type);

            if (del == null)
            {
                del = CreatePropertyInjectionDelegate(type);
                propertyInjectionDelegates = propertyInjectionDelegates.Add(type, del);
            }

            return del(constants.Items, instance);            
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
        public void RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory)
        {
            factoryRules.Add(new FactoryRule { CanCreateInstance = predicate, Factory = factory });
        }

        /// <summary>
        /// Registers a custom factory delegate used to create services that is otherwise unknown to the service container.
        /// </summary>
        /// <param name="predicate">Determines if the service can be created by the <paramref name="factory"/> delegate.</param>
        /// <param name="factory">Creates a service instance according to the <paramref name="predicate"/> predicate.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime)
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
        /// Registers services from the given <typeparamref name="TCompositionRoot"/> type.
        /// </summary>
        /// <typeparam name="TCompositionRoot">The type of <see cref="ICompositionRoot"/> to register from.</typeparam>
        public void RegisterFrom<TCompositionRoot>() where TCompositionRoot : ICompositionRoot, new()
        {
            compositionRootExecutor.Execute(typeof(TCompositionRoot));
        }

#if NET || NET45 || NETFX_CORE
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
        /// <param name="serviceType">The concrete type to register.</param>
        public void Register(Type serviceType)
        {
            Register(serviceType, serviceType);
        }

        /// <summary>
        /// Registers a concrete type as a service.
        /// </summary>
        /// <param name="serviceType">The concrete type to register.</param>
        /// <param name="lifetime">The <see cref="ILifetime"/> instance that controls the lifetime of the registered service.</param>
        public void Register(Type serviceType, ILifetime lifetime)
        {
            Register(serviceType, serviceType, lifetime);
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
        public void RegisterInstance<TService>(TService instance, string serviceName)
        {
            RegisterInstance(typeof(TService), instance, serviceName);
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="instance">The instance returned when this service is requested.</param>
        public void RegisterInstance<TService>(TService instance)
        {
            RegisterInstance(typeof(TService), instance);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="instance">The instance returned when this service is requested.</param>
        public void RegisterInstance(Type serviceType, object instance)
        {
            RegisterInstance(serviceType, instance, string.Empty);
        }
       
        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        public void RegisterInstance(Type serviceType, object instance, string serviceName)
        {
            RegisterValue(serviceType, instance, serviceName);
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
            var instanceDelegate = delegates.Search(serviceType);
            if (instanceDelegate == null)
            {
                instanceDelegate = CreateDefaultDelegate(serviceType, throwError: true);
            }

            return instanceDelegate(constants.Items);            
        }

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="arguments">The arguments to be passed to the target instance.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, object[] arguments)
        {
            Func<object[], object> del = delegates.Search(serviceType);
            if (del == null)
            {
                del = CreateDelegate(serviceType, string.Empty, true);
                delegates = delegates.Add(serviceType, del);
            }
                                               
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
            var key = Tuple.Create(serviceType, serviceName);
            var instanceDelegate = namedDelegates.Search(key);
            if (instanceDelegate == null)
            {
                instanceDelegate = CreateNamedDelegate(key, throwError: true);
            }
            
            object[] constantsWithArguments = constants.Items.Concat(new object[] { arguments }).ToArray();

            return instanceDelegate(constantsWithArguments);                                    
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
            var instanceDelegate = delegates.Search(serviceType);
            if (instanceDelegate == null)
            {
                instanceDelegate = CreateDefaultDelegate(serviceType, throwError: false);
            }

            return instanceDelegate(constants.Items); 
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise null.</returns>
        public object TryGetInstance(Type serviceType, string serviceName)
        {
            var key = Tuple.Create(serviceType, serviceName);
            var instanceDelegate = namedDelegates.Search(key);
            if (instanceDelegate == null)
            {
                instanceDelegate = CreateNamedDelegate(key, throwError: false);
            }

            return instanceDelegate(constants.Items);       
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
            var key = Tuple.Create(serviceType, serviceName);
            var instanceDelegate = namedDelegates.Search(key);
            if (instanceDelegate == null)
            {
                instanceDelegate = CreateNamedDelegate(key, throwError: true);
            }

            return instanceDelegate(constants.Items);              
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
                        && cd.ServiceType.GetGenericTypeArguments()[0] == decoratorRegistration.ServiceType));
            return constructorDependency;
        }

        private static DecoratorRegistration CreateClosedGenericDecoratorRegistration(
            ServiceRegistration serviceRegistration, DecoratorRegistration openGenericDecorator)
        {
            var closedGenericDecoratorType =
                openGenericDecorator.ImplementingType.MakeGenericType(
                    serviceRegistration.ServiceType.GetGenericTypeArguments());                    
            var decoratorInfo = new DecoratorRegistration
            {
                ServiceType = serviceRegistration.ServiceType,
                ImplementingType = closedGenericDecoratorType,
                CanDecorate = openGenericDecorator.CanDecorate,
                Index = openGenericDecorator.Index
            };
            return decoratorInfo;
        }

        private static bool PassesGenericConstraints(Type implementingType, Type[] closedGenericArguments)
        {
            try
            {
                implementingType.MakeGenericType(closedGenericArguments);
            }
            catch (Exception)
            {
                return false;
            }

            return true;                                    
        }

        private void EmitEnumerable(IList<Action<IMethodSkeleton>> serviceEmitters, Type elementType, IMethodSkeleton dynamicMethodSkeleton)
        {
            EmitNewArray(serviceEmitters, elementType, dynamicMethodSkeleton);                       
        }

        private Func<object[], object, object> CreatePropertyInjectionDelegate(Type concreteType)
        {
            lock (lockObject)
            {
                IMethodSkeleton methodSkeleton = methodSkeletonFactory(typeof(object), new[] { typeof(object[]), typeof(object) });
                ConstructionInfo constructionInfo = GetContructionInfoForConcreteType(concreteType);
                var generator = methodSkeleton.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Castclass, concreteType);
                try
                {
                    EmitPropertyDependencies(constructionInfo, methodSkeleton);
                }
                catch (Exception)
                {
                    dependencyStack.Clear();
                    throw;
                }

                return (Func<object[], object, object>)methodSkeleton.CreateDelegate(typeof(Func<object[], object, object>));                                        
            }            
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
            return new TypeConstructionInfoBuilder(ConstructorSelector, ConstructorDependencySelector, PropertyDependencySelector);
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
                AssemblyScanner.Scan(serviceType.GetAssembly(), this);                
                emitter = GetRegisteredEmitMethod(serviceType, serviceName);                
            }

            if (emitter == null)
            {
                var rule = factoryRules.Items.FirstOrDefault(r => r.CanCreateInstance(serviceType, serviceName));
                if (rule != null)
                {
                    emitter = CreateServiceEmitterBasedOnFactoryRule(rule, serviceType, serviceName);
                }
            }

            return CreateEmitMethodWrapper(emitter, serviceType, serviceName);
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
                try
                {
                    emitter(ms);
                }
                finally
                {
                    dependencyStack.Pop();
                }
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
#if NET || NET45
            registeredDecorators.AddRange(GetDeferredDecoratorRegistrations(serviceRegistration));                      
#endif            
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

#if NET || NET45
        private IEnumerable<DecoratorRegistration> GetDeferredDecoratorRegistrations(
            ServiceRegistration serviceRegistration)
        {
            var registrations = new List<DecoratorRegistration>();
            
            var deferredDecorators =
                decorators.Items.Where(ds => ds.CanDecorate(serviceRegistration) && ds.HasDeferredImplementingType);            
            foreach (var deferredDecorator in deferredDecorators)
            {
                var decoratorRegistration = new DecoratorRegistration
                {
                    ServiceType = serviceRegistration.ServiceType,
                    ImplementingType =
                        deferredDecorator.ImplementingTypeFactory(this, serviceRegistration),
                    CanDecorate = sr => true, 
                    Index = deferredDecorator.Index
                };
                registrations.Add(decoratorRegistration);
            }

            return registrations;
        }

#endif
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
                    EmitConstructorDependency(dynamicMethodSkeleton, dependency);
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
        
        private void EmitConstructorDependency(IMethodSkeleton dynamicMethodSkeleton, Dependency dependency)
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
           
            if (serviceType.IsLazy())
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
            else if (serviceType.IsArray)
            {
                emitter = CreateArrayServiceEmitter(serviceType);
            }
#if NET45 || NETFX_CORE || WINDOWS_PHONE 
            else if (serviceType.IsReadOnlyCollectionOfT() || serviceType.IsReadOnlyListOfT())
            {
                emitter = CreateReadOnlyCollectionServiceEmitter(serviceType);
            }            
#endif
            else if (serviceType.IsListOfT())
            {
                emitter = CreateListServiceEmitter(serviceType);
            }
            else if (serviceType.IsCollectionOfT())
            {
                emitter = CreateListServiceEmitter(serviceType);
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
            Type[] genericArguments = serviceType.GetGenericTypeArguments();
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
            var returnType = serviceType.GetGenericTypeArguments().Single();
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
            Type actualServiceType = TypeHelper.GetElementType(serviceType);
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

        private Action<IMethodSkeleton> CreateArrayServiceEmitter(Type serviceType)
        {
            Action<IMethodSkeleton> enumerableEmitter = CreateEnumerableServiceEmitter(serviceType);

            MethodInfo openGenericToArrayMethod = typeof(Enumerable).GetMethod("ToArray");
            MethodInfo closedGenericToArrayMethod = openGenericToArrayMethod.MakeGenericMethod(TypeHelper.GetElementType(serviceType));
            return ms =>
                {
                    enumerableEmitter(ms);
                    ms.GetILGenerator().Emit(OpCodes.Call, closedGenericToArrayMethod);
                };
        }

        private Action<IMethodSkeleton> CreateListServiceEmitter(Type serviceType)
        {
            Action<IMethodSkeleton> enumerableEmitter = CreateEnumerableServiceEmitter(serviceType);

            MethodInfo openGenericToArrayMethod = typeof(Enumerable).GetMethod("ToList");
            MethodInfo closedGenericToListMethod = openGenericToArrayMethod.MakeGenericMethod(TypeHelper.GetElementType(serviceType));
            return ms =>
            {
                enumerableEmitter(ms);
                ms.GetILGenerator().Emit(OpCodes.Call, closedGenericToListMethod);
            };
        }
#if NET45 || NETFX_CORE || WINDOWS_PHONE
        
        private Action<IMethodSkeleton> CreateReadOnlyCollectionServiceEmitter(Type serviceType)
        {
            Type elementType = TypeHelper.GetElementType(serviceType);
            Type closedGenericReadOnlyCollectionType = typeof(ReadOnlyCollection<>).MakeGenericType(elementType);
            ConstructorInfo constructorInfo = closedGenericReadOnlyCollectionType.GetConstructors()[0];

            Action<IMethodSkeleton> listEmitter = CreateListServiceEmitter(serviceType);
            
            return ms =>
            {
                listEmitter(ms);
                ms.GetILGenerator().Emit(OpCodes.Newobj, constructorInfo);
            };
        }
#endif
        
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
            Type actualServiceType = serviceType.GetGenericTypeArguments()[0];
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
            
            Type[] closedGenericArguments = closedGenericServiceType.GetGenericTypeArguments();

            if (!PassesGenericConstraints(openGenericServiceRegistration.ImplementingType, closedGenericArguments))
            {
                return null;
            }

            Type closedGenericImplementingType =
                openGenericServiceRegistration.ImplementingType.MakeGenericType(closedGenericArguments);                    

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
                int scopeManagerProviderIndex = CreateScopeManagerProviderIndex();
                var getInstanceMethod = ReflectionHelper.LifetimeGetInstanceMethod;
                EmitLoadConstant(dynamicMethodSkeleton, lifetimeIndex, typeof(ILifetime));
                EmitLoadConstant(dynamicMethodSkeleton, instanceDelegateIndex, typeof(Func<object>));
                EmitLoadConstant(dynamicMethodSkeleton, scopeManagerProviderIndex, typeof(IScopeManagerProvider));
                generator.Emit(OpCodes.Callvirt, ReflectionHelper.GetCurrentScopeManagerMethod);
                generator.Emit(OpCodes.Callvirt, ReflectionHelper.GetCurrentScopeMethod);
                generator.Emit(OpCodes.Callvirt, getInstanceMethod);
                generator.Emit(serviceRegistration.ServiceType.IsValueType() ? OpCodes.Unbox_Any : OpCodes.Castclass, serviceRegistration.ServiceType);
            }
        }
       
        private int CreateScopeManagerProviderIndex()
        {
            return constants.Add(ScopeManagerProvider);
        }

        private int CreateInstanceDelegateIndex(Action<IMethodSkeleton> instanceEmitter, Type serviceType)
        {
            return constants.Add(WrapAsFuncDelegate(CreateDynamicMethodDelegate(instanceEmitter, serviceType)));
        }                

        private int CreateLifetimeIndex(ILifetime lifetime)
        {
            return constants.Add(lifetime);
        }

        private Func<object[], object> CreateDefaultDelegate(Type serviceType, bool throwError)
        {
            var instanceDelegate = CreateDelegate(serviceType, string.Empty, throwError);
            if (instanceDelegate == null)
            {
                return c => null;
            }

            Interlocked.Exchange(ref delegates, delegates.Add(serviceType, instanceDelegate));
            return instanceDelegate;
        }

        private Func<object[], object> CreateNamedDelegate(Tuple<Type, string> key, bool throwError)
        {
            var instanceDelegate = CreateDelegate(key.Item1, key.Item2, throwError);
            if (instanceDelegate == null)
            {
                return c => null;
            }

            Interlocked.Exchange(ref namedDelegates, namedDelegates.Add(key, instanceDelegate));
            return instanceDelegate;
        }

        private Func<object[], object> CreateDelegate(Type serviceType, string serviceName, bool throwError)
        {            
            lock (lockObject)
            {
                var serviceEmitter = GetEmitMethod(serviceType, serviceName);
                if (serviceEmitter == null && throwError)
                {
                    throw new InvalidOperationException(
                        string.Format("Unable to resolve type: {0}, service name: {1}", serviceType, serviceName));
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
                            string.Format("Unable to resolve type: {0}, service name: {1}", serviceType, serviceName),
                            ex);
                    }
                }

                return null;
            }
        }
    
        private void Invalidate()
        {
            Interlocked.Exchange(ref delegates, ImmutableHashTree<Type, Func<object[], object>>.Empty);
            Interlocked.Exchange(ref namedDelegates, ImmutableHashTree<Tuple<Type, string>, Func<object[], object>>.Empty);
            Interlocked.Exchange(ref propertyInjectionDelegates, ImmutableHashTree<Type, Func<object[], object, object>>.Empty);
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
            public T[] Items = new T[0];

            private readonly object lockObject = new object();

            public int Add(T value)
            {
                int index = Array.IndexOf(Items, value);
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
                    Items = new T[0];
                }
            }

            private int TryAddValue(T value)
            {
                lock (lockObject)
                {
                    int index = Array.IndexOf(Items, value);
                    if (index == -1)
                    {
                        index = AddValue(value);
                    }

                    return index;
                }
            }

            private int AddValue(T value)
            {
                int index = Items.Length;
                T[] snapshot = CreateSnapshot();
                snapshot[index] = value;
                Items = snapshot;
                return index;
            }

            private T[] CreateSnapshot()
            {
                var snapshot = new T[Items.Length + 1];
                Array.Copy(Items, snapshot, Items.Length);
                return snapshot;
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
#if NET || NET45
                dynamicMethod = new DynamicMethod(
                    "DynamicMethod", returnType, parameterTypes, typeof(ServiceContainer).Module, true);
#endif
#if NETFX_CORE || WINDOWS_PHONE 
                dynamicMethod = new DynamicMethod(returnType, parameterTypes);                    
#endif
            }
        }

        private class ServiceRegistry<T> : ThreadSafeDictionary<Type, ThreadSafeDictionary<string, T>>
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
    /// A <see cref="IScopeManagerProvider"/> that provides a <see cref="ScopeManager"/> per thread.
    /// </summary>
    internal class PerThreadScopeManagerProvider : IScopeManagerProvider
    {
        private readonly ThreadLocal<ScopeManager> scopeManagers =
            new ThreadLocal<ScopeManager>(() => new ScopeManager());

        /// <summary>
        /// Returns the <see cref="ScopeManager"/> that is responsible for managing scopes.
        /// </summary>
        /// <returns>The <see cref="ScopeManager"/> that is responsible for managing scopes.</returns>
        public ScopeManager GetScopeManager()
        {
            return scopeManagers.Value;
        }
    }

#if NET || NET45 || NETFX_CORE
    /// <summary>
    /// A thread safe dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    internal class ThreadSafeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
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
#endif
#if WINDOWS_PHONE
    /// <summary>
    /// A thread safe dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    internal class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly object syncObject = new object();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeDictionary{TKey,TValue}"/> class. 
        /// </summary>
        public ThreadSafeDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeDictionary{TKey,TValue}"/> class using the 
        /// given <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys</param>
        public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(comparer);
        }
       
        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
        public int Count
        {
            get { return dictionary.Count; }
        }

        /// <summary>
        /// Gets a collection that contains the values in the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
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

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value of the key/value pair at the specified index.</returns>
        public TValue this[TKey key]
        {
            set
            {
                lock (syncObject)
                {
                    dictionary[key] = value;
                }                    
            }                
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ThreadSafeDictionary{TKey,TValue}"/> by using the specified function, if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valuefactory">The function used to generate a value for the key</param>
        /// <returns>The value for the key.</returns>
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
        
        /// <summary>
        /// Uses the specified functions to add a key/value pair to the <see cref="ThreadSafeDictionary{TKey,TValue}"/> if the key does not already exist, 
        /// or to update a key/value pair in the <see cref="ThreadSafeDictionary{TKey,TValue}"/> if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
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

        /// <summary>
        /// Attempts to get the value associated with the specified key from the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the object from the <see cref="ThreadSafeDictionary{TKey,TValue}"/> that has the specified key, 
        /// or the default value of <typeparamref name="TValue"/>, if the operation failed.</param>
        public void TryGetValue(TKey key, out TValue value)
        {
            lock (syncObject)
            {
                dictionary.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// Attempts to remove and return the value that has the specified key from the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove and return.</param>
        /// <param name="value">When this method returns, contains the object removed from the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.</param>
        /// <returns>When this method returns, contains the object removed from the <see cref="ThreadSafeDictionary{TKey,TValue}"/>, 
        /// or the default value of the <typeparamref name="TValue"/> type if key does not exist.</returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            lock (syncObject)
            {                    
                if (dictionary.TryGetValue(key, out value))
                {                 
                    return true;
                }
                
                value = default(TValue);
                return false;                
            }
        }

        /// <summary>
        /// Attempts to add the specified key and value to the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>true if the key/value pair was added to the <see cref="ThreadSafeDictionary{TKey,TValue}"/> successfully; false if the key already exists.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            lock (syncObject)
            {
                if (dictionary.ContainsKey(key))
                {
                    return false;
                }
              
                dictionary.Add(key, value);
                return true;              
            }
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>        
        public void Clear()
        {
            lock (syncObject)
            {
                dictionary.Clear();    
            }                
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="ThreadSafeDictionary{TKey,TValue}"/></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (syncObject)
            {
                return dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ThreadSafeDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="ThreadSafeDictionary{TKey,TValue}"/></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }                

#endif

#if NETFX_CORE || WINDOWS_PHONE || SILVERLIGHT
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
    internal class ILGenerator
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
            else if (code == OpCodes.Ldarg_1)      
            {
                stack.Push(parameters[1]);
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
            if (code == OpCodes.Callvirt || code == OpCodes.Call)
            {
                var parameterCount = methodInfo.GetParameters().Length;
                Expression[] arguments = Pop(parameterCount);

                MethodCallExpression methodCallExpression;

                if (!methodInfo.IsStatic)
                {
                    var instance = stack.Pop();
                    methodCallExpression = Expression.Call(instance, methodInfo, arguments);    
                }
                else
                {
                    methodCallExpression = Expression.Call(null, methodInfo, arguments);
                }
                
                if (methodInfo.ReturnType == typeof(void))
                {
                    expressions.Add(methodCallExpression);
                }
                else
                {
                    stack.Push(methodCallExpression);    
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
    internal class LocalBuilder
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
  
#endif
    /// <summary>
    /// Selects the <see cref="ConstructionInfo"/> from a given type that represents the most resolvable constructor.
    /// </summary>
    internal class MostResolvableConstructorSelector : IConstructorSelector
    {
        private readonly Func<Type, string, bool> canGetInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="MostResolvableConstructorSelector"/> class.
        /// </summary>
        /// <param name="canGetInstance">A function delegate that determines if a service type can be resolved.</param>
        public MostResolvableConstructorSelector(Func<Type, string, bool> canGetInstance)
        {
            this.canGetInstance = canGetInstance;
        }

        /// <summary>
        /// Selects the constructor to be used when creating a new instance of the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="implementingType">The <see cref="Type"/> for which to return a <see cref="ConstructionInfo"/>.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance that represents the constructor to be used
        /// when creating a new instance of the <paramref name="implementingType"/>.</returns>
        public ConstructorInfo Execute(Type implementingType)
        {
            ConstructorInfo[] constructorCandidates = implementingType.GetConstructors();
            if (constructorCandidates.Length == 0)
            {
                throw new InvalidOperationException("Missing public constructor for Type: " + implementingType.FullName);
            }

            if (constructorCandidates.Length == 1)
            {
                return constructorCandidates[0];
            }

            foreach (var constructorCandidate in constructorCandidates.OrderByDescending(c => c.GetParameters().Count()))
            {
                ParameterInfo[] parameters = constructorCandidate.GetParameters();
                if (CanCreateParameterDependencies(parameters))
                {
                    return constructorCandidate;
                }
            }

            throw new InvalidOperationException("No resolvable constructor found for Type: " + implementingType.FullName);
        }

        /// <summary>
        /// Gets the service name based on the given <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterInfo"/> for which to get the service name.</param>
        /// <returns>The name of the service for the given <paramref name="parameter"/>.</returns>
        protected virtual string GetServiceName(ParameterInfo parameter)
        {
            return parameter.Name;
        }

        private bool CanCreateParameterDependencies(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.All(CanCreateParameterDependency);
        }

        private bool CanCreateParameterDependency(ParameterInfo parameterInfo)
        {
            return canGetInstance(parameterInfo.ParameterType, string.Empty) || canGetInstance(parameterInfo.ParameterType, GetServiceName(parameterInfo));
        }       
    }

    /// <summary>
    /// Selects the constructor dependencies for a given <see cref="ConstructorInfo"/>.
    /// </summary>
    internal class ConstructorDependencySelector : IConstructorDependencySelector
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
    internal class PropertyDependencySelector : IPropertyDependencySelector
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
    internal class TypeConstructionInfoBuilder : ITypeConstructionInfoBuilder
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
    internal class ConstructionInfoProvider : IConstructionInfoProvider
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
    internal class ConstructionInfoBuilder : IConstructionInfoBuilder
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
    internal class LambdaConstructionInfoBuilder : ILambdaConstructionInfoBuilder
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
    internal class LambdaExpressionValidator : ExpressionVisitor
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

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any sub-expression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            canParse = false;
            return base.VisitNewArray(node);
        }
    }

    /// <summary>
    /// Contains information about a service request that originates from a rule based service registration.
    /// </summary>    
    internal class ServiceRequest
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
    internal abstract class Registration
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
    internal class DecoratorRegistration : Registration
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
#if NET || NET45

        /// <summary>
        /// Gets a value indicating whether this registration has a deferred implementing type.
        /// </summary>
        public bool HasDeferredImplementingType
        {
            get
            {
                return ImplementingType == null && FactoryExpression == null;
            }
        }       
#endif
    }

    /// <summary>
    /// Contains information about a registered service.
    /// </summary>
    internal class ServiceRegistration : Registration
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
    internal class ConstructionInfo
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
    internal abstract class Dependency
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
    internal class PropertyDependency : Dependency
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
    internal class ConstructorDependency : Dependency
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
    internal class PerContainerLifetime : ILifetime, IDisposable
    {
        private readonly object syncRoot = new object();
        private volatile object singleton;

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
    internal class PerRequestLifeTime : ILifetime
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
    internal class PerScopeLifetime : ILifetime
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
    internal class ScopeManager
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
    internal class Scope : IDisposable
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
    internal class CompositionRootTypeAttribute : Attribute
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
    /// Extracts concrete <see cref="ICompositionRoot"/> implementations from an <see cref="Assembly"/>.
    /// </summary>
    internal class CompositionRootTypeExtractor : ITypeExtractor
    {
        /// <summary>
        /// Extracts concrete <see cref="ICompositionRoot"/> implementations found in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to extract types.</param>
        /// <returns>A set of concrete <see cref="ICompositionRoot"/> implementations found in the given <paramref name="assembly"/>.</returns>
        public Type[] Execute(Assembly assembly)
        {
            CompositionRootTypeAttribute[] compositionRootAttributes =
                assembly.GetCustomAttributes(typeof(CompositionRootTypeAttribute))
                        .Cast<CompositionRootTypeAttribute>().ToArray();
            
            if (compositionRootAttributes.Length > 0)
            {
                return compositionRootAttributes.Select(a => a.CompositionRootType).ToArray();
            }

            return assembly.GetTypes().Where(t => !t.IsAbstract() && typeof(ICompositionRoot).IsAssignableFrom(t)).ToArray();
        }
    }

    /// <summary>
    /// A <see cref="ITypeExtractor"/> cache decorator.
    /// </summary>
    internal class CachedTypeExtractor : ITypeExtractor
    {
        private readonly ITypeExtractor typeExtractor;

        private readonly ThreadSafeDictionary<Assembly, Type[]> cache =
            new ThreadSafeDictionary<Assembly, Type[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedTypeExtractor"/> class.
        /// </summary>
        /// <param name="typeExtractor">The target <see cref="ITypeExtractor"/>.</param>
        public CachedTypeExtractor(ITypeExtractor typeExtractor)
        {
            this.typeExtractor = typeExtractor;
        }

        /// <summary>
        /// Extracts types found in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to extract types.</param>
        /// <returns>A set of types found in the given <paramref name="assembly"/>.</returns>
        public Type[] Execute(Assembly assembly)
        {
            return cache.GetOrAdd(assembly, typeExtractor.Execute);
        }
    }

    /// <summary>
    /// Extracts concrete types from an <see cref="Assembly"/>.
    /// </summary>
    internal class ConcreteTypeExtractor : ITypeExtractor
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
#if NET || NET45
            InternalTypes.Add(typeof(AssemblyLoader));
#endif
            InternalTypes.Add(typeof(TypeConstructionInfoBuilder));
            InternalTypes.Add(typeof(ConstructionInfoProvider));
            InternalTypes.Add(typeof(ConstructionInfoBuilder));            
            InternalTypes.Add(typeof(MostResolvableConstructorSelector));            
            InternalTypes.Add(typeof(PerContainerLifetime));
            InternalTypes.Add(typeof(PerContainerLifetime));
            InternalTypes.Add(typeof(PerRequestLifeTime));
            InternalTypes.Add(typeof(PropertySelector));
            InternalTypes.Add(typeof(AssemblyScanner));
            InternalTypes.Add(typeof(ConstructorDependencySelector));
            InternalTypes.Add(typeof(PropertyDependencySelector));
            InternalTypes.Add(typeof(CompositionRootTypeAttribute));
            InternalTypes.Add(typeof(ConcreteTypeExtractor));
            InternalTypes.Add(typeof(CompositionRootExecutor));
            InternalTypes.Add(typeof(CompositionRootTypeExtractor));
            InternalTypes.Add(typeof(CachedTypeExtractor));
            InternalTypes.Add(typeof(ImmutableList<>));
            InternalTypes.Add(typeof(KeyValue<,>));
            InternalTypes.Add(typeof(ImmutableHashTree<,>));
            InternalTypes.Add(typeof(PerThreadScopeManagerProvider));            
#if NETFX_CORE || WINDOWS_PHONE            
            InternalTypes.Add(typeof(DynamicMethod));
            InternalTypes.Add(typeof(ILGenerator));
            InternalTypes.Add(typeof(LocalBuilder));
#endif
#if SILVERLIGHT
            InternalTypes.Add(typeof(ThreadLocal<>));
            InternalTypes.Add(typeof(ThreadLocal<>.Holder));
#endif
        }

        /// <summary>
        /// Extracts concrete types found in the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to extract types.</param>
        /// <returns>A set of concrete types found in the given <paramref name="assembly"/>.</returns>
        public Type[] Execute(Assembly assembly)
        {            
            return assembly.GetTypes().Where(t => t.IsClass()
                                               && !t.IsNestedPrivate()
                                               && !t.IsAbstract() 
                                               && t.GetAssembly() != typeof(string).GetAssembly()                                          
                                               && !IsCompilerGenerated(t)).Except(InternalTypes).ToArray();
        }

        private static bool IsCompilerGenerated(Type type)
        {
            return type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }
    }

    /// <summary>
    /// A class that is responsible for instantiating and executing an <see cref="ICompositionRoot"/>.
    /// </summary>
    internal class CompositionRootExecutor : ICompositionRootExecutor
    {
        private readonly IServiceRegistry serviceRegistry;

        private readonly IList<Type> executedCompositionRoots = new List<Type>();
        
        private readonly object syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionRootExecutor"/> class.
        /// </summary>
        /// <param name="serviceRegistry">The <see cref="IServiceRegistry"/> to be configured by the <see cref="ICompositionRoot"/>.</param>
        public CompositionRootExecutor(IServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;
        }

        /// <summary>
        /// Creates an instance of the <paramref name="compositionRootType"/> and executes the <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="compositionRootType">The concrete <see cref="ICompositionRoot"/> type to be instantiated and executed.</param>
        public void Execute(Type compositionRootType)
        {
            if (!executedCompositionRoots.Contains(compositionRootType))
            {
                lock (syncRoot)
                {
                    if (!executedCompositionRoots.Contains(compositionRootType))
                    {
                        executedCompositionRoots.Add(compositionRootType);
                        var compositionRoot = (ICompositionRoot)Activator.CreateInstance(compositionRootType);
                        compositionRoot.Compose(serviceRegistry);
                    }
                }
            }            
        }       
    }

    /// <summary>
    /// An assembly scanner that registers services based on the types contained within an <see cref="Assembly"/>.
    /// </summary>    
    internal class AssemblyScanner : IAssemblyScanner
    {
        private readonly ITypeExtractor concreteTypeExtractor;
        private readonly ITypeExtractor compositionRootTypeExtractor;
        private readonly ICompositionRootExecutor compositionRootExecutor;
        private Assembly currentAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyScanner"/> class.
        /// </summary>
        /// <param name="concreteTypeExtractor">The <see cref="ITypeExtractor"/> that is responsible for 
        /// extracting concrete types from the assembly being scanned.</param>
        /// <param name="compositionRootTypeExtractor">The <see cref="ITypeExtractor"/> that is responsible for 
        /// extracting <see cref="ICompositionRoot"/> implementations from the assembly being scanned.</param>
        /// <param name="compositionRootExecutor">The <see cref="ICompositionRootExecutor"/> that is 
        /// responsible for creating and executing an <see cref="ICompositionRoot"/>.</param>
        public AssemblyScanner(ITypeExtractor concreteTypeExtractor, ITypeExtractor compositionRootTypeExtractor, ICompositionRootExecutor compositionRootExecutor)
        {
            this.concreteTypeExtractor = concreteTypeExtractor;
            this.compositionRootTypeExtractor = compositionRootTypeExtractor;
            this.compositionRootExecutor = compositionRootExecutor;
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
            Type[] compositionRootTypes = GetCompositionRootTypes(assembly);            
            if (compositionRootTypes.Length > 0 && !Equals(currentAssembly, assembly))
            {
                currentAssembly = assembly;                
                ExecuteCompositionRoots(compositionRootTypes);
            }
            else
            {
                Type[] concreteTypes = GetConcreteTypes(assembly);
                foreach (Type type in concreteTypes)
                {
                    BuildImplementationMap(type, serviceRegistry, lifetimeFactory, shouldRegister);
                }
            }
        }

        /// <summary>
        /// Scans the target <paramref name="assembly"/> and executes composition roots found within the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
        {
            Type[] compositionRootTypes = GetCompositionRootTypes(assembly);
            if (compositionRootTypes.Length > 0 && !Equals(currentAssembly, assembly))
            {
                currentAssembly = assembly;
                ExecuteCompositionRoots(compositionRootTypes);
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

        private void ExecuteCompositionRoots(IEnumerable<Type> compositionRoots)
        {
            foreach (var compositionRoot in compositionRoots)
            {
                compositionRootExecutor.Execute(compositionRoot);
            }
        }

        private Type[] GetConcreteTypes(Assembly assembly)
        {
            return concreteTypeExtractor.Execute(assembly);            
        }

        private Type[] GetCompositionRootTypes(Assembly assembly)
        {
            return compositionRootTypeExtractor.Execute(assembly);
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
    internal class PropertySelector : IPropertySelector
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
#if NET || NET45

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
  
    /// <summary>
    /// Defines an immutable representation of a key and a value.  
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    internal sealed class KeyValue<TKey, TValue>
    {
        /// <summary>
        /// The key of this <see cref="KeyValue{TKey,TValue}"/> instance.
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        /// The key of this <see cref="KeyValue{TKey,TValue}"/> instance.
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValue{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="key">The key of this <see cref="KeyValue{TKey,TValue}"/> instance.</param>
        /// <param name="value">The value of this <see cref="KeyValue{TKey,TValue}"/> instance.</param>
        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// Represents a simple "add only" immutable list.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the list.</typeparam>
    internal sealed class ImmutableList<T>
    {
        /// <summary>
        /// Represents an empty <see cref="ImmutableList{T}"/>.
        /// </summary>
        public static readonly ImmutableList<T> Empty = new ImmutableList<T>();

        /// <summary>
        /// An array that contains the items in the <see cref="ImmutableList{T}"/>.
        /// </summary>
        public readonly T[] Items;

        /// <summary>
        /// The number of items in the <see cref="ImmutableList{T}"/>.
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableList{T}"/> class.
        /// </summary>
        /// <param name="previousList">The list from which the previous items are copied.</param>
        /// <param name="value">The value to be added to the list.</param>
        public ImmutableList(ImmutableList<T> previousList, T value)
        {
            Items = new T[previousList.Items.Length + 1];
            Array.Copy(previousList.Items, Items, previousList.Items.Length);
            Items[Items.Length - 1] = value;
            Count = Items.Length;
        }

        private ImmutableList()
        {
            Items = new T[0];
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableList{T}"/> that contains the new <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to be added to the new list.</param>
        /// <returns>A new <see cref="ImmutableList{T}"/> that contains the new <paramref name="value"/>.</returns>
        public ImmutableList<T> Add(T value)
        {
            return new ImmutableList<T>(this, value);
        }
    }

    /// <summary>
    /// A balanced binary search tree implemented as an AVL tree.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    internal sealed class ImmutableHashTree<TKey, TValue>
    {
        /// <summary>
        /// An empty <see cref="ImmutableHashTree{TKey,TValue}"/>.
        /// </summary>
        public static readonly ImmutableHashTree<TKey, TValue> Empty = new ImmutableHashTree<TKey, TValue>();

        /// <summary>
        /// The key of this <see cref="ImmutableHashTree{TKey,TValue}"/>.
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        /// The value of this <see cref="ImmutableHashTree{TKey,TValue}"/>.
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// The list of <see cref="KeyValue{TKey,TValue}"/> instances where the 
        /// <see cref="KeyValue{TKey,TValue}.Key"/> has the same hash code as this <see cref="ImmutableHashTree{TKey,TValue}"/>.
        /// </summary>
        public readonly ImmutableList<KeyValue<TKey, TValue>> Duplicates;

        /// <summary>
        /// The hash code retrieved from the <see cref="Key"/>.
        /// </summary>
        public readonly int HashCode;

        /// <summary>
        /// The left node of this <see cref="ImmutableHashTree{TKey,TValue}"/>.
        /// </summary>
        public readonly ImmutableHashTree<TKey, TValue> Left;

        /// <summary>
        /// The right node of this <see cref="ImmutableHashTree{TKey,TValue}"/>.
        /// </summary>
        public readonly ImmutableHashTree<TKey, TValue> Right;

        /// <summary>
        /// The height of this node.
        /// </summary>
        /// <remarks>
        /// An empty node has a height of 0 and a node without children has a height of 1.
        /// </remarks>
        public readonly int Height;

        /// <summary>
        /// Indicates that this <see cref="ImmutableHashTree{TKey,TValue}"/> is empty.
        /// </summary>
        public readonly bool IsEmpty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableHashTree{TKey,TValue}"/> class
        /// and adds a new entry in the <see cref="Duplicates"/> list.
        /// </summary>
        /// <param name="key">The key for this node.</param>
        /// <param name="value">The value for this node.</param>
        /// <param name="hashTree">The <see cref="ImmutableHashTree{TKey,TValue}"/> that contains existing duplicates.</param>
        public ImmutableHashTree(TKey key, TValue value, ImmutableHashTree<TKey, TValue> hashTree)
        {
            Duplicates = hashTree.Duplicates.Add(new KeyValue<TKey, TValue>(key, value));
            Key = hashTree.Key;
            Value = hashTree.Value;
            Height = hashTree.Height;
            HashCode = hashTree.HashCode;
            Left = hashTree.Left;
            Right = hashTree.Right;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableHashTree{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="key">The key for this node.</param>
        /// <param name="value">The value for this node.</param>
        /// <param name="left">The left node.</param>
        /// <param name="right">The right node.</param>
        public ImmutableHashTree(TKey key, TValue value, ImmutableHashTree<TKey, TValue> left, ImmutableHashTree<TKey, TValue> right)
        {
            var balance = left.Height - right.Height;

            if (balance == -2)
            {
                if (right.IsLeftHeavy())
                {
                    right = RotateRight(right);
                }

                // Rotate left
                Key = right.Key;
                Value = right.Value;
                Left = new ImmutableHashTree<TKey, TValue>(key, value, left, right.Left);
                Right = right.Right;
            }
            else if (balance == 2)
            {
                if (left.IsRightHeavy())
                {
                    left = RotateLeft(left);
                }

                // Rotate right                    
                Key = left.Key;
                Value = left.Value;
                Right = new ImmutableHashTree<TKey, TValue>(key, value, left.Right, right);
                Left = left.Left;
            }
            else
            {
                Key = key;
                Value = value;
                Left = left;
                Right = right;
            }

            Height = 1 + Math.Max(Left.Height, Right.Height);

            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Empty;

            HashCode = key.GetHashCode();
        }

        private ImmutableHashTree()
        {
            IsEmpty = true;
            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Empty;
        }

        private static ImmutableHashTree<TKey, TValue> RotateLeft(ImmutableHashTree<TKey, TValue> left)
        {
            return new ImmutableHashTree<TKey, TValue>(
                left.Right.Key,
                left.Right.Value,
                new ImmutableHashTree<TKey, TValue>(left.Key, left.Value, left.Right.Left, left.Left),
                left.Right.Right);
        }

        private static ImmutableHashTree<TKey, TValue> RotateRight(ImmutableHashTree<TKey, TValue> right)
        {
            return new ImmutableHashTree<TKey, TValue>(
                right.Left.Key,
                right.Left.Value,
                right.Left.Left,
                new ImmutableHashTree<TKey, TValue>(right.Key, right.Value, right.Left.Right, right.Right));
        }

        private bool IsLeftHeavy()
        {
            return Left.Height > Right.Height;
        }

        private bool IsRightHeavy()
        {
            return Right.Height > Left.Height;
        }
    }
}