/*********************************************************************************
    The MIT License (MIT)

    Copyright (c) 2014 bernhard.richter@gmail.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    LightInject.AutoFactory version 1.0.0.1
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Product name starts with lowercase letter.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("MaintainabilityRules", "SA1403", Justification = "One source file")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("DocumentationRules", "SA1649", Justification = "One source file")]

namespace LightInject
{
    using System;
    using AutoFactory;

    /// <summary>
    /// Extends the <see cref="IServiceContainer"/> interface with
    /// a method that makes it possible to register factory interfaces
    /// that is automatically implemented.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Enables LightInject to automatically implement factory interfaces.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/>.</param>
        public static void EnableAutoFactories(this IServiceContainer container)
        {
            container.RegisterConstructorDependency<IServiceFactory>((factory, info) => container);
            container.Register<IAutoFactoryBuilder, AutoFactoryBuilder>();
            container.Register<ITypeBuilderFactory, TypeBuilderFactory>(new PerContainerLifetime());
            container.Register<IServiceNameResolver, ServiceNameResolver>(new PerContainerLifetime());
            container.RegisterConstructorDependency<IServiceFactory>((factory, info) => container);
        }

        /// <summary>
        /// Registers a factory of type <typeparamref name="TFactory"/> to be implemented.
        /// </summary>
        /// <typeparam name="TFactory">The type of factory to be implemented.</typeparam>
        /// <param name="container">The target <see cref="IServiceContainer"/>.</param>
        public static void RegisterAutoFactory<TFactory>(this IServiceContainer container)
        {
            container.Register(CreateFactoryInstance<TFactory>, new PerContainerLifetime());
        }

        private static TFactory CreateFactoryInstance<TFactory>(IServiceFactory container)
        {
            IAutoFactoryBuilder builder = container.GetInstance<IAutoFactoryBuilder>();
            Type factoryType = builder.GetFactoryType(typeof(TFactory));
            return (TFactory)Activator.CreateInstance(factoryType, container);
        }
    }
}

namespace LightInject.AutoFactory
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a class that is capable of creating a
    /// factory type that implements a given factory interface.
    /// </summary>
    public interface IAutoFactoryBuilder
    {
        /// <summary>
        /// Gets a <see cref="Type"/> that implements the given <paramref name="factoryInterface"/>.
        /// </summary>
        /// <param name="factoryInterface">The interface to be implemented by the factory type.</param>
        /// <returns>A factory type that implements the <paramref name="factoryInterface"/>.</returns>
        Type GetFactoryType(Type factoryInterface);
    }

    /// <summary>
    /// Represents a class that is capable of creating a <see cref="TypeBuilder"/> that
    /// is used to build the proxy type.
    /// </summary>
    public interface ITypeBuilderFactory
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
    /// Represents a class that is capable of resolving the
    /// name of the service to be retrived based on a given method.
    /// </summary>
    public interface IServiceNameResolver
    {
        /// <summary>
        /// Resolves the name of the service to be retrieved based on the given <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method for which to resolve the service name.</param>
        /// <returns>The name of the service if the method represent a named service, otherwise null.</returns>
        string Resolve(MethodInfo method);
    }

    /// <summary>
    /// A class that is capable of creating a
    /// factory type that implements a given factory interface.
    /// </summary>
    public class AutoFactoryBuilder : IAutoFactoryBuilder
    {
        private static readonly MethodInfo[] GetInstanceMethods;
        private static readonly MethodInfo[] NamedGetInstanceMethods;
        private static readonly ConstructorInfo ObjectConstructor;
        private readonly IServiceNameResolver serviceNameResolver;
        private readonly ITypeBuilderFactory typeBuilderFactory;

        static AutoFactoryBuilder()
        {
            GetInstanceMethods =
                typeof(IServiceFactory).GetTypeInfo()
                    .DeclaredMethods.Where(m => m.Name == "GetInstance" && m.IsGenericMethod && m.GetParameters().Length < m.GetGenericArguments().Length)
                    .OrderBy(m => m.GetGenericArguments().Length)
                    .ToArray();

            NamedGetInstanceMethods =
                typeof(IServiceFactory).GetTypeInfo()
                    .DeclaredMethods.Where(
                        m =>
                            m.Name == "GetInstance" && m.IsGenericMethod && m.GetParameters().Length > 0 &&
                            m.GetParameters().Last().ParameterType == typeof(string))
                    .OrderBy(m => m.GetGenericArguments().Length)
                    .ToArray();

            ObjectConstructor =
                typeof(object).GetTypeInfo().DeclaredConstructors.Single(c => c.GetParameters().Length == 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFactoryBuilder"/> class.
        /// </summary>
        /// <param name="typeBuilderFactory">The <see cref="ITypeBuilderFactory"/> that is responsible for
        /// creating a <see cref="TypeBuilder"/> instance.</param>
        /// <param name="serviceNameResolver">The <see cref="IServiceNameResolver"/> that is
        /// responsible for resolving the service name from a given factory method.</param>
        public AutoFactoryBuilder(ITypeBuilderFactory typeBuilderFactory, IServiceNameResolver serviceNameResolver)
        {
            this.serviceNameResolver = serviceNameResolver;
            this.typeBuilderFactory = typeBuilderFactory;
        }

        /// <summary>
        /// Gets a <see cref="Type"/> that implements the given <paramref name="factoryInterface"/>.
        /// </summary>
        /// <param name="factoryInterface">The interface to be implemented by the factory type.</param>
        /// <returns>A factory type that implements the <paramref name="factoryInterface"/>.</returns>
        public Type GetFactoryType(Type factoryInterface)
        {
            if (!factoryInterface.GetTypeInfo().IsInterface)
            {
                throw new InvalidOperationException("The factory interface type must be an interface");
            }

            var typeBuilder = typeBuilderFactory.CreateTypeBuilder(factoryInterface, Type.EmptyTypes);
            var containerField = ImplementConstructor(typeBuilder);
            ImplementMethods(typeBuilder, factoryInterface, containerField);
            return typeBuilderFactory.CreateType(typeBuilder);
        }

        private static FieldBuilder ImplementConstructor(TypeBuilder typeBuilder)
        {
            var containerField = typeBuilder.DefineField("container", typeof(IServiceFactory), FieldAttributes.Private);
            const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName
                                                | MethodAttributes.RTSpecialName;
            var constructorBuilder = typeBuilder.DefineConstructor(attributes, CallingConventions.Standard, new[] { typeof(IServiceFactory) });

            var il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectConstructor);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, containerField);
            il.Emit(OpCodes.Ret);
            return containerField;
        }

        private static void DefineGenericParameters(MethodInfo targetMethod, MethodBuilder methodBuilder)
        {
            Type[] genericArguments = targetMethod.GetGenericArguments().ToArray();
            GenericTypeParameterBuilder[] genericTypeParameters = methodBuilder.DefineGenericParameters(genericArguments.Select(a => a.Name).ToArray());
            for (int i = 0; i < genericArguments.Length; i++)
            {
                genericTypeParameters[i].SetGenericParameterAttributes(genericArguments[i].GetTypeInfo().GenericParameterAttributes);
                ApplyGenericConstraints(genericArguments[i], genericTypeParameters[i]);
            }
        }

        private static void ApplyGenericConstraints(Type genericArgument, GenericTypeParameterBuilder genericTypeParameter)
        {
            var genericConstraints = genericArgument.GetTypeInfo().GetGenericParameterConstraints();
            genericTypeParameter.SetInterfaceConstraints(genericConstraints.Where(gc => gc.GetTypeInfo().IsInterface).ToArray());
            genericTypeParameter.SetBaseTypeConstraint(genericConstraints.FirstOrDefault(t => t.GetTypeInfo().IsClass));
        }

        private void ImplementMethods(TypeBuilder typeBuilder, Type interfaceType, FieldBuilder containerField)
        {
            var methods = interfaceType.GetTypeInfo().DeclaredMethods;
            foreach (var method in methods)
            {
                ImplementMethod(typeBuilder, method, containerField);
            }
        }

        private void ImplementMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder containerField)
        {
            var methodAttributes = method.Attributes ^ MethodAttributes.Abstract;
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            string serviceName = GetServiceName(method);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                                            method.Name,
                                            methodAttributes,
                                            method.ReturnType,
                                            parameterTypes);

            if (method.IsGenericMethod)
            {
                DefineGenericParameters(method, methodBuilder);
            }

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, containerField);
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
            }

            MethodInfo closedGenericGetInstanceMethod;

            if (string.IsNullOrEmpty(serviceName))
            {
                var openGenericGetInstanceMethod = GetInstanceMethods[parameterTypes.Length];
                closedGenericGetInstanceMethod = openGenericGetInstanceMethod.MakeGenericMethod(parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
            }
            else
            {
                var openGenericGetInstanceMethod = NamedGetInstanceMethods[parameterTypes.Length];
                closedGenericGetInstanceMethod = openGenericGetInstanceMethod.MakeGenericMethod(parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
                il.Emit(OpCodes.Ldstr, serviceName);
            }

            il.Emit(OpCodes.Callvirt, closedGenericGetInstanceMethod);
            il.Emit(OpCodes.Ret);
        }

        private string GetServiceName(MethodInfo method)
        {
            return serviceNameResolver.Resolve(method);
        }
    }

    /// <summary>
    /// A class that is capable of creating a <see cref="TypeBuilder"/> that
    /// is used to build the proxy type.
    /// </summary>
    public class TypeBuilderFactory : ITypeBuilderFactory
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
            const TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Class;
            var typeName = targetType.Name + "AutoFactory";

            if (targetType.GetTypeInfo().IsInterface)
            {
                Type[] interfaceTypes = new[] { targetType }.Concat(additionalInterfaces).ToArray();
                var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, null, interfaceTypes);
                return typeBuilder;
            }
            else
            {
                var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, targetType, additionalInterfaces);
                return typeBuilder;
            }
        }

        /// <summary>
        /// Creates a proxy <see cref="Type"/> based on the given <paramref name="typeBuilder"/>.
        /// </summary>
        /// <param name="typeBuilder">The <see cref="TypeBuilder"/> that represents the proxy type.</param>
        /// <returns>The proxy <see cref="Type"/>.</returns>
        public Type CreateType(TypeBuilder typeBuilder)
        {
            return typeBuilder.CreateTypeInfo().AsType();
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            AssemblyBuilder assemblyBuilder = GetAssemblyBuilder();
            return assemblyBuilder.DefineDynamicModule("LightInject.AutoFactory.AutoFactoryAssembly");
        }

        private static AssemblyBuilder GetAssemblyBuilder()
        {
            var assemblybuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("LightInject.AutoFactory.AutoFactoryAssembly"), AssemblyBuilderAccess.Run);
            return assemblybuilder;
        }
    }

    /// <summary>
    /// A class that is capable of resolving the
    /// name of the service to be retrived based on a given method.
    /// </summary>
    public class ServiceNameResolver : IServiceNameResolver
    {
        /// <summary>
        /// Resolves the name of the service to be retrieved based on the given <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method for which to resolve the service name.</param>
        /// <returns>The name of the service if the method represent a named service, otherwise null.</returns>
        public string Resolve(MethodInfo method)
        {
            var returnTypeName = ResolveReturnTypeName(method);
            string serviceName = null;
            var match = Regex.Match(method.Name, @"Get(?!" + returnTypeName + ")(.+)");
            if (match.Success)
            {
                serviceName = match.Groups[1].Captures[0].Value;
            }

            return serviceName;
        }

        private static string ResolveReturnTypeName(MethodInfo method)
        {            
            var returnTypeName = Regex.Match(method.ReturnType.Name, @"([a-zA-Z]+)").Groups[1].Captures[0].Value;
            
            if (returnTypeName.StartsWith("I"))
            {
                return returnTypeName.Substring(1);
            }

            return returnTypeName;
        }
    }
}
