using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.Web
{
    using System.Net;
    using System.Web;

    internal class PerWebRequestLifetime : ILifetime
    {
        private readonly ThreadSafeDictionary<HttpWebRequest, object> instances = new ThreadSafeDictionary<HttpWebRequest, object>();

        public object GetInstance(Func<object> createInstance, ResolutionContext resolutionContext)
        {
            HttpContext.Current.ApplicationInstance.EndRequest
        }
    }
}
