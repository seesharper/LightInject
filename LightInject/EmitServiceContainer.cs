using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LightInject
{
    using System.Text;

    /// <summary>
    /// Specifies the lifecycle type of a registered service.
    /// </summary>
    public enum LifeCycleType
    {
        /// <summary>
        /// Specifies that a new instance is created for each request.
        /// </summary>
        Transient,
        
        /// <summary>
        /// Specifies that the same instance is returned across multiple requests.
        /// </summary>
        Singleton,

        /// <summary>
        /// Specifies that the same instance is returned throughout the dependency graph.
        /// </summary>
        Request
    }
    
    /// <summary>
    /// Defines a set of methods used to register services into the service container.
    /// </summary>
    public interface IServiceRegistry
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
        /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register(Type serviceType, Type implementingType, LifeCycleType lifeCycle);

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
        /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register(Type serviceType, Type implementingType, string serviceName, LifeCycleType lifeCycle);

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
        /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register<TService, TImplementation>(LifeCycleType lifeCycle) where TImplementation : TService;

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
        /// /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register<TService, TImplementation>(string serviceName, LifeCycleType lifeCycle) where TImplementation : TService;
        
        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <remarks>
        /// The <paramref name="instance"/> is registered as a singleton service.
        /// </remarks>
        void Register(Type serviceType, object instance);

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the given <paramref name="instance"/>. 
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="instance">The instance returned when this service is requested.</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <remarks>
        /// The <paramref name="instance"/> is registered as a singleton service.
        /// </remarks>
        void Register(Type serviceType, object instance, string serviceName);

        ///// <summary>
        ///// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        ///// </summary>
        ///// <typeparam name="TService">The service type to register.</typeparam>
        ///// <param name="instance">The instance returned when this service is requested.</param>
        //void Register<TService>(TService instance);

        ///// <summary>
        ///// Registers the <typeparamref name="TService"/> with the given <paramref name="instance"/>. 
        ///// </summary>
        ///// <typeparam name="TService">The service type to register.</typeparam>
        ///// <param name="instance">The instance returned when this service is requested.</param>
        ///// <param name="serviceName">The name of the service.</param>
        //void Register<TService>(TService instance, string serviceName);

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
        /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, LifeCycleType lifeCycle);

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
        /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, string serviceName, LifeCycleType lifeCycle);      
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

    /// <summary>
    /// Represents a factory class that is capable of returning an object instance.
    /// </summary>    
    public interface IFactory
    {
        /// <summary>
        /// Returns an instance of the given type indicated by the <paramref name="serviceRequest"/>. 
        /// </summary>        
        /// <param name="serviceRequest">The <see cref="ServiceRequest"/> instance that contains information about the service request.</param>
        /// <returns>An object instance corresponding to the <paramref name="serviceRequest"/>.</returns>
        object GetInstance(ServiceRequest serviceRequest);

        /// <summary>
        /// Determines if this factory can return an instance of the given <paramref name="serviceType"/> and <paramref name="serviceName"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns><b>true</b>, if the instance can be created, otherwise <b>false</b>.</returns>
        bool CanGetInstance(Type serviceType, string serviceName);
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
    /// Represents a class that is responsible for selecting properties that represents a dependecy to the target <see cref="Type"/>.
    /// </summary>
    public interface IPropertySelector
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
        void Scan(Assembly assembly, IServiceRegistry serviceRegistry);
    }

    /// <summary>
    /// Represents an inversion of control container.
    /// </summary>
    public interface IServiceContainer : IServiceRegistry, IServiceFactory
    {
        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>        
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        void Scan(Assembly assembly);

        /// <summary>
        /// Registers services from assemblies in the base directory that mathes the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern used to filter the assembly files.</param>
        void Scan(string searchPattern);
    }

    /// <summary>
    /// An ultra lightweight service container.
    /// </summary>
    public class EmitServiceContainer : IServiceContainer
    {
        private const string ConstructorInjectionError = "Unresolved dependency {0}";        
        private static readonly MethodInfo GetInstanceMethod;
        private readonly ServiceRegistry<Action<DynamicMethodInfo>> services = new ServiceRegistry<Action<DynamicMethodInfo>>();
        private readonly ServiceRegistry<OpenGenericServiceInfo> openGenericServices = new ServiceRegistry<OpenGenericServiceInfo>();        
        private readonly DelegateRegistry<Type> delegates = new DelegateRegistry<Type>();
        private readonly DelegateRegistry<Tuple<Type, string>> namedDelegates = new DelegateRegistry<Tuple<Type, string>>();        
        private readonly ThreadSafeDictionary<Type, ServiceInfo> implementations = new ThreadSafeDictionary<Type, ServiceInfo>();
        private readonly ThreadSafeDictionary<Type, Lazy<object>> singletons = new ThreadSafeDictionary<Type, Lazy<object>>();        
        private readonly List<object> constants = new List<object>();

        
        static EmitServiceContainer()
        {
            GetInstanceMethod = typeof(IFactory).GetMethod("GetInstance");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitServiceContainer"/> class.
        /// </summary>
        public EmitServiceContainer()
        {
            AssemblyScanner = new AssemblyScanner();
            PropertySelector = new PropertySelector();
            AssemblyLoader = new AssemblyLoader();
        }

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyScanner"/> instance that is reponsible for scanning assemblies.
        /// </summary>
        public IAssemblyScanner AssemblyScanner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPropertySelector"/> instance that is reponsible selecting the properties
        /// that represents a dependency for a given <see cref="Type"/>.
        /// </summary>
        public IPropertySelector PropertySelector { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyLoader"/> instance that is reponsible for loading assemblies during assembly scanning. 
        /// </summary>
        public IAssemblyLoader AssemblyLoader { get; set; }
        
        public void Scan(Assembly assembly)
        {
            AssemblyScanner.Scan(assembly, this);
        }

        public void Scan(string searchPattern)
        {
            foreach (Assembly assembly in AssemblyLoader.Load(searchPattern))
            {
                Scan(assembly);
            }            
        }

        public void Register(Type serviceType, Type implementingType, LifeCycleType lifeCycle)
        {
            RegisterService(serviceType, implementingType, lifeCycle, string.Empty);
        }

        public void Register(Type serviceType, Type implementingType, string serviceName, LifeCycleType lifeCycle)
        {
            RegisterService(serviceType, implementingType, lifeCycle, serviceName);
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation));
        }

        public void Register<TService, TImplementation>(LifeCycleType lifeCycle) where TImplementation : TService
        {
            Register(typeof(TService), typeof(TImplementation), lifeCycle);
        }

        public void Register<TService, TImplementation>(string serviceName) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void Register<TService, TImplementation>(string serviceName, LifeCycleType lifeCycle) where TImplementation : TService
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, object value)
        {
            RegisterValue(serviceType, value, string.Empty);
        }

        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory, LifeCycleType lifeCycle)
        {
            RegisterServiceFromLambdaExpression(factory, lifeCycle, string.Empty);
        }

        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory, string serviceName)
        {
            RegisterServiceFromLambdaExpression(factory, LifeCycleType.Transient, serviceName);
        }

        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory, string serviceName, LifeCycleType lifeCycle)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, object value, string serviceName)
        {
            RegisterValue(serviceType, value, serviceName);
        }

        public void Register<TService>(TService instance)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(TService instance, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register<TService>(Expression<Func<IServiceFactory, TService>> factory)
        {
            RegisterServiceFromLambdaExpression(factory, LifeCycleType.Transient, string.Empty);
        }

        public void Register(Type serviceType, Type implementingType, string serviceName)
        {
            RegisterService(serviceType, implementingType, LifeCycleType.Transient, serviceName);
        }
        
        public void Register(Type serviceType, Type implementingType)
        {
            RegisterService(serviceType, implementingType, LifeCycleType.Transient, string.Empty);
        }

        public object GetInstance(Type serviceType)
        {
            return delegates.GetOrAdd(
                serviceType,
                t =>
                new Lazy<Func<List<object>, object>>(() => CreateDelegate(serviceType, string.Empty))).Value(constants);
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

        public object GetInstance(Type serviceType, string serviceName)
        {
            return namedDelegates.GetOrAdd(
                Tuple.Create(serviceType, serviceName),
                t =>
                new Lazy<Func<List<object>, object>>(() => CreateDelegate(serviceType, serviceName))).Value(constants);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (IEnumerable<object>)GetInstance(typeof(IEnumerable<>).MakeGenericType(serviceType));
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return GetInstance<IEnumerable<TService>>();
        }

        private static Func<List<object>, object> CreateDynamicMethodDelegate(Action<DynamicMethodInfo> serviceEmitter, Type serviceType)
        {
            var dynamicMethodInfo = new DynamicMethodInfo();
            serviceEmitter(dynamicMethodInfo);
            if (serviceType.IsValueType)
                dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Box, serviceType);
            return dynamicMethodInfo.CreateDelegate();
        }

        private static void EmitLoadConstant(DynamicMethodInfo dynamicMethodInfo, int index, Type type)
        {
            MethodInfo method = typeof(List<object>).GetMethod("get_Item");
            var generator = dynamicMethodInfo.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, index);
            generator.Emit(OpCodes.Callvirt, method);
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

        private static void EmitCallCustomFactory(DynamicMethodInfo dynamicMethodInfo, int serviceRequestConstantIndex, int factoryConstantIndex, Type serviceType)
        {
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            EmitLoadConstant(dynamicMethodInfo, factoryConstantIndex, typeof(IFactory));
            EmitLoadConstant(dynamicMethodInfo, serviceRequestConstantIndex, typeof(ServiceRequest));
            generator.Emit(OpCodes.Callvirt, GetInstanceMethod);
            if (serviceType.IsValueType)
                generator.Emit(OpCodes.Unbox_Any, serviceType);
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

        private static bool IsFactory(Type type)
        {
            return typeof(IFactory).IsAssignableFrom(type);
        }

        private static IEnumerable<ConstructorDependency> GetConstructorDependencies(ConstructorInfo constructorInfo)
        {
            return
                constructorInfo.GetParameters().OrderBy(p => p.Position).Select(
                    p => new ConstructorDependency { ServiceName = string.Empty, ServiceType = p.ParameterType, Parameter = p });
        }

        private static void ThrowUnresolvedConstructorDependencyException(ConstructorDependency dependency)
        {
            throw new InvalidOperationException(string.Format(ConstructorInjectionError, dependency));                                    
        }

        private ServiceInfo CreateServiceInfo(Type implementingType)
        {
            var serviceInfo = new ServiceInfo();
            ConstructorInfo constructorInfo = GetConstructorWithTheMostParameters(implementingType);
            serviceInfo.Constructor = constructorInfo;
            serviceInfo.ConstructorDependencies.AddRange(GetConstructorDependencies(constructorInfo));
            serviceInfo.PropertyDependencies.AddRange(GetPropertyDependencies(implementingType));
            return serviceInfo;
        }

        private IEnumerable<PropertyDependecy> GetPropertyDependencies(Type implementingType)
        {
            return GetInjectableProperties(implementingType).Select(
                p => new PropertyDependecy { PropertySetter = p.GetSetMethod(), ServiceName = string.Empty, ServiceType = p.PropertyType });
        }

        private IEnumerable<PropertyInfo> GetInjectableProperties(Type implementingType)
        {
            return PropertySelector.Select(implementingType);            
        }

        private ServiceInfo CreateServiceInfoFromExpression(LambdaExpression lambdaExpression)
        {
            var methodCallVisitor = new ServiceInfoBuilder();
            ServiceInfo serviceInfo = methodCallVisitor.Build(lambdaExpression);
            return serviceInfo;
        }
        
        private Action<DynamicMethodInfo> GetServiceEmitter(Type serviceType, string serviceName)
        {
            var registrations = GetServiceRegistrations(serviceType);
            Action<DynamicMethodInfo> emitter;

            registrations.TryGetValue(serviceName, out emitter);

            if (emitter == null)
                emitter = ResolveUnknownServiceEmitter(serviceType, serviceName);

            IFactory factory = GetCustomFactory(serviceType, serviceName);
            if (factory != null)
            {
                if (emitter != null)
                {
                    var del = CreateDynamicMethodDelegate(emitter, typeof(IFactory));
                    emitter = CreateServiceEmitterBasedOnCustomFactory(serviceType, serviceName, factory, () => del(constants));    
                }
                else
                {
                    return CreateServiceEmitterBasedOnCustomFactory(serviceType, serviceName, factory, null);
                }                
            }

            if (emitter != null) registrations.AddOrUpdate(serviceName, s => emitter, (s, d) => emitter);

            if (emitter == null)
                return null;

            return services[serviceType][serviceName];
        }

        private void EmitRequestInstance(Type implementingType, DynamicMethodInfo dynamicMethodInfo)
        {
            if (!dynamicMethodInfo.ContainsLocalVariable(implementingType))
            {
                EmitNewInstance(implementingType, dynamicMethodInfo);
                dynamicMethodInfo.EmitStoreLocalVariable(implementingType);
            }

            dynamicMethodInfo.EmitLoadLocalVariable(implementingType);
        }

        private void EmitNewInstance(Type implementingType, DynamicMethodInfo dynamicMethodInfo)
        {
            ServiceInfo serviceInfo = GetServiceInfo(implementingType);
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            EmitConstructorDependencies(serviceInfo, dynamicMethodInfo);            
            generator.Emit(OpCodes.Newobj, serviceInfo.Constructor);
            EmitPropertyDependencies(serviceInfo, dynamicMethodInfo);
        }

        private void EmitConstructorDependencies(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            foreach (ConstructorDependency dependency in serviceInfo.ConstructorDependencies)
            {
                if (dependency.Expression != null)
                {
                    var lambda = Expression.Lambda(dependency.Expression, new ParameterExpression[] { }).Compile();
                    MethodInfo methodInfo = lambda.GetType().GetMethod("Invoke");
                    EmitLoadConstant(dynamicMethodInfo, GetConstantIndex(lambda), lambda.GetType());
                    generator.Emit(OpCodes.Callvirt, methodInfo);
                }
                else
                {
                    var emitter = GetServiceEmitter(dependency.ServiceType, dependency.ServiceName);
                    if (emitter == null)
                        ThrowUnresolvedConstructorDependencyException(dependency);
                    emitter(dynamicMethodInfo);
                }
            }
        }
        
        private void EmitPropertyDependencies(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            ILGenerator generator = dynamicMethodInfo.GetILGenerator();
            LocalBuilder instance = generator.DeclareLocal(serviceInfo.Constructor.DeclaringType);
            generator.Emit(OpCodes.Stloc, instance);
            foreach (var propertyDependency in serviceInfo.PropertyDependencies)
            {
                generator.Emit(OpCodes.Ldloc, instance);
                if (propertyDependency.Expression != null)
                {
                    var lambda = Expression.Lambda(propertyDependency.Expression, new ParameterExpression[] { }).Compile();
                    MethodInfo methodInfo = lambda.GetType().GetMethod("Invoke");
                    EmitLoadConstant(dynamicMethodInfo, GetConstantIndex(lambda), lambda.GetType());
                    generator.Emit(OpCodes.Callvirt, methodInfo);
                }
                else
                {
                    GetServiceEmitter(propertyDependency.ServiceType, propertyDependency.ServiceName)(dynamicMethodInfo);
                }

                dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Callvirt, propertyDependency.PropertySetter);
            }

            generator.Emit(OpCodes.Ldloc, instance);
        }

        private Action<DynamicMethodInfo> ResolveUnknownServiceEmitter(Type serviceType, string serviceName)
        {
            if (IsFunc(serviceType)) 
                return CreateServiceEmitterBasedOnFuncServiceRequest(serviceType, false);

            if (IsEnumerableOfT(serviceType)) 
                return CreateEnumerableServiceEmitter(serviceType);

            if (IsFuncWithStringArgument(serviceType)) 
                return CreateServiceEmitterBasedOnFuncServiceRequest(serviceType, true);

            if (CanRedirectRequestForDefaultServiceToSingleNamedService(serviceType, serviceName)) 
                return CreateServiceEmitterBasedOnSingleNamedInstance(serviceType);

            if (IsClosedGeneric(serviceType)) 
                return CreateServiceEmitterBasedOnClosedGenericServiceRequest(serviceType, serviceName);
           
            return null;
        }

        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnCustomFactory(Type serviceType, string serviceName, IFactory factory, Func<object> proceed)
        {
            int serviceRequestConstantIndex = CreateServiceRequestConstant(serviceType, serviceName, proceed);
            int factoryConstantIndex = GetConstantIndex(factory);
            return dmi => EmitCallCustomFactory(dmi, serviceRequestConstantIndex, factoryConstantIndex, serviceType);
        }

        private int CreateServiceRequestConstant(Type serviceType, string serviceName, Func<object> proceed)
        {
            var serviceRequest = new ServiceRequest { ServiceType = serviceType, ServiceName = serviceName, Proceed = proceed };
            return GetConstantIndex(serviceRequest);
        }

        private IFactory GetCustomFactory(Type serviceType, string serviceName)
        {
            if (IsFactory(serviceType) ||
                (IsEnumerableOfT(serviceType) && IsFactory(serviceType.GetGenericArguments().First())))
                return null;
            return GetInstance<IEnumerable<IFactory>>().FirstOrDefault(f => f.CanGetInstance(serviceType, serviceName));
        }

        private Action<DynamicMethodInfo> CreateEnumerableServiceEmitter(Type serviceType)
        {
            Type actualServiceType = serviceType.GetGenericArguments()[0];
            IList<Action<DynamicMethodInfo>> serviceEmitters = GetServiceRegistrations(actualServiceType).Values.ToList();
            var dynamicMethodInfo = new DynamicMethodInfo();
            EmitEnumerable(serviceEmitters, actualServiceType, dynamicMethodInfo);
            var array = dynamicMethodInfo.CreateDelegate()(constants);
            int index = GetConstantIndex(array);
            return dmi => EmitLoadConstant(dmi, index, actualServiceType.MakeArrayType());
        }

        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnFuncServiceRequest(Type serviceType, bool namedService)
        {
            var actualServiceType = serviceType.GetGenericArguments().Last();
            var methodInfo = typeof(EmitServiceContainer).GetMethod("CreateFuncGetInstanceDelegate", BindingFlags.Instance | BindingFlags.NonPublic);
            var del = methodInfo.MakeGenericMethod(actualServiceType).Invoke(this, new object[] { namedService });
            var constantIndex = GetConstantIndex(del);
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

        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnClosedGenericServiceRequest(Type serviceType, string serviceName)
        {
            Type openGenericType = serviceType.GetGenericTypeDefinition();
            
            OpenGenericServiceInfo openGenericServiceInfo = GetOpenGenericTypeInfo(openGenericType, serviceName);
            if (openGenericServiceInfo == null) 
                return null;
        
            Type closedGenericType = openGenericServiceInfo.ImplementingType.MakeGenericType(serviceType.GetGenericArguments());
            return dmi => openGenericServiceInfo.EmitMethod(dmi, closedGenericType);                        
        }

        private OpenGenericServiceInfo GetOpenGenericTypeInfo(Type serviceType, string serviceName)
        {
            var openGenericRegistrations = GetOpenGenericRegistrations(serviceType);
            if (CanRedirectRequestForDefaultOpenGenericServiceToSingleNamedService(serviceType, serviceName))
                return openGenericRegistrations.First().Value;

            OpenGenericServiceInfo openGenericServiceInfo;
            openGenericRegistrations.TryGetValue(serviceName, out openGenericServiceInfo);
            return openGenericServiceInfo;
        }

        private Action<DynamicMethodInfo> CreateServiceEmitterBasedOnSingleNamedInstance(Type serviceType)
        {
            return GetServiceEmitter(serviceType, GetServiceRegistrations(serviceType).First().Key);
        }

        private bool CanRedirectRequestForDefaultServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetServiceRegistrations(serviceType).Count == 1;
        }

        private bool CanRedirectRequestForDefaultOpenGenericServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetOpenGenericRegistrations(serviceType).Count == 1;
        }

        private ServiceInfo GetServiceInfo(Type implementingType)
        {
            return implementations.GetOrAdd(implementingType, CreateServiceInfo);
        }

        private ThreadSafeDictionary<string, Action<DynamicMethodInfo>> GetServiceRegistrations(Type serviceType)
        {
            return services.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, Action<DynamicMethodInfo>>(StringComparer.InvariantCultureIgnoreCase));
        }
      
        private ThreadSafeDictionary<string, OpenGenericServiceInfo> GetOpenGenericRegistrations(Type serviceType)
        {
            return openGenericServices.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, OpenGenericServiceInfo>(StringComparer.InvariantCultureIgnoreCase));
        }
            
        private void RegisterService(Type serviceType, Type implementingType, LifeCycleType lifeCycleType, string serviceName)
        {
            if (serviceType.IsGenericTypeDefinition)
                RegisterOpenGenericService(serviceType, implementingType, lifeCycleType, serviceName);
            else
            {                
                Action<DynamicMethodInfo> emitDelegate = GetEmitDelegate(implementingType, IsFactory(serviceType) ? LifeCycleType.Singleton : lifeCycleType);
                GetServiceRegistrations(serviceType).AddOrUpdate(serviceName, s => emitDelegate, (s, i) => emitDelegate);    
            }            
        }

        private void RegisterOpenGenericService(Type serviceType, Type implementingType, LifeCycleType lifeCycleType, string serviceName)
        {
            var openGenericTypeInfo = new OpenGenericServiceInfo { ImplementingType = implementingType };
            if (lifeCycleType == LifeCycleType.Transient)
                openGenericTypeInfo.EmitMethod = (d, t) => EmitNewInstance(t, d);
            else if (lifeCycleType == LifeCycleType.Singleton)
                openGenericTypeInfo.EmitMethod = (d, t) => EmitSingletonInstance(t, d);
            else if (lifeCycleType == LifeCycleType.Singleton)
                openGenericTypeInfo.EmitMethod = (d, t) => EmitRequestInstance(t, d);
            
            GetOpenGenericRegistrations(serviceType).AddOrUpdate(serviceName, s => openGenericTypeInfo, (s, o) => openGenericTypeInfo);
        }

        private Action<DynamicMethodInfo> GetEmitDelegate(Type implementingType, LifeCycleType lifeCycleType)
        {
            Action<DynamicMethodInfo> emitDelegate = null;            
            switch (lifeCycleType)
            {
                case LifeCycleType.Transient:
                    emitDelegate = dynamicMethodInfo => EmitNewInstance(implementingType, dynamicMethodInfo);
                    break;
                case LifeCycleType.Request:
                    emitDelegate = dynamicMethodInfo => EmitRequestInstance(implementingType, dynamicMethodInfo);
                    break;
                case LifeCycleType.Singleton:
                    emitDelegate = dynamicMethodInfo => EmitSingletonInstance(implementingType, dynamicMethodInfo);
                    break;
            }

            return emitDelegate;
        }

        private void EmitSingletonInstance(Type implementingType, DynamicMethodInfo dynamicMethodInfo)
        {
            EmitLoadConstant(dynamicMethodInfo, GetConstantIndex(GetSingletonInstance(implementingType)), implementingType);
        }

        private object GetSingletonInstance(Type implementingType)
        {
            return singletons.GetOrAdd(implementingType, t => new Lazy<object>(() => CreateSingletonInstance(t))).Value;
        }

        private object CreateSingletonInstance(Type implementingType)
        {
            var dynamicMethodInfo = new DynamicMethodInfo();
            EmitNewInstance(implementingType, dynamicMethodInfo);
            object instance = dynamicMethodInfo.CreateDelegate()(constants);
            return instance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetConstantIndex(object value)
        {
            if (!constants.Contains(value))
                constants.Add(value);
            return constants.IndexOf(value);
        }
                    
        private Func<List<object>, object> CreateDelegate(Type serviceType, string serviceName)
        {
            EnsureThatServiceRegistryIsConfigured();
            var serviceEmitter = GetServiceEmitter(serviceType, serviceName);
            return CreateDynamicMethodDelegate(serviceEmitter, serviceType);
        }

        private void EnsureThatServiceRegistryIsConfigured()
        {
            if (ServiceRegistryIsEmpty())
                Scan(typeof(EmitServiceContainer).Assembly);
        }

        private bool ServiceRegistryIsEmpty()
        {
            return services.Count == 0 && openGenericServices.Count == 0;
        }

        private void RegisterValue(Type serviceType, object value, string serviceName)
        {
            int index = GetConstantIndex(value);
            Action<DynamicMethodInfo> emitter = dmi => EmitLoadConstant(dmi, index, serviceType);
            GetServiceRegistrations(serviceType).AddOrUpdate(serviceName, d => emitter, (s, d) => emitter);
        }

        private void RegisterServiceFromLambdaExpression<TService>(
            Expression<Func<IServiceFactory, TService>> factory, LifeCycleType lifeCycleType, string serviceName)
        {
            var serviceinfo = CreateServiceInfoFromExpression(factory);
            Type implementingType = serviceinfo.ImplementingType;
            implementations.AddOrUpdate(implementingType, t => serviceinfo, (t, s) => serviceinfo);
            RegisterService(typeof(TService), implementingType, lifeCycleType, serviceName);
        }
         
        /// <summary>
        /// Builds a <see cref="ServiceInfo"/> instance based on a <see cref="LambdaExpression"/>.
        /// </summary>
        public class ServiceInfoBuilder : ExpressionVisitor
        {            
            private ServiceInfo serviceInfo;
            
            /// <summary>
            /// Builds a <see cref="ServiceInfo"/> instance based on the given <paramref name="lambdaExpression"/>.
            /// </summary>
            /// <param name="lambdaExpression">The <see cref="LambdaExpression"/> from which to build a <see cref="ServiceInfo"/> instance.</param>
            /// <returns>A <see cref="ServiceInfo"/> instance.</returns>
            public ServiceInfo Build(LambdaExpression lambdaExpression)
            {
                Visit(lambdaExpression.Body);
                return serviceInfo;
            }

            /// <summary>
            /// Visits <see cref="MemberAssignment"/> expressions and creates a <see cref="PropertyDependecy"/>.
            /// </summary>
            /// <param name="memberAssignment">The target <see cref="MemberAssignment"/> expression.</param>
            /// <returns>The original expression.</returns>
            protected override MemberAssignment VisitMemberAssignment(MemberAssignment memberAssignment)
            {
                var propertyDependecy = CreatePropertyDependency(memberAssignment);
                ApplyDependencyDetails(memberAssignment.Expression, propertyDependecy);                                               
                serviceInfo.PropertyDependencies.Add(propertyDependecy);                
                return base.VisitMemberAssignment(memberAssignment);
            }

            /// <summary>
            /// Visits <see cref="NewExpression"/> expressions and creates a <see cref="ConstructorDependency"/>.
            /// </summary>
            /// <param name="newExpression">The target <see cref="NewExpression"/> expression.</param>
            /// <returns>The original expression.</returns>
            protected override Expression VisitNew(NewExpression newExpression)
            {
                serviceInfo = new ServiceInfo { Constructor = newExpression.Constructor, ImplementingType = newExpression.Constructor.DeclaringType };
                ParameterInfo[] parameters = newExpression.Constructor.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    ConstructorDependency constructorDependency = CreateConstructorDependency(parameters[i]);
                    ApplyDependencyDetails(newExpression.Arguments[i], constructorDependency);
                }

                return base.VisitNew(newExpression);
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

            private static PropertyDependecy CreatePropertyDependency(MemberAssignment memberAssignment)
            {                
                var propertyDependecy = new PropertyDependecy
                                        {
                                            PropertySetter = ((PropertyInfo)memberAssignment.Member).GetSetMethod(),
                                            ServiceType = ((PropertyInfo)memberAssignment.Member).PropertyType                                            
                                        };
                return propertyDependecy;
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
                dependency.Expression = expression;
                dependency.ServiceName = string.Empty;
            }

            private static void ApplyDependencyDetailsFromMethodCall(MethodCallExpression methodCallExpression, Dependency dependency)
            {
                dependency.ServiceType = methodCallExpression.Method.GetGenericArguments()[0];
                if (RepresentsGetNamedInstanceMethod(methodCallExpression))
                {
                    dependency.ServiceName = (string)((ConstantExpression)methodCallExpression.Arguments[0]).Value;
                }
                else
                {
                    dependency.ServiceName = string.Empty;
                }
            }
            
            private static bool RepresentsGetAllInstancesMethod(MethodCallExpression node)
            {
                return IsGetAllInstancesMethod(node.Method);
            }

            private static bool RepresentsGetNamedInstanceMethod(MethodCallExpression node)
            {
                return IsGetInstanceMethod(node.Method) && HasOneArgumentRepresentingServiceName(node);
            }

            private static bool RepresentsGetInstanceMethod(MethodCallExpression node)
            {
                return IsGetInstanceMethod(node.Method) && node.Arguments.Count == 0;
            }

            private static bool IsGetInstanceMethod(MethodInfo methodInfo)
            {
                return typeof(IServiceFactory).IsAssignableFrom(methodInfo.DeclaringType) && methodInfo.Name == "GetInstance";
            }

            private static bool IsGetAllInstancesMethod(MethodInfo methodInfo)
            {
                return typeof(IServiceFactory).IsAssignableFrom(methodInfo.DeclaringType) && methodInfo.Name == "GetAllInstances";
            }

            private static bool HasOneArgumentRepresentingServiceName(MethodCallExpression node)
            {
                return HasOneArgument(node) && IsStringConstantExpression(node.Arguments[0]);
            }

            private static bool HasOneArgument(MethodCallExpression node)
            {
                return node.Arguments.Count == 1;
            }

            private static bool IsStringConstantExpression(Expression argument)
            {
                return argument.NodeType == ExpressionType.Constant && argument.Type == typeof(string);
            }           
        }
       
        /// <summary>
        /// Contains information about how to create a service instance.
        /// </summary>
        public class ServiceInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
            /// </summary>
            public ServiceInfo()
            {
                PropertyDependencies = new List<PropertyDependecy>();
                ConstructorDependencies = new List<ConstructorDependency>();
            }

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
            /// Gets or sets the <see cref="Expression"/> that represent getting the value of the <see cref="Dependency"/>.
            /// </summary>            
            public Expression Expression { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("[Requested dependency: ServiceType:{0}, ServiceName:{1}]", ServiceType, ServiceName);
                if (Expression != null)
                {
                    sb.AppendFormat(" ,Expression: {0}", Expression);
                }

                return sb.ToString();
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
            public MethodInfo PropertySetter { get; set; }
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

            public override string ToString()
            {
                return string.Format("[Target Type: {0}], [Parameter: {1}({2})]", Parameter.Member.DeclaringType, Parameter.Name, Parameter.ParameterType) + ", " + base.ToString();
            }
        }

        private class DynamicMethodInfo
        {
            private readonly IDictionary<Type, LocalBuilder> localVariables = new Dictionary<Type, LocalBuilder>();

            private DynamicMethod dynamicMethod;

            public DynamicMethodInfo()
            {
                CreateDynamicMethod();
            }

            public ILGenerator GetILGenerator()
            {
                return dynamicMethod.GetILGenerator();
            }

            public Func<List<object>, object> CreateDelegate()
            {
                dynamicMethod.GetILGenerator().Emit(OpCodes.Ret);
                return (Func<List<object>, object>)dynamicMethod.CreateDelegate(typeof(Func<List<object>, object>));
            }

            public bool ContainsLocalVariable(Type implementingType)
            {
                return localVariables.ContainsKey(implementingType);
            }

            public void EmitLoadLocalVariable(Type implementingType)
            {
                dynamicMethod.GetILGenerator().Emit(OpCodes.Ldloc, localVariables[implementingType]);
            }

            public void EmitStoreLocalVariable(Type implementingType)
            {
                localVariables.Add(implementingType, dynamicMethod.GetILGenerator().DeclareLocal(implementingType));
                dynamicMethod.GetILGenerator().Emit(OpCodes.Stloc, localVariables[implementingType]);
            }

            private void CreateDynamicMethod()
            {
                dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(List<object>) }, typeof(EmitServiceContainer).Module, false);
            }
        }

        private class ThreadSafeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
        {
            public ThreadSafeDictionary()
            {
            }

            public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
                : base(comparer)
            {
            }
        }

        private class ServiceRegistry<T> : ThreadSafeDictionary<Type, ThreadSafeDictionary<string, T>>
        {
        }

        private class DelegateRegistry<TKey> : ThreadSafeDictionary<TKey, Lazy<Func<List<object>, object>>>
        {
        }

        private class OpenGenericServiceInfo
        {
            public Type ImplementingType { get; set; }

            public Action<DynamicMethodInfo, Type> EmitMethod { get; set; }
        }
    }

    /// <summary>
    /// Contains information about a service request passed to an <see cref="IFactory"/> instance.
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        /// Gets a value indicating whether the service request can be resolved by the underlying container.  
        /// </summary>
        public bool CanProceed
        {
            get { return Proceed != null; }
        }

        /// <summary>
        /// Gets the requested service type.
        /// </summary>
        public Type ServiceType { get; internal set; }

        /// <summary>
        /// Gets the requested service name.
        /// </summary>
        public string ServiceName { get; internal set; }

        /// <summary>
        /// Gets the function delegate used to proceed.
        /// </summary>
        public Func<object> Proceed { get; internal set; }
    }

    /// <summary>
    /// An assembly scanner that registers services based on the types contained within an <see cref="Assembly"/>.
    /// </summary>
    /// NOTE MUST NOT SCAN LIGHTINJECT TYPES
    public class AssemblyScanner : IAssemblyScanner
    {        
        /// <summary>
        /// Scans the target <paramref name="assembly"/> and registers services found within the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>        
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
        {
            IEnumerable<Type> types = GetConcreteTypes(assembly);
            var compositionRoots = types.Where(t => typeof(ICompositionRoot).IsAssignableFrom(t)).ToList();
            if (compositionRoots.Count > 0)
                ExecuteCompositionRoots(compositionRoots, serviceRegistry);
            else
            {
                foreach (Type type in types)
                    BuildImplementationMap(type, serviceRegistry);    
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
                implementingTypeName = string.Empty;
            return implementingTypeName;
        }

        private static IEnumerable<Type> GetBaseTypes(Type concreteType)
        {
            Type baseType = concreteType;
            while (baseType != typeof(object) && baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        private static IEnumerable<Type> GetConcreteTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsPublic);
        }
       
        private void BuildImplementationMap(Type implementingType, IServiceRegistry serviceRegistry)
        {
            Type[] interfaces = implementingType.GetInterfaces();
            foreach (Type interfaceType in interfaces)
                RegisterInternal(interfaceType, implementingType, serviceRegistry);
            foreach (Type baseType in GetBaseTypes(implementingType))
                RegisterInternal(baseType, implementingType, serviceRegistry);
        }

        private void RegisterInternal(Type serviceType, Type implementingType, IServiceRegistry serviceRegistry)
        {
            if (serviceType.IsGenericType && serviceType.ContainsGenericParameters)
                serviceType = serviceType.GetGenericTypeDefinition();           
            serviceRegistry.Register(serviceType, implementingType, GetServiceName(serviceType, implementingType));
        }
    }
    
    /// <summary>
    /// Selects the properties that represents a dependecy to the target <see cref="Type"/>.
    /// </summary>
    public class PropertySelector : IPropertySelector
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
            return propertyInfo.GetSetMethod(false) == null;
        }        
    }

    /// <summary>
    /// Loads all assemblies from the application base directory that matches the given search pattern.
    /// </summary>
    public class AssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Loads a set of assemblies based on the given <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern to use.</param>
        /// <returns>A list of assemblies based on the given <paramref name="searchPattern"/>.</returns>
        public IEnumerable<Assembly> Load(string searchPattern)
        {
            string directory = Path.GetDirectoryName(new Uri(typeof(EmitServiceContainer).Assembly.CodeBase).LocalPath);
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
}