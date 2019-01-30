namespace LightInject.Tests
{
    using System.Collections.Generic;

    using Xunit;

    /// <summary>
    /// https://github.com/seesharper/LightInject/issues/473
    /// </summary>
    public class PropertyDependencySelector
    {
        [Fact]
        public void PropertyDependencySelectorTest()
        {
            var container = new TestContainer();

            container.Register<Class1>();
            container.Register<Class3>();
            container.Register(new CustomRegistration
            {
                ImplementingType = typeof(Class2),
                ServiceType = typeof(Class2),
                EnablePropertyInjection = false
            });

            var cls1 = container.GetInstance<Class1>();

            Assert.NotNull(cls1.ContructableClass2);
            Assert.NotNull(cls1.ContructableClass2.ContructableClass3);
            Assert.Null(cls1.ContructableClass2.PropertyClass3);

            Assert.NotNull(cls1.PropertyClass2);
            Assert.NotNull(cls1.PropertyClass2.ContructableClass3);
            Assert.Null(cls1.PropertyClass2.PropertyClass3);
        }

        private class TestContainer : ServiceContainer
        {
            public TestContainer() : base()
            {
                this.PropertyDependencySelector = new CustomPropertyDependencySelector(this.PropertyDependencySelector);
            }

            private class CustomPropertyDependencySelector : IPropertyDependencySelector
            {
                private readonly IPropertyDependencySelector innerPropertyDependencySelector;

                public CustomPropertyDependencySelector(IPropertyDependencySelector innerPropertyDependencySelector)
                {
                    this.innerPropertyDependencySelector = innerPropertyDependencySelector;
                }

                public IEnumerable<PropertyDependency> Execute(Registration registration)
                {
                    if (registration is CustomRegistration microsoftRegistration && !microsoftRegistration.EnablePropertyInjection)
                    {
                        return new PropertyDependency[0];
                    }

                    return this.innerPropertyDependencySelector.Execute(registration);
                }
            }
        }

        private class CustomRegistration : ServiceRegistration
        {
            public CustomRegistration() : base()
            {
                this.EnablePropertyInjection = true;
            }

            /// <summary>
            /// Gets or sets a value indicating whether property injection is enabled.
            /// </summary>
            public bool EnablePropertyInjection { get; set; }
        }

        private class Class1
        {
            public Class1(Class2 contructableClass2)
            {
                ContructableClass2 = contructableClass2;
            }

            public Class2 PropertyClass2 { get; set; }

            public Class2 ContructableClass2 { get; }
        }

        private class Class2
        {
            public Class2(Class3 contructableClass3)
            {
                ContructableClass3 = contructableClass3;
            }

            public Class3 PropertyClass3 { get; set; }

            public Class3 ContructableClass3 { get; }
        }

        private class Class3
        {

        }
    }
}