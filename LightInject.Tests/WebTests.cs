using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.Tests
{
    using System.Reflection;
    using System.Web;

    using LightInject.SampleLibrary;
    using LightInject.Web;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WebTests
    {
        private static ServiceContainer serviceContainer;

        static WebTests()
        {
            serviceContainer = new ServiceContainer();
            serviceContainer.Register<IFoo, Foo>(new PerScopeLifetime());
            serviceContainer.EnablePerWebRequestScope();
        }
                
        [TestMethod]
        public void GetInstance_InsideWebRequest_ReturnsSameInstance()
        {                                                         
            var mockHttpApplication = new MockHttpApplication();                                    
            mockHttpApplication.BeginRequest();

            var firstInstance = serviceContainer.GetInstance<IFoo>();
            var secondInstance = serviceContainer.GetInstance<IFoo>();

            Assert.AreEqual(firstInstance, secondInstance);

            mockHttpApplication.EndRequest();
        }

        [TestMethod]
        public void GetInstance_TwoDifferentRequests_ReturnsNewInstances()
        {            
            var firstInstance = GetInstanceWithinWebRequest();
            var secondInstance = GetInstanceWithinWebRequest();
            
            Assert.AreNotSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_MultipleThreads_DoesNotThrowException()
        {
            ParallelInvoker.Invoke(10, () => GetInstanceWithinWebRequest());
        }


        [TestMethod]
        public void Initialize_ModuleInitializer_DoesNotThrowException()
        {
            LightInjectHttpModuleInitializer.Initialize();
        }


        private static IFoo GetInstanceWithinWebRequest()
        {
            serviceContainer.EnablePerWebRequestScope();
            var mockHttpApplication = new MockHttpApplication();
            mockHttpApplication.BeginRequest();
            IFoo firstInstance = serviceContainer.GetInstance<IFoo>();
            mockHttpApplication.EndRequest();
            mockHttpApplication.Dispose();
            return firstInstance;
        }

        public class MockHttpApplication : HttpApplication
        {
            private static readonly object EndEventHandlerKey;
            private static readonly object BeginEventHandlerKey;
            private LightInjectHttpModule module = new LightInjectHttpModule();
            
            public MockHttpApplication()
            {                
                module.Init(this);
            }

            static MockHttpApplication()
            {                
                EndEventHandlerKey = typeof(HttpApplication).GetField("EventEndRequest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                BeginEventHandlerKey = typeof(HttpApplication).GetField("EventBeginRequest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            }
           
            public new void BeginRequest()
            {
                HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));
                this.Events[BeginEventHandlerKey].DynamicInvoke(null, null);
            }

            public new void EndRequest()
            {
                this.Events[EndEventHandlerKey].DynamicInvoke(null, null);
                HttpContext.Current = null;
                
            }

            public override void Dispose()
            {
                module.Dispose();
                base.Dispose();
            }
        }
    }
}
