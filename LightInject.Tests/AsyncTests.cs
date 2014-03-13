//namespace LightInject.Tests
//{
//    using System;
//    using System.Threading;

//    using LightInject.SampleLibrary;

    
//    using Microsoft.VisualStudio.TestTools.UnitTesting;
//    using System.Threading.Tasks;

//    [TestClass]
//    public class AsyncTests : TestBase
//    {
//        [TestMethod]
//        public void GetInstance_Continuation_ReturnsInstance()
//        {
//            var container = CreateContainer();
//            ((ServiceContainer)container).ScopeManagerProvider = new PerCallContextScopeManagerProvider();

//            container.Register<IFoo, Foo>(new PerScopeLifetime());

//            var scope = container.BeginScope();

//            Task.Run(
//                async () =>
//                    {
//                        await Task.Delay(100);
//                        var instance = container.GetInstance<IFoo>();
//                        Assert.IsNotNull(instance);
//                    }).Wait();

//            scope.Dispose();
//        }

//        [TestMethod]
//        public void GetInstance_DifferentExecutionContext_InstancesAreNotSame()
//        {
//            var container = CreateContainer();
//            ((ServiceContainer)container).ScopeManagerProvider = new PerCallContextScopeManagerProvider();

//            container.Register<IFoo, Foo>(new PerScopeLifetime());

//            IFoo[] result = Task.WhenAll(GetInstanceAsync(container), GetInstanceAsync(container)).Result;

//            Assert.AreNotSame(result[0], result[1]);
//        }

//        [TestMethod]
//        public void GetInstance_DifferentExecutionContextWithParentScope_InstancesAreSame()
//        {
//            var container = CreateContainer();
//            ((ServiceContainer)container).ScopeManagerProvider = new PerCallContextScopeManagerProvider();

//            container.Register<IFoo, Foo>(new PerScopeLifetime());
//            var scope = container.BeginScope();
//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);   
//            IFoo[] result = Task.WhenAll(GetInstanceAsync(container), GetInstanceAsync(container)).Result;

//            scope.Dispose();

//            Assert.AreNotSame(result[0], result[1]);
//        }

//        [TestMethod]
//        public void Test()
//        {
//            var container = CreateContainer();
//            container.Register<IFoo, Foo>(new PerScopeLifetime());
//            IFoo instance1;
//            IFoo instance2;
//            using (container.BeginScope())
//            {
//                using (container.BeginScope())
//                {
//                    instance1 = container.GetInstance<IFoo>();
//                }
//                using (container.BeginScope())
//                {
//                    instance2 = container.GetInstance<IFoo>();
//                }
//            }
//            Assert.AreNotSame(instance1, instance2);
//        }


//        private async Task<IFoo> GetInstanceAsync(IServiceContainer container)
//        {
//            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);   
//            using (container.BeginScope())
//            {
//                await Task.Delay(10);
//                return container.GetInstance<IFoo>();
//                //return await Task.Run(() => container.GetInstance<IFoo>());
//            }
//        }





//    }
//}