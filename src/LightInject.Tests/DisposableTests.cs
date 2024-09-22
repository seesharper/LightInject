using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using LightInject.SampleLibrary;
using Xunit;
namespace LightInject.Tests
{
    public class DisposableTests
    {
        [Fact]
        public void Dispose_ServiceWithPerScopeLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableFoo = new DisposableFoo();
            container.Register<IFoo>(factory => disposableFoo, new PerScopeLifetime());
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
            }

            Assert.True(disposableFoo.IsDisposed);
        }

        [Fact]
        public void Dispose_ServiceWithPerRequestLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableFoo = new DisposableFoo();
            container.Register<IFoo>(factory => disposableFoo, new PerRequestLifeTime());
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
            }

            Assert.True(disposableFoo.IsDisposed);
        }

        [Fact]
        public void Dispose_Singeltons_DisposesInReverseOrderOfCreation()
        {
            var container = CreateContainer();
            container.Register<FakeDisposeCallback>(new PerContainerLifetime());
            container.Register<ISingleton, Singleton1>("1", new PerContainerLifetime());
            container.Register<ISingleton, Singleton2>("2", new PerContainerLifetime());
            container.Register<ISingleton, Singleton3>("3", new PerContainerLifetime());
            container.Register<ISingleton, Singleton4>("4", new PerContainerLifetime());
            container.Register<ISingleton, Singleton5>("5", new PerContainerLifetime());

            var instances = container.GetAllInstances<ISingleton>();

            var disposableCallback = container.GetInstance<FakeDisposeCallback>();

            container.Dispose();

            Assert.IsType<Singleton5>(disposableCallback.Disposed[0]);
            Assert.IsType<Singleton4>(disposableCallback.Disposed[1]);
            Assert.IsType<Singleton3>(disposableCallback.Disposed[2]);
            Assert.IsType<Singleton2>(disposableCallback.Disposed[3]);
            Assert.IsType<Singleton1>(disposableCallback.Disposed[4]);
        }

        [Fact]
        public void Dispose_SingeltonWithFactory_DisposesInReverseOrderOfCreation()
        {
            var container = CreateContainer();
            container.Register<FakeDisposeCallback>(new PerContainerLifetime());
            container.Register<ISingleton, Singleton1>("1", new PerContainerLifetime());
            container.Register<ISingleton>(sf => new Singleton2(sf.GetInstance<FakeDisposeCallback>()), "2", new PerContainerLifetime());
            container.Register<ISingleton>(sf => new Singleton3(sf.GetInstance<FakeDisposeCallback>()), "3", new PerContainerLifetime());
            container.Register<ISingleton, Singleton4>("4", new PerContainerLifetime());
            container.Register<ISingleton, Singleton5>("5", new PerContainerLifetime());

            var instances = container.GetAllInstances<ISingleton>();

            var disposableCallback = container.GetInstance<FakeDisposeCallback>();

            container.Dispose();

            Assert.IsType<Singleton5>(disposableCallback.Disposed[0]);
            Assert.IsType<Singleton4>(disposableCallback.Disposed[1]);
            Assert.IsType<Singleton3>(disposableCallback.Disposed[2]);
            Assert.IsType<Singleton2>(disposableCallback.Disposed[3]);
            Assert.IsType<Singleton1>(disposableCallback.Disposed[4]);
        }


        [Fact]
        public void Dispose_Scoped_DisposesInReverseOrderOfCreation()
        {
            var container = CreateContainer();
            container.Register<FakeDisposeCallback>(new PerContainerLifetime());
            container.Register<ISingleton, Singleton1>("1", new PerScopeLifetime());
            container.Register<ISingleton, Singleton2>("2", new PerScopeLifetime());
            container.Register<ISingleton, Singleton3>("3", new PerScopeLifetime());
            container.Register<ISingleton, Singleton4>("4", new PerScopeLifetime());
            container.Register<ISingleton, Singleton5>("5", new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var instances = container.GetAllInstances<ISingleton>();
            }

            var disposableCallback = container.GetInstance<FakeDisposeCallback>();

            container.Dispose();

            Assert.IsType<Singleton5>(disposableCallback.Disposed[0]);
            Assert.IsType<Singleton4>(disposableCallback.Disposed[1]);
            Assert.IsType<Singleton3>(disposableCallback.Disposed[2]);
            Assert.IsType<Singleton2>(disposableCallback.Disposed[3]);
            Assert.IsType<Singleton1>(disposableCallback.Disposed[4]);
        }


        [Fact]
        public void Dispose_ScopedWithFactory_DisposesInReverseOrderOfCreation()
        {
            var container = CreateContainer();
            container.Register<FakeDisposeCallback>(new PerContainerLifetime());
            container.Register<ISingleton, Singleton1>("1", new PerScopeLifetime());
            container.Register<ISingleton, Singleton2>("2", new PerScopeLifetime());
            container.Register<ISingleton, Singleton3>("3", new PerScopeLifetime());
            container.Register<ISingleton, Singleton4>("4", new PerScopeLifetime());
            container.Register<ISingleton, Singleton5>("5", new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var instances = container.GetAllInstances<ISingleton>();
            }

            var disposableCallback = container.GetInstance<FakeDisposeCallback>();

            container.Dispose();

            Assert.IsType<Singleton5>(disposableCallback.Disposed[0]);
            Assert.IsType<Singleton4>(disposableCallback.Disposed[1]);
            Assert.IsType<Singleton3>(disposableCallback.Disposed[2]);
            Assert.IsType<Singleton2>(disposableCallback.Disposed[3]);
            Assert.IsType<Singleton1>(disposableCallback.Disposed[4]);
        }

        [Fact]
        public void Dispose_Scope_CallsCompletedHandler()
        {
            var container = CreateContainer();
            bool wasCalled = false;
            using (var scope = container.BeginScope())
            {
                scope.Completed += (s, a) => wasCalled = true;
            }

            Assert.True(wasCalled);
        }


        //[Fact]
        //public void Dispose_Singletons_DisposesInReverseOrderWhenInjected()
        //{


        //    var container = new ServiceContainer(new ContainerOptions() { DefaultServiceSelector = services => services.Last() });
        //    FakeDisposeCallback callback;
        //    using (var scope = container.BeginScope())
        //    {
        //        container.Register<FakeDisposeCallback>(new PerRootScopeLifetime(scope));
        //        container.Register<IFakeOuterService, FakeDisposableCallbackOuterService>(new PerRequestLifeTime());
        //        container.Register<IFakeMultipleService, FakeDisposableCallbackInnerService>("1", new PerRootScopeLifetime(scope));
        //        container.Register<IFakeMultipleService, FakeDisposableCallbackInnerService>("2", new PerRootScopeLifetime(scope));
        //        container.Register<IFakeMultipleService, FakeDisposableCallbackInnerService>("3", new PerRequestLifeTime());
        //        container.Register<IFakeService, FakeDisposableCallbackInnerService>(new PerRootScopeLifetime(scope));

        //        callback = container.GetInstance<FakeDisposeCallback>();
        //        var outer = container.GetInstance<IFakeOuterService>();
        //        var multipleServices = outer.MultipleServices.ToArray();

        //        scope.Dispose();
        //        Assert.Equal(outer, callback.Disposed[0]);
        //        Assert.Equal(multipleServices.Reverse(), callback.Disposed.Skip(1).Take(3).OfType<IFakeMultipleService>());
        //        Assert.Equal(outer.SingleService, callback.Disposed[4]);
        //    }

        //}

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }



        public class FakeDisposeCallback
        {
            public List<object> Disposed
            {
                get;
            } = new List<object>();
        }

        public interface ISingleton
        {

        }

        public class Singleton1 : IDisposable, ISingleton
        {
            private readonly FakeDisposeCallback fakeDisposeCallback;

            public Singleton1(FakeDisposeCallback fakeDisposeCallback)
            {
                this.fakeDisposeCallback = fakeDisposeCallback;
            }

            public void Dispose()
            {
                fakeDisposeCallback.Disposed.Add(this);
            }
        }

        public class Singleton2 : Singleton1
        {
            public Singleton2(FakeDisposeCallback fakeDisposeCallback) : base(fakeDisposeCallback)
            {
            }
        }

        public class Singleton3 : Singleton1
        {
            public Singleton3(FakeDisposeCallback fakeDisposeCallback) : base(fakeDisposeCallback)
            {
            }
        }

        public class Singleton4 : Singleton1
        {
            public Singleton4(FakeDisposeCallback fakeDisposeCallback) : base(fakeDisposeCallback)
            {
            }
        }

        public class Singleton5 : Singleton1
        {
            public Singleton5(FakeDisposeCallback fakeDisposeCallback) : base(fakeDisposeCallback)
            {
            }
        }


        public class FakeDisposableCallbackService : IDisposable
        {
            private static int _globalId;

            private readonly int _id;

            private readonly FakeDisposeCallback _callback;

            public FakeDisposableCallbackService(FakeDisposeCallback callback)
            {
                _id = _globalId++;
                _callback = callback;
            }

            public void Dispose()
            {
                _callback.Disposed.Add((object)this);
            }

            public override string ToString()
            {
                return _id.ToString();
            }
        }

        public class FakeDisposableCallbackInnerService : FakeDisposableCallbackService, IFakeMultipleService, IFakeService
        {
            public FakeDisposableCallbackInnerService(FakeDisposeCallback callback)
                : base(callback)
            {
            }
        }

        public interface IFakeMultipleService : IFakeService
        {
        }

        public interface IFakeService
        {
        }

        public interface IFakeOuterService
        {
            IFakeService SingleService
            {
                get;
            }

            IEnumerable<IFakeMultipleService> MultipleServices
            {
                get;
            }
        }

        public class FakeDisposableCallbackOuterService : FakeDisposableCallbackService, IFakeOuterService
        {
            public IFakeService SingleService
            {
                get;
            }

            public IEnumerable<IFakeMultipleService> MultipleServices
            {
                get;
            }

            public FakeDisposableCallbackOuterService(IFakeService singleService, IEnumerable<IFakeMultipleService> multipleServices, FakeDisposeCallback callback)
                : base(callback)
            {
                SingleService = singleService;
                MultipleServices = multipleServices;
            }
        }
    }

    /// <summary>
    /// An <see cref="ILifetime"/> implementation that makes it possible to mimic the notion of a root scope.
    /// </summary>
    [LifeSpan(30)]
    internal class PerRootScopeLifetime : ILifetime, ICloneableLifeTime
    {
        private readonly object syncRoot = new object();
        private readonly Scope rootScope;
        private object instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRootScopeLifetime"/> class.
        /// </summary>
        /// <param name="rootScope">The root <see cref="Scope"/>.</param>
        public PerRootScopeLifetime(Scope rootScope)
            => this.rootScope = rootScope;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public object GetInstance(Func<object> createInstance, Scope scope)
            => throw new NotImplementedException("Uses optimized non closing method");

        /// <inheritdoc/>
        public ILifetime Clone()
            => new PerRootScopeLifetime(rootScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060
        public object GetInstance(GetInstanceDelegate createInstance, Scope scope, object[] arguments)
        {
#pragma warning restore IDE0060
            if (instance != null)
            {
                return instance;
            }

            lock (syncRoot)
            {
                if (instance == null)
                {
                    instance = createInstance(arguments, rootScope);
                    RegisterForDisposal(instance);
                }
            }

            return instance;
        }

        private void RegisterForDisposal(object instance)
        {
            if (instance is IDisposable disposable)
            {
                rootScope.TrackInstance(disposable);
            }
            else if (instance is IAsyncDisposable asyncDisposable)
            {
                rootScope.TrackInstance(asyncDisposable);
            }
        }
    }
}