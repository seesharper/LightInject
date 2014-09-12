namespace LightInject.Tests
{
    using System;
    using System.Diagnostics;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using LightInject.SampleLibrary;

    public class SampleWebApiActionFilter : ActionFilterAttribute
    {
        public string Value
        {
            set
            {
                StaticValue = value;
            }
            get
            {
                return null;
            }
        }

        public static string StaticValue { get; set; }
        

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
        }
    }


    public class WebApiActionFilterWithFuncDependency : ActionFilterAttribute
    {
        public Func<IFoo> Value
        {
            set
            {
                StaticValue = value;
            }
            get
            {
                return null;
            }
        }

        public static Func<IFoo> StaticValue { get; set; }
    }
}