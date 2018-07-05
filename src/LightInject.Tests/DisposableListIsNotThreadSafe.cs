//namespace LightInject.Tests
//{
//    using System.Linq;
//    using LightInject.SampleLibrary;
//    using Xunit;    
//    public class DisposableListIsNotThreadSafe
//    {
//        [Fact]
//        public void Init_ScopesWithDisposableService_DontThrowError()
//        {
//            ServiceContainer container = new ServiceContainer();

//            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            
//            // Register disposable Foo.
//            container.Register<IFoo>(factory => new DisposableFoo(), new PerRequestLifeTime());

//            using (var scope = container.BeginScope())
//            {
//                Enumerable.Range(0, 100)
//                          .AsParallel()
//                          .ForAll(num => scope.GetInstance<IFoo>());
//            }
//        }
//    }
//}