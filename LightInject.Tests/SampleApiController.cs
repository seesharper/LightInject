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
}