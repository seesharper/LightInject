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
    LightInject.Fixie version 1.0.0.1
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

namespace LightInject.Fixie
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Fixie;
    using global::Fixie.Conventions;
    using LightInject;

    /// <summary>
    /// Enables LightInject to be used as the IoC container in the Fixie unit test framework.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class LightInjectConvention : DefaultConvention
    {
        private readonly ConcurrentDictionary<Type, IServiceContainer> containers =
            new ConcurrentDictionary<Type, IServiceContainer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectConvention"/> class.
        /// </summary>
        public LightInjectConvention()
        {
            FixtureExecution.Wrap(WrapFixtureExecution);

            CaseExecution.Wrap(WrapCaseExecution);

            Parameters.Add(InitializeParameters);

            ClassExecution.Wrap(WrapClassExecution);

            ClassExecution.UsingFactory(CreateTestClassInstance);
        }

        private static List<object[]> InitializeParameters(MethodInfo method)
        {
            return new List<object[]> { new object[method.GetParameters().Length] };
        }

        private static Action<IServiceContainer> ResolveConfigureAction(Type type)
        {
            var configureMethod = TryGetConfigureMethod(type);

            if (configureMethod != null)
            {
                return container => configureMethod.Invoke(null, new object[] { container });
            }

            return container => container.RegisterAssembly(type.Assembly);
        }

        private static MethodInfo TryGetConfigureMethod(Type type)
        {
            var configureMethod = type.GetMethod(
                "Configure",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy,
                null,
                new[] { typeof(IServiceContainer) },
                null);
            return configureMethod;
        }

        private object CreateTestClassInstance(Type type)
        {
            containers[type].BeginScope();
            return containers[type].GetInstance(type);
        }

        private void WrapClassExecution(Class context, Action next)
        {
            var container = CreateContainer(context);
            next();
            CleanupContainer(context, container);
        }

        private void CleanupContainer(Class context, IServiceContainer container)
        {
            container.Dispose();
            containers.TryRemove(context.Type, out container);
        }

        private IServiceContainer CreateContainer(Class context)
        {
            IServiceContainer container = new ServiceContainer();
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            container.Register(context.Type, new PerScopeLifetime());
            var configureAction = ResolveConfigureAction(context.Type);
            configureAction(container);
            containers.TryAdd(context.Type, container);
            return container;
        }

        private void WrapCaseExecution(Case context, Action next)
        {
            if (Config.ParameterSources.Any())
            {
                return;
            }
            
            var container = containers[context.Class];
            using (container.BeginScope())
            {
                ParameterInfo[] parameters = context.Method.GetParameters();

                for (int index = 0; index < parameters.Length; index++)
                {
                    context.Parameters[index] = container.GetInstance(parameters[index].ParameterType);
                }

                next();
            }
        }

        private void WrapFixtureExecution(Fixture context, Action next)
        {
            next();
            containers[context.Class.Type].EndCurrentScope();
        }
    }
}