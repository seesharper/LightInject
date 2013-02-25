namespace LightInject.Mocking
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> interface.
    /// </summary>
    internal static class ServiceRegistryExtensions
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
            var key = Tuple.Create(serviceRegistry, typeof(TService), serviceName);
            ILifetime lifeTime = null;
            var serviceRegistration = serviceRegistry.AvailableServices.FirstOrDefault(sr => sr.ServiceType == typeof(TService) && sr.ServiceName == serviceName);
            if (serviceRegistration != null)
            {
                MockedServices.TryAdd(key, serviceRegistration);
                if (serviceRegistration.Lifetime != null)
                {
                    lifeTime = (ILifetime)Activator.CreateInstance(serviceRegistration.Lifetime.GetType());
                }
            }

            var mockServiceRegistration = new ServiceRegistration { ServiceType = typeof(TService), ServiceName = serviceName, Lifetime = lifeTime };
            Expression<Func<IServiceFactory, TService>> factoryExpression = factory => mockFactory();
            mockServiceRegistration.FactoryExpression = factoryExpression;
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
            if (MockedServices.TryGetValue(key, out serviceRegistration))
            {
                serviceRegistry.Register(serviceRegistration);
            }

            if (MockedServices.TryGetValue(key, out serviceRegistration))
            {
                MockedServices.TryRemove(key, out serviceRegistration);
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
    }
}
