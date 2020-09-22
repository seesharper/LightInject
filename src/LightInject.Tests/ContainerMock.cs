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

        public IServiceRegistry Register(Type serviceType, Type implementingType)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register(Type serviceType, Type implementingType, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register(Type serviceType, Type implementingType, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register(Type serviceType, Type implementingType, string serviceName, ILifetime lifetime)
        {
            return ((IInvocationContext<IServiceContainer>)this).Invoke(c => c.Register(serviceType, implementingType, serviceName, lifetime));
        }

        public IServiceRegistry Register<TService, TImplementation>() where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService, TImplementation>(ILifetime lifetime) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService, TImplementation>(string serviceName) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService, TImplementation>(string serviceName, ILifetime lifetime) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterInstance<TService>(TService instance)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterInstance<TService>(TService instance, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterInstance(Type serviceType, object instance)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterInstance(Type serviceType, object instance, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService>()
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService>(ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register(Type serviceType, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T, TService>(Func<IServiceFactory, T, TService> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T, TService>(Func<IServiceFactory, T, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register<TService>(Func<IServiceFactory, TService> factory, string serviceName, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterOrdered(Type serviceType, Type[] implementingTypes, Func<Type, ILifetime> lifetimeFactory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterOrdered(Type serviceType, Type[] implementingTypes, Func<Type, ILifetime> lifeTimeFactory,
            Func<int, string> serviceNameFormatter)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterFallback(Func<Type, string, bool> predicate, Func<ServiceRequest, object> factory, ILifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Register(ServiceRegistration serviceRegistration)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterAssembly(Assembly assembly)
        {
            return ((IInvocationContext<IServiceContainer>)this).Invoke(c => c.RegisterAssembly(assembly));
        }

        public IServiceRegistry RegisterAssembly(Assembly assembly, Func<Type, Type, bool> shouldRegister)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterAssembly(Assembly assembly, Func<ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister,
            Func<Type, Type, string> serviceNameProvider)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterFrom<TCompositionRoot>() where TCompositionRoot : ICompositionRoot, new()
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterFrom<TCompositionRoot>(TCompositionRoot compositionRoot) where TCompositionRoot : ICompositionRoot
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterConstructorDependency<TDependency>(Func<IServiceFactory, ParameterInfo, TDependency> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterConstructorDependency<TDependency>(Func<IServiceFactory, ParameterInfo, object[], TDependency> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterPropertyDependency<TDependency>(Func<IServiceFactory, PropertyInfo, TDependency> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry RegisterAssembly(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Decorate(Type serviceType, Type decoratorType, Func<ServiceRegistration, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Decorate(Type serviceType, Type decoratorType)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Decorate<TService, TDecorator>() where TDecorator : TService
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Decorate<TService>(Func<IServiceFactory, TService, TService> factory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Decorate(DecoratorRegistration decoratorRegistration)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Override(Func<ServiceRegistration, bool> serviceSelector, Func<IServiceFactory, ServiceRegistration, ServiceRegistration> serviceRegistrationFactory)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry Initialize(Func<ServiceRegistration, bool> predicate, Action<IServiceFactory, object> processor)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistry SetDefaultLifetime<T>() where T : ILifetime, new()
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

        public void Compile(Func<ServiceRegistration, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Compile()
        {
            throw new NotImplementedException();
        }

        public void Compile(Type serviceType, string serviceName = null)
        {
            throw new NotImplementedException();
        }

        public void Compile<TService>(string serviceType = null)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, Scope scope)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, Scope scope, string serviceName)
        {
            throw new NotImplementedException();
        }
    }
}