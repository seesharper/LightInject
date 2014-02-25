namespace LightInject.Tests
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EmitterTests
    {
        [TestMethod]
        public void Push_Zero_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            emitter.Push(0);
            emitter.Return();
            Assert.AreEqual(OpCodes.Ldc_I4_0, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_One_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(1);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_1, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Two_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(2);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_2, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Three_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(3);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_3, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Four_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(4);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_4, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Five_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(5);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_5, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Six_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(6);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_6, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Seven_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(7);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_7, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Eigth_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(8);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_8, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_Nine_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(9);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_SignedByteMaxValue_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(sbyte.MaxValue);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_SignedByteMinValue_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(sbyte.MinValue);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_SignedByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(sbyte.MaxValue + 1);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Push_SignedByteMinValueMinusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);

            emitter.Push(sbyte.MinValue - 1);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldc_I4, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_Zero_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = 0;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldarg_0, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_One_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = 1;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldarg_1, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_Two_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = 2;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldarg_2, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_Three_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = 3;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldarg_3, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_Four_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = 4;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldarg_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_ByteMaxValue_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = byte.MaxValue;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldarg_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushArgument_ByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = byte.MaxValue + 1;
            var emitter = new Emitter(CreateDummyGenerator(), CreateArgumentArray(ArgumentNumber));

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();
            
            Assert.AreEqual(OpCodes.Ldarg, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushVariable_One_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 1);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc_0, emitter.Instructions[0].Code);            
        }

        [TestMethod]
        public void PushVariable_Two_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 2);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc_1, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushVariable_Three_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 3);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc_2, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushVariable_Four_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 4);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc_3, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushVariable_Five_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 5);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushVariable_ByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 1);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc_S, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void PushVariable_ByteMaxValuePlusTwo_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 2);
            
            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Ldloc, emitter.Instructions[0].Code);
        }

        [TestMethod]
        public void Store_One_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 1);
            
            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc_0, emitter.Instructions[1].Code);
        }

        [TestMethod]
        public void Store_Two_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 2);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc_1, emitter.Instructions[1].Code);
        }

        [TestMethod]
        public void Store_Three_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 3);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc_2, emitter.Instructions[1].Code);
        }

        [TestMethod]
        public void Store_Four_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 4);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc_3, emitter.Instructions[1].Code);
        }

        [TestMethod]
        public void Store_Five_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, 5);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc_S, emitter.Instructions[1].Code);
        }

        [TestMethod]
        public void Store_ByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 1);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc_S, emitter.Instructions[1].Code);
        }

        [TestMethod]
        public void Store_ByteMaxValuePlusTwo_EmitsMostEffectiveInstruction()
        {
            var emitter = new Emitter(CreateDummyGenerator(), Type.EmptyTypes);
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 2);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.AreEqual(OpCodes.Stloc, emitter.Instructions[1].Code);
        }


        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithInteger_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, 42);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithString_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, "SomeValue");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithLocalBuilder_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, (LocalBuilder)null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithSignedByte_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, (sbyte)42);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithByte_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, (byte)42);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithType_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithMethodInfo_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, (MethodInfo)null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCodeWithConstructorInfo_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Ldarg_0, (ConstructorInfo)null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Emit_InvalidOpCode_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, Type.EmptyTypes);
            emitter.Emit(OpCodes.Xor);
        }

        private ILGenerator CreateDummyGenerator()
        {
            return new DynamicMethod(string.Empty, typeof(object), new Type[]{typeof(object[])}).GetILGenerator();
        }

        private LocalBuilder[] CreateLocalBuilders(IEmitter emitter, int count)
        {
            var localBuilders = new LocalBuilder[count];
            
            for (int i = 0; i < count; i++)
            {
                localBuilders[i] = emitter.DeclareLocal(typeof(int));
            }

            return localBuilders;
        }


        private Type[] CreateArgumentArray(int parameterCount)
        {
            Type[] parameterTypes = new Type[parameterCount + 1];
            for (int i = 0; i < parameterCount + 1; i++)
            {
                parameterTypes[i] = typeof(object);
            }
            return parameterTypes;
        }


    }

    

}