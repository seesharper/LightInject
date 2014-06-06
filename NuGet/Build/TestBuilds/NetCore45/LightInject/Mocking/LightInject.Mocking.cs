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
    LightInject.Mocking version 1.0.0.4
    http://seesharper.github.io/LightInject/
    http://twitter.com/bernhardrichter       
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Mocking
{
    using System;    
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> interface.
    /// </summary>
    internal static class LightInjectMocking
    {
        private static readonly ThreadSafeDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration> MockedServices
            = new ThreadSafeDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration>();

        private static readonly ThreadSafeDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration> ServicesMocks
            = new ThreadSafeDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration>();

        /// <summary>
        /// Allows a service to be mocked using the given <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceType">The type of service to mock.</param>        
        /// <param name="implementingType">The type that represents the mock to be created.</param>
        public static void StartMocking(this IServiceRegistry serviceRegistry, Type serviceType, Type implementingType)
        {
            StartMocking(serviceRegistry, serviceType, string.Empty, implementingType);
        }

        /// <summary>
        /// Allows a named service to be mocked using the given <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceType">The type of service to mock.</param>
        /// <param name="serviceName">The name of the service to mock.</param>
        /// <param name="implementingType">The type that represents the mock to be created.</param>
        public static void StartMocking(this IServiceRegistry serviceRegistry, Type serviceType, string serviceName, Type implementingType)
        {
            Tuple<IServiceRegistry, Type, string> key = CreateServiceKey(serviceRegistry, serviceType, serviceName);
            ILifetime lifeTime = null;
            var serviceRegistration = GetExistingServiceRegistration(serviceRegistry, serviceType, serviceName);

            if (serviceRegistration != null)
            {
                MockedServices.TryAdd(key, serviceRegistration);
                lifeTime = CreateLifeTimeBasedOnExistingServiceRegistration(serviceRegistration);
            }

            var mockServiceRegistration = CreateTypeBasedMockServiceRegistration(serviceType, serviceName, implementingType, lifeTime);
            serviceRegistry.Register(mockServiceRegistration);
            ServicesMocks.TryAdd(key, mockServiceRegistration);
        }

        /// <summary>
        /// Allows a named service to be mocked using the given <paramref name="mockFactory"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to mock.</typeparam>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="mockFactory">The factory delegate that creates the mock instance.</param>
        /// <param name="serviceName">The name of the service to mock.</param>
        public static void StartMocking<TService>(this IServiceRegistry serviceRegistry, Func<TService> mockFactory, string serviceName) where TService : class
        {            
            Tuple<IServiceRegistry, Type, string> key = CreateServiceKey(serviceRegistry, typeof(TService), serviceName);
            ILifetime lifeTime = null;
            var serviceRegistration = GetExistingServiceRegistration(serviceRegistry, typeof(TService), serviceName);
            
            if (serviceRegistration != null)
            {
                MockedServices.TryAdd(key, serviceRegistration);
                lifeTime = CreateLifeTimeBasedOnExistingServiceRegistration(serviceRegistration);
            }

            var mockServiceRegistration = CreateFactoryBasedMockServiceRegistration(mockFactory, serviceName, lifeTime);
            serviceRegistry.Register(mockServiceRegistration);
            ServicesMocks.TryAdd(key, mockServiceRegistration);
        }
       
        /// <summary>
        /// Allows a service to be mocked using the given <paramref name="mockFactory"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to mock.</typeparam>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="mockFactory">The factory delegate that creates the mock instance.</param>        
        public static void StartMocking<TService>(this IServiceRegistry serviceRegistry, Func<TService> mockFactory) where TService : class
        {
            StartMocking(serviceRegistry, mockFactory, string.Empty);
        }

        /// <summary>
        /// Ends mocking the <typeparamref name="TService"/> with the given <paramref name="serviceName"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service for which to end mocking.</typeparam>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceName">The name of the service for which to end mocking.</param>
        public static void EndMocking<TService>(this IServiceRegistry serviceRegistry, string serviceName)
        {
            EndMocking(serviceRegistry, typeof(TService), serviceName);
        }

        /// <summary>
        /// Ends mocking the <paramref name="serviceType"/>.
        /// </summary>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceType">The type of service for which to end mocking.</param>        
        public static void EndMocking(this IServiceRegistry serviceRegistry, Type serviceType)
        {
            EndMocking(serviceRegistry, serviceType, string.Empty);
        }

        /// <summary>
        /// Ends mocking the <paramref name="serviceType"/> with the given <paramref name="serviceName"/>.
        /// </summary>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceType">The type of service for which to end mocking.</param>
        /// <param name="serviceName">The name of the service for which to end mocking.</param>
        public static void EndMocking(this IServiceRegistry serviceRegistry, Type serviceType, string serviceName)
        {
            var key = Tuple.Create(serviceRegistry, serviceType, serviceName);
            ServiceRegistration serviceRegistration;

            ServicesMocks.TryRemove(key, out serviceRegistration);

            if (serviceRegistration == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "No mocked service found. ServiceType: {0}, ServiceName: {1}", serviceType, serviceName));
            }

            serviceRegistration.IsReadOnly = false;

            if (MockedServices.TryRemove(key, out serviceRegistration))
            {
                serviceRegistry.Register(serviceRegistration);
            }           
        }

        /// <summary>
        /// Ends mocking the <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service for which to end mocking.</typeparam>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>        
        public static void EndMocking<TService>(this IServiceRegistry serviceRegistry)
        {
            EndMocking<TService>(serviceRegistry, string.Empty);
        }

        private static ServiceRegistration CreateFactoryBasedMockServiceRegistration<TService>(Func<TService> mockFactory, string serviceName, ILifetime lifeTime)
           where TService : class
        {
            var mockServiceRegistration = new ServiceRegistration { ServiceType = typeof(TService), ServiceName = serviceName, Lifetime = lifeTime, IsReadOnly = true };
            Expression<Func<IServiceFactory, TService>> factoryExpression = factory => mockFactory();
            mockServiceRegistration.FactoryExpression = factoryExpression;
            return mockServiceRegistration;
        }

        private static ServiceRegistration CreateTypeBasedMockServiceRegistration(Type serviceType, string serviceName, Type implementingType, ILifetime lifeTime)        
        {
            var mockServiceRegistration = new ServiceRegistration { ServiceType = serviceType, ImplementingType = implementingType, ServiceName = serviceName, Lifetime = lifeTime, IsReadOnly = true };            
            return mockServiceRegistration;
        }

        private static Tuple<IServiceRegistry, Type, string> CreateServiceKey(IServiceRegistry serviceRegistry, Type serviceType, string serviceName)
        {
            return Tuple.Create(serviceRegistry, serviceType, serviceName);
        }

        private static ILifetime CreateLifeTimeBasedOnExistingServiceRegistration(ServiceRegistration serviceRegistration)
        {
            ILifetime lifeTime = null;
            if (serviceRegistration.Lifetime != null)
            {
                lifeTime = (ILifetime)Activator.CreateInstance(serviceRegistration.Lifetime.GetType());
            }

            return lifeTime;
        }

        private static ServiceRegistration GetExistingServiceRegistration(IServiceRegistry serviceRegistry, Type serviceType, string serviceName)
        {            
            return serviceRegistry.AvailableServices.FirstOrDefault(sr => sr.ServiceType == serviceType && sr.ServiceName == serviceName);
        }
    }
}