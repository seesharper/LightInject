namespace LightInject.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;
    using LightInject;
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class PerformanceTests
    {
        private const int Iterations = 5000000;
        private Func<object[], object> dynamicMethodDelegate;

        public delegate object TestDelegate(object[] contants);

        private TestDelegate myDelegate;

        private IFactory factory;

        private IFactory typedFactory = new TypedFactory();

        private Func<object> expressionDelegate; 
        private ServiceContainer container;
        private ConcurrentDictionary<Type,Func<List<object>,object>> concurrentDictionary = new ConcurrentDictionary<Type, Func<List<object>, object>>();
        private ConcurrentDictionary<Type, Lazy<Func<List<object>, object>>> lazyConcurrentDictionary = new ConcurrentDictionary<Type, Lazy<Func<List<object>, object>>>();
        private Dictionary<Type,Func<List<object>,object>> regularDictionary = new Dictionary<Type, Func<List<object>, object>>();
        private DelegateRegistry<Type> delegateRegistry = new DelegateRegistry<Type>();
        private List<object> constants = new List<object>();
        [TestMethod]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void NewOperatorVersusServiceContainer()
        {
            this.MeasureMethod(() => this.RunDynamicMethodDelegate());                        
            this.MeasureMethod(() => this.RunNewOperator());
            this.MeasureMethod(() => this.RunFactory());
            this.MeasureMethod(() => this.RunTypedFactory());
            this.MeasureMethod(() => this.RunStaticDelegate());


            //this.MeasureMethod(() => this.RunDynamicMethodDelegateWithDelegate());
            this.MeasureMethod(() => this.RunServiceContainer());
            //this.MeasureMethod(() => this.RunDynamicMethodDelegate());
            //this.MeasureMethod(() => this.TypeGetHashCode());
            //this.MeasureMethod(() => this.RuntimehelperGetHashCode());
            //this.MeasureMethod(() => this.RunExpressionDelegate());
            //this.MeasureMethod(() => this.RunRegularTryGetValue());
            //this.MeasureMethod(() => this.RunConcurrentTryGetValue());
            //this.MeasureMethod(() => this.RunLazyConcurrentTryGetValue());
            //this.MeasureMethod(() => this.RunConcurrentGetOrAdd());
            //this.MeasureMethod(() => this.RunDelegateRegistry());
        }

        private void RunServiceContainer()
        {            
            for (int i = 0; i < Iterations; i++)
            {
                //var foo = container.GetInstance<IFoo>();                
                var foo = container.GetInstance(typeof(IFoo));                
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RunNewOperator()
        {            
            for (int i = 0; i < Iterations; i++)
            {
                var foo = new Foo();               
            }            
        }

        private void RunStaticDelegate()
        {
            var arr = constants.ToArray();
            for (int i = 0; i < Iterations; i++)
            {
                var foo = myDelegate(arr);
            }
        }


        private void RunFactory()
        {
            var arr = constants.ToArray();
            for (int i = 0; i < Iterations; i++)
            {
                var foo = (IFoo)factory.GetInstance(arr);
            }
        }

        private void RunTypedFactory()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var foo = (IFoo)typedFactory.GetInstance(new object[] { });
            }
        }

        
        private void RunDynamicMethodDelegate()
        {
            var arr = constants.ToArray();
            for (int i = 0; i < Iterations; i++)
            {
                var instance = (IFoo)dynamicMethodDelegate(arr);
            }            
        }


        private void RunDynamicMethodDelegateWithDelegate()
        {
            var arr = constants.ToArray();
            for (int i = 0; i < Iterations; i++)
            {
                var instance = (IFoo)myDelegate(arr);
            }
        }


        private void RunExpressionDelegate()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var instance = (IFoo)expressionDelegate();
            }            
        }

        private void TypeGetHashCode()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var hashCode = typeof(IFoo).GetHashCode();
            }            
        }

        private void RuntimehelperGetHashCode()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var hashCode = RuntimeHelpers.GetHashCode(typeof(IFoo));
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
            //for (int i = 0; i < Iterations; i++)
            //{
            //    var del = delegateRegistry.GetOrAdd(typeof(IFoo), t => new Lazy<Func<List<object>, object>>(() => this.CreateDelegate(t)));
            //    del.Value(constants);
            //}
        }

        private void RunConcurrentGetOrAdd()
        {
            //for (int i = 0; i < Iterations; i++)
            //{
            //    var del = concurrentDictionary.GetOrAdd(typeof(IFoo), this.CreateDelegate);
            //}
        }

        private Func<object[], object> CreateDelegate(Type type)
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
            this.CreateSimpleDynamicMethodMethod();
            this.CreateDynamicMethodWithDelegate();
            this.CreateFactory();
            this.CreateStaticFactory();
        }

        private void CreateDynamicMethod()
        {
            var dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(object[]) }, typeof(ServiceContainer).Module, true);
            var constructorInfo = typeof(Foo).GetConstructor(Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            dynamicMethodDelegate = (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));          
        }

        private void CreateDynamicMethodWithDelegate()
        {
            var dynamicMethod = new DynamicMethod(
                    "DynamicMethod", typeof(object), new[] { typeof(object[]) }, typeof(ServiceContainer).Module, false);
            var constructorInfo = typeof(Foo).GetConstructor(Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            myDelegate = (TestDelegate)dynamicMethod.CreateDelegate(typeof(TestDelegate));
        }

        private void CreateSimpleDynamicMethodMethod()
        {
            var dynamicMethod = new DynamicMethod("DynamicMethod", typeof(Foo), Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, typeof(Foo).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Ret);
            var del = (Func<Foo>)dynamicMethod.CreateDelegate(typeof(Func<Foo>));
            var type = del.GetType();
        }



        private void CreateExpressionDelegate()
        {
            var constructorInfo = typeof(Foo).GetConstructor(Type.EmptyTypes);
            var newExpression = Expression.New(constructorInfo);
            var lambda = Expression.Lambda<Func<Foo>>(newExpression);
            expressionDelegate = lambda.Compile();
            var type = expressionDelegate.GetType();
        }

        private void CreateFactory()
        {
            ModuleBuilder moduleBuilder = DefineDynamicModuleBuilder();

            TypeBuilder typeBuilder = moduleBuilder.DefineType("MyClass", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, typeof(Object));

            typeBuilder.AddInterfaceImplementation(typeof(IFactory));

            {
                ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.HasThis,
                    Type.EmptyTypes);
                var constructorGenerator = constructorBuilder.GetILGenerator();
                constructorGenerator.Emit(OpCodes.Ldarg_0);
                ConstructorInfo objectConstructor = typeof(object).GetConstructor(new Type[] { });
                constructorGenerator.Emit(OpCodes.Call, objectConstructor);
                constructorGenerator.Emit(OpCodes.Ret);

                MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                    "GetInstance",
                    MethodAttributes.Virtual | MethodAttributes.Public,
                    typeof(object),
                    new Type[] { typeof(object[]) });

                var generator = methodBuilder.GetILGenerator();
                generator.Emit(OpCodes.Newobj, typeof(Foo).GetConstructor(Type.EmptyTypes));                
                generator.Emit(OpCodes.Ret);

                Type type = typeBuilder.CreateType();
                factory = (IFactory)Activator.CreateInstance(type);
            }
        }

        private void CreateStaticFactory()
        {
            ModuleBuilder moduleBuilder = DefineDynamicModuleBuilder();

            var typeBuilder = moduleBuilder.DefineType("Factory", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);
            
            var methodBuilder = typeBuilder.DefineMethod("GetService", MethodAttributes.Public | MethodAttributes.Static,
                typeof(object), new[] { typeof(object[])});

            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Newobj, typeof(Foo).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Ret);

            var dynamicType = typeBuilder.CreateType();

            myDelegate = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), dynamicType.GetMethod("GetService"));
        }



        private static ModuleBuilder DefineDynamicModuleBuilder()
        {
            var assemblyName = new AssemblyName("LightInject.DynamicAssembly");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName,
                AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            return moduleBuilder;
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

    public interface IFactory
    {
        object GetInstance(object[] arguments);
    }


    public class TypedFactory : IFactory
    {
        public object GetInstance(object[] arguments)
        {
            return new Foo();
        }
    }
}