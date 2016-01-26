namespace LightInject.Tests
{
    using System.Web.Mvc;

    using LightInject.SampleLibrary;

    public class SampleController : Controller
    {
        [SampleFilter]        
        public void Execute()
        {
             
        }
    }

    public class SampleFilterAttribute : ActionFilterAttribute
    {
        public IFoo Foo { get; set; }
    }
}