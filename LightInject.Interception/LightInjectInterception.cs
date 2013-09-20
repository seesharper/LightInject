[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Interception
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Implemented by all proxy typeArguments.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// Gets the proxy target.
        /// </summary>
        object Target { get; }
    }

    public interface IInvocationInfo
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> currently being invoked.
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// Gets the <see cref="IProxy"/> instance.
        /// </summary>
        IProxy Proxy { get; }

        /// <summary>
        /// Gets the arguments currently being passed to the target method.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Proceeds to the next interceptor.
        /// </summary>
        /// <returns>The return value.</returns>
        object Proceed();
    }

    /// <summary>
    /// Represents a class that is capable of creating a delegate used to invoke 
    /// a method without using late-bound invocation.
    /// </summary>
    public interface IMethodBuilder
    {
        /// <summary>
        /// Creates a delegate that is used to invoke the <paramref name="targetMethod"/>.
        /// </summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> that represents the target method to invoke.</param>
        /// <returns>A delegate that represents compiled code used to invoke the <paramref name="targetMethod"/>.</returns>
        Func<object, object[], object> CreateDelegate(MethodInfo targetMethod);
    }

    /// <summary>
    /// Represents the skeleton of a dynamic method.
    /// </summary>    
    public interface IDynamicMethodSkeleton
    {
        /// <summary>
        /// Gets the <see cref="ILGenerator"/> used to emit the method body.
        /// </summary>
        /// <returns>An <see cref="ILGenerator"/> instance.</returns>
        ILGenerator GetILGenerator();

        /// <summary>
        /// Create a delegate used to invoke the dynamic method.
        /// </summary>
        /// <returns>A function delegate.</returns>
        Func<object, object[], object> CreateDelegate();
    }

    /// <summary>
    /// Represents a class that is capable of creating a proxy <see cref="Type"/>.
    /// </summary>
    public interface IProxyBuilder
    {
        /// <summary>
        /// Gets a proxy type based on the given <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">A <see cref="ProxyDefinition"/> instance that contains information about the 
        /// proxy type to be created.</param>
        /// <returns>A proxy <see cref="Type"/>.</returns>
        Type GetProxyType(ProxyDefinition definition);
    }
     
    /// <summary>
    /// Represents a class that intercepts method calls.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Invoked when a method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        object Invoke(IInvocationInfo invocationInfo);
    }

    /// <summary>
    /// Represents a class that is capable of creating a <see cref="TypeBuilder"/> that 
    /// is used to build the proxy type.
    /// </summary>
    internal interface ITypeBuilderFactory
    {
        TypeBuilder CreateTypeBuilder(Type targetType, Type[] additionalInterfaces);

        Type CreateType(TypeBuilder typeBuilder);
    }

    public static class MethodInterceptorFactory
    {
        public static Lazy<IInterceptor> CreateMethodInterceptor(Lazy<IInterceptor>[] interceptors)
        {
            if (interceptors.Length > 1)
            {
                return
                    new Lazy<IInterceptor>(() => new CompositeInterceptor(interceptors.Select(i => i.Value).ToArray()));
            }

            return interceptors[0];            
        }
    }

    public static class ProceedDelegateBuilder
    {
        private static readonly DynamicMethodBuilder Builder = new DynamicMethodBuilder();

        public static Func<object, object[], object> CreateDelegate(MethodInfo methodInfo)
        {
            return Builder.CreateDelegate(methodInfo);
        }
    }

    /// <summary>
    /// Contains information about the method being intercepted.
    /// </summary>
    public class InterceptedMethodInfo
    {
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Loading a field is faster than going through a property.")]
        public Lazy<Func<object, object[], object>> ProceedDelegate;

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Loading a field is faster than going through a property.")]
        public MethodInfo Method;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptedMethodInfo"/> class.
        /// </summary>
        /// <param name="method">The target <see cref="MethodInfo"/> being intercepted.</param>
        public InterceptedMethodInfo(MethodInfo method)
        {
            ProceedDelegate = new Lazy<Func<object, object[], object>>(() => ProceedDelegateBuilder.CreateDelegate(method));
            Method = method;
        }
    }

    public class OpenGenericInterceptedMethodInfo
    {
        private readonly MethodInfo openGenericMethod;

        private readonly Dictionary<Type[], InterceptedMethodInfo> cache =
            new Dictionary<Type[], InterceptedMethodInfo>(new TypeArrayComparer());

        private readonly object lockObject = new object();

        public OpenGenericInterceptedMethodInfo(MethodInfo openGenericMethod)
        {
            this.openGenericMethod = openGenericMethod;
        }

        public InterceptedMethodInfo GetInterceptedMethodInfo(Type[] typeArguments)
        {
            InterceptedMethodInfo delegateInfo;
            if (!cache.TryGetValue(typeArguments, out delegateInfo))
            {
                lock (lockObject)
                {
                    if (!cache.TryGetValue(typeArguments, out delegateInfo))
                    {
                        delegateInfo = CreateDelegateInfo(typeArguments);
                        cache.Add(typeArguments, delegateInfo);
                    }
                }
            }

            return delegateInfo;
        }

        private InterceptedMethodInfo CreateDelegateInfo(Type[] types)
        {
            var closedGenericMethod = openGenericMethod.MakeGenericMethod(types);
            return new InterceptedMethodInfo(closedGenericMethod);
        }
    }

    public class DynamicMethodBuilder : IMethodBuilder
    {       
        private static readonly Dictionary<MethodInfo, Func<object, object[], object>> Cache
            = new Dictionary<MethodInfo, Func<object, object[], object>>();

        private static readonly object SyncRoot = new object();

        private readonly Func<IDynamicMethodSkeleton> methodSkeletonFactory;
#if TEST
        public DynamicMethodBuilder(Func<IDynamicMethodSkeleton> methodSkeletonFactory)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
        }
