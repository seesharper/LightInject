using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReflectionBasedDynamicMethod
{
    using System.Reflection;
    using System.Reflection.Emit;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new[] { typeof(object[]), typeof(object) });

            var generator = dynamicMethod.GetILGenerator();

            var constructor = typeof(Foo).GetConstructor(Type.EmptyTypes);

            generator.Emit(OpCodes.Newobj, constructor);

            var func = (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));

            var instance = func(null);

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }



        public class Foo
        {
            public Foo()
            {
            }
        }
    }
}
