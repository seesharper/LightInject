//#define SILVERLIGHT
//#define NET35
//Uncomment the line above to have the container working in a SilverLight or CLR 3.5 environment
using System;
#if !SILVERLIGHT && !NET35
using System.Collections.Concurrent;
#else
    using System.Collections;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LightInject
{    
    /// <summary>
    /// An ultra lightweight service container.
    /// </summary>
    public class TddContainer
    {
        private readonly ThreadSafeDictionary<Type, ThreadSafeDictionary<string, ImplementationInfo>> _availableServices =
           new ThreadSafeDictionary<Type, ThreadSafeDictionary<string, ImplementationInfo>>();

        private readonly ThreadSafeDictionary<Type, Func<object>> _defaultFactories =
            new ThreadSafeDictionary<Type, Func<object>>();

        private readonly ThreadSafeDictionary<Tuple<Type, string>, Func<object>> _namedFactories =
            new ThreadSafeDictionary<Tuple<Type, string>, Func<object>>();

        private static readonly MethodInfo CreateInstanceMethod;

        static TddContainer()
        {
            CreateInstanceMethod = typeof(IFactory).GetMethod("CreateInstance");
        }
     
        /// <summary>
        /// Register services from the given <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to scan for services.</param>        
        public void Register(Assembly assembly)
        {
            Register(assembly, t => true);
        }

        /// <summary>
        /// Register services from the given <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to scan for services.</param>
        /// <param name="shouldLoad">Determines if the current to should be registered.</param>        
        public void Register(Assembly assembly, Func<Type,bool> shouldLoad)
        {
            IEnumerable<Type> types = GetConcreteTypes(assembly);
            foreach (var type in types.Where(shouldLoad))
                BuildImplementationMap(type);
        }

        /// <summary>
        /// Registers services from assemblies currently in the <see cref="AppDomain"/>.        
        /// </summary>
        /// <param name="shouldLoad">Determines if the current to should be registered.</param>     
        public void Register(Func<Type,bool> shouldLoad)
        {
            foreach (Assembly assembly in GetAssemblies())
            {
                Register(assembly,shouldLoad);
            }
        }
#if !SILVERLIGHT

        /// <summary>
        /// Loads service from the current directory.
        /// </summary>
        /// <param name="searchPattern">The search pattern used to filter the assembly files.</param>
        public void Register(string searchPattern)
        {
            var directory = Path.GetDirectoryName(new Uri(typeof(TddContainer).Assembly.CodeBase).LocalPath);
            string[] searchPatterns = searchPattern.Split('|');
            foreach (var file in searchPatterns.SelectMany(sp => Directory.GetFiles(directory, sp)))
                Register(Assembly.LoadFrom(file));
        }
#endif

        /// <summary>
        /// Registers services from assemblies currently in the <see cref="AppDomain"/>.        
        /// </summary>
        public void Register()
        {
            Register(t => true);
        }

        private void BuildImplementationMap(Type implementingType)
        {
            Type[] interfaces = implementingType.GetInterfaces();
            foreach (var interfaceType in interfaces)
                RegisterInternal(interfaceType, implementingType);
            foreach (var baseType in GetBaseTypes(implementingType))
                RegisterInternal(baseType, implementingType);
        }

        private void RegisterInternal(Type serviceType, Type implementingType)
        {
            if (ShouldCreateSingletonExpression(implementingType))
                RegisterAsSingleton(serviceType,implementingType,GetServiceName(serviceType,implementingType));
            Register(serviceType, implementingType, GetServiceName(serviceType, implementingType));
        }

        private static string GetServiceName(Type serviceType, Type implementingType)
        {
            string serviceName = implementingType.Name;
            if (implementingType.IsGenericTypeDefinition)
            {
                var regex = new Regex("([a-z])", RegexOptions.IgnoreCase);
                serviceName = regex.Match(implementingType.Name).Groups[1].Value;
            }
            if (serviceName.Substring(1) == serviceType.Name)
                serviceName = "";
            return serviceName;
        }
#if SILVERLIGHT
        private IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            foreach (AssemblyPart part in Deployment.Current.Parts)
            {
                StreamResourceInfo resourceStream = Application.GetResourceStream(new Uri(part.Source, UriKind.Relative));
                if (resourceStream != null)
                {
                    Assembly item = part.Load(resourceStream.Stream);
                    assemblies.Add(item);
                }
            }
        }
#else
        private static IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.GlobalAssemblyCache);
        }
