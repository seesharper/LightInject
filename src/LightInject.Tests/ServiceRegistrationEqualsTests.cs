using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests;

public class ServiceRegistrationEqualsTests
{
    [Fact]
    public void ShouldBeEqualWhenServiceTypeAndServiceNameAreTheSame()
    {
        var serviceRegistration1 = new ServiceRegistration
        {
            ServiceType = typeof(int),
            ServiceName = string.Empty
        };

        var serviceRegistration2 = new ServiceRegistration
        {
            ServiceType = typeof(int),
            ServiceName = string.Empty
        };

        Assert.Equal(serviceRegistration1, serviceRegistration2);
    }

    [Fact]
    public void ShouldNotBeEqualIfImplementingTypeIsDifferent()
    {
        var serviceRegistration1 = new ServiceRegistration
        {
            ServiceType = typeof(IFoo),
            ServiceName = string.Empty,
            ImplementingType = typeof(Foo)
        };

        var serviceRegistration2 = new ServiceRegistration
        {
            ServiceType = typeof(IFoo),
            ServiceName = string.Empty,
            ImplementingType = typeof(AnotherFoo)
        };

        Assert.NotEqual(serviceRegistration1, serviceRegistration2);
    }

    [Fact]
    public void ShouldNotBeEqualWithMixedImplementingTypeAndFactoryExpression()
    {
        var serviceRegistration1 = new ServiceRegistration
        {
            ServiceType = typeof(IFoo),
            ServiceName = string.Empty,
            ImplementingType = typeof(Foo)
        };

        var serviceRegistration2 = new ServiceRegistration
        {
            ServiceType = typeof(IFoo),
            ServiceName = string.Empty,
            FactoryExpression = (IServiceFactory f) => new Foo()
        };

        Assert.NotEqual(serviceRegistration1, serviceRegistration2);
    }
}