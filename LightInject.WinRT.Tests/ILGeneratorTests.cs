namespace LightInject.WinRT.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    [TestClass]
    public class ILGeneratorTests
    {
        [TestMethod]
        public void TestMethod4()
        {
            ConstructorInfo barConstructorInfo = typeof(Bar).GetTypeInfo().DeclaredConstructors.First();
            ConstructorInfo fooConstructorInfo = typeof(FooWithDependency).GetTypeInfo().DeclaredConstructors.First();            
            var parameterExpression = Expression.Parameter(typeof(object[]), "constants");
            var generator = new ILGenerator(parameterExpression);
            generator.Emit(OpCodes.Newobj, barConstructorInfo);
            generator.Emit(OpCodes.Newobj, fooConstructorInfo);
            Expression body = generator.CurrentExpression;

            var lambda = Expression.Lambda<Func<object[], object>>(Expression.Block(body), parameterExpression);
            var del = lambda.Compile();

            var instance = del(new object[] { });

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestMethod5()
        {
            
            ConstructorInfo barConstructorInfo = TypeHelper.GetConstructors(typeof(Bar)).First();
            ConstructorInfo fooConstructorInfo = TypeHelper.GetConstructors(typeof(Foo)).First();
            DynamicMethodSkeleton dynamicMethodSkeleton = new DynamicMethodSkeleton();
            var generator = dynamicMethodSkeleton.GetILGenerator();
            generator.Emit(OpCodes.Newobj, barConstructorInfo);
            generator.Emit(OpCodes.Newobj, fooConstructorInfo);            
            var del = dynamicMethodSkeleton.CreateDelegate();
            var instance = del(new object[] { });
            Assert.IsNotNull(instance);

            
        }

        [TestMethod]
        public void TestMethod6()
        {

            
            
            DynamicMethodSkeleton dynamicMethodSkeleton = new DynamicMethodSkeleton();
            Action<IMethodSkeleton> firstEmitter =
                skeleton =>
                    {
                        ConstructorInfo barConstructorInfo = typeof(Bar).GetConstructors().First();
                        skeleton.GetILGenerator().Emit(OpCodes.Newobj, barConstructorInfo);
                    };
            Action<IMethodSkeleton> secondEmitter =
                skeleton =>
                    {
                        ConstructorInfo fooConstructorInfo = typeof(Foo).GetConstructors().First();
                        skeleton.GetILGenerator().Emit(OpCodes.Newobj, fooConstructorInfo);
                    };

            Action<IMethodSkeleton> doit = skeleton =>
                {
                    firstEmitter(dynamicMethodSkeleton);
                    secondEmitter(dynamicMethodSkeleton);
                };

            doit(dynamicMethodSkeleton);

            var del = dynamicMethodSkeleton.CreateDelegate();
            var instance = del(new object[] { });
            Assert.IsNotNull(instance);

            
        }

        [TestMethod]
        public void test()
        {
            
            var container = new ServiceContainer();            
            container.Register<IFoo, FooWithDependency>();
            container.Register<IBar, Bar>();
            try
            {
                container.GetInstance<IFoo>();  
            }
            catch (Exception)
            {                
             
            }
            container.GetInstance<IFoo>();  
          
            
        }
    }
}