namespace LightInject.Tests
{
    using System.Web.Http;

    using LightInject.SampleLibrary;

    public class SampleApiController : ApiController
    {
        private readonly string[] values;

        public SampleApiController(string[] values)
        {
            this.values = values;
        }

        [SampleWebApiActionFilter]
        public string Get(int id)
        {
            return values[id];
        }
    }


    public class AnotherSampleApiController : ApiController
    {
        private readonly string[] values;

        //public AnotherSampleApiController(string[] values)
        //{
        //    this.values = values;
        //}

        [WebApiActionFilterWithFuncDependency]
        public string Get(int id)
        {
            //return values[id];
            return "42";
        }
    }



    public class FuncApiController : ApiController
    {
        
        
        
        [WebApiActionFilterWithFuncDependency]
        public string Get(int value)
        {
            return value.ToString();
        }
    }
}