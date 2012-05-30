using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace LightInject
{
    using System.Text.RegularExpressions;

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
        /// <param name="expression">The expression that describes the dependencies of the service.</param>
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
        /// <param name="expression">The expression that describes the dependencies of the service.</param>
        /// <param name="lifeCycle">The <see cref="LifeCycleType"/> that specifies the life cycle of the service.</param>
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, LifeCycleType lifeCycle);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The expression that describes the dependencies of the service.</param>
        /// <param name="serviceName">The name of the service.</param>        
        void Register<TService>(Expression<Func<IServiceFactory, TService>> expression, string serviceName);

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with the <paramref name="expression"/> that 
        /// describes the dependencies of the service. 
        /// </summary>
        /// <typeparam name="TService">The service type to register.</typeparam>
        /// <param name="expression">The expression that describes the dependencies of the service.</param>
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
    /// Represents an inversion of control container.
    /// </summary>
    public interface IContainer : IServiceRegistry, IServiceFactory
    {
        /// <summary>
        /// Registers services from the given <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned for services.</param>        
        /// <remarks>
        /// If the target <paramref name="assembly"/> contains an implementation of the <see cref="ICompositionRoot"/> interface, this 
        /// will be used to configure the container.
        /// </remarks>     
        void Register(Assembly assembly);

        /// <summary>
        /// Registers services from assemblies in the base directory that mathes the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern"></param>
        void Register(string searchPattern);

    }

    /// <summary>
    /// An ultra lightweight service container.
    /// </summary>
    public class EmitServiceContainer : IContainer
    {
        private readonly ServiceRegistry<Action<DynamicMethodInfo>> _services = new ServiceRegistry<Action<DynamicMethodInfo>>();
        private readonly ServiceRegistry<OpenGenericServiceInfo> _openGenericServices = new ServiceRegistry<OpenGenericServiceInfo>();        
        private readonly DelegateRegistry<Type> _delegates = new DelegateRegistry<Type>();
        private readonly DelegateRegistry<Tuple<Type, string>> _namedDelegates = new DelegateRegistry<Tuple<Type, string>>();        
        private readonly ThreadSafeDictionary<Type, ServiceInfo> _implementations = new ThreadSafeDictionary<Type, ServiceInfo>();
        private readonly ThreadSafeDictionary<Type, Lazy<object>> _singletons = new ThreadSafeDictionary<Type, Lazy<object>>();
        private static readonly MethodInfo GetInstanceMethod;
        private readonly List<object> _constants = new List<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitServiceContainer"/> class.
        /// </summary>
        public EmitServiceContainer()
        {
            AssemblyScanner = new AssemblyScanner();
        }

        /// <summary>
        /// Gets or sets the <see cref="IAssemblyScanner"/> instance that is reponsible for scanning assemblies.
        /// </summary>
        public IAssemblyScanner AssemblyScanner { get; set; }

        static EmitServiceContainer()
        {
            GetInstanceMethod = typeof(IFactory).GetMethod("GetInstance");
        }

        public void Register(Assembly assembly)
        {                        
            AssemblyScanner.Scan(assembly,this);
        }

        public void Register(string searchPattern)
        {
            string directory = Path.GetDirectoryName(new Uri(typeof(ServiceContainer).Assembly.CodeBase).LocalPath);
            if (directory != null)
            {
                string[] searchPatterns = searchPattern.Split('|');
                foreach (string file in searchPatterns.SelectMany(sp => Directory.GetFiles(directory, sp)))
                    Register(Assembly.LoadFrom(file));
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
            Register(typeof(TService),typeof(TImplementation),lifeCycle);
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
            return _delegates.GetOrAdd(
                serviceType,
                t =>
                new Lazy<Func<List<object>, object>>(() => CreateDelegate(serviceType, string.Empty))).Value(_constants);
        }

        public TService GetInstance<TService>()
        {
            return (TService)GetInstance(typeof(TService));
        }

        public TService GetInstance<TService>(string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName);
        }

        public object GetInstance(Type serviceType, string serviceName)
        {
            return _namedDelegates.GetOrAdd(
                Tuple.Create(serviceType, serviceName),
                t =>
                new Lazy<Func<List<object>, object>>(() => CreateDelegate(serviceType, serviceName))).Value(_constants);
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
            var ilGenerator = dynamicMethodInfo.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldc_I4, index);
            ilGenerator.Emit(OpCodes.Callvirt, method);
            ilGenerator.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        private static void EmitEnumerable(IList<Action<DynamicMethodInfo>> serviceEmitters, Type elementType, DynamicMethodInfo dynamicMethodInfo)
        {
            ILGenerator ilGenerator = dynamicMethodInfo.GetILGenerator();
            LocalBuilder array = ilGenerator.DeclareLocal(elementType.MakeArrayType());
            ilGenerator.Emit(OpCodes.Ldc_I4, serviceEmitters.Count);
            ilGenerator.Emit(OpCodes.Newarr, elementType);
            ilGenerator.Emit(OpCodes.Stloc, array);

            for (int index = 0; index < serviceEmitters.Count; index++)
            {
                ilGenerator.Emit(OpCodes.Ldloc, array);
                ilGenerator.Emit(OpCodes.Ldc_I4, index);
                var serviceEmitter = serviceEmitters[index];
                serviceEmitter(dynamicMethodInfo);                                
                ilGenerator.Emit(OpCodes.Stelem, elementType);
            }

            ilGenerator.Emit(OpCodes.Ldloc, array);
        }

        private static void EmitCallCustomFactory(DynamicMethodInfo dynamicMethodInfo, int serviceRequestConstantIndex, int factoryConstantIndex, Type serviceType)
        {
            ILGenerator ilGenerator = dynamicMethodInfo.GetILGenerator();
            EmitLoadConstant(dynamicMethodInfo, factoryConstantIndex, typeof(IFactory));
            EmitLoadConstant(dynamicMethodInfo, serviceRequestConstantIndex, typeof(ServiceRequest));
            ilGenerator.Emit(OpCodes.Callvirt, GetInstanceMethod);
            if (serviceType.IsValueType)
                ilGenerator.Emit(OpCodes.Unbox_Any, serviceType);
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

        private static bool IsFactory(Type type)
        {
            return typeof(IFactory).IsAssignableFrom(type);
        }

        private static ServiceInfo CreateServiceInfo(Type implementingType)
        {
            var serviceInfo = new ServiceInfo();
            ConstructorInfo constructorInfo = GetConstructorWithTheMostParameters(implementingType);
            serviceInfo.Constructor = constructorInfo;            
            serviceInfo.ConstructorDependencies.AddRange(GetConstructorDependencies(constructorInfo));
            serviceInfo.PropertyDependencies.AddRange(GetPropertyDependencies(implementingType));
            return serviceInfo;
        }

        private static IEnumerable<Dependency> GetConstructorDependencies(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().OrderBy(p => p.Position).Select(
                p => new Dependency { ServiceName = string.Empty, ServiceType = p.ParameterType });
        }

        private static IEnumerable<PropertyDependecy> GetPropertyDependencies(Type implementingType)
        {
            return GetInjectableProperties(implementingType).Select(
                p => new PropertyDependecy { PropertySetter = p.GetSetMethod(), ServiceName = string.Empty, ServiceType = p.PropertyType });
        }

        private static IEnumerable<PropertyInfo> GetInjectableProperties(Type implementingType)
        {
            var properties = implementingType.GetProperties().Where(IsInjectable).ToList();
            return properties;
        }

        private static bool IsInjectable(PropertyInfo propertyInfo)
        {
            return !IsReadOnly(propertyInfo);
        }

        private static bool IsReadOnly(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod(false) == null;
        }

        private static ServiceInfo CreateServiceInfoFromExpression(LambdaExpression lambdaExpression)
        {
            var methodCallVisitor = new MethodCallVisitor();
            ServiceInfo serviceInfo = methodCallVisitor.CreateServiceInfo(lambdaExpression);
            return serviceInfo;
        }

        private static ConstructorInfo GetConstructorWithTheMostParameters(Type implementingType)
        {
            return implementingType.GetConstructors().OrderBy(c => c.GetParameters().Count()).LastOrDefault();
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
                    emitter = CreateServiceEmitterBasedOnCustomFactory(serviceType, serviceName, factory, () => del(_constants));    
                }
                else
                {
                    return CreateServiceEmitterBasedOnCustomFactory(serviceType, serviceName, factory, null);
                }
                
            }

            if (emitter != null) registrations.AddOrUpdate(serviceName, s => emitter, (s, d) => emitter);

            if (emitter == null)
                throw new InvalidOperationException(
                    string.Format("Unable to resolve an implementation based on request (Type = {0}, Name = {1}", serviceType, serviceName));

            return _services[serviceType][serviceName];
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
            ILGenerator ilGenerator = dynamicMethodInfo.GetILGenerator();
            EmitConstructorDependencies(serviceInfo, dynamicMethodInfo);            
            ilGenerator.Emit(OpCodes.Newobj, serviceInfo.Constructor);
            EmitPropertyDependencies(serviceInfo, dynamicMethodInfo);
        }

        private void EmitConstructorDependencies(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            foreach (Dependency dependency in serviceInfo.ConstructorDependencies)
            {
                if (dependency.Value != null)
                {
                    int index = GetConstantIndex(dependency.Value);
                    EmitLoadConstant(dynamicMethodInfo, index, dependency.Value.GetType());
                }
                else
                {
                    GetServiceEmitter(dependency.ServiceType, dependency.ServiceName)(dynamicMethodInfo);
                }
            }
        }

        private void EmitPropertyDependencies(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            ILGenerator ilGenerator = dynamicMethodInfo.GetILGenerator();
            LocalBuilder instance = ilGenerator.DeclareLocal(serviceInfo.Constructor.DeclaringType);
            ilGenerator.Emit(OpCodes.Stloc, instance);
            foreach (var propertyDependency in serviceInfo.PropertyDependencies)
            {
                ilGenerator.Emit(OpCodes.Ldloc, instance);
                if (propertyDependency.Value != null)
                {
                    int index = GetConstantIndex(propertyDependency.Value);
                    EmitLoadConstant(dynamicMethodInfo, index, propertyDependency.Value.GetType());
                }
                else
                {
                    GetServiceEmitter(propertyDependency.ServiceType, propertyDependency.ServiceName)(dynamicMethodInfo);
                }

                dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Callvirt, propertyDependency.PropertySetter);
            }
            ilGenerator.Emit(OpCodes.Ldloc, instance);
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
            return GetInstance<IEnumerable<IFactory>>().Where(f => f.CanGetInstance(serviceType, serviceName)).FirstOrDefault();
        }

        private Action<DynamicMethodInfo> CreateEnumerableServiceEmitter(Type serviceType)
        {
            Type actualServiceType = serviceType.GetGenericArguments()[0];
            IList<Action<DynamicMethodInfo>> serviceEmitters = GetServiceRegistrations(actualServiceType).Values.ToList();
            var dynamicMethodInfo = new DynamicMethodInfo();
            EmitEnumerable(serviceEmitters, actualServiceType, dynamicMethodInfo);
            var array = dynamicMethodInfo.CreateDelegate()(_constants);
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
                Func<string, TServiceType> func = s => GetInstance<TServiceType>(s);                
                return func;
            }
            else
            {
                Func<TServiceType> func = () => GetInstance<TServiceType>();
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
            return _implementations.GetOrAdd(implementingType, CreateServiceInfo);
        }

        private ThreadSafeDictionary<string, Action<DynamicMethodInfo>> GetServiceRegistrations(Type serviceType)
        {
            return _services.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, Action<DynamicMethodInfo>>(StringComparer.InvariantCultureIgnoreCase));
        }
      
        private ThreadSafeDictionary<string, OpenGenericServiceInfo> GetOpenGenericRegistrations(Type serviceType)
        {
            return _openGenericServices.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, OpenGenericServiceInfo>(StringComparer.InvariantCultureIgnoreCase));
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
            return _singletons.GetOrAdd(implementingType, t => new Lazy<object>(() => CreateSingletonInstance(t))).Value;
        }

        private object CreateSingletonInstance(Type implementingType)
        {
            var dynamicMethodInfo = new DynamicMethodInfo();
            EmitNewInstance(implementingType, dynamicMethodInfo);
            object instance = dynamicMethodInfo.CreateDelegate()(_constants);
            return instance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetConstantIndex(object value)
        {
            if (!_constants.Contains(value))
                _constants.Add(value);
            return _constants.IndexOf(value);
        }
                    
        private Func<List<object>, object> CreateDelegate(Type serviceType, string serviceName)
        {
            EnsureThatServiceRegistryIsConfigured();
            var serviceEmitter = GetServiceEmitter(serviceType, serviceName);
            return CreateDynamicMethodDelegate(serviceEmitter, serviceType);
        }

        private void EnsureThatServiceRegistryIsConfigured()
        {
            if (_services.Count == 0 && _openGenericServices.Count == 0)
            {
                Register("*.dll|*.exe");
            }
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
            Type implementingType = serviceinfo.Constructor.DeclaringType;
            _implementations.AddOrUpdate(implementingType, t => serviceinfo, (t, s) => serviceinfo);
            RegisterService(typeof(TService), implementingType, lifeCycleType, serviceName);
        }
         
        public class MethodCallVisitor : ExpressionVisitor
        {
            private ServiceInfo _serviceInfo;

            public ServiceInfo CreateServiceInfo(LambdaExpression expression)
            {
                Visit(expression.Body);
                return _serviceInfo;
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                var propertyDependecy = new PropertyDependecy { PropertySetter = ((PropertyInfo)node.Member).GetSetMethod() };
                switch (node.Expression.NodeType)
                {
                    case ExpressionType.Call:
                        {
                            ApplyDependencyDetailsFromMethodCall((MethodCallExpression)node.Expression, propertyDependecy);
                            break;
                        }
                    default:
                        {
                            ApplyDependecyDetailsFromExpression(node.Expression, propertyDependecy);
                            break;
                        }
                }
                _serviceInfo.PropertyDependencies.Add(propertyDependecy);                
                return base.VisitMemberAssignment(node);
            }

            private void ApplyDependecyDetailsFromExpression(Expression expression, Dependency dependency)
            {
                var lambda = Expression.Lambda(expression, new ParameterExpression[] { }).Compile();
                var value = lambda.DynamicInvoke();
                dependency.Value = value;
            }

            private void ApplyDependencyDetailsFromMethodCall(MethodCallExpression methodCallExpression, Dependency dependency)
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

            protected override Expression VisitNew(NewExpression node)
            {
                _serviceInfo = new ServiceInfo { Constructor = node.Constructor };

                foreach (var expression in node.Arguments)
                {
                    switch (expression.NodeType)
                    {
                        case ExpressionType.Call:
                            {
                                _serviceInfo.ConstructorDependencies.Add(CreateDependecyFromMethodCallExpression((MethodCallExpression)expression));
                                break;
                            }

                        case ExpressionType.Constant:
                            {
                                _serviceInfo.ConstructorDependencies.Add(CreateDependencyFromConstantExpression((ConstantExpression)expression));
                                break;
                            }

                        default:
                            {
                                _serviceInfo.ConstructorDependencies.Add(CreateDepenedencyFromExpression(expression));
                                break;
                            }
                    }                                       
                }

                return base.VisitNew(node);
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

            private static Dependency CreateDepenedencyFromExpression(Expression expression)
            {                
                var lambda = Expression.Lambda(expression, new ParameterExpression[] { }).Compile();
                var value = lambda.DynamicInvoke();
                return new Dependency { Value = value };
            }

            private static Dependency CreateDependencyFromConstantExpression(ConstantExpression constantExpression)
            {
                var dependency = new Dependency { Value = constantExpression.Value };
                return dependency;
            }

            private static Dependency CreateDependecyFromMethodCallExpression(MethodCallExpression methodCallExpression)
            {
                var dependency = new Dependency();
                if (RepresentsGetInstanceMethod(methodCallExpression))
                {
                    return new Dependency { ServiceName = string.Empty, ServiceType = methodCallExpression.Method.GetGenericArguments().First() };                    
                }

                if (RepresentsGetNamedInstanceMethod(methodCallExpression))
                {
                    return new Dependency { ServiceName = (string)((ConstantExpression)methodCallExpression.Arguments[0]).Value, ServiceType = methodCallExpression.Method.GetGenericArguments().First() };                    
                }

                return dependency;
            }
        }

        private class DynamicMethodInfo
        {
            private readonly IDictionary<Type, LocalBuilder> _localVariables = new Dictionary<Type, LocalBuilder>();

            private DynamicMethod _dynamicMethod;

            public DynamicMethodInfo()
            {
                CreateDynamicMethod();
            }

            public ILGenerator GetILGenerator()
            {
                return _dynamicMethod.GetILGenerator();
            }

            public Func<List<object>, object> CreateDelegate()
            {
                _dynamicMethod.GetILGenerator().Emit(OpCodes.Ret);
                return (Func<List<object>, object>)_dynamicMethod.CreateDelegate(typeof(Func<List<object>, object>));
            }

            public bool ContainsLocalVariable(Type implementingType)
            {
                return _localVariables.ContainsKey(implementingType);
            }

            public void EmitLoadLocalVariable(Type implementingType)
            {
                _dynamicMethod.GetILGenerator().Emit(OpCodes.Ldloc, _localVariables[implementingType]);
            }

            public void EmitStoreLocalVariable(Type implementingType)
            {
                _localVariables.Add(implementingType, _dynamicMethod.GetILGenerator().DeclareLocal(implementingType));
                _dynamicMethod.GetILGenerator().Emit(OpCodes.Stloc, _localVariables[implementingType]);
            }

            private void CreateDynamicMethod()
            {
                _dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(List<object>) }, typeof(ServiceContainer).Module, false);
            }
        }

        public class ServiceInfo
        {
            public ServiceInfo()
            {
                PropertyDependencies = new List<PropertyDependecy>();
                ConstructorDependencies = new List<Dependency>();
            }

            public ConstructorInfo Constructor { get; set; }

            public List<PropertyDependecy> PropertyDependencies { get; private set; }

            public List<Dependency> ConstructorDependencies { get; private set; }
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

        public class Dependency
        {
            public Type ServiceType { get; set; }

            public string ServiceName { get; set; }

            public object Value { get; set; }
        }

        public class PropertyDependecy : Dependency
        {
            public MethodInfo PropertySetter { get; set; }
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

        private void ExecuteCompositionRoots(List<Type> compositionRoots, IServiceRegistry serviceRegistry)
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
}