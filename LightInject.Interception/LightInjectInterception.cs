[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Interception
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Represents a class that is capable of creating a proxy <see cref="Type"/>.
    /// </summary>
    internal interface IProxyBuilder
    {
        /// <summary>
        /// Gets a proxy type based on the given <paramref name="proxyDefinition"/>.
        /// </summary>
        /// <param name="proxyDefinition">A <see cref="ProxyDefinition"/> instance that contains information about the 
        /// proxy type to be created.</param>
        /// <returns>A proxy <see cref="Type"/>.</returns>
        Type GetProxyType(ProxyDefinition proxyDefinition);
    }

    /// <summary>
    /// Represents a class that is capable of creating a <see cref="TypeBuilder"/> that 
    /// is used to build the proxy type.
    /// </summary>
    internal interface ITypeBuilderFactory
    {
        TypeBuilder CreateTypeBuilder(Type baseType, Type[] additionalInterfaces);
    }

    /// <summary>
    /// Implemented by all proxy types.
    /// </summary>
    internal interface IProxy
    {
        /// <summary>
        /// Gets the proxy target.
        /// </summary>
        object Target { get; }
    }

    /// <summary>
    /// Represents a class that intercepts method calls.
    /// </summary>
    internal interface IInterceptor
    {
        /// <summary>
        /// Invoked when a method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="InvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        object Invoke(InvocationInfo invocationInfo);
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

    internal class InvocationInfo
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
        /// <param name="baseType">The base type of the proxy type.</param>
        /// <param name="targetFactory">A function delegate used to create the target instance.</param>
        /// <param name="additionalInterfaces">A list of additional interfaces to be implemented by the proxy type.</param>
        public ProxyDefinition(Type baseType, Func<object> targetFactory, params Type[] additionalInterfaces)
        {
            BaseType = baseType;
            TargetFactory = targetFactory;
            AdditionalInterfaces = additionalInterfaces;
        }

        internal Type BaseType { get; private set; }

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
        public TypeBuilder CreateTypeBuilder(Type baseType, Type[] additionalInterfaces)
        {
            ModuleBuilder moduleBuilder = GetModuleBuilder();
            const TypeAttributes TypeAttributes = TypeAttributes.Public | TypeAttributes.Class;
            var typeName = baseType.Name + "Proxy";
            Type[] interfaceTypes = new[] { baseType }.Concat(additionalInterfaces).ToArray();
            return moduleBuilder.DefineType(typeName, TypeAttributes, null, interfaceTypes);    
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

        private readonly ITypeBuilderFactory typeBuilderFactory;
        
        private FieldBuilder targetFactoryField;
        private FieldBuilder lazyTargetField;
        private FieldInfo[] lazyInterceptorFields;

        private TypeBuilder typeBuilder;

        private MethodBuilder initializerMethodBuilder;

        static ProxyBuilder()
        {
            LazyInterceptorConstructor = typeof(Lazy<IInterceptor>).GetConstructor(new[] { typeof(Func<IInterceptor>) });
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
        public Type GetProxyType(ProxyDefinition proxyDefinition)
        {
            InitializeTypeBuilder(proxyDefinition);
            DefineLazyTargetField(proxyDefinition.BaseType);
            DefineStaticTargetFactoryField();
            DefineInitializerMethod();
            DefineInterceptorFields(proxyDefinition);

            FinalizeInitializerMethod();
            return typeBuilder.CreateType();
        }

        private void FinalizeInitializerMethod()
        {
            initializerMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
        }

        private void InitializeTypeBuilder(ProxyDefinition proxyDefinition)
        {
            typeBuilder = typeBuilderFactory.CreateTypeBuilder(proxyDefinition.BaseType, proxyDefinition.AdditionalInterfaces);
        }

        private void DefineLazyTargetField(Type baseType)
        {
            Type targetFieldType = typeof(Lazy<>).MakeGenericType(baseType);
            lazyTargetField = typeBuilder.DefineField("target", targetFieldType, FieldAttributes.Private);
        }

        private void DefineStaticTargetFactoryField()
        {            
            targetFactoryField = typeBuilder.DefineField("TargetFactory", typeof(Func<object>), FieldAttributes.Public | FieldAttributes.Static);
        }

        private void DefineInitializerMethod()
        {
            initializerMethodBuilder = typeBuilder.DefineMethod(
                "InitializeProxy", MethodAttributes.Private | MethodAttributes.HideBySig);
        }

        private void DefineInterceptorFields(ProxyDefinition proxyDefinition)
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

        private void ImplementLazyInterceptorInitialization(FieldInfo interceptorField, FieldInfo interceptorFactoryField)
        {
            var il = initializerMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldsfld, interceptorFactoryField);
            il.Emit(OpCodes.Newobj, LazyInterceptorConstructor);
            il.Emit(OpCodes.Stfld, interceptorField);
        }
    }
}