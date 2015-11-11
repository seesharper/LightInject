namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using LightMock;

    internal class ContainerMock : MockContext<IServiceContainer>, IServiceContainer
    {        

        public IEnumerable<ServiceRegistration> AvailableServices { get; private set; }

        public void Register(Type serviceType, Type implementingType)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implementingType, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implementingType, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implementingType, string serviceName, ILifetime lifetime)
        {

            ((IInvocationContext<IServiceContainer>)this).Invoke(c => c.Register(serviceType, implementingType, serviceName, lifetime));
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void Register<TService, TImplementation>(ILifetime lifetime) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void Register<TService, TImplementation>(string serviceName) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void Register<TService, TImplementation>(string serviceName, ILifetime lifetime) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<TService>(TService instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<TService>(TService instance, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance(Type serviceType, object instance, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>()
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(Func<IServiceFactory, TService> factory)
        {
            throw new NotImplementedException();
        }

        public void Register<T, TService>(Func<IServiceFactory, T, TService> factory)
        {
            throw new NotImplementedException();
        }

        public void Register<T, TService>(Func<IServiceFactory, T, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory)
        {
            throw new NotImplementedException();
        }

        public void Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory)
        {
            throw new NotImplementedException();
        }

        public void Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory)
        {
            throw new NotImplementedException();
        }

        public void Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(Func<IServiceFactory, TService> factory, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(Func<IServiceFactory, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(Func<IServiceFactory, TService> factory, string serviceName, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory)
        {
            throw new NotImplementedException();
        }

        public void RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void Register(ServiceRegistration serviceRegistration)
        {
            throw new NotImplementedException();
        }

        public void RegisterAssembly(Assembly assembly)
        {
            ((IInvocationContext<IServiceContainer>)this).Invoke(c => c.RegisterAssembly(assembly));            
        }

        public void RegisterAssembly(Assembly assembly, Func<Type, Type, bool> shouldRegister)
        {
            throw new NotImplementedException();
        }

        public void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetime)
        {
            throw new NotImplementedException();
        }

        public void RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            throw new NotImplementedException();
        }

        public void RegisterFrom<TCompositionRoot>() where TCompositionRoot : ICompositionRoot, new()
        {
            throw new NotImplementedException();
        }

        public void RegisterConstructorDependency<TDependency>(Func<IServiceFactory, ParameterInfo, TDependency> factory)
        {
            throw new NotImplementedException();
        }

        public void RegisterPropertyDependency<TDependency>(Func<IServiceFactory, PropertyInfo, TDependency> factory)
        {
            throw new NotImplementedException();
        }

        public void RegisterAssembly(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public void Decorate(Type serviceType, Type decoratorType, Func<ServiceRegistration, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Decorate(Type serviceType, Type decoratorType)
        {
            throw new NotImplementedException();
        }

        public void Decorate<TService, TDecorator>() where TDecorator : TService
        {
            throw new NotImplementedException();
        }

        public void Decorate<TService>(Func<IServiceFactory, TService, TService> factory)
        {
            throw new NotImplementedException();
        }

        public void Decorate(DecoratorRegistration decoratorRegistration)
        {
            throw new NotImplementedException();
        }

        public void Override(Func<ServiceRegistration, bool> serviceSelector, Func<IServiceFactory, ServiceRegistration, ServiceRegistration> serviceRegistrationFactory)
        {
            throw new NotImplementedException();
        }

        public void Initialize(Func<ServiceRegistration, bool> predicate, Action<IServiceFactory, object> processor)
        {
            throw new NotImplementedException();
        }

        public Scope BeginScope()
        {
            throw new NotImplementedException();
        }

        public void EndCurrentScope()
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, object[] arguments)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, string serviceName, object[] arguments)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, string serviceName)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>()
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>(string serviceName)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T, TService>(T value)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T, TService>(T value, string serviceName)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2, string serviceName)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T1, T2, T3, TService>(T1 arg1, T2 arg2, T3 arg3)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T1, T2, T3, TService>(T1 arg1, T2 arg2, T3 arg3, string serviceName)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T1, T2, T3, T4, TService>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<T1, T2, T3, T4, TService>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, string serviceName)
        {
            throw new NotImplementedException();
        }

        public object TryGetInstance(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object TryGetInstance(Type serviceType, string serviceName)
        {
            throw new NotImplementedException();
        }

        public TService TryGetInstance<TService>()
        {
            throw new NotImplementedException();
        }

        public TService TryGetInstance<TService>(string serviceName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }

        public TService Create<TService>() where TService : class
        {
            throw new NotImplementedException();
        }

        public object Create(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IScopeManagerProvider ScopeManagerProvider { get; set; }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            throw new NotImplementedException();
        }

        public object InjectProperties(object instance)
        {
            throw new NotImplementedException();
        }
    }
}