using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using LightInject;
    using LightInject.Interception;

    class Program
    {
        static void Main(string[] args)
        {
            var container = new ServiceContainer();
            container.Register<Foo>();
            container.Intercept(sr => sr.ServiceType == typeof(Foo), factory => new SampleInterceptor());

            var foo = container.GetInstance<Foo>();

            foo.A();
        }
    }


    public class Foo
    {
        public virtual void A()
        {
        }
    }


    public class SampleInterceptor : IInterceptor
    {
        public object Invoke(IInvocationInfo invocationInfo)
        {
            Console.WriteLine("Invoke");
            return null;
        }
    }

}
