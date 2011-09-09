using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;

namespace $rootnamespace$
{
    /*****************************************************************************************************
     * Copyright (c) 2011 Bernhard Richter (bernhard.richter@gmail.com)
     *
     * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
     * associated documentation files (the "Software"), to deal in the Software without restriction, 
     * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
     * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
     * subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in all copies or substantial 
     * portions of the Software. 
     * 
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
     * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
     * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
     * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.    
     *****************************************************************************************************/
    /// <summary>
    /// A simple service container implementation.
    /// </summary>
    public class ServiceContainer
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>> _implementationMap =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>>();

        private readonly ConcurrentDictionary<Type, Func<object>> _defaultFactories =
            new ConcurrentDictionary<Type, Func<object>>();

        private readonly ConcurrentDictionary<Tuple<Type, string>, Func<object>> _factories =
            new ConcurrentDictionary<Tuple<Type, string>, Func<object>>();

        private readonly ConcurrentDictionary<Tuple<Type, string>, Expression> _bodyExpressions =
            new ConcurrentDictionary<Tuple<Type, string>, Expression>();

        private static readonly MethodInfo CreateInstanceMethod;

        private List<IFactory> _customFactories;

        private bool _isLoaded;

        static ServiceContainer()
        {
            CreateInstanceMethod = typeof(IFactory).GetMethod("CreateInstance");
        }

