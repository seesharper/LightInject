namespace LightInject.Interception.Tests
{
    using System.Reflection;

    using LightInject.Interception;
    using LightInject.Interception.Tests;
    using System;

    using LightInject.Interception;
    using LightInject.Interception.Tests;
    using System;

    public class ICalculatorProxy : ICalculator, IProxy
    {
        private static readonly InterceptedMethodInfo AddInterceptedMethodInfo = new InterceptedMethodInfo(typeof(ICalculator).GetMethod("Add"));
        private Lazy<IInterceptor> addInterceptor;
        private Lazy<IInterceptor> interceptor0;
        public static Func<IInterceptor> InterceptorFactory0;
        private Lazy<ICalculator> target;
        public static Func<ICalculator> TargetFactory;

        public ICalculatorProxy(Lazy<ICalculator> lazy1)
        {
            this.InitializeProxy(lazy1);
        }

        public virtual int Add(int num1, int num2)
        {
            //return num1 + num2;
            //return
            //    (int)this.addInterceptor.Value.Invoke(new AddInvocationInfo(null, new object[] { num1, num2 }, target));
            return (int)this.addInterceptor.Value.Invoke(new InvocationInfo(AddInterceptedMethodInfo.Method, AddInterceptedMethodInfo.ProceedDelegate, this, new object[] { num1, num2 }));
        }

        public override bool Equals(object obj1)
        {
            return this.target.Value.Equals(obj1);
        }

        public override int GetHashCode()
        {
            return this.target.Value.GetHashCode();
        }

        private void InitializeProxy(Lazy<ICalculator> lazy1)
        {
            this.target = lazy1;
            this.interceptor0 = new Lazy<IInterceptor>(InterceptorFactory0);
            this.addInterceptor = MethodInterceptorFactory.CreateMethodInterceptor(new Lazy<IInterceptor>[] { this.interceptor0 });
        }

        public override string ToString()
        {
            return this.target.Value.ToString();
        }

        object IProxy.Target
        {
            get
            {
                return this.target.Value;
            }
        }

        public class AddInvocationInfo : IInvocationInfo
        {
            private readonly Lazy<ICalculator> lazyTarget;

            public AddInvocationInfo(MethodInfo method, object[] arguments, Lazy<ICalculator> lazyTarget)
            {
                this.lazyTarget = lazyTarget;

                Method = method;
                Arguments = arguments;
            }

            public MethodInfo Method { get; private set; }

            public IProxy Proxy { get; private set; }

            public object[] Arguments { get; private set; }

            public object Proceed()
            {
                return lazyTarget.Value.Add((int)Arguments[0], (int)Arguments[1]);
            }
        }
    }


    

}