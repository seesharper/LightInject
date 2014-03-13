namespace LightInject.Wcf.Tests
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using LightInject.Tests;
    using LightInject.Wcf;
    using LightInject.Wcf.SampleLibrary;
    using LightInject.Wcf.SampleLibrary.Implementation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Important: Before running these test, the following command needs to be executed
    /// in a command prompt with administrator privileges.
    /// c:\netsh http add urlacl url=http://+:6000/ user=[username]
    /// </summary>
    [TestClass]
    public class InvocationTests
    {          
        [TestMethod]
        public void Invoke_ServiceWithoutDependency_IsInvoked()
        {
            using (StartService<IService>())
            {
                var result = Invoke<IService, int>(c => c.Execute());
                Assert.AreEqual(42, result);            
            }                        
        }

        [TestMethod]
        public void Invoke_ServiceWithSameDependencyTwice_CreatesScopedDependency()
        {
            Foo.InitializeCount = 0;
            using (StartService<IServiceWithSameDependencyTwice>())
            {
                Invoke<IServiceWithSameDependencyTwice, int>(c => c.Execute());    
            }  
          
            Assert.AreEqual(1, Foo.InitializeCount);            
        }

        [TestMethod]
        public void Invoke_ServiceWithAsyncCode_CreatesScopedInstance()
        {            
            using (StartService<IAsyncService>())
            {
                //ParallelInvoker.Invoke(50, () => Invoke<IAsyncService, int>(c => c.Execute()));
                Invoke<IAsyncService, Task<IFoo>>(c => c.Execute());
            }                        
        }


        [TestMethod]
        public void Invoke_PerCallInstanceSingleConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<IPerCallInstanceAndSingleConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<IPerCallInstanceAndSingleConcurrency, int>( c=> c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_PerCallInstanceMultipleConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<IPerCallInstanceAndMultipleConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<IPerCallInstanceAndMultipleConcurrency, int>(c => c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_PerCallInstanceReentrantConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<IPerCallInstanceAndReentrantConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<IPerCallInstanceAndReentrantConcurrency, int>(c => c.Execute()));
            }
        }


        [TestMethod]
        public void Invoke_PerSessionInstanceSingleConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<IPerSessionInstanceAndSingleConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<IPerSessionInstanceAndSingleConcurrency, int>(c => c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_PerSessionInstanceMultipleConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<IPerSessionInstanceAndMultipleConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<IPerSessionInstanceAndMultipleConcurrency, int>(c => c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_PerSessionInstanceReentrantConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<IPerSessionInstanceAndReentrantConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<IPerSessionInstanceAndReentrantConcurrency, int>(c => c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_SingleInstanceSingleConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<ISingleInstanceAndSingleConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<ISingleInstanceAndSingleConcurrency, int>(c => c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_SingleInstanceMultipleConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<ISingleInstanceAndMultipleConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<ISingleInstanceAndMultipleConcurrency, int>(c => c.Execute()));
            }
        }

        [TestMethod]
        public void Invoke_SingleInstanceReentrantConcurrency_CanHandleMultipleThreads()
        {
            using (StartService<ISingleInstanceAndReentrantConcurrency>())
            {
                ParallelInvoker.Invoke(50, () => Invoke<ISingleInstanceAndReentrantConcurrency, int>(c => c.Execute()));
            }
        }


        private ServiceHost StartService<TService>()
        {
            var container = new ServiceContainer();
            container.EnablePerWcfOperationScope();

            var serviceHost = new LightInjectServiceHostFactory().CreateServiceHost<TService>("http://localhost:6000");
            serviceHost.Open();
            return serviceHost;
        }

        private TResult Invoke<TService, TResult>(Func<TService, TResult> func)
        {            
            var calculatorFactory = new ChannelFactory<TService>(
                new BasicHttpBinding(), new EndpointAddress("http://localhost:6000"));
            TService service = calculatorFactory.CreateChannel();
            var result = func(service);
            ((IClientChannel)service).Close();
            return result;            
        }      
    }
}