#endif

        public DynamicMethodBuilder()
        {
            methodSkeletonFactory = () => new DynamicMethodSkeleton();
        }

        public object Invoke(MethodInfo method, object instance, object[] arguments)
        {
            Func<object, object[], object> del;
            if (!Cache.TryGetValue(method, out del))
            {
                lock (SyncRoot)
                {
                    if (!Cache.TryGetValue(method, out del))
                    {
                        del = CreateDelegate(method);
                        Cache.Add(method, del);
                    }
                }
            }

            return del(instance, arguments);            
        }

        public Func<object, object[], object> CreateDelegate(MethodInfo method)
        {
            var parameters = method.GetParameters();
            IDynamicMethodSkeleton methodSkeleton = methodSkeletonFactory();            
            var il = methodSkeleton.GetILGenerator();            
            PushInstance(method, il);
            PushArguments(parameters, il);
            CallTargetMethod(method, il);
            UpdateOutAndRefArguments(parameters, il);
            PushReturnValue(method, il);
            return methodSkeleton.CreateDelegate();
        }

        private static void PushReturnValue(MethodInfo method, ILGenerator il)
        {
            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else
            {
                BoxIfNecessary(method.ReturnType, il);                
            }

            il.Emit(OpCodes.Ret);
        }
        
        private static void PushArguments(ParameterInfo[] parameters, ILGenerator il)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                PushObjectValueFromArgumentArray(il, i);
                PushArgument(parameters[i], il);
            }
        }

        private static void PushArgument(ParameterInfo parameter, ILGenerator il)
        {
            Type parameterType = parameter.ParameterType;
            if (parameter.IsOut || parameter.ParameterType.IsByRef)
            {
                PushOutOrRefArgument(parameter, il);
            }
            else
            {
                UnboxOrCast(parameterType, il);
            }
        }

        private static void PushOutOrRefArgument(ParameterInfo parameter, ILGenerator il)
        {
            Type parameterType = parameter.ParameterType.GetElementType();
            LocalBuilder outValue = il.DeclareLocal(parameterType);
            UnboxOrCast(parameterType, il);
            il.Emit(OpCodes.Stloc, outValue);
            il.Emit(OpCodes.Ldloca, outValue);
        }

        private static void PushObjectValueFromArgumentArray(ILGenerator il, int parameterIndex)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4, parameterIndex);
            il.Emit(OpCodes.Ldelem_Ref);
        }

        private static void CallTargetMethod(MethodInfo method, ILGenerator il)
        {
            il.Emit(OpCodes.Callvirt, method);
        }

        private static void PushInstance(MethodInfo method, ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, method.GetDeclaringType());
        }
       
        private static void UnboxOrCast(Type parameterType, ILGenerator il)
        {
            il.Emit(parameterType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, parameterType);
        }

        private static void UpdateOutAndRefArguments(ParameterInfo[] parameters, ILGenerator il)
        {
            int localIndex = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].IsOut || parameters[i].ParameterType.IsByRef)
                {
                    var parameterType = parameters[i].ParameterType.GetElementType();
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldloc, localIndex);
                    BoxIfNecessary(parameterType, il);
                    il.Emit(OpCodes.Stelem_Ref);
                    localIndex++;
                }
            }
        }

        private static void BoxIfNecessary(Type parameterType, ILGenerator il)
        {
            if (parameterType.IsValueType)
            {
                il.Emit(OpCodes.Box, parameterType);
            }
        }

        private class DynamicMethodSkeleton : IDynamicMethodSkeleton
        {
            private readonly DynamicMethod dynamicMethod = new DynamicMethod("DynamicMethod", typeof(object), new[] { typeof(object), typeof(object[]) }, typeof(DynamicMethodSkeleton).Module);

            /// <summary>
            /// Gets the <see cref="ILGenerator"/> used to emit the method body.
            /// </summary>
            /// <returns>An <see cref="ILGenerator"/> instance.</returns>
            public ILGenerator GetILGenerator()
            {
                return dynamicMethod.GetILGenerator();
            }

            /// <summary>
            /// Create a delegate used to invoke the dynamic method.
            /// </summary>
            /// <returns>A function delegate.</returns>
            public Func<object, object[], object> CreateDelegate()
            {
                return (Func<object, object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object, object[], object>));
            }
        }
    }

    public class InvocationInfo : IInvocationInfo
    {
        private readonly Lazy<Func<object, object[], object>> lazyProceed;

        private readonly IProxy proxy;

        private readonly object[] arguments;

        private readonly MethodInfo method;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationInfo"/> class.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> currently being invoked.</param>
        /// <param name="proceed">The function delegate use to invoke the target method.</param>        
        /// <param name="proxy">The <see cref="IProxy"/> object from which methods are intercepted.</param>
        /// <param name="arguments">The arguments currently being passed to the target method.</param>        
        public InvocationInfo(MethodInfo method, Lazy<Func<object, object[], object>> proceed, IProxy proxy, object[] arguments)            
        {
            this.method = method;
            this.proxy = proxy;
            this.arguments = arguments;
            lazyProceed = proceed;            
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> currently being invoked.
        /// </summary>
        public MethodInfo Method
        {
            get
            {
                return method;
            }            
        }

        /// <summary>
        /// Gets the <see cref="IProxy"/> instance.
        /// </summary>
        public IProxy Proxy
        {
            get
            {
                return proxy;
            }           
        }

        /// <summary>
        /// Gets the arguments currently being passed to the target method.
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return arguments;
            }            
        }

        public object Proceed()
        {
            return lazyProceed.Value(proxy.Target, arguments);
        }
    }

    /// <summary>
    /// An <see cref="IInterceptor"/> that is responsible for 
    /// passing the <see cref="InvocationInfo"/> down the interceptor chain.
    /// </summary>
    public class CompositeInterceptor : IInterceptor
    {
        private readonly IInterceptor[] interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeInterceptor"/> class.
        /// </summary>
        /// <param name="interceptors">The <see cref="IInterceptor"/> chain to be invoked.</param>
        public CompositeInterceptor(IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
        }

        /// <summary>
        /// Invoked when a method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="InvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        public object Invoke(IInvocationInfo invocationInfo)
        {
            for (int i = interceptors.Length - 1; i >= 0; i--)
            {
                int index = i;
                IInvocationInfo nextInvocationInfo = invocationInfo;                
                var lazyNextProceedDelegate = new Lazy<Func<object, object[], object>>(() => (instance, arguments) => interceptors[index].Invoke(nextInvocationInfo));                    
                invocationInfo = new InvocationInfo(invocationInfo.Method, lazyNextProceedDelegate, invocationInfo.Proxy, invocationInfo.Arguments);
            }

            return interceptors[0].Invoke(invocationInfo);
        }
    }

    /// <summary>
    /// Contains information about a registered <see cref="IInterceptor"/>.
    /// </summary>
    public class InterceptorInfo
    {
        /// <summary>
        /// Gets or sets the function delegate used to create the <see cref="IInterceptor"/> instance.
        /// </summary>
        public Func<IInterceptor> InterceptorFactory { get; set; }

        /// <summary>
        /// Gets or sets the function delegate used to selected the methods to be intercepted.
        /// </summary>
        public Func<MethodInfo, bool> MethodSelector { get; set; }

        /// <summary>
        /// Gets or sets the index of this <see cref="InterceptorInfo"/> instance.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Represents the definition of a proxy type.
    /// </summary>
    public class ProxyDefinition
    {
        private readonly ICollection<InterceptorInfo> interceptors = new Collection<InterceptorInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyDefinition"/> class.
        /// </summary>
        /// <param name="targetType">The type of object to proxy.</param>        
        /// <param name="additionalInterfaces">A list of additional interfaces to be implemented by the proxy type.</param>
        public ProxyDefinition(Type targetType, params Type[] additionalInterfaces) 
            : this(targetType, null, additionalInterfaces)
        {            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyDefinition"/> class.
        /// </summary>
        /// <param name="targetType">The type of object to proxy.</param>
        /// <param name="targetFactory">A function delegate used to create the target instance.</param>
        /// <param name="additionalInterfaces">A list of additional interfaces to be implemented by the proxy type.</param>
        public ProxyDefinition(Type targetType, Func<object> targetFactory, params Type[] additionalInterfaces)
        {
            TargetType = targetType;
            TargetFactory = targetFactory;
            AdditionalInterfaces = additionalInterfaces;
        }

        /// <summary>
        /// Gets the proxy target type.
        /// </summary>
        internal Type TargetType { get; private set; }

        /// <summary>
        /// Gets the function delegate used to create the proxy target.
        /// </summary>
        internal Func<object> TargetFactory { get; private set; }

        /// <summary>
        /// Gets an list of additional interfaces to be implemented by the proxy type.
        /// </summary>
        internal Type[] AdditionalInterfaces { get; private set; }

        /// <summary>
        /// Gets a list of the registered <see cref="InterceptorInfo"/> instances.
        /// </summary>
        internal IEnumerable<InterceptorInfo> Interceptors
        {
            get
            {
                return interceptors.AsEnumerable();
            }
        }

        /// <summary>
        /// Implements the methods identified by the <paramref name="methodSelector"/> by forwarding method calls
        /// to the <see cref="IInterceptor"/> created by the <paramref name="interceptorFactory"/>.
        /// </summary>
        /// <param name="methodSelector">A function delegate used to select the methods to be implemented</param>
        /// <param name="interceptorFactory">A function delegate used to create the <see cref="IInterceptor"/> instance.</param>
        public void Implement(Func<MethodInfo, bool> methodSelector, Func<IInterceptor> interceptorFactory)
        {
            interceptors.Add(new InterceptorInfo
            {
                InterceptorFactory = interceptorFactory,
                MethodSelector = methodSelector,
                Index = interceptors.Count
            });
        }
    }

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> interface by adding methods for 
    /// creating proxy-based decorators.
    /// </summary>
    internal static class InterceptionContainerExtensions
    {
        private static readonly ConcurrentDictionary<ServiceRegistration, Type> ProxyCache =
            new ConcurrentDictionary<ServiceRegistration, Type>();

        /// <summary>
        /// Decorates the service identified by the <paramref name="serviceSelector"/> delegate with a dynamic proxy type
        /// that is used to decorate the target type.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceSelector">A function delegate that is used to determine if the proxy-based decorator should be applied to the target service.</param>
        /// <param name="additionalInterfaces">A list of additional interface that will be implemented by the proxy type.</param>
        /// <param name="defineProxyType">An action delegate that is used to define the proxy type.</param>
        public static void Decorate(this IServiceRegistry serviceRegistry, Func<ServiceRegistration, bool> serviceSelector, Type[] additionalInterfaces, Action<ProxyDefinition> defineProxyType)
        {
            var decoratorRegistration = new DecoratorRegistration();
            decoratorRegistration.CanDecorate = serviceSelector;
            decoratorRegistration.ImplementingTypeFactory = sr => GetProxyType(sr, additionalInterfaces, defineProxyType);
            serviceRegistry.Decorate(decoratorRegistration);
        }

        /// <summary>
        /// Decorates the service identified by the <paramref name="serviceSelector"/> delegate with a dynamic proxy type
        /// that is used to decorate the target type.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceSelector">A function delegate that is used to determine if the proxy-based decorator should be applied to the target service.</param>        
        /// <param name="defineProxyType">An action delegate that is used to define the proxy type.</param>
        public static void Decorate(this IServiceRegistry serviceRegistry, Func<ServiceRegistration, bool> serviceSelector, Action<ProxyDefinition> defineProxyType)
        {
            Decorate(serviceRegistry, serviceSelector, Type.EmptyTypes, defineProxyType);
        }

        private static Type GetProxyType(ServiceRegistration serviceRegistration, Type[] additionalInterfaces, Action<ProxyDefinition> defineProxyType)
        {
            return ProxyCache.GetOrAdd(serviceRegistration, sr => CreateProxyType(sr, additionalInterfaces, defineProxyType));
        }

        private static Type CreateProxyType(
            ServiceRegistration registration, Type[] additionalInterfaces, Action<ProxyDefinition> defineProxyType)
        {
            var proxyBuilder = new ProxyBuilder();
            var proxyDefinition = new ProxyDefinition(registration.ServiceType, additionalInterfaces);
            defineProxyType(proxyDefinition);
            return proxyBuilder.GetProxyType(proxyDefinition);
        }
    }

    /// <summary>
    /// Extends the <see cref="MethodInfo"/> class.
    /// </summary>
    internal static class MethodInfoExtensions
    {
        /// <summary>
        /// Gets the declaring type of the target <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> for which to return the declaring type.</param>
        /// <returns>The type that declares the target <paramref name="method"/>.</returns>
        public static Type GetDeclaringType(this MethodInfo method)
        {
            Type declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentException(string.Format("Method {0} does not have a declaring type", method), "method");
            }

            return declaringType;
        }
    }
     
    internal static class TypeBuilderExtensions
    {
        public static FieldBuilder DefinePrivateField(this TypeBuilder typeBuilder, string fieldName, Type type)
        {
            return typeBuilder.DefineField(fieldName, type, FieldAttributes.Private);
        }

        public static FieldBuilder DefinePublicStaticField(this TypeBuilder typeBuilder, string fieldName, Type type)
        {
            return typeBuilder.DefineField(fieldName, type, FieldAttributes.Public | FieldAttributes.Static);
        }
    }

    internal class TypeBuilderFactory : ITypeBuilderFactory
    {
        public TypeBuilder CreateTypeBuilder(Type targetType, Type[] additionalInterfaces)
        {
            ModuleBuilder moduleBuilder = GetModuleBuilder();
            const TypeAttributes TypeAttributes = TypeAttributes.Public | TypeAttributes.Class;
            var typeName = targetType.Name + "Proxy";
            Type[] interfaceTypes = new[] { targetType }.Concat(additionalInterfaces).ToArray();
            return moduleBuilder.DefineType(typeName, TypeAttributes, null, interfaceTypes);    
        }

        public Type CreateType(TypeBuilder typeBuilder)
        {
            return typeBuilder.CreateType();
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            AssemblyBuilder assemblyBuilder = GetAssemblyBuilder();
            return assemblyBuilder.DefineDynamicModule("ProxyAssembly");
        }

        private static AssemblyBuilder GetAssemblyBuilder()
        {
            var assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
            new AssemblyName("ProxyAssembly"), AssemblyBuilderAccess.Run);
            return assemblybuilder;
        }
    }

    internal class ProxyBuilder : IProxyBuilder
    {        
        private static readonly ConstructorInfo LazyInterceptorConstructor;
        private static readonly ConstructorInfo InvocationInfoConstructor;
        private static readonly ConstructorInfo InterceptedMethodInfoConstructor;
        private static readonly ConstructorInfo OpenGenericInterceptedMethodInfoConstructor;
        private static readonly ConstructorInfo ObjectConstructor;
        private static readonly MethodInfo GetTargetMethod;        
        private static readonly MethodInfo CreateMethodInterceptorMethod;
        private static readonly MethodInfo GetMethodFromHandleMethod;
        private static readonly MethodInfo GetTypeFromHandleMethod;        
        private static readonly MethodInfo LazyInterceptorGetValueMethod;
        private static readonly MethodInfo InterceptorInvokeMethod;
        private static readonly MethodInfo GetInterceptedMethodInfoMethod;
        
        private static readonly FieldInfo InterceptedMethodInfoProceedDelegateField;
        private static readonly FieldInfo InterceptedMethodInfoMethodField;
        
        private readonly Dictionary<string, int> memberNames = new Dictionary<string, int>();
        private readonly ITypeBuilderFactory typeBuilderFactory;
        
        private FieldBuilder targetFactoryField;
        private FieldBuilder lazyTargetField;
        private FieldInfo[] lazyInterceptorFields;
        private MethodInfo[] targetMethods;
        private TypeBuilder typeBuilder;
        private MethodBuilder initializerMethodBuilder;
        private ConstructorBuilder staticConstructorBuilder;        
        private ProxyDefinition proxyDefinition;

        static ProxyBuilder()
        {
            GetTargetMethod = typeof(IProxy).GetMethod("get_Target");
            LazyInterceptorConstructor = typeof(Lazy<IInterceptor>).GetConstructor(new[] { typeof(Func<IInterceptor>) });
            ObjectConstructor = typeof(object).GetConstructor(Type.EmptyTypes);
            CreateMethodInterceptorMethod = typeof(MethodInterceptorFactory).GetMethod("CreateMethodInterceptor");
            GetMethodFromHandleMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) });
            GetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            InterceptedMethodInfoConstructor = typeof(InterceptedMethodInfo).GetConstructors()[0];
            OpenGenericInterceptedMethodInfoConstructor = typeof(OpenGenericInterceptedMethodInfo).GetConstructors()[0];
            LazyInterceptorGetValueMethod = typeof(Lazy<IInterceptor>).GetProperty("Value").GetGetMethod();
            InterceptedMethodInfoMethodField = typeof(InterceptedMethodInfo).GetField("Method");
            InterceptedMethodInfoProceedDelegateField = typeof(InterceptedMethodInfo).GetField("ProceedDelegate");
            InvocationInfoConstructor = typeof(InvocationInfo).GetConstructors()[0];
            InterceptorInvokeMethod = typeof(IInterceptor).GetMethod("Invoke");
            GetInterceptedMethodInfoMethod = typeof(OpenGenericInterceptedMethodInfo).GetMethod("GetInterceptedMethodInfo");
        }

        public ProxyBuilder()
        {
            typeBuilderFactory = new TypeBuilderFactory();
        }
