namespace LightInject.Interception.Tests
{
    using System;

    using LightInject.SampleLibrary;

    public  class SampleInterceptor : IInterceptor
    {
        private readonly bool doInvoke;

        //public SampleInterceptor(bool doInvoke)
        //{
        //    this.doInvoke = doInvoke;
        //}

        public object Invoke(IInvocationInfo invocationInfo)
        {            
            return invocationInfo.Proceed();
        }
    }

    internal class AnotherInterceptor : IInterceptor
    {
        private readonly bool doInvoke;

        //public SampleInterceptor(bool doInvoke)
        //{
        //    this.doInvoke = doInvoke;
        //}

        public object Invoke(IInvocationInfo invocationInfo)
        {
            //return invocationInfo.Proceed();
            return null;
        }
    }

    public class InterceptorWithDependency : IInterceptor
    {
        public IBar Bar { get; private set; }

        public InterceptorWithDependency(IBar bar)
        {
            Bar = (IBar)((IProxy)bar).Target;
        }

        public object Invoke(IInvocationInfo invocationInfo)
        {
            Bar.ToString();
            return invocationInfo.Proceed();
        }
    }
}