namespace LightInject.Interception.Tests
{
    using System;

    using LightInject.SampleLibrary;

    public  class SampleInterceptor : IInterceptor
    {        
        public object Invoke(IInvocationInfo invocationInfo)
        {            
            return invocationInfo.Proceed();
        }
    }

    internal class AnotherInterceptor : IInterceptor
    {
        

        public object Invoke(IInvocationInfo invocationInfo)
        {
        
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