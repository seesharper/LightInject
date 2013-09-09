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
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Implemented by all proxy types.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// Gets the proxy target.
        /// </summary>
        object Target { get; }
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
    public interface IMethodSkeleton
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
    internal interface IProxyBuilder
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
    /// Represents a class that is capable of creating a <see cref="TypeBuilder"/> that 
    /// is used to build the proxy type.
    /// </summary>
    internal interface ITypeBuilderFactory
    {
        TypeBuilder CreateTypeBuilder(Type targetType, Type[] additionalInterfaces);

        Type CreateType(TypeBuilder typeBuilder);
    }
    
    /// <summary>
    /// Represents a class that intercepts method calls.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Invoked when a method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="InvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        object Invoke(InvocationInfo invocationInfo);
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

            return new Lazy<IInterceptor>(() => interceptors[0].Value);
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


    public class DynamicMethodBuilder : IMethodBuilder
    {
        private static readonly Dictionary<IntPtr, Func<object, object[], object>> DelegateCache 
            = new Dictionary<IntPtr, Func<object, object[], object>>();
                
        private static readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> Cache
            = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();

        private static readonly object SyncRoot = new object();

        private readonly Func<IMethodSkeleton> methodSkeletonFactory;
#if TEST
        public DynamicMethodBuilder(Func<IMethodSkeleton> methodSkeletonFactory)
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
            if (!DelegateCache.TryGetValue(method.MethodHandle.Value, out del))
            {
                lock (SyncRoot)
                {
                    if (!DelegateCache.TryGetValue(method.MethodHandle.Value, out del))
                    {
                        del = CreateDelegate(method);
                        DelegateCache.Add(method.MethodHandle.Value, del);
                    }
                }
            }

            return del(instance, arguments);            
        }

        public Func<object, object[], object> CreateDelegate(MethodInfo method)
        {
            var parameters = method.GetParameters();
            IMethodSkeleton methodSkeleton = methodSkeletonFactory();            
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
            il.Emit(OpCodes.Castclass, method.DeclaringType);
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

        private class DynamicMethodSkeleton : IMethodSkeleton
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




    public class InvocationInfo
    {         
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationInfo"/> class.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> currently being invoked.</param>
        /// <param name="proceed">The function delegate use to invoke the target method.</param>        
        /// <param name="proxy">The <see cref="IProxy"/> object from which methods are intercepted.</param>
        /// <param name="arguments">The arguments currently being passed to the target method.</param>        
        public InvocationInfo(MethodInfo method, Func<object, object[], object> proceed, IProxy proxy, object[] arguments)            
        {
            Method = method;
            Proxy = proxy;
            Arguments = arguments;
            Proceed = () => proceed(proxy.Target, arguments);
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> currently being invoked.
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the <see cref="IProxy"/> instance.
        /// </summary>
        public IProxy Proxy { get; private set; }

        /// <summary>
        /// Gets the arguments currently being passed to the target method.
        /// </summary>
        public object[] Arguments { get; private set; }

        /// <summary>
        /// Gets a function delegate used to invoke the target method.
        /// </summary>
        public Func<object> Proceed { get; private set; }
    }

    /// <summary>
    /// An <see cref="IInterceptor"/> that is responsible for 
    /// passing the <see cref="InvocationInfo"/> down the interceptor chain.
    /// </summary>
    internal class CompositeInterceptor : IInterceptor
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
        public object Invoke(InvocationInfo invocationInfo)
        {
            for (int i = interceptors.Length - 1; i >= 0; i--)
            {
                int index = i;
                InvocationInfo nextInvocationInfo = invocationInfo;
                Func<object, object[], object> nextProceedDelegate =
                    (instance, arguments) => interceptors[index].Invoke(nextInvocationInfo);
                invocationInfo = new InvocationInfo(invocationInfo.Method, nextProceedDelegate, invocationInfo.Proxy, invocationInfo.Arguments);
            }

            return interceptors[0].Invoke(invocationInfo);
        }
    }

    /// <summary>
    /// Contains information about a registered <see cref="IInterceptor"/>.
    /// </summary>
    internal class InterceptorInfo
    {
        /// <summary>
        /// Gets or sets the function delegate used to create the <see cref="IInterceptor"/> instance.
        /// </summary>
        public Func<IInterceptor> InterceptionFactory { get; set; }

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
    internal class ProxyDefinition
    {
        private readonly ICollection<InterceptorInfo> interceptors = new Collection<InterceptorInfo>();

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

        internal Type TargetType { get; private set; }

        internal Func<object> TargetFactory { get; private set; }

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
        /// Registers a new function delegate to create an <see cref="IInterceptor"/> instance that 
        /// is capable of intercepting the methods defined by the <paramref name="methodSelector"/>.
        /// </summary>
        /// <param name="interceptorFactory">A function delegate used to create the <see cref="IInterceptor"/> instance.</param>
        /// <param name="methodSelector">A function delegate used to select the methods to be intercepted.</param>
        public void Intercept(Func<IInterceptor> interceptorFactory, Func<MethodInfo, bool> methodSelector)
        {
            interceptors.Add(new InterceptorInfo
            {
                InterceptionFactory = interceptorFactory,
                MethodSelector = methodSelector,
                Index = interceptors.Count
            });
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
        private static readonly MethodInfo GetTargetMethod;
        private static readonly ConstructorInfo ObjectConstructor;
        private static readonly MethodInfo CreateMethodInterceptorMethod;
        private readonly Dictionary<string, int> memberNames = new Dictionary<string, int>();
        private readonly ITypeBuilderFactory typeBuilderFactory;
        
        private FieldBuilder targetFactoryField;
        private FieldBuilder lazyTargetField;
        private FieldInfo[] lazyInterceptorFields;
        private MethodInfo[] targetMethods;

        private TypeBuilder typeBuilder;

        private MethodBuilder initializerMethodBuilder;
       
        private ProxyDefinition proxyDefinition;

        static ProxyBuilder()
        {
            GetTargetMethod = typeof(IProxy).GetMethod("get_Target");
            LazyInterceptorConstructor = typeof(Lazy<IInterceptor>).GetConstructor(new[] { typeof(Func<IInterceptor>) });
            ObjectConstructor = typeof(object).GetConstructor(Type.EmptyTypes);
            CreateMethodInterceptorMethod = typeof(MethodInterceptorFactory).GetMethod("CreateMethodInterceptor");
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
            DefineInitializerMethod();
            ImplementParameterlessConstructor();
            DefineStaticTargetFactoryField();
            DefineLazyTargetField();                        
            DefineInterceptorFields(); 
            ImplementProxyInterface();
            PopulateTargetMethods();
            ImplementMethods();
            FinalizeInitializerMethod();
            Type proxyType = typeBuilderFactory.CreateType(typeBuilder);
            var del = CreateTypedInstanceDelegate(definition.TargetFactory, definition.TargetType);
            proxyType.GetField("TargetFactory").SetValue(null, del);            
            return proxyType;
        }

        private static Func<T> CreateGenericTargetFactory<T>(Func<object> del)
        {
            return () => (T)del();
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

        private void ImplementMethod(MethodInfo targetMethod)
        {
            int[] interceptorIndicies = proxyDefinition.Interceptors
                                                       .Where(i => i.MethodSelector(targetMethod)).Select(i => i.Index).ToArray();
            if (interceptorIndicies.Length > 0)
            {
                ImplementInterceptedMethod(targetMethod, interceptorIndicies);
            }
            else
            {
                ImplementPassThroughMethod(targetMethod);
            }
        }

        private void ImplementInterceptedMethod(MethodInfo targetMethod, int[] interceptorIndicies)
        {
            FieldInfo interceptorField = DeclareMethodInterceptorField(targetMethod);
            ImplementLazyMethodInterceptorInitialization(interceptorField, interceptorIndicies);

            //MethodBuilder methodBuilder = GetMethodBuilder(targetMethod);
            //ILGenerator il = methodBuilder.GetILGenerator();

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


        private FieldBuilder DeclareMethodInterceptorField(MethodInfo targetMethod)
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

        private void ImplementPassThroughMethod(MethodInfo targetMethod)
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
        }

        private void FinalizeInitializerMethod()
        {
            initializerMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
        }

        private void InitializeTypeBuilder()
        {
            typeBuilder = typeBuilderFactory.CreateTypeBuilder(proxyDefinition.TargetType, proxyDefinition.AdditionalInterfaces);            
        }

        private void DefineLazyTargetField()
        {
            Type targetFieldType = typeof(Lazy<>).MakeGenericType(proxyDefinition.TargetType);
            lazyTargetField = typeBuilder.DefineField("target", targetFieldType, FieldAttributes.Private);
            var lazyConstructor = targetFieldType.GetConstructor(new[] { targetFactoryField.FieldType });
            var il = initializerMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldsfld, targetFactoryField);
            il.Emit(OpCodes.Newobj, lazyConstructor);
            il.Emit(OpCodes.Stfld, lazyTargetField);
        }

        private void DefineStaticTargetFactoryField()
        {
            Type funcType = typeof(Func<>).MakeGenericType(proxyDefinition.TargetType);
            targetFactoryField = typeBuilder.DefineField("TargetFactory", funcType, FieldAttributes.Public | FieldAttributes.Static);
        }

        private void DefineInitializerMethod()
        {
            initializerMethodBuilder = typeBuilder.DefineMethod(
                "InitializeProxy", MethodAttributes.Private | MethodAttributes.HideBySig);
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
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);
            il.Emit(OpCodes.Ldarg_0);
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
            
            if (targetMethod.DeclaringType.IsInterface)
            {
                methodAttributes = targetMethod.Attributes ^ MethodAttributes.Abstract;

                if (targetMethod.DeclaringType != proxyDefinition.TargetType)
                {
                    methodName = targetMethod.DeclaringType.FullName + "." + targetMethod.Name;
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
}