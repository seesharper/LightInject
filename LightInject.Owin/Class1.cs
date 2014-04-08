using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Owin
{
    using global::Owin;

    using Microsoft.Owin;

    internal class LightInjectMiddleware : OwinMiddleware
    {
        private readonly IServiceContainer container;

        public LightInjectMiddleware(IServiceContainer container, OwinMiddleware next)
            : base(next)
        {
            this.container = container;
        }

        public async override Task Invoke(IOwinContext context)
        {
            using (container.BeginScope())
            {
                await Next.Invoke(context);
            }
        }
    }

    internal static class LightInjectOwinExtensions
    {
        public static void UseLightInject(this IAppBuilder appBuilder, IServiceContainer container)
        {
            appBuilder.Use<LightInjectMiddleware>();
        }
    }
}
