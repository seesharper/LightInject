namespace LightInject.Tests
{
    using System;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OpCodeTests
    {
        [TestMethod]     
        public void Equals_SameOpCodes_AreEqual()
        {
            OpCode first = OpCodes.Ldarg;
            OpCode second = OpCodes.Ldarg;

            Assert.IsTrue(first == second);
        }

        [TestMethod]
        public void Equals_DifferentOpCodes_AreNotEqual()
        {
            OpCode first = OpCodes.Ldarg;
            OpCode second = OpCodes.Ldc_I4;

            Assert.IsTrue(first != second);
        }
        
        [TestMethod]
        public void GetHashCode_OpCode_ReturnsHashCodeFromValue()
        {
            Assert.AreEqual(OpCodes.Ldarg.Value.GetHashCode(), OpCodes.Ldarg.GetHashCode());
        }

        [TestMethod]
        public void Emit_UnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode));
        }

        [TestMethod]
        public void Emit_ConstructorInfoWithUnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode, (ConstructorInfo)null));
        }

        [TestMethod]
        public void Emit_IntWithUnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode, 42));
        }

        [TestMethod]
        public void Emit_LocalBuilderWithUnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode, (LocalBuilder)null));
        }

        [TestMethod]
        public void Emit_MethodInfoWithUnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode, (MethodInfo)null));
        }

        [TestMethod]
        public void Emit_StringWithUnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode, (string)null));
        }

        [TestMethod]
        public void Emit_TypeWithUnknownOpCode_ThrowsNotSupportedException()
        {
            var ilGenerator = new ILGenerator(null);
            var opCode = new OpCode(-1);

            ExceptionAssert.Throws<NotSupportedException>(() => ilGenerator.Emit(opCode, (Type)null));
        }
    }
}