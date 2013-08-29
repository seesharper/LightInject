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
   LightInject.Mocking version 1.0.0.2
   http://seesharper.github.io/LightInject/
   http://twitter.com/bernhardrichter       
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]

namespace LightInject.Mocking
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> interface.
    /// </summary>
    internal static class LightInjectMocking
    {
        private static readonly ConcurrentDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration> MockedServices
            = new ConcurrentDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration>();

        private static readonly ConcurrentDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration> ServicesMocks
            = new ConcurrentDictionary<Tuple<IServiceRegistry, Type, string>, ServiceRegistration>();

        /// <summary>
        /// Allows a named service to be mocked using the given <paramref name="mockFactory"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service to mock.</typeparam>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="mockFactory">The factory delegate that creates the mock instance.</param>
        /// <param name="serviceName">The name of the service to mock.</param>
        public static void StartMocking<TService>(this IServiceRegistry serviceRegistry, Func<TService> mockFactory, string serviceName) where TService : class
        {            
            Tuple<IServiceRegistry, Type, string> key = CreateServiceKey<TService>(serviceRegistry, serviceName);
            ILifetime lifeTime = null;
            var serviceRegistration = GetExistingServiceRegistration<TService>(serviceRegistry, serviceName);
            
            if (serviceRegistration != null)
            {
                MockedServices.TryAdd(key, serviceRegistration);
                lifeTime = CreateLifeTimeBasedOnExistingServiceRegistration(serviceRegistration);
            }

            var mockServiceRegistration = CreateMockServiceRegistration(mockFactory, serviceName, lifeTime);
            serviceRegistry.Register(mockServiceRegistration);
            ServicesMocks.TryAdd(key, mockServiceRegistration);
        }
       
        /// <summary>
        /// Allows a named service to be mocked using the given <paramref name="mockFactory"/>.
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
            var key = Tuple.Create(serviceRegistry, typeof(TService), serviceName);
            ServiceRegistration serviceRegistration;
            
            if (MockedServices.TryRemove(key, out serviceRegistration))
            {
                serviceRegistry.Register(serviceRegistration);
            }

            ServicesMocks.TryRemove(key, out serviceRegistration);
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

        private static ServiceRegistration CreateMockServiceRegistration<TService>(Func<TService> mockFactory, string serviceName, ILifetime lifeTime)
           where TService : class
        {
            var mockServiceRegistration = new ServiceRegistration { ServiceType = typeof(TService), ServiceName = serviceName, Lifetime = lifeTime, IsReadOnly = true };
            Expression<Func<IServiceFactory, TService>> factoryExpression = factory => mockFactory();
            mockServiceRegistration.FactoryExpression = factoryExpression;
            return mockServiceRegistration;
        }

        private static Tuple<IServiceRegistry, Type, string> CreateServiceKey<TService>(IServiceRegistry serviceRegistry, string serviceName) where TService : class
        {
            return Tuple.Create(serviceRegistry, typeof(TService), serviceName);
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

        private static ServiceRegistration GetExistingServiceRegistration<TService>(IServiceRegistry serviceRegistry, string serviceName) where TService : class
        {
            return serviceRegistry.AvailableServices.FirstOrDefault(sr => sr.ServiceType == typeof(TService) && sr.ServiceName == serviceName);
        }
    }
}
