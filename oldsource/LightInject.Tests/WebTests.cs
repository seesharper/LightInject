using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.Tests
{
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;

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
            var mockHttpApplication = new MockHttpApplication(new LightInjectHttpModule());                                    
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

        [TestMethod]
        public void GetInstance_WithoutBeginRequest_ThrowsMeaningfulException()
        {
            var mockHttpApplication = new MockHttpApplication(null);        
            mockHttpApplication.BeginRequest();            
            ExceptionAssert.Throws<InvalidOperationException>(
                () => serviceContainer.GetInstance<IFoo>(),
                e => e.ToString().Contains("Unable to locate a scope manager for the current HttpRequest."));
        }

        private static IFoo GetInstanceWithinWebRequest()
        {
            serviceContainer.EnablePerWebRequestScope();
            var mockHttpApplication = new MockHttpApplication(new LightInjectHttpModule());
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

            private static readonly FieldInfo ContextField;
            private readonly IHttpModule module;
            
            public MockHttpApplication(IHttpModule module) 
            {
                if (module != null)
                {
                    module.Init(this);
                }
                this.module = module;
                
            }
           
            static MockHttpApplication()
            {                
                EndEventHandlerKey = typeof(HttpApplication).GetField("EventEndRequest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                BeginEventHandlerKey = typeof(HttpApplication).GetField("EventBeginRequest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                ContextField = typeof(HttpApplication).GetField(
                    "_context",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            }
           
            public new void BeginRequest()
            {
                HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));
                SetContext(HttpContext.Current);
                if (module != null)
                {
                    this.Events[BeginEventHandlerKey].DynamicInvoke(this, null);
                }
            }

            private void SetContext(HttpContext context)
            {
                ContextField.SetValue(this, context);
            }


            public new void EndRequest()
            {
                if (module != null)
                {
                    this.Events[EndEventHandlerKey].DynamicInvoke(this, null);
                }                                
                HttpContext.Current = null;
                SetContext(null);
            }

            public override void Dispose()
            {
                if (module != null)
                {
                    module.Dispose();
                }               
                base.Dispose();
            }
        }
    }
}
