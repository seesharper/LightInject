namespace LightInject.Interception.Tests
{
    internal class SampleInterceptor : IInterceptor
    {
        public object Invoke(IInvocationInfo invocationInfo)
        {            
            return invocationInfo.Proceed();
        }
    }
}