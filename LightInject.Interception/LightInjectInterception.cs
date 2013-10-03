/*****************************************************************************   
   Copyright 2013 bernhard.richter@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
******************************************************************************
   LightInject.Interception version 1.0.0.1 
   http://www.lightinject.net/
   http://twitter.com/bernhardrichter
******************************************************************************/
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
    using System.Diagnostics.CodeAnalysis;
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
    /// Represents a class that contains detailed information about the method being invoked.
    /// </summary>
    public interface IInvocationInfo
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> currently being invoked.
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// Gets the <see cref="IProxy"/> instance that intercepted the method call.
        /// </summary>
        IProxy Proxy { get; }

        /// <summary>
        /// Gets the arguments currently being passed to the target method.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Proceeds to the next <see cref="IInterceptor"/>, or of at the end of the interceptor chain, 
        /// proceeds to the actual target.
        /// </summary>
        /// <returns>The return value from the method call.</returns>
        object Proceed();
    }

    /// <summary>
    /// Represents a class that is capable of creating a delegate used to invoke 
    /// a method without using late-bound invocation.
    /// </summary>
    public interface IMethodBuilder
    {
        /// <summary>
        /// Gets a delegate that is used to invoke the <paramref name="targetMethod"/>.
        /// </summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> that represents the target method to invoke.</param>
        /// <returns>A delegate that represents compiled code used to invoke the <paramref name="targetMethod"/>.</returns>
        Func<object, object[], object> GetDelegate(MethodInfo targetMethod);
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
        /// <summary>
        /// Creates a <see cref="TypeBuilder"/> instance that is used to build a proxy 
        /// type that inherits/implements the <paramref name="targetType"/> with an optional 
        /// set of <paramref name="additionalInterfaces"/>.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/> that the <see cref="TypeBuilder"/> will inherit/implement.</param>
        /// <param name="additionalInterfaces">A set of additional interfaces to be implemented.</param>
        /// <returns>A <see cref="TypeBuilder"/> instance for which to build the proxy type.</returns>
        TypeBuilder CreateTypeBuilder(Type targetType, Type[] additionalInterfaces);

        /// <summary>
        /// Creates a proxy <see cref="Type"/> based on the given <paramref name="typeBuilder"/>.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> that represents the proxy type.</param>
        /// <returns>The proxy <see cref="Type"/>.</returns>
        Type CreateType(TypeBuilder typeBuilder);
    }
   
    /// <summary>
    /// A factory class used to create a <see cref="CompositeInterceptor"/> if the target method has 
    /// multiple interceptors.
    /// </summary>
    public static class MethodInterceptorFactory
    {
        /// <summary>
        /// Creates a new <see cref="Lazy{T}"/> that represents getting the <see cref="IInterceptor"/> for a given method.
        /// </summary>
        /// <param name="interceptors">A list of interceptors that represents the interceptor chain for a given method.</param>
        /// <returns>A new <see cref="Lazy{T}"/> that represents getting the <see cref="IInterceptor"/> for a given method.</returns>
        public static Lazy<IInterceptor> CreateMethodInterceptor(Lazy<IInterceptor>[] interceptors)
        {
            if (interceptors.Length > 1)
            {
                return
                    new Lazy<IInterceptor>(() => new CompositeInterceptor(interceptors));
            }

            return interceptors[0];            
        }
    }

    /// <summary>
    /// Contains information about the target method being intercepted.
    /// </summary>
    public class TargetMethodInfo
    {                
        /// <summary>
        /// The function delegate used to invoke the target method.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Loading a field is faster than going through a property.")]        
        public Lazy<Func<object, object[], object>> ProceedDelegate;

        /// <summary>
        /// The <see cref="MethodInfo"/> that represents the target method.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Loading a field is faster than going through a property.")]
        public MethodInfo Method;

        private static readonly IMethodBuilder ProceedDelegateBuilder = new CachedMethodBuilder(new DynamicMethodBuilder());

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetMethodInfo"/> class.
        /// </summary>
        /// <param name="method">The target <see cref="MethodInfo"/> being intercepted.</param>
        public TargetMethodInfo(MethodInfo method)
        {
            ProceedDelegate = new Lazy<Func<object, object[], object>>(() => ProceedDelegateBuilder.GetDelegate(method));
            Method = method;
        }
    }

    /// <summary>
    /// Contains information about the open generic target method being intercepted.
    /// </summary>
    public class OpenGenericTargetMethodInfo
    {
        private readonly MethodInfo openGenericMethod;

        private readonly Dictionary<Type[], TargetMethodInfo> cache =
            new Dictionary<Type[], TargetMethodInfo>(new TypeArrayComparer());

        private readonly object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGenericTargetMethodInfo"/> class.
        /// </summary>
        /// <param name="openGenericMethod">The open generic target <see cref="MethodInfo"/>.</param>
        public OpenGenericTargetMethodInfo(MethodInfo openGenericMethod)
        {
            this.openGenericMethod = openGenericMethod;
        }

        /// <summary>
        /// Gets the <see cref="TargetMethodInfo"/> that represents the closed generic <see cref="MethodInfo"/>.
        /// based on the given <paramref name="typeArguments"/>.
        /// </summary>
        /// <param name="typeArguments">A list of types used to create a closed generic target <see cref="MethodInfo"/>.</param>
        /// <returns>The <see cref="TargetMethodInfo"/> that represents the closed generic <see cref="MethodInfo"/>.</returns>
        public TargetMethodInfo GetTargetMethodInfo(Type[] typeArguments)
        {
            TargetMethodInfo targetMethodInfo;
            if (!cache.TryGetValue(typeArguments, out targetMethodInfo))
            {
                lock (lockObject)
                {
                    if (!cache.TryGetValue(typeArguments, out targetMethodInfo))
                    {
                        targetMethodInfo = CreateTargetMethodInfo(typeArguments);
                        cache.Add(typeArguments, targetMethodInfo);
                    }
                }
            }

            return targetMethodInfo;
        }

        private TargetMethodInfo CreateTargetMethodInfo(Type[] types)
        {
            var closedGenericMethod = openGenericMethod.MakeGenericMethod(types);
            return new TargetMethodInfo(closedGenericMethod);
        }
    }

    /// <summary>
    /// A class that is capable of creating a delegate used to invoke 
    /// a method without using late-bound invocation.
    /// </summary>
    public class DynamicMethodBuilder : IMethodBuilder
    {             
        private readonly Func<IDynamicMethodSkeleton> methodSkeletonFactory;
#if TEST
        public DynamicMethodBuilder(Func<IDynamicMethodSkeleton> methodSkeletonFactory)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMethodBuilder"/> class.
        /// </summary>
        public DynamicMethodBuilder()
        {
            methodSkeletonFactory = () => new DynamicMethodSkeleton();
        }

        /// <summary>
        /// Gets a delegate that is used to invoke the <paramref name="targetMethod"/>.
        /// </summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> that represents the target method to invoke.</param>
        /// <returns>A delegate that represents compiled code used to invoke the <paramref name="targetMethod"/>.</returns>
        public Func<object, object[], object> GetDelegate(MethodInfo targetMethod)
        {
            var parameters = targetMethod.GetParameters();
            IDynamicMethodSkeleton methodSkeleton = methodSkeletonFactory();            
            var il = methodSkeleton.GetILGenerator();            
            PushInstance(targetMethod, il);
            PushArguments(parameters, il);
            CallTargetMethod(targetMethod, il);
            UpdateOutAndRefArguments(parameters, il);
            PushReturnValue(targetMethod, il);
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

    /// <summary>
    /// An <see cref="IMethodBuilder"/> cache decorator that ensures that 
    /// for a given <see cref="MethodInfo"/>, only a single dynamic method is created.
    /// </summary>
    public class CachedMethodBuilder : IMethodBuilder
    {
        private readonly IMethodBuilder methodBuilder;

        private readonly ThreadSafeDictionary<MethodInfo, Func<object, object[], object>> cache
            = new ThreadSafeDictionary<MethodInfo, Func<object, object[], object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMethodBuilder"/> class.
        /// </summary>
        /// <param name="methodBuilder">The target <see cref="IMethodBuilder"/> instance.</param>
        public CachedMethodBuilder(IMethodBuilder methodBuilder)
        {
            this.methodBuilder = methodBuilder;
        }

        /// <summary>
        /// Gets a delegate that is used to invoke the <paramref name="targetMethod"/>.
        /// </summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> that represents the target method to invoke.</param>
        /// <returns>A delegate that represents compiled code used to invoke the <paramref name="targetMethod"/>.</returns>
        public Func<object, object[], object> GetDelegate(MethodInfo targetMethod)
        {
            return cache.GetOrAdd(targetMethod, methodBuilder.GetDelegate);
        }
    }

    /// <summary>
    /// An implementation of the <see cref="IInvocationInfo"/> interface that forwards 
    /// method calls the actual target.
    /// </summary>    
    public class TargetInvocationInfo : IInvocationInfo
    {
        private readonly TargetMethodInfo targetMethodInfo;
        private readonly IProxy proxy;
        private readonly object[] arguments;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInvocationInfo"/> class.
        /// </summary>
        /// <param name="targetMethodInfo">The <see cref="TargetMethodInfo"/> that contains 
        /// information about the target method.</param>
        /// <param name="proxy">The <see cref="IProxy"/> instance that intercepted the method call.</param>
        /// <param name="arguments">The arguments currently being passed to the target method.</param>
        public TargetInvocationInfo(TargetMethodInfo targetMethodInfo, IProxy proxy, object[] arguments)
        {
            this.targetMethodInfo = targetMethodInfo;
            this.proxy = proxy;
            this.arguments = arguments;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> currently being invoked.
        /// </summary>
        public MethodInfo Method
        {
            get
            {
                return targetMethodInfo.Method;
            }
        }

        /// <summary>
        /// Gets the <see cref="IProxy"/> instance that intercepted the method call.
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

        /// <summary>
        /// Proceeds to the actual <see cref="IProxy.Target"/>.
        /// </summary>
        /// <returns>The return value from the method call.</returns>
        public object Proceed()
        {
            return targetMethodInfo.ProceedDelegate.Value(proxy.Target, arguments);
        }
    }

    /// <summary>
    /// An implementation of the <see cref="IInvocationInfo"/> interface that forwards 
    /// method calls to the next <see cref="IInterceptor"/> in the interceptor chain.
    /// </summary>    
    public class InterceptorInvocationInfo : IInvocationInfo
    {
        private readonly IInvocationInfo nextInvocationInfo;
        private readonly Lazy<IInterceptor> nextInterceptor;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptorInvocationInfo"/> class.
        /// </summary>
        /// <param name="nextInvocationInfo">The next <see cref="IInvocationInfo"/> used to invoke the <paramref name="nextInterceptor"/>.</param>
        /// <param name="nextInterceptor">The next <see cref="IInterceptor"/> in the interceptor chain.</param>
        public InterceptorInvocationInfo(IInvocationInfo nextInvocationInfo, Lazy<IInterceptor> nextInterceptor)
        {
            this.nextInvocationInfo = nextInvocationInfo;
            this.nextInterceptor = nextInterceptor;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> currently being invoked.
        /// </summary>
        public MethodInfo Method
        {
            get
            {
                return nextInvocationInfo.Method;
            }        
        }

        /// <summary>
        /// Gets the <see cref="IProxy"/> instance that intercepted the method call.
        /// </summary>
        public IProxy Proxy
        {
            get
            {
                return nextInvocationInfo.Proxy;
            }            
        }

        /// <summary>
        /// Gets the arguments currently being passed to the target method.
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return nextInvocationInfo.Arguments;
            }            
        }

        /// <summary>
        /// Proceeds to the next <see cref="IInterceptor"/> in the interceptor chain.
        /// </summary>
        /// <returns>The return value from the method call.</returns>
        public object Proceed()
        {
            return nextInterceptor.Value.Invoke(nextInvocationInfo);
        }
    }

    /// <summary>
    /// A composite <see cref="IInterceptor"/> that is responsible for 
    /// passing the <see cref="IInvocationInfo"/> down the interceptor chain.
    /// </summary>
    public class CompositeInterceptor : IInterceptor
    {
        private readonly Lazy<IInterceptor>[] interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeInterceptor"/> class.
        /// </summary>
        /// <param name="interceptors">The <see cref="IInterceptor"/> chain to be invoked.</param>
        public CompositeInterceptor(Lazy<IInterceptor>[] interceptors)
        {                                 
            this.interceptors = interceptors;
        }

        /// <summary>
        /// Invoked when a method call is intercepted.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> instance that 
        /// contains information about the current method call.</param>
        /// <returns>The return value from the method.</returns>
        public object Invoke(IInvocationInfo invocationInfo)
        {
            for (int i = interceptors.Length - 1; i >= 1; i--)
            {                                                
                IInvocationInfo nextInvocationInfo = invocationInfo;                
                invocationInfo = new InterceptorInvocationInfo(nextInvocationInfo, interceptors[i]);                               
            }

            return interceptors[0].Value.Invoke(invocationInfo);
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
        /// <param name="interceptorFactory">A function delegate used to create the <see cref="IInterceptor"/> instance.</param>
        /// <param name="methodSelector">A function delegate used to select the methods to be implemented</param>
        /// <returns>This instance.</returns>
        public ProxyDefinition Implement(Func<IInterceptor> interceptorFactory, Func<MethodInfo, bool> methodSelector)
        {
            interceptors.Add(new InterceptorInfo
            {
                InterceptorFactory = interceptorFactory,
                MethodSelector = methodSelector,
                Index = interceptors.Count
            });

            return this;
        }

        /// <summary>
        /// Implements all methods by forwarding method calls
        /// to the <see cref="IInterceptor"/> created by the <paramref name="interceptorFactory"/>.
        /// </summary>        
        /// <param name="interceptorFactory">A function delegate used to create the <see cref="IInterceptor"/> instance.</param>       
        /// <returns>This instance.</returns>
        public ProxyDefinition Implement(Func<IInterceptor> interceptorFactory)
        {
            Implement(interceptorFactory, method => !method.IsDeclaredBy(typeof(object)));

            return this;
        }
    }

    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> interface by adding methods for 
    /// creating proxy-based decorators.
    /// </summary>
    internal static class InterceptionContainerExtensions
    {        
        /// <summary>
        /// Decorates the service identified by the <paramref name="serviceSelector"/> delegate with a dynamic proxy type
        /// that is used to decorate the target type.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceSelector">A function delegate that is used to determine if the proxy-based decorator should be applied to the target service.</param>
        /// <param name="additionalInterfaces">A list of additional interface that will be implemented by the proxy type.</param>
        /// <param name="defineProxyType">An action delegate that is used to define the proxy type.</param>
        public static void Intercept(this IServiceRegistry serviceRegistry, Func<ServiceRegistration, bool> serviceSelector, Type[] additionalInterfaces, Action<IServiceFactory, ProxyDefinition> defineProxyType)
        {
            var decoratorRegistration = new DecoratorRegistration();            
            decoratorRegistration.CanDecorate =
                registration => serviceSelector(registration) && registration.ServiceType != typeof(IInterceptor);
            decoratorRegistration.ImplementingTypeFactory = (serviceFactory, serviceRegistration) => CreateProxyType(serviceRegistration, additionalInterfaces, serviceFactory, defineProxyType);
            serviceRegistry.Decorate(decoratorRegistration);
        }

        /// <summary>
        /// Decorates the service identified by the <paramref name="serviceSelector"/> delegate with a dynamic proxy type
        /// that is used to decorate the target type.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceSelector">A function delegate that is used to determine if the proxy-based decorator should be applied to the target service.</param>        
        /// <param name="defineProxyType">An action delegate that is used to define the proxy type.</param>
        public static void Intercept(this IServiceRegistry serviceRegistry, Func<ServiceRegistration, bool> serviceSelector, Action<IServiceFactory, ProxyDefinition> defineProxyType)
        {
            Intercept(serviceRegistry, serviceSelector, Type.EmptyTypes, defineProxyType);
        }

        /// <summary>
        /// Decorates the service identified by the <paramref name="serviceSelector"/> delegate with a dynamic proxy type
        /// that is used to decorate the target type.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/> instance.</param>
        /// <param name="serviceSelector">A function delegate that is used to determine if the proxy-based decorator should be applied to the target service.</param>        
        /// <param name="getInterceptor">A function delegate that is used to create the <see cref="IInterceptor"/> instance.</param>        
        public static void Intercept(this IServiceRegistry serviceRegistry, Func<ServiceRegistration, bool> serviceSelector, Func<IServiceFactory, IInterceptor> getInterceptor)
        {
            Intercept(serviceRegistry, serviceSelector, Type.EmptyTypes, (factory, definition) => definition.Implement(() => getInterceptor(factory)));
        }
        
        private static Type CreateProxyType(
            ServiceRegistration registration, Type[] additionalInterfaces, IServiceFactory serviceFactory,  Action<IServiceFactory, ProxyDefinition> defineProxyType)
        {           
            var proxyBuilder = new ProxyBuilder();
            var proxyDefinition = new ProxyDefinition(registration.ServiceType, additionalInterfaces);
            defineProxyType(serviceFactory, proxyDefinition);
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

        /// <summary>
        /// Gets a value that indicates whether the <paramref name="method"/> is declared by the given <paramref name="type"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> for which to check the declaring type.</param>
        /// <param name="type">The <see cref="Type"/> to check against the declaring type of the <paramref name="method"/>.</param>
        /// <returns>true if the <paramref name="method"/> is declared by the given <paramref name="type"/>, otherwise, false.</returns>
        public static bool IsDeclaredBy(this MethodInfo method, Type type)
        {
            return method.DeclaringType == type;
        }

        /// <summary>
        /// Gets a value that indicates whether the <paramref name="method"/> is declared <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to check against the declaring type of the <paramref name="method"/>.</typeparam>
        /// <param name="method">The <see cref="MethodInfo"/> for which to check the declaring type.</param>
        /// <returns>true if the <paramref name="method"/> is declared by <typeparamref name="T"/>, otherwise, false.</returns>
        public static bool IsDeclaredBy<T>(this MethodInfo method)
        {
            return IsDeclaredBy(method, typeof(T));
        }

        /// <summary>
        /// Gets a value that indicates whether the <paramref name="method"/> represent a property setter.
        /// </summary>
        /// <param name="method">The target <see cref="MethodInfo"/>.</param>
        /// <returns>true if the <paramref name="method"/> represent a property setter, otherwise , false.</returns>
        public static bool IsPropertySetter(this MethodInfo method)
        {
            return method.GetDeclaringType().GetProperties().Any(prop => prop.GetSetMethod() == method);
        }

        /// <summary>
        /// Gets a value that indicates whether the <paramref name="method"/> represent a property getter.
        /// </summary>
        /// <param name="method">The target <see cref="MethodInfo"/>.</param>
        /// <returns>true if the <paramref name="method"/> represent a property getter, otherwise , false.</returns>
        public static bool IsPropertyGetter(this MethodInfo method)
        {
            return method.GetDeclaringType().GetProperties().Any(prop => prop.GetGetMethod() == method);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for which the given <paramref name="method"/> represents a property accessor.
        /// </summary>
        /// <param name="method">The target <see cref="MethodInfo"/>.</param>
        /// <returns>The <see cref="PropertyInfo"/> for which the given <paramref name="method"/> represents a property accessor.</returns>
        public static PropertyInfo GetProperty(this MethodInfo method)
        {
            return method.GetDeclaringType().GetProperties().FirstOrDefault(p => p.GetGetMethod() == method || p.GetSetMethod() == method);
        }        
    }

    /// <summary>
    /// A class that is capable of creating a <see cref="TypeBuilder"/> that 
    /// is used to build the proxy type.
    /// </summary>
    internal class TypeBuilderFactory : ITypeBuilderFactory
    {
        /// <summary>
        /// Creates a <see cref="TypeBuilder"/> instance that is used to build a proxy 
        /// type that inherits/implements the <paramref name="targetType"/> with an optional 
        /// set of <paramref name="additionalInterfaces"/>.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/> that the <see cref="TypeBuilder"/> will inherit/implement.</param>
        /// <param name="additionalInterfaces">A set of additional interfaces to be implemented.</param>
        /// <returns>A <see cref="TypeBuilder"/> instance for which to build the proxy type.</returns>
        public TypeBuilder CreateTypeBuilder(Type targetType, Type[] additionalInterfaces)
        {
            ModuleBuilder moduleBuilder = GetModuleBuilder();
            const TypeAttributes TypeAttributes = TypeAttributes.Public | TypeAttributes.Class;
            var typeName = targetType.Name + "Proxy";
            Type[] interfaceTypes = new[] { targetType }.Concat(additionalInterfaces).ToArray();
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes, null, interfaceTypes);            
            return typeBuilder;
        }

        /// <summary>
        /// Creates a proxy <see cref="Type"/> based on the given <paramref name="typeBuilder"/>.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> that represents the proxy type.</param>
        /// <returns>The proxy <see cref="Type"/>.</returns>
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

    /// <summary>
    /// A class that is capable of creating a proxy <see cref="Type"/>.
    /// </summary>
    internal class ProxyBuilder : IProxyBuilder
    {        
        private static readonly ConstructorInfo LazyInterceptorConstructor;        
        private static readonly ConstructorInfo TargetInvocationInfoConstructor;
        private static readonly ConstructorInfo TargetMethodInfoConstructor;
        private static readonly ConstructorInfo OpenGenericTargetMethodInfoConstructor;
        private static readonly ConstructorInfo ObjectConstructor;
        private static readonly MethodInfo GetTargetMethod;        
        private static readonly MethodInfo CreateMethodInterceptorMethod;
        private static readonly MethodInfo GetMethodFromHandleMethod;
        private static readonly MethodInfo GetTypeFromHandleMethod;        
        private static readonly MethodInfo LazyInterceptorGetValueMethod;        
        private static readonly MethodInfo InterceptorInvokeMethod;
        private static readonly MethodInfo GetTargetMethodInfoMethod;                
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
            GetMethodFromHandleMethod = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });
            GetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            TargetMethodInfoConstructor = typeof(TargetMethodInfo).GetConstructors()[0];
            OpenGenericTargetMethodInfoConstructor = typeof(OpenGenericTargetMethodInfo).GetConstructors()[0];
            LazyInterceptorGetValueMethod = typeof(Lazy<IInterceptor>).GetProperty("Value").GetGetMethod();                        
            TargetInvocationInfoConstructor = typeof(TargetInvocationInfo).GetConstructors()[0];
            InterceptorInvokeMethod = typeof(IInterceptor).GetMethod("Invoke");
            GetTargetMethodInfoMethod = typeof(OpenGenericTargetMethodInfo).GetMethod("GetTargetMethodInfo");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBuilder"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets a proxy type based on the given <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">A <see cref="ProxyDefinition"/> instance that contains information about the 
        /// proxy type to be created.</param>
        /// <returns>A proxy <see cref="Type"/>.</returns>
        public Type GetProxyType(ProxyDefinition definition)
        {
            proxyDefinition = definition;
            InitializeTypeBuilder();
            DefineLazyTargetField();                        
            DefineInitializerMethod();
            DefineStaticTargetFactoryField();
            ImplementConstructor();                                       
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
            AssignTargetFactory(proxyType, del);
            AssignInterceptorFactories(definition, proxyType);
            
            return proxyType;
        }

        private static void AssignInterceptorFactories(ProxyDefinition definition, Type proxyType)
        {
            foreach (var interceptorInfo in definition.Interceptors)
            {
                var fieldName = "InterceptorFactory" + interceptorInfo.Index;
                var field = proxyType.GetField(fieldName);
                field.SetValue(null, interceptorInfo.InterceptorFactory);
            }
        }

        private static void AssignTargetFactory(Type proxyType, Delegate del)
        {            
            proxyType.GetField("TargetFactory").SetValue(null, del);
        }

        private static void PushInvocationInfoForNonGenericMethod(FieldInfo staticTargetMethodInfoField, ILGenerator il, ParameterInfo[] parameters, LocalBuilder argumentsArrayVariable)
        {            
            PushTargetMethodInfoForNonGenericMethod(staticTargetMethodInfoField, il);            
            PushProxyInstance(il);
            PushArguments(il, parameters, argumentsArrayVariable);
            il.Emit(OpCodes.Newobj, TargetInvocationInfoConstructor);
        }

        private static void PushInvocationInfoForGenericMethod(FieldInfo staticOpenGenericTargetMethodInfoField, ILGenerator il, ParameterInfo[] parameters, LocalBuilder argumentsArrayVariable, GenericTypeParameterBuilder[] genericParameters)
        {
            PushTargetMethodInfoForGenericMethod(staticOpenGenericTargetMethodInfoField, il, genericParameters);                        
            PushProxyInstance(il);
            PushArguments(il, parameters, argumentsArrayVariable);            
            il.Emit(OpCodes.Newobj, TargetInvocationInfoConstructor);
        }

        private static GenericTypeParameterBuilder[] CreateGenericTypeParameters(MethodInfo targetMethod, MethodBuilder methodBuilder)
        {            
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

        private static void PushTargetMethodInfoForGenericMethod(FieldInfo staticOpenGenericTargetMethodInfoField, ILGenerator il, GenericTypeParameterBuilder[] genericParameters)
        {
            var typeArrayField = il.DeclareLocal(typeof(Type[]));
            il.Emit(OpCodes.Ldc_I4, genericParameters.Length);
            il.Emit(OpCodes.Newarr, typeof(Type));
            il.Emit(OpCodes.Stloc, typeArrayField);            
            for (int i = 0; i < genericParameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, typeArrayField);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldtoken, genericParameters[i]);
                il.Emit(OpCodes.Call, GetTypeFromHandleMethod);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Ldsfld, staticOpenGenericTargetMethodInfoField);
            il.Emit(OpCodes.Ldloc, typeArrayField);
            il.Emit(OpCodes.Call, GetTargetMethodInfoMethod);
        }

        private static void PushTargetMethodInfoForNonGenericMethod(FieldInfo staticTargetMethodInfoField, ILGenerator il)
        {
            il.Emit(OpCodes.Ldsfld, staticTargetMethodInfoField);            
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
            il.Emit(OpCodes.Ldtoken, targetMethod.GetDeclaringType());
            il.Emit(OpCodes.Call, GetMethodFromHandleMethod);
            il.Emit(OpCodes.Castclass, typeof(MethodInfo));
        }

        private static void PushArguments(ILGenerator il, MethodInfo targetMethod)
        {
            for (int i = 1; i <= targetMethod.GetParameters().Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }
        }
   
        private static void CallObjectConstructor(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);
            il.Emit(OpCodes.Ldarg_0);
        }
        
        // ReSharper disable UnusedMember.Local
        private static Func<T> CreateGenericTargetFactory<T>(Func<object> del)
        // ReSharper restore UnusedMember.Local
        {
            return () => (T)del();
        }

        private static void Call(ILGenerator il, MethodInfo targetMethod)
        {
            il.Emit(OpCodes.Callvirt, targetMethod);
        }

        private static void Return(ILGenerator il)
        {
            il.Emit(OpCodes.Ret);            
        }

        private void PushTargetInstance(ILGenerator il)
        {
            il.Emit(OpCodes.Ldfld, lazyTargetField);
            var getTargetValueMethod = lazyTargetField.FieldType.GetProperty("Value").GetGetMethod();
            il.Emit(OpCodes.Call, getTargetValueMethod);
        }

        private void PushReturnValue(ILGenerator il, Type returnType)
        {
            if (returnType == typeof(void))
            {
                il.Emit(OpCodes.Pop);
            }
            else
            {
                if (returnType.IsAssignableFrom(proxyDefinition.TargetType))
                {
                    PushProxyInstanceIfReturnValueEqualsTargetInstance(il, typeof(object));
                }

                il.Emit(returnType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, returnType);
            }

            Return(il);
        }

        private void PushProxyInstanceIfReturnValueEqualsTargetInstance(ILGenerator il, Type returnType)
        {
            var returnValueVariable = il.DeclareLocal(returnType);
            il.Emit(OpCodes.Stloc, returnValueVariable);
            var lazyTargetType = typeof(Lazy<>).MakeGenericType(proxyDefinition.TargetType);
            var getValueMethod = lazyTargetType.GetMethod("get_Value");
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, lazyTargetField);
            il.Emit(OpCodes.Callvirt, getValueMethod);
            var equalsMethod = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
            var returnProxyLabel = il.DefineLabel();
            var returnReturnValueLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldloc, returnValueVariable);
            il.Emit(OpCodes.Call, equalsMethod);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse, returnProxyLabel);
            il.Emit(OpCodes.Br, returnReturnValueLabel);
            il.MarkLabel(returnProxyLabel);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc, returnValueVariable);
            il.MarkLabel(returnReturnValueLabel);
            il.Emit(OpCodes.Ldloc, returnValueVariable);
        }

        private IEnumerable<PropertyInfo> GetTargetProperties()
        {
            return proxyDefinition.TargetType.GetProperties().ToArray();
        }

        private void ImplementConstructor()
        {
            if (proxyDefinition.TargetFactory == null)
            {
                ImplementConstructorWithLazyTargetParameter();
            }
            else
            {
                ImplementParameterlessConstructor();
            }
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
            PushInterceptorInstance(lazyMethodInterceptorField, il);
            
            if (targetMethod.IsGenericMethod)
            {                                                
                var genericParameters = CreateGenericTypeParameters(targetMethod, methodBuilder);
                FieldInfo staticOpenGenericTargetMethodInfoField =
                       DefineStaticOpenGenericTargetMethodInfoField(targetMethod);
                PushInvocationInfoForGenericMethod(staticOpenGenericTargetMethodInfoField, il, parameters, argumentsArrayVariable, genericParameters);                                                
            }
            else
            {                
                FieldInfo staticTargetMethodInfoField = DefineStaticTargetMethodInfoField(targetMethod);                
                PushInvocationInfoForNonGenericMethod(staticTargetMethodInfoField, il, parameters, argumentsArrayVariable);                
            }

            Call(il, InterceptorInvokeMethod);
            UpdateRefArguments(parameters, il, argumentsArrayVariable);
            PushReturnValue(il, targetMethod.ReturnType);              
            return methodBuilder;
        }
       
        private FieldBuilder DefineStaticTargetMethodInfoField(MethodInfo targetMethod)
        {
            var fieldBuilder = typeBuilder.DefineField(
                GetUniqueMemberName(targetMethod.Name + "TargetMethodInfo"),
                typeof(TargetMethodInfo),
                FieldAttributes.InitOnly | FieldAttributes.Private | FieldAttributes.Static);
            var il = staticConstructorBuilder.GetILGenerator();
            PushMethodInfo(targetMethod, il);
            il.Emit(OpCodes.Newobj, TargetMethodInfoConstructor);
            il.Emit(OpCodes.Stsfld, fieldBuilder);
            return fieldBuilder;
        }

        private FieldBuilder DefineStaticOpenGenericTargetMethodInfoField(MethodInfo targetMethod)
        {
            var fieldBuilder = typeBuilder.DefineField(
                GetUniqueMemberName(targetMethod.Name + "OpenGenericMethodInfo"),
                typeof(OpenGenericTargetMethodInfo),
                FieldAttributes.InitOnly | FieldAttributes.Private | FieldAttributes.Static);
            var il = staticConstructorBuilder.GetILGenerator();            
            PushMethodInfo(targetMethod, il);
            il.Emit(OpCodes.Newobj, OpenGenericTargetMethodInfoConstructor);
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

            return typeBuilder.DefineField(memberName, typeof(Lazy<IInterceptor>), FieldAttributes.Private);            
        }

        private string GetUniqueMemberName(string memberName)
        {
            int count;
            if (!memberNames.TryGetValue(memberName, out count))
            {
                memberNames.Add(memberName, 0);                
            }
            else
            {
                memberNames[memberName] = count + 1;    
            }

            return memberName + memberNames[memberName];
        }

        private MethodBuilder ImplementPassThroughMethod(MethodInfo targetMethod)
        {
            MethodBuilder methodBuilder = GetMethodBuilder(targetMethod);
            ILGenerator il = methodBuilder.GetILGenerator();

            PushProxyInstance(il);
            PushTargetInstance(il);            
            PushArguments(il, targetMethod);
            Call(il, targetMethod);

            if (targetMethod.ReturnType.IsAssignableFrom(proxyDefinition.TargetType))
            {
                PushProxyInstanceIfReturnValueEqualsTargetInstance(il, targetMethod.ReturnType);
            }

            Return(il);            
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
                var interceptorField = typeBuilder.DefineField(
                    "interceptor" + index, typeof(Lazy<IInterceptor>), FieldAttributes.Private);

                var interceptorFactoryField = typeBuilder.DefineField(
                    "InterceptorFactory" + index,
                    typeof(Func<IInterceptor>),
                    FieldAttributes.Public | FieldAttributes.Static);
                
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
            var lazyConstructor = GetLazyConstructorForTargetType();
            il.Emit(OpCodes.Ldsfld, targetFactoryField);
            il.Emit(OpCodes.Newobj, lazyConstructor);
            il.Emit(OpCodes.Call, initializerMethodBuilder);           
        }

        private ConstructorInfo GetLazyConstructorForTargetType()
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
                                           .Where(m => !m.IsSpecialName)
                                           .Concat(typeof(object).GetMethods().Where(m => m.IsVirtual))
                                           .Concat(proxyDefinition.AdditionalInterfaces.SelectMany(i => i.GetMethods()))
                                           .Distinct()
                                           .ToArray();
        }
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> that is capable of comparing 
    /// <see cref="Type"/> arrays.
    /// </summary>
    internal class TypeArrayComparer : IEqualityComparer<Type[]>
    {
        /// <summary>
        /// Determines if two <see cref="Type"/> arrays are equal.
        /// </summary>
        /// <param name="x">The first <see cref="Type"/> array.</param>
        /// <param name="y">The second <see cref="Type"/> array.</param>
        /// <returns>true if the specified type arrays are equal; otherwise, false.</returns>
        public bool Equals(Type[] x, Type[] y)
        {
            return x.SequenceEqual(y);
        }

        /// <summary>
        /// Returns a hash code for the given set of <paramref name="types"/>.
        /// </summary>
        /// <param name="types">The <see cref="Type"/> array for which to get the hash code.</param>        
        /// <returns>
        /// The hash code for the given set of <paramref name="types"/>.
        /// </returns>
        public int GetHashCode(Type[] types)
        {
            return types.Aggregate(0, (current, type) => current ^ type.GetHashCode());
        }
    }
}