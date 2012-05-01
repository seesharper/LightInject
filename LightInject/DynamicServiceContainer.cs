using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LightInject
{
    //TODO Add ConstructorArguments at delegate construction time

    //Lazy<ServiceInfo>


    
    public class DynamicServiceContainer : IServiceContainer
    {
        private readonly ThreadSafeDictionary<Type, ThreadSafeDictionary<string, ServiceInfo>> m_availableServices
           =
           new ThreadSafeDictionary<Type, ThreadSafeDictionary<string, ServiceInfo>>();

        


        private readonly ThreadSafeDictionary<Type, Lazy<Func<List<object>, object>>> m_defaultFactories =
            new ThreadSafeDictionary<Type, Lazy<Func<List<object>, object>>>();

        private readonly ThreadSafeDictionary<Tuple<Type, string>, Lazy<Func<List<object>, object>>> _namedFactories =
           new ThreadSafeDictionary<Tuple<Type, string>, Lazy<Func<List<object>, object>>>();

        private readonly List<object> m_constants = new List<object>();

        public void Register(Assembly assembly, Func<Type, bool> shouldLoad)
        {
            throw new NotImplementedException();
        }

        public void Register(string searchPattern, Func<Type, bool> shouldLoad)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implementingType)
        {            
            Register(serviceType, implementingType, string.Empty);
        }

        public void RegisterAsSingleton(Type serviceType, Type implementingType)
        {
            Register(serviceType, string.Empty, new ServiceInfo(){ImplementingType = implementingType, LifeCycle = LifeCycleType.Singleton});
                
        }

        public void RegisterAsSingleton(Type serviceType, Type implementingType, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implementingType, string serviceName)
        {
            Register(serviceType,serviceName, CreateServiceInfo(implementingType, LifeCycleType.Transient));

            ServiceInfoBase serviceInfoBase = new ServiceInfoBase(dmi => this.EmitServiceActivation(null,null));
        }

        public void Register<TService>(Expression<Func<IServiceContainer, TService>> factory)
        {
            Register(typeof(TService), string.Empty, CreateServiceInfoFromExpression(factory,LifeCycleType.Transient));            
        }

        public void RegisterAsSingleton<TService>(Expression<Func<IServiceContainer, TService>> factory)
        {
            Register(typeof(TService), string.Empty, CreateServiceInfoFromExpression(factory, LifeCycleType.Singleton));
        }

        public void Register<TService>(Expression<Func<IServiceContainer, TService>> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public void RegisterAsSingleton<TService>(Expression<Func<IServiceContainer, TService>> factory, string serviceName)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType)
        {
            return m_defaultFactories.GetOrAdd(
                serviceType,
                t =>
                new Lazy<Func<List<object>, object>>(() => CreateDelegate(serviceType, string.Empty))).Value(m_constants);
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
            return _namedFactories.GetOrAdd(Tuple.Create(serviceType, serviceName), s => new Lazy<Func<List<object>, object>>(() => CreateDelegate(serviceType, serviceName))).Value(m_constants);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }

        private ThreadSafeDictionary<string, ServiceInfo> GetImplementations(Type serviceType)
        {
            return m_availableServices.GetOrAdd(serviceType, s => new ThreadSafeDictionary<string, ServiceInfo>(StringComparer.InvariantCultureIgnoreCase));
        }





        private void Register(Type serviceType, string serviceName, ServiceInfo serviceInfo)
        {
            GetImplementations(serviceType).AddOrUpdate(serviceName, s => serviceInfo, (s, i) => serviceInfo);
        }

        
        private ServiceInfo CreateServiceInfoFromExpression(LambdaExpression lambdaExpression, LifeCycleType lifeCycleType)
        {
            var methodCallVisitor = new MethodCallVisitor();
            ServiceInfo serviceInfo = methodCallVisitor.CreateServiceInfo(lambdaExpression);
            serviceInfo.LifeCycle = lifeCycleType;
            return serviceInfo;
        }

        private static ServiceInfo CreateServiceInfo(Type implementingType, LifeCycleType lifeCycleType)
        {                        
            ConstructorInfo constructorInfo = GetConstructorWithTheMostParameters(implementingType);
            var serviceInfo = new ServiceInfo { ImplementingType = implementingType, Constructor = constructorInfo, LifeCycle = lifeCycleType };
            foreach (var parameterInfo in constructorInfo.GetParameters().OrderBy(p => p.Position))
            {
                serviceInfo.ConstructorArguments.Add(new Dependency {ServiceName = string.Empty, ServiceType = parameterInfo.ParameterType});
            }
            return serviceInfo;
        }


        private Func<List<object>, object> CreateDelegate(Type serviceType, string serviceName)
        {
            return CreateDynamicMethodDelegate(serviceType, serviceName);
        }

        private Func<List<object>, object> CreateDynamicMethodDelegate(Type serviceType, string serviceName)
        {                                    
            var dynamicMethodInfo = new DynamicMethodInfo();
            ServiceInfo serviceInfo = GetServiceInfo(serviceType, serviceName);
            EmitServiceActivation(serviceInfo, dynamicMethodInfo);
            dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Ret);        
            return dynamicMethodInfo.CreateDelegate();            
        }
       
        private void EmitServiceActivation(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            switch (serviceInfo.LifeCycle)
            {
                case LifeCycleType.Singleton:
                    EmitSingletonInstance(serviceInfo, dynamicMethodInfo);
                    break;
                case LifeCycleType.Transient:
                    EmitTransientInstance(serviceInfo, dynamicMethodInfo);
                    break;
            }                        
        }



        private void EmitSingletonInstance(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            EmitConstant(dynamicMethodInfo, CreateSingletonInstance(serviceInfo));
        }

        private void EmitTransientInstance(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {
            if (IsFunc(serviceInfo.ImplementingType))
            {
                EmitFuncInstance(serviceInfo, dynamicMethodInfo);
                return;
            }                        
            
            ILGenerator ilGenerator = dynamicMethodInfo.GetILGenerator();
            
            if (dynamicMethodInfo.ContainsLocalVariable(serviceInfo.ImplementingType))
            {
                ilGenerator.Emit(OpCodes.Ldloc, dynamicMethodInfo.GetLocalVariable(serviceInfo.ImplementingType));
            }
            else
            {
                                                                
                foreach (Dependency dependency in serviceInfo.ConstructorArguments)
                {
                    var parameterServiceInfo = GetServiceInfo(dependency.ServiceType, string.Empty);
                    EmitServiceActivation(parameterServiceInfo, dynamicMethodInfo);
                }
               
                ilGenerator.Emit(OpCodes.Newobj, serviceInfo.Constructor);
                if (serviceInfo.LifeCycle == LifeCycleType.Request)
                {
                    LocalBuilder local = ilGenerator.DeclareLocal(serviceInfo.ImplementingType);
                    ilGenerator.Emit(OpCodes.Stloc, local);
                    ilGenerator.Emit(OpCodes.Ldloc, local);
                    dynamicMethodInfo.RegisterLocalVaribele(serviceInfo.ImplementingType, local);
                }
            }
        }
       
        private void EmitFuncInstance<TServiceType>(Type serviceType, DynamicMethodInfo dynamicMethodInfo)
        {            
            Func<List<object>, object> bodyDelegate = CreateDelegate(serviceType, string.Empty);
            Func<TServiceType> func = () => (TServiceType)bodyDelegate(m_constants);
            EmitConstant(dynamicMethodInfo, func);                       
        }

        private void EmitFuncInstance(ServiceInfo serviceInfo, DynamicMethodInfo dynamicMethodInfo)
        {          
            Type actualServiceType = serviceInfo.ImplementingType.GetGenericArguments().First();
            var methodInfo = typeof(DynamicServiceContainer).GetMethod(
                "EmitFuncInstance", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Type), typeof(DynamicMethodInfo) }, null);            
            methodInfo.MakeGenericMethod(actualServiceType).Invoke(this, new object[] { actualServiceType, dynamicMethodInfo });
        }

        private object CreateSingletonInstance(ServiceInfo serviceInfo)
        {
            var transientServiceInfo = new ServiceInfo { ImplementingType = serviceInfo.ImplementingType, LifeCycle = LifeCycleType.Transient, Constructor = serviceInfo.Constructor, ConstructorArguments = serviceInfo.ConstructorArguments};
            var dynamicMethodInfo = new DynamicMethodInfo();
            EmitServiceActivation(transientServiceInfo, dynamicMethodInfo);
            dynamicMethodInfo.GetILGenerator().Emit(OpCodes.Ret);       
            object instance = dynamicMethodInfo.CreateDelegate()(m_constants);
            return instance;
        }


        private void EmitConstant(DynamicMethodInfo dynamicMethodInfo, object value)
        {
            MethodInfo method = typeof(List<object>).GetMethod("get_Item");
            int index = GetConstantIndex(value);
            var ilGenerator = dynamicMethodInfo.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldc_I4, index);
            ilGenerator.Emit(OpCodes.Callvirt, method);
            ilGenerator.Emit(OpCodes.Castclass, value.GetType());
        }
       

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int GetConstantIndex(object value)
        {
            if (!m_constants.Contains(value))
                m_constants.Add(value);
            return m_constants.IndexOf(value);
        }

        private static ConstructorInfo GetConstructorWithTheMostParameters(Type implementingType)
        {                                                           
            return implementingType.GetConstructors().OrderBy(c => c.GetParameters().Count()).LastOrDefault();           
        }

        private ServiceInfo GetServiceInfo(Type serviceType, string serviceName)
        {
            ServiceInfo implementationInfo;
            var implementations = GetImplementations(serviceType);

            implementations.TryGetValue(serviceName, out implementationInfo);
            
            if (implementationInfo == null)
                return ResolveUnknownServiceType(serviceType, serviceName);

            return implementationInfo;
        }

        private ServiceInfo ResolveUnknownServiceType(Type serviceType, string serviceName)
        {
            if (CanRedirectRequestForDefaultServiceToSingleNamedService(serviceType, serviceName))
                return CreateImplementationInfoBasedOnFirstNamedInstance(serviceType);

            if (IsEnumerableOfT(serviceType))
            {
                
            }

            if (IsClosedGeneric(serviceType))
            {
                return CreateServiceInfoBasedOnClosedGenericServiceRequest(serviceType, serviceName);
            }

            if (IsFunc(serviceType))
            {
                return new ServiceInfo(){ImplementingType = serviceType,LifeCycle = LifeCycleType.Singleton};
            }
               
            return null;
        }

        private static bool IsEnumerableOfT(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private ServiceInfo CreateImplementationInfoBasedOnFirstNamedInstance(Type serviceType)
        {
            return GetServiceInfo(serviceType, GetImplementations(serviceType).First().Key);
        }

        private bool CanRedirectRequestForDefaultServiceToSingleNamedService(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) && GetImplementations(serviceType).Count == 1;
        }

        private ServiceInfo CreateServiceInfoBasedOnClosedGenericServiceRequest(Type serviceType, string serviceName)
        {
            Type openGenericServiceType = serviceType.GetGenericTypeDefinition();
            ServiceInfo openGenericImplementationInfo = GetServiceInfo(openGenericServiceType, serviceName);
            if (openGenericImplementationInfo == null)
                return null;
            Type closedGenericImplementingType =
                openGenericImplementationInfo.ImplementingType.MakeGenericType(serviceType.GetGenericArguments());                                   
            var serviceInfo = new ServiceInfo()
                                      { ImplementingType = closedGenericImplementingType, LifeCycle = openGenericImplementationInfo.LifeCycle };

            return serviceInfo;
            
        }

        private static bool IsClosedGeneric(Type serviceType)
        {
            return serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition;
        }

        private static bool IsFunc(Type serviceType)
        {
            return serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Func<>);
        }

        private bool HasMultipleImplementations(Type serviceType)
        {
            return this.GetImplementations(serviceType).Count > 1;
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return (GetServiceInfo(serviceType, serviceName) != null);
        }




        private class DynamicMethodInfo
        {
            private DynamicMethod m_dynamicMethod;

            private IDictionary<Type, LocalBuilder> m_localVariables = new Dictionary<Type, LocalBuilder>();

            public DynamicMethodInfo()
            {
                CreateDynamicMethod();
            }

            public ILGenerator GetILGenerator()
            {
                return m_dynamicMethod.GetILGenerator();
            }

            private void CreateDynamicMethod()
            {
                m_dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(List<object>) }, typeof(ServiceContainer).Module, false);
            }

            public Func<List<object>, object> CreateDelegate()
            {
                return (Func<List<object>, object>)m_dynamicMethod.CreateDelegate(typeof(Func<List<object>, object>));
            }

            public void RegisterLocalVaribele(Type implementingType, LocalBuilder localBuilder)
            {
                m_localVariables.Add(implementingType, localBuilder);
            }

            public bool ContainsLocalVariable(Type implementingType)
            {
                return m_localVariables.ContainsKey(implementingType);
            }


            public LocalBuilder GetLocalVariable(Type implementingType)
            {
                return m_localVariables[implementingType];
            }
        }

        

        private class ServiceInfo        
        {
            public ServiceInfo()
            {
                ConstructorArguments = new List<Dependency>();
            }
            
            public Type ImplementingType { get; set; }

            public LifeCycleType LifeCycle { get; set; }

            public IList<Dependency> ConstructorArguments { get; set; }

            public ConstructorInfo Constructor { get; set; }            
        }



        private class ServiceInfoBase
        {
            private readonly Action<DynamicMethodInfo> emitMethod;

            public ServiceInfoBase(Action<DynamicMethodInfo> emitMethod)
            {
                this.emitMethod = emitMethod;
            }

            public void Emit(DynamicMethodInfo dynamicMethodInfo)
            {
                emitMethod(dynamicMethodInfo);
            }
        }
       
        private class Dependency
        {
            public Type ServiceType { get; set; }

            public string ServiceName { get; set; }
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


        private class MethodCallVisitor : ExpressionVisitor
        {
            private ServiceInfo m_serviceInfo;
                       
            public ServiceInfo CreateServiceInfo(LambdaExpression expression)
            {
                Visit(expression.Body);
                
                return m_serviceInfo;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (RepresentsGetInstanceMethod(node))
                {
                    var constructorArgument = new Dependency
                                              { ServiceName = string.Empty, ServiceType = node.Method.GetGenericArguments().First() }; 
                    m_serviceInfo.ConstructorArguments.Add(constructorArgument);                                                          
                }

                if (RepresentsGetNamedInstanceMethod(node))
                {
                    var constructorArgument = new Dependency { ServiceName = (string)((ConstantExpression)node.Arguments[0]).Value, ServiceType = node.Method.GetGenericArguments().First() };
                    m_serviceInfo.ConstructorArguments.Add(constructorArgument);      
                }

                return base.VisitMethodCall(node);
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
                return typeof(IServiceContainer).IsAssignableFrom(methodInfo.DeclaringType) && methodInfo.Name == "GetInstance";
            }

            private static bool IsGetAllInstancesMethod(MethodInfo methodInfo)
            {
                return typeof(IServiceContainer).IsAssignableFrom(methodInfo.DeclaringType) && methodInfo.Name == "GetAllInstances";
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

            protected override Expression VisitNew(NewExpression node)
            {
                m_serviceInfo = new ServiceInfo();
                m_serviceInfo.ImplementingType = node.Constructor.DeclaringType;                
                m_serviceInfo.Constructor = node.Constructor;
                return base.VisitNew(node);
            }             
        }

    }
}