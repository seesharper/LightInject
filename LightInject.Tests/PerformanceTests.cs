namespace LightInject.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Text;
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class PerformanceTests
    {
        private const int Iterations = 2000000;

        private IServiceContainer container;

        [TestMethod]
        public void NewOperatorVersusServiceContainer()
        {
            this.MeasureMethod(() => this.RunNewOperator());
            this.MeasureMethod(() => this.RunServiceContainer());            
        }

        private void RunServiceContainer()
        {            
            for (int i = 0; i < Iterations; i++)
            {
                var foo = container.GetInstance<IFoo>();
            }
        }

        private void RunNewOperator()
        {            
            for (int i = 0; i < Iterations; i++)
            {
                var foo = new Foo();
            }            
        }
        
        private void MeasureMethod(Expression<Action> action)
        {
            var compiled = action.Compile();
            Stopwatch stopwatch = Stopwatch.StartNew();
            compiled();
            stopwatch.Stop();
            Console.WriteLine("{0}: {1}", ((MethodCallExpression)action.Body).Method.Name, stopwatch.ElapsedMilliseconds);
        }

        [TestInitialize]
        public void InitializeTests()
        {
            container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            //container.GetInstance<IFoo>();
        }
    }
}