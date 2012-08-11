namespace LightInject.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class PerformanceTests
    {
        private const int Iterations = 2000000;
        private Func<List<object>, object> dynamicMethodDelegate;
        private Func<object> expressionDelegate; 
        private IServiceContainer container;
        private ConcurrentDictionary<Type,Func<List<object>,object>> concurrentDictionary = new ConcurrentDictionary<Type, Func<List<object>, object>>();
        private ConcurrentDictionary<Type, Lazy<Func<List<object>, object>>> lazyConcurrentDictionary = new ConcurrentDictionary<Type, Lazy<Func<List<object>, object>>>();
        private Dictionary<Type,Func<List<object>,object>> regularDictionary = new Dictionary<Type, Func<List<object>, object>>();
        private DelegateRegistry<Type> delegateRegistry = new DelegateRegistry<Type>();
        private List<object> constants = new List<object>();
        [TestMethod]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void NewOperatorVersusServiceContainer()
        {
            this.MeasureMethod(() => this.RunNewOperator());
            this.MeasureMethod(() => this.RunServiceContainer());
            this.MeasureMethod(() => this.RunDynamicMethodDelegate());
            this.MeasureMethod(() => this.RunExpressionDelegate());
            this.MeasureMethod(() => this.RunRegularTryGetValue());
            this.MeasureMethod(() => this.RunConcurrentTryGetValue());
            this.MeasureMethod(() => this.RunLazyConcurrentTryGetValue());
            this.MeasureMethod(() => this.RunConcurrentGetOrAdd());
            this.MeasureMethod(() => this.RunDelegateRegistry());
        }

        private void RunServiceContainer()
        {            
            for (int i = 0; i < Iterations; i++)
            {
                var foo = container.GetInstance<IFoo>();                
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RunNewOperator()
        {            
            for (int i = 0; i < Iterations; i++)
            {
                var foo = (IFoo)(object)new Foo();
                if (foo == null)
                    Console.WriteLine();
            }            
        }
        
        private void RunDynamicMethodDelegate()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var instance = (IFoo)dynamicMethodDelegate(constants);
            }            
        }

        private void RunExpressionDelegate()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var instance = (IFoo)expressionDelegate();
            }            
        }

        private void RunRegularTryGetValue()
        {
            for (int i = 0; i < Iterations; i++)
            {
                Func<List<object>, object> del;
                regularDictionary.TryGetValue(typeof(IFoo), out del);
            }            
        }

        private void RunConcurrentTryGetValue()
        {
            for (int i = 0; i < Iterations; i++)
            {
                Func<List<object>, object> del;
                concurrentDictionary.TryGetValue(typeof(IFoo), out del);
            }
        }

        private void RunDelegateRegistry()
        {          
            for (int i = 0; i < Iterations; i++)
            {
                var del = delegateRegistry.GetOrAdd(typeof(IFoo), t => new Lazy<Func<List<object>, object>>(() => this.CreateDelegate(t)));
                del.Value(constants);
            }
        }

        private void RunConcurrentGetOrAdd()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var del = concurrentDictionary.GetOrAdd(typeof(IFoo), this.CreateDelegate);
            }
        }

        private Func<List<object>, object> CreateDelegate(Type type)
        {
            return dynamicMethodDelegate;
        }


        private void RunLazyConcurrentTryGetValue()
        {
            for (int i = 0; i < Iterations; i++)
            {
                Lazy<Func<List<object>, object>> lazy;
                lazyConcurrentDictionary.TryGetValue(typeof(IFoo), out lazy);
                var del = lazy.Value;
            }
        }

        private void MeasureMethod(Expression<Action> action)
        {
            CollectMemory();
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
            container.GetInstance<IFoo>();
            this.CreateDynamicMethod();
            this.CreateExpressionDelegate();
        }

        private void CreateDynamicMethod()
        {
            var dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(List<object>) }, typeof(ServiceContainer).Module, false);
            var constructorInfo = typeof(Foo).GetConstructor(Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            dynamicMethodDelegate = (Func<List<object>, object>)dynamicMethod.CreateDelegate(typeof(Func<List<object>, object>));
            concurrentDictionary.TryAdd(typeof(IFoo), dynamicMethodDelegate);
            concurrentDictionary.TryAdd(typeof(IBar), dynamicMethodDelegate);
            regularDictionary.Add(typeof(IFoo), dynamicMethodDelegate);
            regularDictionary.Add(typeof(IBar), dynamicMethodDelegate);
            lazyConcurrentDictionary.TryAdd(typeof(IFoo), new Lazy<Func<List<object>, object>>(() => dynamicMethodDelegate));
            lazyConcurrentDictionary.TryAdd(typeof(IBar), new Lazy<Func<List<object>, object>>(() => dynamicMethodDelegate));
            delegateRegistry.TryAdd(typeof(IBar), new Lazy<Func<List<object>, object>>(() => dynamicMethodDelegate));
            delegateRegistry.TryAdd(typeof(IFoo), new Lazy<Func<List<object>, object>>(() => dynamicMethodDelegate));
        }
        
        private void CreateExpressionDelegate()
        {
            var constructorInfo = typeof(Foo).GetConstructor(Type.EmptyTypes);
            var newExpression = Expression.New(constructorInfo);
            var lambda = Expression.Lambda<Func<object>>(newExpression);
            expressionDelegate = lambda.Compile();
        }

        private static void CollectMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Do this a second time to ensure finalizable objects that survived the previous collect are now
            // collected.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

  

    class ThreadSafeDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public ThreadSafeDictionary()
        {
        }

        public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }
    }

    class DelegateRegistry<TKey> : ThreadSafeDictionary<TKey, Lazy<Func<List<object>, object>>>
    {
    }
}