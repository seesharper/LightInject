using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.Mocking
{
    using System.Collections.Concurrent;
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

        public static void StartMocking<TService>(this IServiceRegistry serviceRegistry, string serviceName, Func<TService> mockFactory) where TService : class
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

            var mockServiceRegistration = new ServiceRegistration();
            mockServiceRegistration.ServiceType = typeof(TService);
            mockServiceRegistration.ServiceName = serviceName;
            mockServiceRegistration.Lifetime = lifeTime;
            Expression<Func<IServiceFactory, TService>> factoryExpression = factory => mockFactory();
            mockServiceRegistration.FactoryExpression = factoryExpression;
            serviceRegistry.Register(mockServiceRegistration);
            ServicesMocks.TryAdd(key, mockServiceRegistration);
        }

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



        ///// <summary>
        ///// Allows a a service to be mocked using the given <paramref name="mockFactory"/>.
        ///// </summary>
        ///// <typeparam name="TService">The type of service to mock.</typeparam>
        ///// <param name="mockFactory">The factory delegate that creates the mock instance.</param>
        //void StartMocking<TService>(Func<TService> mockFactory);

        ///// <summary>
        ///// Allows a a service to be mocked using the given <paramref name="mockFactory"/>.
        ///// </summary>
        ///// <typeparam name="TService">The type of service to mock.</typeparam>
        ///// <param name="serviceName">The name of the service to mock.</param>
        ///// <param name="mockFactory">The factory delegate that creates the mock instance.</param>
        //void StartMocking<TService>(string serviceName, Func<TService> mockFactory);

        ///// <summary>
        ///// Stops mocking the <typeparamref name="TService"/> and installs the original <see cref="ServiceRegistration"/>.
        ///// </summary>
        ///// <typeparam name="TService">The type of service for which to stop mocking.</typeparam>
        ///// <param name="serviceName">The name of the service for which to stop mocking.</param>
        //void StopMocking<TService>(string serviceName);

        ///// <summary>
        ///// Stops mocking the <typeparamref name="TService"/> and installs the original <see cref="ServiceRegistration"/>.
        ///// </summary>
        ///// <typeparam name="TService">The type of service for which to stop mocking.</typeparam>
        //void StopMocking<TService>();

        ///// <summary>
        ///// Stops mocking all services and installs the original <see cref="ServiceRegistration"/> for all mocked services.
        ///// </summary>
        //void StopMocking();
    }
}
