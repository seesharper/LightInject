using System;
using System.Collections.Generic;
using Xunit;

namespace LightInject.Tests
{
    public class Issue556
    {

        [Fact]
        public void ShouldDisposeSingletonsInCorrectOrder()
        {
            var container = new ServiceContainer();
            container.Register<FakeDisposeCallback>(new PerContainerLifetime());
            container.Register<Dep1>(new PerContainerLifetime());
            container.Register<Dep2>(sf => new Dep2(sf.GetInstance<Dep1>()));

            //container.Register<Dep2>(new PerContainerLifetime());
            container.Register<Dep3>(new PerContainerLifetime());
            container.GetInstance<Dep3>();

            var disposableCallback = container.GetInstance<FakeDisposeCallback>();

            container.Dispose();

            Assert.IsType<Dep3>(disposableCallback.Disposed[0]);
            Assert.IsType<Dep1>(disposableCallback.Disposed[1]);

        }

        public class FakeDisposeCallback
        {
            public List<object> Disposed
            {
                get;
            } = new List<object>();
        }

        public class Dep1 : IDisposable
        {
            private readonly FakeDisposeCallback fakeDisposeCallback;

            public Dep1(FakeDisposeCallback fakeDisposeCallback)
            {
                this.fakeDisposeCallback = fakeDisposeCallback;
            }

            public void Dispose()
            {
                fakeDisposeCallback.Disposed.Add(this);
            }
        }

        public class Dep2
        {
            public Dep2(Dep1 dep)
            {

            }

            public void DoSomething()
            {
            }
        }

        public class Dep3 : IDisposable
        {
            private readonly Dep2 _dep;
            private readonly FakeDisposeCallback fakeDisposeCallback;

            public Dep3(Dep2 dep, FakeDisposeCallback fakeDisposeCallback)
            {
                _dep = dep;
                this.fakeDisposeCallback = fakeDisposeCallback;
            }

            public void Dispose()
            {
                fakeDisposeCallback.Disposed.Add(this);
            }
        }
    }



}