        #region Public Instance Resolve Methods

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType)
        {
            return _defaultFactories.GetOrAdd(serviceType, type => CreateDelegate(type, string.Empty))();
        }

        /// <summary>
        /// Gets a named instance of the given <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, string serviceName)
        {
            return _factories.GetOrAdd(Tuple.Create(serviceType, serviceName), sr => CreateDelegate(serviceType, serviceName))();
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested service instance.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return (IEnumerable<object>)GetInstance(typeof(IEnumerable<>).MakeGenericType(serviceType));
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
        /// Gets all instances of the given <typeparamref name="TService"/> type.
        /// </summary>
        /// <typeparam name="TService">The type of the requested service.</typeparam>
        /// <returns>The requested service instance.</returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return GetAllInstances(typeof(TService)).Cast<TService>();
        }

        #endregion

        #region Public Container Loader Methods
        /// <summary>
        /// Loads services from the directory that the runtime uses to probe for assemblies
        /// </summary>
        /// <remarks>
        /// If no services has been loaded into the services container, this method will be 
        /// called the first timeon of the GetInstance(s) overloads are called.
        /// </remarks>
        public void LoadFromBaseDirectory()
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            LoadFrom(directory, "*.dll|*.exe", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Loads services from the given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path for which to scan assemblies.</param>
        /// <param name="searchPattern">The search pattern used to filter the assembly files.
        /// <remarks>
        /// Multiple filters kan be applied separated by the "|" charachter.
        /// </remarks>
        /// </param>
        /// <param name="searchOption"></param>
        public void LoadFrom(string path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            foreach (var file in searchPatterns.SelectMany(sp => Directory.GetFiles(path, sp, searchOption)))
                LoadFrom(Assembly.LoadFrom(file));
        }

        /// <summary>
        /// Loads services from the given <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to scan for services.</param>
        public void LoadFrom(Assembly assembly)
        {
            LoadFrom(assembly, t => true);
        }

        /// <summary>
        /// Loads services from the given <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> for which to scan for services.</param>
        /// <param name="shouldLoad">A function delegate that determines if the currently inspected type should be loaded into the servicecontainer.</param>
        public void LoadFrom(Assembly assembly, Func<Type, bool> shouldLoad)
        {
            IEnumerable<Type> types = GetConcreteTypes(assembly);
            foreach (var type in types.Where(shouldLoad))
                BuildImplementationMap(type);
            _isLoaded = true;
        }

        private static IEnumerable<Type> GetConcreteTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsPublic);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return Type.EmptyTypes;
            }
        }

        private void BuildImplementationMap(Type implementingType)
        {
            Type[] interfaces = implementingType.GetInterfaces();
            foreach (var interfaceType in interfaces)
                RegisterMapping(implementingType, interfaceType);
            foreach (var baseType in GetBaseTypes(implementingType))
                RegisterMapping(implementingType, baseType);
        }

        private void RegisterMapping(Type implementingType, Type baseType)
        {
            Type serviceType = baseType.ContainsGenericParameters
                                   ? baseType.GetGenericTypeDefinition() : baseType;
            _implementationMap.GetOrAdd(serviceType,
                                        new ConcurrentDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase))
                .AddOrUpdate(
                    implementingType.Name, implementingType, (t1, t2) => implementingType);
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

        #endregion

        #region Private Methods

        private ConcurrentDictionary<string, Type> GetImplementations(Type serviceType)
        {
            EnsureServiceContainerIsLoaded();

            ConcurrentDictionary<string, Type> implementations;
            _implementationMap.TryGetValue(serviceType, out implementations);

            if (implementations == null && serviceType.IsGenericType)
            {
                Type genericTypeDefinition = serviceType.GetGenericTypeDefinition();
                _implementationMap.TryGetValue(genericTypeDefinition, out implementations);
            }
            return implementations;
        }

        private void EnsureServiceContainerIsLoaded()
        {
            if (!_isLoaded)
                LoadFromBaseDirectory();
            if (_customFactories == null)
                InitializeCustomFactories();
        }

        private void InitializeCustomFactories()
        {
            _customFactories = new List<IFactory>();
            _customFactories.AddRange(GetAllInstances<IFactory>());
        }

        private Func<object> CreateDelegate(Type serviceType, string serviceName)
        {
            try
            {
                var lambda = Expression.Lambda<Func<object>>(GetBodyExpression(serviceType, serviceName), true, null);
                var compiledLambda = lambda.Compile();
                return compiledLambda;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format("An error oocurred while resolving service. ServiceType = {0}, ServiceName = {1}",
                                  serviceType, serviceName), ex);
            }
        }

        private Expression GetBodyExpression(Type serviceType, string serviceName)
        {
            return _bodyExpressions.GetOrAdd(Tuple.Create(serviceType, GetImplementingTypeName(serviceType, serviceName)),
                                             s => CreateBodyExpression(s.Item1, serviceName));
        }

        private Expression CreateBodyExpression(Type serviceType, string serviceName)
        {
            IFactory customFactory = GetCustomFactory(serviceType, serviceName);
            if (customFactory != null)
                return CreateCustomFactoryExpression(customFactory, serviceType, serviceName);

            if (IsEnumerableOfT(serviceType))
                return CreateNewArrayExpression(serviceType);

            if (IsLazy(serviceType))
                return CreateLazyExpression(serviceType, serviceName);

            if (IsFunc(serviceType))
                return CreateFuncExpression(serviceType, serviceName);

            Type implementingType = GetImplementingType(serviceType, serviceName);
            if (ShouldCreateSingletonExpression(implementingType))
                return CreateSingletonExpression(implementingType);

            return CreateNewExpression(implementingType);
        }

        private Expression CreateSingletonExpression(Type implementingType)
        {
            var newExpression = CreateNewExpression(implementingType);
            var singletonInstance = Expression.Lambda<Func<object>>(newExpression).Compile()();
            return Expression.Constant(singletonInstance);
        }

        private static Expression CreateCustomFactoryExpression(IFactory customFactory, Type serviceType, string serviceName)
        {
            return Expression.Call(Expression.Constant(customFactory), CreateInstanceMethod,
                                   new Expression[] { Expression.Constant(serviceType), Expression.Constant(serviceName) });
        }

        private IFactory GetCustomFactory(Type serviceType, string serviceName)
        {
            return _customFactories.Where(cf => cf.CanCreateInstance(serviceType, serviceName)).FirstOrDefault();
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

        private Expression CreateFuncExpression(Type serviceType, string serviceName)
        {
            Type actualServiceType = serviceType.GetGenericArguments().First();
            Expression bodyExression = GetBodyExpression(actualServiceType, serviceName);
            Type funcType = typeof(Func<>).MakeGenericType(actualServiceType);
            var funcInstance = Expression.Lambda(funcType, bodyExression, null).Compile();
            return Expression.Constant(funcInstance);
        }

        private static bool ShouldCreateSingletonExpression(Type concreteType)
        {
            return (TypeNameStartsOrEndsWithSingleton(concreteType)
                    || IsFactory(concreteType));
        }

        private static bool TypeNameStartsOrEndsWithSingleton(Type concreteType)
        {
            return concreteType.Name.EndsWith("Singleton", StringComparison.InvariantCultureIgnoreCase) ||
                   concreteType.Name.StartsWith("Singleton", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsFactory(Type concreteType)
        {
            return concreteType.GetInterfaces().Any(i => i == typeof(IFactory));
        }

        private Expression CreateNewExpression(Type concreteType)
        {
            ConstructorInfo constructorInfo = GetConstructorInfo(concreteType);
            IEnumerable<Expression> parameterExpressions = GetParameterExpressions(constructorInfo);
            NewExpression newExpression = Expression.New(constructorInfo, parameterExpressions);
            return newExpression;
        }

        private static bool IsEnumerableOfT(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsLazy(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        private static bool IsFunc(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Func<>);
        }

        private NewArrayExpression CreateNewArrayExpression(Type enumerableType)
        {
            Type serviceType = enumerableType.GetGenericArguments().First();
            ConcurrentDictionary<string, Type> implementations = GetImplementations(serviceType);
            List<Expression> newExpressions;
            if (implementations != null)
                newExpressions = implementations.Values
                    .Select(implementation => GetBodyExpression(serviceType, implementation.Name)).ToList();
            else
                newExpressions = new List<Expression>();

            NewArrayExpression newArrayExpression = Expression.NewArrayInit(serviceType, newExpressions);
            return newArrayExpression;
        }

        private static ConstructorInfo GetConstructorInfo(Type concreteType)
        {
            return concreteType.GetConstructors().Single();
        }

        private IEnumerable<Expression> GetParameterExpressions(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().Select(
                parameterInfo =>
                GetBodyExpression(parameterInfo.ParameterType, GetImplementingTypeName(parameterInfo.ParameterType, parameterInfo.Name))).
                ToList();
        }

        private string GetImplementingTypeName(Type serviceType, string serviceName)
        {
            if (IsLazy(serviceType) || IsEnumerableOfT(serviceType))
                serviceType = serviceType.GetGenericArguments().First();

            ConcurrentDictionary<string, Type> implementations = GetImplementations(serviceType);
            if (implementations == null)
                return serviceName;

            if (implementations.Count == 1)
                return implementations.First().Key;

            if (string.IsNullOrEmpty(serviceName))
                return serviceType.IsInterface ? serviceType.Name.Substring(1) : serviceType.Name;

            return serviceName;
        }

        private Type GetImplementingType(Type serviceType, string serviceName)
        {
            ConcurrentDictionary<string, Type> implementations = GetImplementations(serviceType);
            if (implementations == null)
                return null;

            string implementingTypeName = GetImplementingTypeName(serviceType, serviceName);

            Type implementingType;
            implementations.TryGetValue(implementingTypeName, out implementingType);

            if (implementingType == null && serviceType.IsGenericType)
                implementingType = implementations.Values.Where(t => t.Name.StartsWith(implementingTypeName)).SingleOrDefault();

            if (implementingType != null && implementingType.IsGenericTypeDefinition)
                implementingType = implementingType.MakeGenericType(serviceType.GetGenericArguments());

            return implementingType;
        }
        #endregion

    }

    /// <summary>
    /// Represents a factory class that is capable of returning an object instance.
    /// </summary>    
    public interface IFactory
    {
        /// <summary>
        /// Returns an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The requested service type.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns>An instance of the given <paramref name="serviceType"/></returns>
        object CreateInstance(Type serviceType, string serviceName);

        /// <summary>
        /// Determines if this factory can create an instance of the given <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns><b>true</b>, if the instance can be created, otherwise <b>false</b></returns>
        bool CanCreateInstance(Type serviceType, string serviceName);
    }
}
