namespace LightInject.Interception.Tests
{
    using System;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PerformanceTests
    {
        
       
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