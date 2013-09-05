namespace LightInject.Interception.Tests
{
    internal class SampleInterceptor : IInterceptor
    {
        public object Invoke(InvocationInfo invocationInfo)
        {
            return invocationInfo.Proceed();
        }
    }
}