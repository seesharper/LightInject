namespace LightInject.Interception.Tests
{
    internal class SampleInterceptor : IInterceptor
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
}