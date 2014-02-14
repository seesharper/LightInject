namespace LightInject.Tests
{
    using System.Diagnostics;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

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
}