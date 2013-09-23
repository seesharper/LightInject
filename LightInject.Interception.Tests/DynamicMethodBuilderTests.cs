namespace LightInject.Interception.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DynamicMethodBuilderTests
    {
        [TestMethod]
        public void Execute_MethodWithNoParameters_IsInvoked()
        {
            var targetMock = new Mock<IMethodWithNoParameters>();
            var method = typeof(IMethodWithNoParameters).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { });
            
            targetMock.Verify(t => t.Execute(), Times.Once());
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeParameter_IsInvoked()
        {
            var targetMock = new Mock<IMethodWithReferenceTypeParameter>();
            var method = typeof(IMethodWithReferenceTypeParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { "SomeValue" });
            
            targetMock.Verify(t => t.Execute("SomeValue"), Times.Once());
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeParameter_IsInvoked()
        {
            var targetMock = new Mock<IMethodWithValueTypeParameter>();
            var method = typeof(IMethodWithValueTypeParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { 42 });

            targetMock.Verify(t => t.Execute(42), Times.Once());
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeOutParameter_ReturnsValueFromTarget()
        {
            var targetMock = new Mock<IMethodWithReferenceTypeOutParameter>();
            string returnValue = "AnotherValue";
            targetMock.Setup(t => t.Execute(out returnValue));
            var method = typeof(IMethodWithReferenceTypeOutParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();
            var arguments = new object[] { "SomeValue" };

            methodBuilder.CreateDelegate(method)(targetMock.Object, arguments);
            
            Assert.AreEqual("AnotherValue", (string)arguments[0]);
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeOutParameter_ReturnsValueFromTarget()
        {
            var targetMock = new Mock<IMethodWithValueTypeOutParameter>();
            int returnValue = 52;
            targetMock.Setup(t => t.Execute(out returnValue));
            var method = typeof(IMethodWithValueTypeOutParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();
            var arguments = new object[] { 42 };

            methodBuilder.CreateDelegate(method)(targetMock.Object, arguments);

            Assert.AreEqual(52, (int)arguments[0]);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeRefParameter_ReturnsValueFromTarget()
        {
            var method = typeof(IMethodWithReferenceTypeRefParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();
            var arguments = new object[] { new ReferenceTypeFoo { Value = "SomeValue" } };

            methodBuilder.CreateDelegate(method)(new MethodWithReferenceTypeRefParameter(), arguments);
            
            Assert.AreEqual("AnotherValue", ((ReferenceTypeFoo)arguments[0]).Value);
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeRefParameter_ReturnsValueFromTarget()
        {
            var method = typeof(IMethodWithValueTypeRefParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();
            var arguments = new object[] { new ValueTypeFoo() { Value = "SomeValue" } };
            
            methodBuilder.CreateDelegate(method)(new MethodWithValueTypeRefParameter(), arguments);
            
            Assert.AreEqual("AnotherValue", ((ValueTypeFoo)arguments[0]).Value);
        }

        [TestMethod]
        public void Execute_MethodWithNullableTypeParameter_PassedValueToTarget()
        {
            var targetMock = new Mock<IMethodWithNullableParameter>();
            var method = typeof(IMethodWithNullableParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { 42 });
            
            targetMock.Verify(t => t.Execute(42), Times.Once());
        }

        [TestMethod]
        public void Execute_MethodWithEnumParameter_PassedValueToTarget()
        {
            var targetMock = new Mock<IMethodWithEnumParameter>();
            var method = typeof(IMethodWithEnumParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { StringSplitOptions.RemoveEmptyEntries });

            targetMock.Verify(t => t.Execute(StringSplitOptions.RemoveEmptyEntries), Times.Once());
        }

        [TestMethod]
        public void Execute_MethodWithValueEnumOutParameter_ReturnsValueFromTarget()
        {
            var targetMock = new Mock<IMethodWithEnumOutParameter>();
            StringSplitOptions returnValue = StringSplitOptions.None;
            targetMock.Setup(t => t.Execute(out returnValue));
            var method = typeof(IMethodWithEnumOutParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();
            var arguments = new object[] { StringSplitOptions.RemoveEmptyEntries };

            methodBuilder.CreateDelegate(method)(targetMock.Object, arguments);

            Assert.AreEqual(StringSplitOptions.None, arguments[0]);
        }

        [TestMethod]
        public void Execute_MethodWithEnumRefParameter_ReturnsValueFromTarget()
        {
            var method = typeof(IMethodWithEnumRefParameter).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();
            var arguments = new object[] { StringSplitOptions.RemoveEmptyEntries };

            methodBuilder.CreateDelegate(method)(new MethodWithEnumRefParameter(), arguments);

            Assert.AreEqual(StringSplitOptions.None, arguments[0]);
        }

        [TestMethod]
        public void Execute_MethodWithReferenceTypeReturnValue_ReturnsValue()
        {
            var targetMock = new Mock<IMethodWithReferenceTypeReturnValue>();
            targetMock.Setup(t => t.Execute()).Returns("SomeValue");
            var method = typeof(IMethodWithReferenceTypeReturnValue).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            var result = methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { });

            Assert.AreEqual("SomeValue", (string)result);
        }

        [TestMethod]
        public void Execute_MethodWithValueTypeReturnValue_ReturnsValue()
        {
            var targetMock = new Mock<IMethodWithValueTypeReturnValue>();
            targetMock.Setup(t => t.Execute()).Returns(42);
            var method = typeof(IMethodWithValueTypeReturnValue).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            var result = methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { });

            Assert.AreEqual(42, (int)result);
        }

        [TestMethod]
        public void Execute_MethodWithEnumReturnValue_ReturnsValue()
        {
            var targetMock = new Mock<IMethodWithEnumReturnValue>();
            targetMock.Setup(t => t.Execute()).Returns(StringSplitOptions.RemoveEmptyEntries);
            var method = typeof(IMethodWithEnumReturnValue).GetMethods()[0];
            var methodBuilder = GetMethodBuilder();

            var result = methodBuilder.CreateDelegate(method)(targetMock.Object, new object[] { });

            Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, result);
        }

        //[TestMethod]
        //public void PerformanceTest()
        //{
        //    int iterations = 1000000;
        //    IMethodWithReferenceTypeParameter target = new MethodWithReferenceTypeParameter();
        //    Measure(() => target.Execute("SomeValue"), iterations, "Direct");

        //    var method = typeof(IMethodWithReferenceTypeParameter).GetMethods()[0];
        //    //Measure(() => method.Invoke(target, new object[]{"SomeValue"}), iterations, "Reflection");

        //    var methodInvoker = GetMethodBuilder();
        //    Measure(() => methodInvoker.Invoke(method, target, new object[] { "SomeValue" }), iterations, "MethodInvoker");

        //    Func<object, object[], object> del = GetMethodBuilder().CreateDelegate(method);
        //    Measure(() => del(target, new object[] { "someValue" }), iterations, "CachedDelegate");

        //    Lazy<Func<object, object[], object>> lazy = new Lazy<Func<object, object[], object>>(() => methodInvoker.CreateDelegate(method));
        //    Measure(() => lazy.Value(target, new object[] { "someValue" }), iterations, "LazyDelegate");

        //}

        //private void Measure(Action action, int iterations, string description)
        //{
        //    Console.Write(description);
        //    var stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    for (int i = 0; i < iterations; i++)
        //    {
        //        action();
        //    }
        //    stopwatch.Stop();
        //    Console.WriteLine(" -> ElapsedMilliSeconds: " + stopwatch.ElapsedMilliseconds);
        //}


        protected virtual IMethodBuilder GetMethodBuilder()
        {
            return new DynamicMethodBuilder();
        }
    } 
    
}