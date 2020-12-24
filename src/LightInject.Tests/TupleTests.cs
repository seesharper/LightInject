using System;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class TupleTests : TestBase
    {

        [Fact]
        public void GetInstance_SingleTuple_ReturnsInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.Register<Tuple<IFoo>>();
            container.GetInstance<Tuple<IFoo>>();
        }
    }
}