#if TEST
        public ProxyBuilder(ITypeBuilderFactory typeBuilderFactory)
        {
            this.typeBuilderFactory = typeBuilderFactory;
        }
#endif
        public Type GetProxyType(ProxyDefinition definition)
        {
            proxyDefinition = definition;
            InitializeTypeBuilder();
            DefineLazyTargetField();                        
            DefineInitializerMethod();
            DefineStaticTargetFactoryField();
            if (definition.TargetFactory == null)
            {
                ImplementConstructorWithLazyTargetParameter();
            }
            else
            {
                ImplementParameterlessConstructor();    
            }
                                       
            DefineInterceptorFields(); 
            ImplementProxyInterface();
            PopulateTargetMethods();
            ImplementMethods();
            ImplementProperties();
            ImplementEvents();
            FinalizeInitializerMethod();
            FinalizeStaticConstructor();
            Type proxyType = typeBuilderFactory.CreateType(typeBuilder);
            var del = CreateTypedInstanceDelegate(definition.TargetFactory, definition.TargetType);
            proxyType.GetField("TargetFactory").SetValue(null, del);

            foreach (var interceptorInfo in definition.Interceptors)
            {
                var fieldName = "InterceptorFactory" + interceptorInfo.Index;
                var field = proxyType.GetField(fieldName);
                field.SetValue(null, interceptorInfo.InterceptorFactory);
            }
            
            return proxyType;
        }

        private static void PushInvocationInfoForNonGenericMethod(FieldInfo staticInterceptedMethodInfoField, ILGenerator il, ParameterInfo[] parameters, LocalBuilder argumentsArrayVariable)
        {            
            PushCurrentMethodForNonGenericMethod(staticInterceptedMethodInfoField, il);
            PushProceedDelegateForNonGenericMethod(staticInterceptedMethodInfoField, il);
            PushProxyInstance(il);
            PushArguments(il, parameters, argumentsArrayVariable);
            il.Emit(OpCodes.Newobj, InvocationInfoConstructor);
        }

        private static void PushInvocationInfoForGenericMethod(FieldInfo staticOpenGenericInterceptedMethodInfoField, ILGenerator il, ParameterInfo[] parameters, LocalBuilder argumentsArrayVariable, GenericTypeParameterBuilder[] genericParameters)
        {
            PushCurrentMethodForGenericMethod(staticOpenGenericInterceptedMethodInfoField, il, genericParameters);                        
            PushProxyInstance(il);
            PushArguments(il, parameters, argumentsArrayVariable);
            il.Emit(OpCodes.Newobj, InvocationInfoConstructor);
        }

        private static GenericTypeParameterBuilder[] CreateGenericTypeParameters(MethodInfo targetMethod, MethodBuilder methodBuilder)
        {
            if (!targetMethod.IsGenericMethodDefinition)
            {
                return null;
            }

            Type[] genericArguments = targetMethod.GetGenericArguments().ToArray();
            GenericTypeParameterBuilder[] genericTypeParameters = methodBuilder.DefineGenericParameters(genericArguments.Select(a => a.Name).ToArray());
            for (int i = 0; i < genericArguments.Length; i++)
            {
                genericTypeParameters[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);
                ApplyGenericConstraints(genericArguments[i], genericTypeParameters[i]);
            }

            return genericTypeParameters;
        }

        private static void ApplyGenericConstraints(Type genericArgument, GenericTypeParameterBuilder genericTypeParameter)
        {
            var genericConstraints = genericArgument.GetGenericParameterConstraints();
            genericTypeParameter.SetInterfaceConstraints(genericConstraints.Where(gc => gc.IsInterface).ToArray());
            genericTypeParameter.SetBaseTypeConstraint(genericConstraints.FirstOrDefault(t => t.IsClass));
        }

        private static void PushArguments(ILGenerator il, ParameterInfo[] parameters, LocalBuilder argumentsArrayVariable)
        {
            int parameterCount = parameters.Length;

            for (int i = 0; i < parameterCount; ++i)
            {
                Type parameterType = parameters[i].ParameterType;
                il.Emit(OpCodes.Ldloc, argumentsArrayVariable);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i + 1);
                if (parameters[i].IsOut || parameterType.IsByRef)
                {
                    parameterType = parameters[i].ParameterType.GetElementType();
                    if (parameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldobj, parameterType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldind_Ref);
                    }
                }

                if (parameterType.IsValueType || parameterType.IsGenericParameter)
                {
                    il.Emit(OpCodes.Box, parameterType);
                }

                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ldloc, argumentsArrayVariable);
        }

        private static void PushProxyInstance(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
        }

        private static void PushReturnValue(ILGenerator il, Type returnType)
        {
            if (returnType != typeof(void))
            {
                il.Emit(returnType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, returnType);
            }
            else
            {
                il.Emit(OpCodes.Pop);
            }

            il.Emit(OpCodes.Ret);
        }

        private static LocalBuilder DeclareArgumentArray(ILGenerator il, int size)
        {
            LocalBuilder argumentArray = il.DeclareLocal(typeof(object[]));
            il.Emit(OpCodes.Ldc_I4, size);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, argumentArray);
            return argumentArray;
        }

        private static void UpdateRefArguments(ParameterInfo[] parameters, ILGenerator il, LocalBuilder argumentsArrayField)
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i].IsOut || parameters[i].ParameterType.IsByRef)
                {
                    Type parameterType = parameters[i].ParameterType.GetElementType();
                    il.Emit(OpCodes.Ldarg, i + 1);
                    il.Emit(OpCodes.Ldloc, argumentsArrayField);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    il.Emit(parameterType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, parameterType);
                    il.Emit(OpCodes.Stobj, parameterType);
                }
            }
        }

        private static void PushCurrentMethodForGenericMethod(FieldInfo staticOpenGenericInterceptedMethodInfoField, ILGenerator il, GenericTypeParameterBuilder[] genericParameters)
        {
            var typeArrayField = il.DeclareLocal(typeof(Type[]));
            il.Emit(OpCodes.Ldc_I4, genericParameters.Length);
            il.Emit(OpCodes.Newarr, typeof(Type));
            il.Emit(OpCodes.Stloc, typeArrayField);
            var delegateInfoField = il.DeclareLocal(typeof(InterceptedMethodInfo));
            for (int i = 0; i < genericParameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, typeArrayField);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldtoken, genericParameters[i]);
                il.Emit(OpCodes.Call, GetTypeFromHandleMethod);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ldsfld, staticOpenGenericInterceptedMethodInfoField);
            il.Emit(OpCodes.Ldloc, typeArrayField);
            il.Emit(OpCodes.Call, GetInterceptedMethodInfoMethod);
            il.Emit(OpCodes.Stloc, delegateInfoField);

            il.Emit(OpCodes.Ldloc, delegateInfoField);
            il.Emit(OpCodes.Ldfld, InterceptedMethodInfoMethodField);
            il.Emit(OpCodes.Ldloc, delegateInfoField);
            il.Emit(OpCodes.Ldfld, InterceptedMethodInfoProceedDelegateField);            
        }

        private static void PushCurrentMethodForNonGenericMethod(FieldInfo staticInterceptedMethodInfoField, ILGenerator il)
        {
            il.Emit(OpCodes.Ldsfld, staticInterceptedMethodInfoField);
            il.Emit(OpCodes.Ldfld, InterceptedMethodInfoMethodField);
        }

        private static void PushProceedDelegateForNonGenericMethod(FieldInfo staticInterceptedMethodInfoField, ILGenerator il)
        {
            il.Emit(OpCodes.Ldsfld, staticInterceptedMethodInfoField);
            il.Emit(OpCodes.Ldfld, InterceptedMethodInfoProceedDelegateField);
        }

        private static void PushInterceptorInstance(FieldInfo lazyMethodInterceptorField, ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, lazyMethodInterceptorField);
            il.Emit(OpCodes.Callvirt, LazyInterceptorGetValueMethod);
        }

        private static void PushMethodInfo(MethodInfo targetMethod, ILGenerator il)
        {
            il.Emit(OpCodes.Ldtoken, targetMethod);
            il.Emit(OpCodes.Call, GetMethodFromHandleMethod);
            il.Emit(OpCodes.Castclass, typeof(MethodInfo));
        }

        private static void Return(ILGenerator il)
        {
            il.Emit(OpCodes.Ret);
        }

        private static void CallObjectConstructor(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);
            il.Emit(OpCodes.Ldarg_0);
        }

        private static Func<T> CreateGenericTargetFactory<T>(Func<object> del)
        {
            return () => (T)del();
        }

        private IEnumerable<PropertyInfo> GetTargetProperties()
        {
            return proxyDefinition.TargetType.GetProperties().ToArray();
        }

        private void ImplementProperties()
        {
            var targetProperties = GetTargetProperties();

            foreach (var property in targetProperties)
            {
                var propertyBuilder = GetPropertyBuilder(property);
                MethodInfo setMethod = property.GetSetMethod();
                if (setMethod != null)
                {
                    propertyBuilder.SetSetMethod(ImplementMethod(setMethod));
                }

                MethodInfo getMethod = property.GetGetMethod();

                if (getMethod != null)
                {
                    propertyBuilder.SetGetMethod(ImplementMethod(getMethod));
                }
            }
        }

        private void ImplementEvents()
        {
            var targetEvents = GetTargetEvents();

            foreach (var targetEvent in targetEvents)
            {
                var eventBuilder = GetEventBuilder(targetEvent);
                MethodInfo addMethod = targetEvent.GetAddMethod();
                eventBuilder.SetAddOnMethod(ImplementMethod(addMethod));
                MethodInfo removeMethod = targetEvent.GetRemoveMethod();
                eventBuilder.SetRemoveOnMethod(ImplementMethod(removeMethod));
            }
        }

        private IEnumerable<EventInfo> GetTargetEvents()
        {
            return proxyDefinition.TargetType.GetEvents().ToArray();
        }

        private PropertyBuilder GetPropertyBuilder(PropertyInfo property)
        {
            var propertyBuilder = typeBuilder.DefineProperty(
                  property.Name, property.Attributes, property.PropertyType, new[] { property.PropertyType });
            return propertyBuilder;
        }

        private EventBuilder GetEventBuilder(EventInfo eventInfo)
        {
            var eventBuilder = typeBuilder.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);
            return eventBuilder;
        }

        private void FinalizeStaticConstructor()
        {
            staticConstructorBuilder.GetILGenerator().Emit(OpCodes.Ret);
        }

        private Delegate CreateTypedInstanceDelegate(Func<object> targetFactory, Type targetType)
        {
            var openGenericMethod = GetType().GetMethod("CreateGenericTargetFactory", BindingFlags.NonPublic | BindingFlags.Static);
            var closedGenericMethod = openGenericMethod.MakeGenericMethod(targetType);            
            return (Delegate)closedGenericMethod.Invoke(this, new object[] { targetFactory });
        }
        
        private void ImplementMethods()
        {
            foreach (var targetMethod in targetMethods)
            {
                ImplementMethod(targetMethod);
            }
        }

        private MethodBuilder ImplementMethod(MethodInfo targetMethod)
        {
            int[] interceptorIndicies = proxyDefinition.Interceptors
                                                       .Where(i => i.MethodSelector(targetMethod)).Select(i => i.Index).ToArray();
            if (interceptorIndicies.Length > 0)
            {
                return ImplementInterceptedMethod(targetMethod, interceptorIndicies);
            }
            
            return ImplementPassThroughMethod(targetMethod);
        }

        private MethodBuilder ImplementInterceptedMethod(MethodInfo targetMethod, int[] interceptorIndicies)
        {
            MethodBuilder methodBuilder = GetMethodBuilder(targetMethod);
            ILGenerator il = methodBuilder.GetILGenerator();
            FieldInfo lazyMethodInterceptorField = DeclareLazyMethodInterceptorField(targetMethod);
            ImplementLazyMethodInterceptorInitialization(lazyMethodInterceptorField, interceptorIndicies);
            ParameterInfo[] parameters = targetMethod.GetParameters();
            LocalBuilder argumentsArrayVariable = DeclareArgumentArray(il, parameters.Length);
            if (!targetMethod.IsGenericMethod)
            {
                FieldInfo staticInterceptedMethodInfoField = DefineStaticInterceptedMethodInfoField(targetMethod);
                PushInterceptorInstance(lazyMethodInterceptorField, il);
                PushInvocationInfoForNonGenericMethod(staticInterceptedMethodInfoField, il, parameters, argumentsArrayVariable);                
                il.Emit(OpCodes.Callvirt, InterceptorInvokeMethod);
                UpdateRefArguments(parameters, il, argumentsArrayVariable);
                PushReturnValue(il, targetMethod.ReturnType);                
            }
            else
            {
                FieldInfo staticOpenGenericInterceptedMethodInfoField =
                    DefineStaticOpenGenericInterceptedMethodInfoField(targetMethod);
                PushInterceptorInstance(lazyMethodInterceptorField, il);
                var genericParameters = CreateGenericTypeParameters(targetMethod, methodBuilder);
                PushInvocationInfoForGenericMethod(staticOpenGenericInterceptedMethodInfoField, il, parameters, argumentsArrayVariable, genericParameters);
                il.Emit(OpCodes.Callvirt, InterceptorInvokeMethod);
                UpdateRefArguments(parameters, il, argumentsArrayVariable);
                PushReturnValue(il, targetMethod.ReturnType);              
            }

            return methodBuilder;
        }
       
        private FieldBuilder DefineStaticInterceptedMethodInfoField(MethodInfo targetMethod)
        {
            var fieldBuilder = typeBuilder.DefineField(
                GetUniqueMemberName(targetMethod.Name) + "InterceptedMethodInfo",
                typeof(InterceptedMethodInfo),
                FieldAttributes.InitOnly | FieldAttributes.Private | FieldAttributes.Static);
            var il = staticConstructorBuilder.GetILGenerator();
            PushMethodInfo(targetMethod, il);
            il.Emit(OpCodes.Newobj, InterceptedMethodInfoConstructor);
            il.Emit(OpCodes.Stsfld, fieldBuilder);
            return fieldBuilder;
        }

        private FieldBuilder DefineStaticOpenGenericInterceptedMethodInfoField(MethodInfo targetMethod)
        {
            var fieldBuilder = typeBuilder.DefineField(
                GetUniqueMemberName(targetMethod.Name) + "DelegateInfoCache",
                typeof(OpenGenericInterceptedMethodInfo),
                FieldAttributes.InitOnly | FieldAttributes.Private | FieldAttributes.Static);
            var il = staticConstructorBuilder.GetILGenerator();            
            PushMethodInfo(targetMethod, il);
            il.Emit(OpCodes.Newobj, OpenGenericInterceptedMethodInfoConstructor);
            il.Emit(OpCodes.Stsfld, fieldBuilder);
            return fieldBuilder;
        }

        private void ImplementLazyMethodInterceptorInitialization(FieldInfo interceptorField, int[] interceptorIndicies)
        {
            var il = initializerMethodBuilder.GetILGenerator();
            var interceptorArray = il.DeclareLocal(typeof(Lazy<IInterceptor>[]));
            for (int i = 0; i < interceptorIndicies.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, interceptorIndicies.Length);
                il.Emit(OpCodes.Newarr, typeof(Lazy<IInterceptor>));
                il.Emit(OpCodes.Stloc, interceptorArray);
                il.Emit(OpCodes.Ldloc, interceptorArray);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, lazyInterceptorFields[interceptorIndicies[i]]);
                il.Emit(OpCodes.Stelem_Ref);
                il.Emit(OpCodes.Ldloc, interceptorArray);
                il.Emit(OpCodes.Call, CreateMethodInterceptorMethod);
                il.Emit(OpCodes.Stfld, interceptorField);
            }
        }

        private FieldBuilder DeclareLazyMethodInterceptorField(MethodInfo targetMethod)
        {
            var methodName = targetMethod.Name.Substring(0, 1).ToLower() + targetMethod.Name.Substring(1);

            var memberName = GetUniqueMemberName(methodName + "Interceptor");
            return typeBuilder.DefinePrivateField(memberName, typeof(Lazy<IInterceptor>));
        }

        private string GetUniqueMemberName(string memberName)
        {
            int count;
            if (!memberNames.TryGetValue(memberName, out count))
            {
                memberNames.Add(memberName, 0);
                return memberName;
            }

            memberNames[memberName] = count + 1;
            return memberName + count;
        }

        private MethodBuilder ImplementPassThroughMethod(MethodInfo targetMethod)
        {
            MethodBuilder methodBuilder = GetMethodBuilder(targetMethod);
            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, lazyTargetField);
            var getTargetValueMethod = lazyTargetField.FieldType.GetProperty("Value").GetGetMethod();
            il.Emit(OpCodes.Call, getTargetValueMethod);
            for (int i = 1; i <= targetMethod.GetParameters().Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Callvirt, targetMethod);
            il.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private void FinalizeInitializerMethod()
        {
            initializerMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
        }

        private void InitializeTypeBuilder()
        {
            typeBuilder = typeBuilderFactory.CreateTypeBuilder(proxyDefinition.TargetType, proxyDefinition.AdditionalInterfaces);
            staticConstructorBuilder = typeBuilder.DefineTypeInitializer();
        }

        private void DefineLazyTargetField()
        {
            Type targetFieldType = typeof(Lazy<>).MakeGenericType(proxyDefinition.TargetType);
            lazyTargetField = typeBuilder.DefineField("target", targetFieldType, FieldAttributes.Private);            
        }

        private void DefineStaticTargetFactoryField()
        {
            Type funcType = typeof(Func<>).MakeGenericType(proxyDefinition.TargetType);
            targetFactoryField = typeBuilder.DefineField("TargetFactory", funcType, FieldAttributes.Public | FieldAttributes.Static);
        }

        private void DefineInitializerMethod()
        {            
            initializerMethodBuilder = typeBuilder.DefineMethod(
                "InitializeProxy", MethodAttributes.Private | MethodAttributes.HideBySig, typeof(void), new[] { lazyTargetField.FieldType });

            var il = initializerMethodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, lazyTargetField);
        }

        private void DefineInterceptorFields()
        {
            InterceptorInfo[] interceptors = proxyDefinition.Interceptors.ToArray();            
            lazyInterceptorFields = new FieldInfo[interceptors.Length];
            for (int index = 0; index < interceptors.Length; index++)
            {
                var interceptorField = typeBuilder.DefinePrivateField(
                     "interceptor" + index, typeof(Lazy<IInterceptor>));

                var interceptorFactoryField =
                   typeBuilder.DefinePublicStaticField("InterceptorFactory" + index, typeof(Func<IInterceptor>));
                ImplementLazyInterceptorInitialization(interceptorField, interceptorFactoryField);

                lazyInterceptorFields[index] = interceptorField;
            }            
        }

        private void ImplementParameterlessConstructor()
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);
            ILGenerator il = constructorBuilder.GetILGenerator();
            CallObjectConstructor(il);
            CallInitializeMethodWithStaticTargetFactory(il);
            Return(il);
        }

        private void CallInitializeMethodWithStaticTargetFactory(ILGenerator il)
        {
            var lazyConstructor = GetLazyConstructorForTargerType();
            il.Emit(OpCodes.Ldsfld, targetFactoryField);
            il.Emit(OpCodes.Newobj, lazyConstructor);
            il.Emit(OpCodes.Call, initializerMethodBuilder);           
        }

        private ConstructorInfo GetLazyConstructorForTargerType()
        {
            Type targetFieldType = typeof(Lazy<>).MakeGenericType(proxyDefinition.TargetType);
            var lazyConstructor = targetFieldType.GetConstructor(new[] { targetFactoryField.FieldType });
            return lazyConstructor;
        }

        private void ImplementConstructorWithLazyTargetParameter()
        {
            var lazyTargetType = typeof(Lazy<>).MakeGenericType(proxyDefinition.TargetType);
            const MethodAttributes Attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName
                                                | MethodAttributes.RTSpecialName;
            var constructorBuilder = typeBuilder.DefineConstructor(Attributes, CallingConventions.Standard, new[] { lazyTargetType });            
            ILGenerator il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, initializerMethodBuilder);
            il.Emit(OpCodes.Ret);
        }

        private void ImplementLazyInterceptorInitialization(FieldInfo interceptorField, FieldInfo interceptorFactoryField)
        {
            var il = initializerMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldsfld, interceptorFactoryField);
            il.Emit(OpCodes.Newobj, LazyInterceptorConstructor);
            il.Emit(OpCodes.Stfld, interceptorField);
        }

        private void ImplementProxyInterface()
        {
            typeBuilder.AddInterfaceImplementation(typeof(IProxy));
            ImplementGetTargetMethod();
        }

        private void ImplementGetTargetMethod()
        {
            MethodBuilder methodBuilder = GetMethodBuilder(GetTargetMethod);
            ILGenerator il = methodBuilder.GetILGenerator();
            var getTargetValueMethod = lazyTargetField.FieldType.GetProperty("Value").GetGetMethod();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, lazyTargetField);
            il.Emit(OpCodes.Call, getTargetValueMethod);
            il.Emit(OpCodes.Ret);
        }

        private MethodBuilder GetMethodBuilder(MethodInfo targetMethod)
        {
            MethodAttributes methodAttributes;

            string methodName = targetMethod.Name;

            Type declaringType = targetMethod.GetDeclaringType();

            if (declaringType.IsInterface)
            {
                methodAttributes = targetMethod.Attributes ^ MethodAttributes.Abstract;

                if (targetMethod.DeclaringType != proxyDefinition.TargetType)
                {
                    methodName = declaringType.FullName + "." + targetMethod.Name;
                }
            }
            else
            {
                methodAttributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual
                                   | MethodAttributes.HideBySig;
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                                            methodName,
                                            methodAttributes,
                                            targetMethod.ReturnType,
                                            targetMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            if (targetMethod.DeclaringType != proxyDefinition.TargetType && targetMethod.DeclaringType != typeof(object))
            {
                typeBuilder.DefineMethodOverride(methodBuilder, targetMethod);
            }

            return methodBuilder;
        }

        private void PopulateTargetMethods()
        {
            targetMethods = proxyDefinition.TargetType.GetMethods()
                                           .Where(m => m.IsVirtual && !m.IsSpecialName)
                                           .Concat(typeof(object).GetMethods().Where(m => m.IsVirtual))
                                           .Concat(proxyDefinition.AdditionalInterfaces.SelectMany(i => i.GetMethods().Where(m => m.IsVirtual && !m.IsSpecialName)))
                                           .Distinct()
                                           .ToArray();
        }
    }

    internal class TypeArrayComparer : IEqualityComparer<Type[]>
    {
        public bool Equals(Type[] x, Type[] y)
        {
            return ReferenceEquals(x, y) || (x != null && y != null && x.SequenceEqual(y));
        }

        public int GetHashCode(Type[] types)
        {
            return types.Aggregate(0, (current, type) => current ^ type.GetHashCode());
        }
    }
}