#endif
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

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        public void Register(Type serviceType, Type implementingType)
        {
            Register(serviceType, implementingType, string.Empty);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/> as a singleton service.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        public void RegisterAsSingleton(Type serviceType, Type implementingType)
        {
            RegisterAsSingleton(serviceType, implementingType, string.Empty);
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/> as a singleton service.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="serviceName">The name of the service.</param>
        public void RegisterAsSingleton(Type serviceType, Type implementingType, string serviceName)
        {
            if (implementingType == null)
            {
                ImplementationInfo implementationInfo = TryResolveImplementationForUnknownService(serviceType,
                                                                                                  serviceName);
                var singletonImplementationInfo = new ImplementationInfo(null,
                                                                         () =>
                                                                         CreateSingletonExpression(
                                                                             () =>
                                                                             implementationInfo.FactoryExpression.Value));
                Register(serviceType, serviceName, singletonImplementationInfo);
            }
            else
                Register(serviceType, serviceName, CreateSingletonImplementationInfo(serviceType, implementingType, serviceName));
                        
        }

        /// <summary>
        /// Registers the <paramref name="serviceType"/> with the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to register.</param>
        /// <param name="implementingType">The implementing type.</param>
        /// <param name="serviceName">The name of the service.</param>
        public void Register(Type serviceType, Type implementingType, string serviceName)
        {
            if (IsFactory(serviceType))
                RegisterAsSingleton(serviceType, implementingType, serviceName);
            else
            {
                var implementationInfo = CreateImplementationInfo(serviceType, implementingType, serviceName);
                Register(serviceType, serviceName, implementationInfo);
            }
        }

        /// <summary>
        /// Registers a factory delegate used to create an instance of <typeparamref name="TService"/> 
        /// </summary>
        /// <typeparam name="TService">The type of service for which to register a factory delegate.</typeparam>
        /// <param name="factory">The delegate used to create a instance of <typeparamref name="TService"/></param>
        public void Register<TService>(Expression<Func<TService>> factory)
        {
            Register(factory, string.Empty);
        }

        /// <summary>
        /// Registers a factory delegate used to create an instance of <typeparamref name="TService"/> 
        /// </summary>
        /// <typeparam name="TService">The type of service for which to register a factory delegate.</typeparam>
        /// <param name="factory">The delegate used to create a instance of <typeparamref name="TService"/></param>
        public void RegisterAsSingleton<TService>(Expression<Func<TService>> factory)
        {
            RegisterAsSingleton(factory, string.Empty);
        }

        /// <summary>
        /// Registers a factory delegate used to create an instance of <typeparamref name="TService"/> 
        /// </summary>
        /// <typeparam name="TService">The type of service for which to register a factory delegate.</typeparam>
        /// <param name="factory">The delegate used to create a instance of <typeparamref name="TService"/></param>
        /// <param name="serviceName">The name of the service.</param>
        public void Register<TService>(Expression<Func<TService>> factory, string serviceName)
        {
            var implementationInfo = new ImplementationInfo(typeof(Func<TService>), () => factory.Body);
            Register(typeof(TService), serviceName, implementationInfo);
        }

        /// <summary>
        /// Registers a factory delegate used to create an instance of <typeparamref name="TService"/> 
        /// </summary>
        /// <typeparam name="TService">The type of service for which to register a factory delegate.</typeparam>
        /// <param name="factory">The delegate used to create a instance of <typeparamref name="TService"/></param>
        /// <param name="serviceName">The name of the service.</param>
        public void RegisterAsSingleton<TService>(Expression<Func<TService>> factory, string serviceName)
        {
            var implementationInfo = new ImplementationInfo(typeof(Func<TService>), () => CreateSingletonExpression(() => factory.Body));
            Register(typeof(TService), serviceName, implementationInfo);
        }

        private void Register(Type serviceType, string serviceName, ImplementationInfo implementationInfo)
        {
            GetImplementations(serviceType).AddOrUpdate(serviceName, s => implementationInfo, (s,i) => implementationInfo);
        }

        private static bool IsEnumerableOfT(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool ShouldCreateSingletonExpression(Type implementingType)
        {
            return (TypeNameStartsOrEndsWithSingleton(implementingType)
                    || IsFactory(implementingType));
        }

        private static bool IsFactory(Type type)
        {
            return typeof(IFactory).IsAssignableFrom(type);
        }

        private static bool TypeNameStartsOrEndsWithSingleton(Type concreteType)
        {
            return concreteType.Name.EndsWith("Singleton", StringComparison.InvariantCultureIgnoreCase) ||
                   concreteType.Name.StartsWith("Singleton", StringComparison.InvariantCultureIgnoreCase);
        }
        private Expression CreateSingletonExpression(Type serviceType, Type implementingType, string serviceName)
        {                        
            var newExpression = CreateNewExpression(serviceType, implementingType, serviceName);
            var singletonInstance = Expression.Lambda<Func<object>>(newExpression).Compile()();
            return Expression.Constant(singletonInstance);
        }

        private static Expression CreateSingletonExpression(Func<Expression> expressionFactory)
        {
            var singletonInstance = Expression.Lambda<Func<object>>(expressionFactory()).Compile()();
            return Expression.Constant(singletonInstance);
        }

        private NewArrayExpression CreateNewArrayExpression(Type enumerableType)
        {
            Type serviceType = enumerableType.GetGenericArguments().First();
            ThreadSafeDictionary<string, ImplementationInfo> implementations = GetImplementations(serviceType);
            List<Expression> newExpressions;
            if (implementations != null)
                newExpressions = implementations
                    .Select(implementation => GetBodyExpression(serviceType, implementation.Key)).ToList();
            else
                newExpressions = new List<Expression>();

            NewArrayExpression newArrayExpression = Expression.NewArrayInit(serviceType, newExpressions);
            return newArrayExpression;
        }

        private ThreadSafeDictionary<string, ImplementationInfo> GetImplementations(Type serviceType)
        {
            return _availableServices.GetOrAdd(serviceType,
                                               s =>
                                               new ThreadSafeDictionary<string, ImplementationInfo>(StringComparer.InvariantCultureIgnoreCase));
        }

        private static bool IsLazy(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        private static bool IsFunc(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Func<>);
        }

        private Expression CreateNewExpression(Type serviceType, Type implementingType, string serviceName)
        {           
            ConstructorInfo constructorInfo = GetConstructorInfo(implementingType);
            IEnumerable<Expression> parameterExpressions = GetParameterExpressions(constructorInfo);
            NewExpression newExpression = Expression.New(constructorInfo, parameterExpressions);
            IFactory factory = GetCustomFactory(serviceType, serviceName);
            return factory != null ? CreateCustomFactoryExpression(factory, serviceType, serviceName, newExpression) : newExpression;
        }

        private static ConstructorInfo GetConstructorInfo(Type concreteType)
        {
            return concreteType.GetConstructors().Single();
        }

        private IEnumerable<Expression> GetParameterExpressions(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().Select(
                parameterInfo =>
                GetBodyExpression(parameterInfo.ParameterType,
                HasMultipleImplementations(parameterInfo.ParameterType) ? parameterInfo.Name : string.Empty)).
                ToList();

        }

        private bool HasMultipleImplementations(Type serviceType)
        {
            return GetImplementations(serviceType).Count > 1;
        }

        private Expression GetBodyExpression(Type serviceType, string serviceName)
        {
            try
            {
                return GetImplementationInfo(serviceType, serviceName).FactoryExpression.Value;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(string.Format("An recursive dependency has been detected while resolving (Type = {0}, Name = {1}", serviceType, serviceName));
            }
        }

        private Expression CreateFuncExpression(Type serviceType, string serviceName)
        {
            Type actualServiceType = serviceType.GetGenericArguments().First();
            Expression bodyExression = GetBodyExpression(actualServiceType, serviceName);
            Type funcType = typeof(Func<>).MakeGenericType(actualServiceType);
            var funcInstance = Expression.Lambda(funcType, bodyExression, null).Compile();
            return Expression.Constant(funcInstance);
        }

        private ImplementationInfo GetImplementationInfo(Type serviceType, string serviceName)
        {
            return GetImplementations(serviceType).GetOrAdd(serviceName,
                                                     s => TryResolveImplementationForUnknownService(serviceType, serviceName));

        }

        private ImplementationInfo CreateImplementationInfo(Type serviceType, Type implementingType, string serviceName)
        {
            return new ImplementationInfo(implementingType,
                                          () => CreateNewExpression(serviceType, implementingType, serviceName));
        }

        private ImplementationInfo CreateSingletonImplementationInfo(Type serviceType, Type implementingType, string serviceName)
        {
            return new ImplementationInfo(implementingType,
                                          () => CreateSingletonExpression(serviceType, implementingType, serviceName)){IsSingleton = true};
        }

        private ImplementationInfo TryResolveImplementationForUnknownService(Type serviceType, string serviceName)
        {
            IFactory factory = GetCustomFactory(serviceType, serviceName);
            if (factory != null)
                return CreateCustomFactoryImplementationInfo(serviceType, serviceName, factory);
            if (IsEnumerableOfT(serviceType))
                return CreateEnumerableImplementationInfo(serviceType);
            if (IsFunc(serviceType))
                return CreateFuncImplementationInfo(serviceType, serviceName);
            if (IsLazy(serviceType))
                return CreateLazyImplementationInfo(serviceType, serviceName);
            if (IsClosedGeneric(serviceType))
                return CreateClosedGenericImplementationInfo(serviceType, serviceName);
            if (CanRedirectRequestForDefaultServiceToSingleNamedService(serviceType, serviceName))
                return CreateImplementationInfoBasedOnFirstNamedInstance(serviceType);
            if (!string.IsNullOrEmpty(serviceName))
                return CreateImplementationInfoThatRedirectsToDefaultService(serviceType);

            throw new InvalidOperationException(string.Format("Unable to resolve an implementation based on request (Type = {0}, Name = {1}", serviceType, serviceName));
        }

        private static bool IsClosedGeneric(Type serviceType)
        {
            return serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition;
        }

        private ImplementationInfo CreateImplementationInfoThatRedirectsToDefaultService(Type serviceType)
        {
            return GetImplementationInfo(serviceType, string.Empty);
        }

        private bool CanRedirectRequestForDefaultServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetImplementations(serviceType).Count == 1;
        }

        private ImplementationInfo CreateImplementationInfoBasedOnFirstNamedInstance(Type serviceType)
        {
            return GetImplementationInfo(serviceType, GetImplementations(serviceType).First().Key);
        }


        private static ServiceRequest CreateServiceRequest(Type serviceType, string serviceName, Expression proceedExpression)
        {
            var serviceRequest = new ServiceRequest
                                     {
                                         ServiceType = serviceType,
                                         ServiceName = serviceName,
                                         Proceed = CreateProceedFunctionDelegate(proceedExpression)
                                     };
            return serviceRequest;
        }

        private static Func<object> CreateProceedFunctionDelegate(Expression proceedExpression)
        {
            return proceedExpression != null ? Expression.Lambda<Func<object>>(proceedExpression).Compile() : null;
        }

        private static ImplementationInfo CreateCustomFactoryImplementationInfo(Type serviceType, string serviceName, IFactory factory)
        {
            return new ImplementationInfo(null, () => CreateCustomFactoryExpression(factory, serviceType, serviceName, null));
        }

        private static Expression CreateCustomFactoryExpression(IFactory customFactory, Type serviceType, string serviceName, Expression proceedExpression)
        {
            ServiceRequest serviceRequest = CreateServiceRequest(serviceType, serviceName, proceedExpression);
            return Expression.Convert(Expression.Call(Expression.Constant(customFactory), CreateInstanceMethod,
                                   new Expression[] { Expression.Constant(serviceRequest) }), serviceType);
        }

        private IFactory GetCustomFactory(Type serviceType, string serviceName)
        {
            if (IsFactory(serviceType) || (IsEnumerableOfT(serviceType) && IsFactory(serviceType.GetGenericArguments().First())))
                return null;
            return GetInstance<IEnumerable<IFactory>>().Where(f => f.CanCreateInstance(serviceType, serviceName)).FirstOrDefault();
        }

        private ImplementationInfo CreateFuncImplementationInfo(Type serviceType, string serviceName)
        {
            return new ImplementationInfo(null, () => CreateFuncExpression(serviceType, serviceName));
        }

        private ImplementationInfo CreateEnumerableImplementationInfo(Type serviceType)
        {
            return new ImplementationInfo(null, () => CreateNewArrayExpression(serviceType));
        }

        private ImplementationInfo CreateLazyImplementationInfo(Type serviceType, string serviceName)
        {
            return new ImplementationInfo(null, () => CreateLazyExpression(serviceType, serviceName));
        }

        private Expression CreateLazyExpression(Type serviceType, string serviceName)
        {
            Type actualServiceType = serviceType.GetGenericArguments().First();
            Type lazyType = typeof(Lazy<>).MakeGenericType(actualServiceType);
            ConstructorInfo ctor = lazyType.GetConstructor(new[] { typeof(Func<>).MakeGenericType(actualServiceType) });
            Expression bodyExression = GetBodyExpression(actualServiceType, serviceName);
            Type funcType = typeof(Func<>).MakeGenericType(actualServiceType);
            LambdaExpression lambdaExpression = Expression.Lambda(funcType, bodyExression, null);
            Expression newExpression = Expression.New(ctor, lambdaExpression);
            return newExpression;
        }

        private ImplementationInfo CreateClosedGenericImplementationInfo(Type serviceType, string serviceName)
        {
            Type openGenericServiceType = serviceType.GetGenericTypeDefinition();
            ImplementationInfo openGenericImplementationInfo = GetImplementationInfo(openGenericServiceType,
                                                                                     serviceName);
            Type closedGenericImplementingType =
                openGenericImplementationInfo.ImplementingType.MakeGenericType(serviceType.GetGenericArguments());
            if (openGenericImplementationInfo.IsSingleton)
                return CreateSingletonImplementationInfo(serviceType, closedGenericImplementingType, serviceName);
            return CreateImplementationInfo(serviceType, closedGenericImplementingType, serviceName);
        }

        /// <summary>
        /// Gets an instance of the given param name="serviceType".
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType)
        {
            return _defaultFactories.GetOrAdd(serviceType, s => CreateDelegate(serviceType, string.Empty))();
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
        /// Gets a named instance of the given <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>    
        public TService GetInstance<TService>(string serviceName)
        {
            return (TService)GetInstance(typeof(TService), serviceName);
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, string serviceName)
        {
            return _namedFactories.GetOrAdd(Tuple.Create(serviceType, serviceName), s => CreateDelegate(serviceType, serviceName))();
        }

        private Func<object> CreateDelegate(Type serviceType, string serviceName)
        {
            var expression = GetBodyExpression(serviceType, serviceName);
            return Expression.Lambda<Func<object>>(expression).Compile();
        }


        private class ImplementationInfo
        {
            public ImplementationInfo(Type implementingType, Func<Expression> factory)
            {
                ImplementingType = implementingType;
                FactoryExpression = new Lazy<Expression>(factory);
            }
            public Type ImplementingType { get; private set; }
            public Lazy<Expression> FactoryExpression { get; private set; }
            public bool IsSingleton { get; set; }
        }

#if !SILVERLIGHT && !NET35
        private class ThreadSafeDictionary<TKey, TValue> : ConcurrentDictionary<TKey,TValue>
        {
            public ThreadSafeDictionary(){}            
            public ThreadSafeDictionary(IEqualityComparer<TKey> comparer) : base(comparer) {}            
        }       
#else
        private class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        {

            private readonly Dictionary<TKey, TValue> _dictionary;
            private readonly object _syncObject = new object();

            public ThreadSafeDictionary() {_dictionary = new Dictionary<TKey, TValue>();}            

            public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
            {
                _dictionary = new Dictionary<TKey, TValue>(comparer);
            }

            public TValue GetOrAdd(TKey key, Func<TKey,TValue> valuefactory)
            {
                lock (_syncObject)
                {
                    TValue value;
                    if (!_dictionary.TryGetValue(key, out value))
                    {
                        value = valuefactory(key);
                        _dictionary.Add(key, value);
                    }
                    return value;
                }
            }    

            public int Count
            {
                get { return _dictionary.Count; }
            }

            public TValue AddOrUpdate(TKey key, Func<TKey,TValue> addValueFactory, Func<TKey,TValue,TValue> updateValueFactory)
            {
                lock (_syncObject)
                {
                    TValue value;
                    if (!_dictionary.TryGetValue(key, out value))
                    {
                        value = addValueFactory(key);
                        _dictionary.Add(key,value);
                    }
                    else
                    {
                        value = updateValueFactory(key, value);
                        _dictionary.Add(key, value);
                    }
                    return value;
                }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                lock (_syncObject)
                    return _dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
#endif

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (IEnumerable<object>)GetInstance(typeof(IEnumerable<>).MakeGenericType(serviceType));
        }

        public IEnumerable<TService>  GetAllInstances<TService>()
        {
            return GetInstance<IEnumerable<TService>>();
        }
    }

    /// <summary>
    /// Contains information about a service request passed to an <see cref="IFactory"/> instance.
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        /// Determines if the service request can be resolved by the underlying container.  
        /// </summary>
        public bool CanProceed { get { return Proceed != null; } }

        /// <summary>
        /// Gets the requested service type.
        /// </summary>
        public Type ServiceType { get; internal set; }

        /// <summary>
        /// Gets the requested service name.
        /// </summary>
        public string ServiceName { get; internal set; }

        /// <summary>
        /// Proceed with resolving the service request.
        /// </summary>
        public Func<object> Proceed { get; internal set; }
    }

    /// <summary>
    /// Represents a factory class that is capable of returning an object instance.
    /// </summary>    
    public interface IFactory
    {
        /// <summary>
        /// Returns an instance of the given type indicated by the <paramref name="serviceRequest"/> 
        /// </summary>        
        /// <returns>An object instance corresponding to the <param name="serviceRequest"/></returns>
        object CreateInstance(ServiceRequest serviceRequest);

        /// <summary>
        /// Determines if this factory can create an instance of the given <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns><b>true</b>, if the instance can be created, otherwise <b>false</b></returns>
        bool CanCreateInstance(Type serviceType, string serviceName);
    }
}
