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
    LightInject.xUnit version 2.0.0.2
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/

using System.Diagnostics;
using System.Threading;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Single source file deployment.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Product name starts with lowercase letter.")]

namespace LightInject.xUnit2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if NET45 || DNX451
    using System.Runtime.Remoting.Messaging;
#endif
    using LightInject;
    using Xunit.Sdk;

    /// <summary>
    /// Allows LightInject to resolve test method arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InjectDataAttribute : DataAttribute
    {
#if NET46 || DNXCORE50
        private static readonly AsyncLocal<IServiceContainer> ContainerAsyncLocal = new AsyncLocal<IServiceContainer>();
#endif
        private readonly Stack<object> data;
#if NET45 || DNXCORE50
        private const string Key = "XunitServiceContainer";
#endif

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
        /// Gets a list of argument data resolved using an <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="methodUnderTest">The test method currently being executed.</param>
        /// <returns>A list of argument data resolved using an <see cref="IServiceContainer"/> instance.</returns>
        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            var container = GetContainer(methodUnderTest.DeclaringType);
            ParameterInfo[] parameters = methodUnderTest.GetParameters();
            if (ShouldStartScope(methodUnderTest))
            {
                container.BeginScope();
            }

            return ResolveParameters(container, parameters);
        }

#if NET46 || DNXCORE50
        internal static void Release(Type type)
        {
            var container = ContainerAsyncLocal.Value;

            if (container != null)
            {
                container.EndCurrentScope();
            }
        }

#endif

#if NET45 || DNX451
        /// <summary>
        /// Releases the current <see cref="Scope"/>.
        /// </summary>
        /// <param name="type">The test class <see cref="Type"/> for which to release the current <see cref="Scope"/>.</param>
        internal static void Release(Type type)
        {
            var key = CreateContainerKey(type);
            var containerWrapper = (ContainerWrapper)CallContext.LogicalGetData(key);

            if (containerWrapper != null)
            {
                containerWrapper.Value.EndCurrentScope();
            }
        }

        private static string CreateContainerKey(Type type)
        {
            return Key + type.AssemblyQualifiedName;
        }
#endif
        private static bool ShouldStartScope(MethodInfo methodUnderTest)
        {
            return methodUnderTest.IsDefined(typeof(ScopedAttribute), true);
        }

#if NET46 || DNXCORE50 || DNX451
        private static IServiceContainer GetContainer(Type type)
        {
            var container = ContainerAsyncLocal.Value;

            if (container == null)
            {
                container = new ServiceContainer();
                InvokeConfigureMethodIfPresent(type, container);
                ContainerAsyncLocal.Value = container;
            }

            return container;
        }

#endif

#if NET45

        private static IServiceContainer GetContainer(Type type)
        {            
            var key = CreateContainerKey(type);
            var containerWrapper = (ContainerWrapper)CallContext.LogicalGetData(key);
            if (containerWrapper == null)
            {
                containerWrapper = new ContainerWrapper { Value = new ServiceContainer() };
                InvokeConfigureMethodIfPresent(type, containerWrapper.Value);
                CallContext.LogicalSetData(key, containerWrapper);
            }

            return containerWrapper.Value;
        }
#endif

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

#if NET45 || DNX451
        [Serializable]
        private class ContainerWrapper : MarshalByRefObject
        {
            [NonSerialized]
            private IServiceContainer value;

            public IServiceContainer Value
            {
                get
                {
                    return value;
                }

                set
                {
                    this.value = value;
                }
            }
        }
#endif
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
            Debug.WriteLine("TEST");
            InjectDataAttribute.Release(methodUnderTest.DeclaringType);
        }
    }
}