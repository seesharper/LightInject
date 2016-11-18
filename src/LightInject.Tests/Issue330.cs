using System;
using System.Collections.Generic;
using System.Reflection;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class Issue330 : TestBase
    {
        [Fact]
        public void Scan_assembly_ignores_open_generic_with_two_parameters_implementing_open_generic_with_one_parameter()
        {
            var container = CreateContainer();

            container.RegisterAssembly(Assembly.GetExecutingAssembly(), (s, t) => t.FullName.Contains(nameof(Issue330)));

            container.GetAllInstances<IFoo>();
            container.TryGetInstance<IFoo<string>>();
            container.TryGetInstance<ICollection<string>>();
            container.TryGetInstance<IList<string>>();
        }

        [Fact]
        public void Resolving_invalid_generic_registration_gives_detailed_error_message()
        {
            var container = CreateContainer();

            container.Register(typeof(IFoo<>), typeof(Foo<,>));

            var exception = Assert.Throws<InvalidOperationException>(() => container.TryGetInstance<IFoo<string>>());
            Assert.StartsWith("Generic parameter mismatch", exception.Message);
        }

        public interface IFoo<T> {}

        public class Foo<T1, T2> : IFoo<T1> {}
    }
}