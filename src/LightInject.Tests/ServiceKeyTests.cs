using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests;


public class ServiceKeyTests
{
    [Fact]
    public void ShouldBeEqualWhenServiceTypeAndServiceNameIsSame()
    {
        var serviceKey1 = new ServiceKey(typeof(IFoo), "Foo");
        var serviceKey2 = new ServiceKey(typeof(IFoo), "Foo");
        Assert.Equal(serviceKey1, serviceKey2);
    }

    [Fact]
    public void ShouldNotBeEqualWhenServiceTypeIsDifferent()
    {
        var serviceKey1 = new ServiceKey(typeof(IFoo), "Foo");
        var serviceKey2 = new ServiceKey(typeof(IBar), "Foo");
        Assert.NotEqual(serviceKey1, serviceKey2);
    }

    [Fact]
    public void ShouldNotBeEqualWhenServiceNameIsDifferent()
    {
        var serviceKey1 = new ServiceKey(typeof(IFoo), "Foo");
        var serviceKey2 = new ServiceKey(typeof(IFoo), "Bar");
        Assert.NotEqual(serviceKey1, serviceKey2);
    }

    [Fact]
    public void ShouldHaveToStringWithServiceTypeAndServiceName()
    {
        var serviceKey = new ServiceKey(typeof(IFoo), "Foo");
        Assert.Equal("LightInject.SampleLibrary.IFoo - Foo", serviceKey.ToString());
    }
}