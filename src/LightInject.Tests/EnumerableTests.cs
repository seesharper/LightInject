using System.Collections.Generic;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests;

public class EnumerableTests
{
    [Fact]
    public void Method()
    {
        var container = new ServiceContainer();
        container.RegisterSingleton<IEnumerable<IFoo>>(f => new IFoo[] { new Foo(), new AnotherFoo() });
        var test = container.GetAllInstances<IFoo>();
    }
}