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
    LightInject.xUnit version 2.0.0.4
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Product name starts with lowercase letter.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Single source file deployment")]
namespace LightInject.xUnit2
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using LightInject;
    using Xunit.Sdk;

    /// <summary>
    /// Allows LightInject to resolve test method arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InjectDataAttribute : DataAttribute
    {
        private static readonly ConcurrentDictionary<MethodInfo, Scope> Scopes =
            new ConcurrentDictionary<MethodInfo, Scope>();

        private static readonly ConcurrentDictionary<Type, IServiceContainer> Containers =
            new ConcurrentDictionary<Type, IServiceContainer>();

        private readonly Stack<object> data;

        static InjectDataAttribute()
        {
            // Since the GetData method is also executed during test discovery
            // we need to ensure that we close all existing scopes.
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => EndAllScopes();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectDataAttribute"/> class.
        /// </summary>
        public InjectDataAttribute()
        {
            data = new Stack<object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectDataAttribute"/> class.
        /// </summary>
        /// <param name="data">An array of values to be passed to the test method.</param>
        public InjectDataAttribute(params object[] data)
        {
            this.data = new Stack<object>(data.Reverse());
        }

        /// <summary>
        /// Ends the <see cref="Scope"/> represented by the given <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> representing the method for which to end the <see cref="Scope"/>.</param>
        public static void EndScope(MethodInfo method)
        {
            Scopes[method].Dispose();
        }

        /// <summary>
        /// Gets a list of argument data resolved using an <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="methodUnderTest">The test method currently being executed.</param>
        /// <returns>A list of argument data resolved using an <see cref="IServiceContainer"/> instance.</returns>
        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            var container = GetContainer(methodUnderTest.ReflectedType);
            ParameterInfo[] parameters = methodUnderTest.GetParameters();
            if (ShouldStartScope(methodUnderTest))
            {
                Scopes.TryAdd(methodUnderTest, container.BeginScope());
            }

            return ResolveParameters(container, parameters);
        }

        private static void EndAllScopes()
        {
            foreach (var scope in Scopes)
            {
                scope.Value.Dispose();
            }
        }

        private static bool ShouldStartScope(MethodInfo methodUnderTest)
        {
            return methodUnderTest.IsDefined(typeof(ScopedAttribute), true);
        }

        private static IServiceContainer GetContainer(Type type)
        {
            return Containers.GetOrAdd(type, CreateContainer);
        }

        private static IServiceContainer CreateContainer(Type type)
        {
            var container = new ServiceContainer();
            InvokeConfigureMethodIfPresent(type, container);
            return container;
        }

        private static object GetInstance(IServiceFactory factory, ParameterInfo parameter)
        {
            var instance = factory.TryGetInstance(parameter.ParameterType);
            if (instance != null)
            {
                return instance;
            }

            return factory.TryGetInstance(parameter.ParameterType, parameter.Name);
        }

        private static void InvokeConfigureMethodIfPresent(Type type, IServiceContainer container)
        {
            var callSequence = new Stack<Action>();

            while (type != typeof(object))
            {
                var method = GetConfigureMethod(type);
                if (method != null)
                {
                    callSequence.Push(() => method.Invoke(null, new object[] { container }));
                }

                type = type.BaseType;
            }

            while (callSequence.Count > 0)
            {
                var action = callSequence.Pop();
                action();
            }
        }

        private static MethodInfo GetConfigureMethod(Type type)
        {
            var composeMethod = type.GetMethod(
                "Configure",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return composeMethod;
        }

        private IEnumerable<object[]> ResolveParameters(IServiceFactory factory, IEnumerable<ParameterInfo> parameters)
        {
            return new[] { parameters.Select(p => ResolveParameter(factory, p)).ToArray() };
        }

        private object ResolveParameter(IServiceFactory factory, ParameterInfo parameter)
        {
            object instance;
            try
            {
                instance = GetInstance(factory, parameter);
            }
            catch (InvalidOperationException exception)
            {
                const string errorMessage = "Unable to inject test method arguments. "
               + "Create a static method in the test class with the following signature "
               + "to configure the container: public static void Configure(IServiceContainer container)";
                throw new InvalidOperationException(errorMessage, exception);
            }

            if (instance != null)
            {
                return instance;
            }

            if (data.Count > 0)
            {
                return data.Pop();
            }

            throw new InvalidOperationException(string.Format("No value specified for parameter: {0}", parameter));
        }
    }

    /// <summary>
    /// Indicates that a new <see cref="Scope"/> should be started for this test method.
    /// </summary>
    public class ScopedAttribute : BeforeAfterTestAttribute
    {
        /// <summary>
        /// This method is called after the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void After(MethodInfo methodUnderTest)
        {
            InjectDataAttribute.EndScope(methodUnderTest);
        }
    }
}