namespace LightInject.Interception.Tests
{
    using System;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PerformanceTests
    {
        
        [TestMethod]
        public void RunManual()
        {
            int iterations = 100;
            IInterceptor interceptor = new SampleInterceptor();
            ICalculatorProxy.InterceptorFactory0 = () => interceptor;
            ICalculatorProxy.TargetFactory = () => new Calculator();
            
            Func<ICalculator> targetFactory = () => new Calculator();

            var proxy = new ICalculatorProxy(new Lazy<ICalculator>(targetFactory));
            Measure(() => proxy.Add(42,42), iterations, "Manual just add");

            Measure(() => {var p = new ICalculatorProxy(new Lazy<ICalculator>(targetFactory)); p.Add(42,42);}, iterations, "Create and add");


            Measure(() => Create(targetFactory), iterations, "Just Create");
        }

        private void Create(Func<ICalculator> targetFactory)
        {
            var p = new ICalculatorProxy(new Lazy<ICalculator>(targetFactory));

        }

        
        
        [TestMethod]
        public void Run()
        {
            int iterations = 1000000;
            var method = typeof(ICalculator).GetMethods()[0];

            ICalculator target = new Calculator();
            //Measure(() => target.Add(42,42), iterations, "Direct");

            var methodBuilder = GetMethodBuilder();

            Lazy<Func<object, object[], object>> lazy = new Lazy<Func<object, object[], object>>(() => methodBuilder.CreateDelegate(method));
            //Measure(() => lazy.Value(target, new object[] { 42, 42 }), iterations, "LazyDelegate");


            var proxyBuilder = new ProxyBuilder();
            ProxyDefinition proxyDefinition = new ProxyDefinition(typeof(ICalculator), () => new Calculator());
            IInterceptor interceptor = new SampleInterceptor();
            proxyDefinition.Implement(m => m.Name == "Add", () => interceptor);
            var proxyType = proxyBuilder.GetProxyType(proxyDefinition);
            var instance = (ICalculator)Activator.CreateInstance(proxyType);
            //Measure(() => instance.Add(42, 42), iterations, "Proxy");


            DynamicMethod dynamicMethod = new DynamicMethod("TEST", typeof(ICalculator), Type.EmptyTypes, typeof(ICalculator).Module);
            var il = dynamicMethod.GetILGenerator();

            var ctor = proxyType.GetConstructor(Type.EmptyTypes);

            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);

            var createInstanceDelegate = (Func<ICalculator>)dynamicMethod.CreateDelegate(typeof(Func<ICalculator>));

            Measure(() => createInstanceDelegate().Add(42,42), iterations, "New proxy");

        }

        private IMethodBuilder GetMethodBuilder()
        {
            return new DynamicMethodBuilder();
        }

        private void Measure(Action action, int iterations, string description)
        {
            Console.Write(description);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }
            stopwatch.Stop();
            Console.WriteLine(" -> ElapsedMilliSeconds: " + stopwatch.ElapsedMilliseconds);
        } 
    }


}