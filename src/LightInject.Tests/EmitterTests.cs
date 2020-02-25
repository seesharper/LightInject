namespace LightInject.Tests
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using Xunit;


#if NETCOREAPP1_1
    using LocalBuilder = LightInject.LocalBuilder;
    using ILGenerator = LightInject.ILGenerator;
#endif


    public class EmitterTests
    {

        [Fact]
        public void Push_Zero_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            emitter.Push(0);
            emitter.Return();
            Assert.Equal(OpCodes.Ldc_I4_0, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_One_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(1);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_1, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Two_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(2);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_2, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Three_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(3);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_3, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Four_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(4);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_4, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Five_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(5);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_5, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Six_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(6);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_6, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Seven_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(7);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_7, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Eigth_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(8);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_8, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_Nine_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(9);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_SignedByteMaxValue_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(sbyte.MaxValue);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_SignedByteMinValue_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(sbyte.MinValue);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_SignedByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(sbyte.MaxValue + 1);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Push_SignedByteMinValueMinusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();

            emitter.Push(sbyte.MinValue - 1);
            emitter.Return();

            Assert.Equal(OpCodes.Ldc_I4, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushArgument_Zero_EmitsMostEffectiveInstruction()
        {
            const int ArgumentNumber = 0;
            var emitter = CreateEmitter(ArgumentNumber);

            emitter.PushArgument(ArgumentNumber);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg_0, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushArgument_One_EmitsMostEffectiveInstruction()
        {
            const int ArgumentCount = 1;
            var emitter = CreateEmitter(ArgumentCount);

            emitter.PushArgument(ArgumentCount);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg_1, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushArgument_Two_EmitsMostEffectiveInstruction()
        {
            const int ArgumentCount = 2;
            var emitter = CreateEmitter(ArgumentCount);

            emitter.PushArgument(ArgumentCount);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg_2, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushArgument_Three_EmitsMostEffectiveInstruction()
        {
            const int ArgumentCount = 3;
            var emitter = CreateEmitter(ArgumentCount);

            emitter.PushArgument(ArgumentCount);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg_3, emitter.Instructions[0].Code);
        }

        private Emitter CreateEmitter(int ArgumentNumber)
        {
            Type[] arguments = CreateArgumentArray(ArgumentNumber);
            var emitter = new Emitter(CreateDummyGenerator(arguments), arguments);
            return emitter;
        }

        private Emitter CreateEmitter()
        {
            Type[] arguments = CreateArgumentArray(0);
            var emitter = new Emitter(CreateDummyGenerator(arguments), arguments);
            return emitter;
        }

        [Fact]
        public void PushArgument_Four_EmitsMostEffectiveInstruction()
        {
            const int ArgumentCount = 4;
            var emitter = CreateEmitter(ArgumentCount);

            emitter.PushArgument(ArgumentCount);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushArgument_ByteMaxValue_EmitsMostEffectiveInstruction()
        {
            const int ArgumentCount = byte.MaxValue;
            var emitter = CreateEmitter(ArgumentCount);

            emitter.PushArgument(ArgumentCount);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushArgument_ByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            const int ArgumentCount = byte.MaxValue + 1;
            var emitter = CreateEmitter(ArgumentCount);

            emitter.PushArgument(ArgumentCount);
            emitter.Return();

            Assert.Equal(OpCodes.Ldarg, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_One_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 1);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc_0, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_Two_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 2);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc_1, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_Three_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 3);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc_2, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_Four_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 4);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc_3, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_Five_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 5);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_ByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 1);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc_S, emitter.Instructions[0].Code);
        }

        [Fact]
        public void PushVariable_ByteMaxValuePlusTwo_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 2);

            emitter.Push(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Ldloc, emitter.Instructions[0].Code);
        }

        [Fact]
        public void Store_One_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 1);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc_0, emitter.Instructions[1].Code);
        }

        [Fact]
        public void Store_Two_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 2);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc_1, emitter.Instructions[1].Code);
        }

        [Fact]
        public void Store_Three_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 3);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc_2, emitter.Instructions[1].Code);
        }

        [Fact]
        public void Store_Four_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 4);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc_3, emitter.Instructions[1].Code);
        }

        [Fact]
        public void Store_Five_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, 5);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc_S, emitter.Instructions[1].Code);
        }

        [Fact]
        public void Store_ByteMaxValuePlusOne_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 1);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc_S, emitter.Instructions[1].Code);
        }

        [Fact]
        public void Store_ByteMaxValuePlusTwo_EmitsMostEffectiveInstruction()
        {
            var emitter = CreateEmitter();
            var localBuilders = CreateLocalBuilders(emitter, byte.MaxValue + 2);

            emitter.Push(42);
            emitter.Store(localBuilders[localBuilders.Length - 1]);
            emitter.Return();

            Assert.Equal(OpCodes.Stloc, emitter.Instructions[1].Code);
        }


        [Fact]
        public void Emit_InvalidOpCodeWithInteger_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, 42));

        }

        [Fact]
        public void Emit_InvalidOpCodeWithLocalBuilder_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, (LocalBuilder)null));
        }

        [Fact]
        public void Emit_InvalidOpCodeWithSignedByte_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, (sbyte)42));
        }

        [Fact]
        public void Emit_InvalidOpCodeWithByte_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, (byte)42));
        }

        [Fact]
        public void Emit_InvalidOpCodeWithType_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, typeof(object)));
        }

        [Fact]
        public void Emit_InvalidOpCodeWithMethodInfo_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, (MethodInfo)null));
        }

        [Fact]
        public void Emit_InvalidOpCodeWithConstructorInfo_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Ldarg_0, (ConstructorInfo)null));
        }

#if NET40 || NET452 || NETSTANDARD11 || NETSTANDARD13 || NET46 || NETCOREAPP2_0
        [Fact]
        public void Emit_InvalidOpCode_ThrowsNotSupportedException()
        {
            var emitter = new Emitter(null, new Type[] { });
            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Xor));
        }
#endif
        [Fact]
        public void Push_Zero_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(0);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_One_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(1);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Two_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(2);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Three_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(3);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Four_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(4);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Five_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(5);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Six_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(6);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Seven_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(7);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Eight_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(8);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_Nine_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(9);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_SignedByteMaxValue_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(sbyte.MaxValue);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_SignedByteMaxValuePlusOne_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(sbyte.MaxValue + 1);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_SignedByteMinValue_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(sbyte.MinValue);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void Push_SignedByteMinValueMinusOne_ReturnsCorrectStackType()
        {
            var emitter = new Emitter(null, new Type[] { });

            emitter.Push(sbyte.MinValue - 1);

            Assert.Equal(typeof(int), emitter.StackType);
        }

        [Fact]
        public void StackType_EmptyMethod_IsNull()
        {
            var emitter = new Emitter(null, new Type[] { });

            Assert.Null(emitter.StackType);
        }

        [Fact]
        public void Emit_InvalidOpCodeForString_ThrowsException()
        {
            var emitter = new Emitter(null, new Type[] { });

            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Call, "somestring"));
        }

        [Fact]
        public void Emit_InvalidOpCodeForLong_ThrowsException()
        {
            var emitter = new Emitter(null, new Type[] { });

            Assert.Throws<NotSupportedException>(() => emitter.Emit(OpCodes.Call, (long)42));
        }

        [Fact]
        public void ToString_Instruction_ReturnsCodeAsString()
        {
            var instruction = new Instruction(OpCodes.Stloc, null);

            Assert.Equal("stloc", instruction.ToString(), StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void ToString_InstructionOfT_ReturnsCodeAndArgumentAsString()
        {
            var instruction = new Instruction<int>(OpCodes.Ldarg, 1, null);

            Assert.Equal("ldarg 1", instruction.ToString(), StringComparer.OrdinalIgnoreCase);
        }

#if NET452 || NET46 || NETCOREAPP2_0
        private ILGenerator CreateDummyGenerator(Type[] parameterTypes)
        {
            return new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object[]) }).GetILGenerator();
        }
#endif
#if NETCOREAPP1_1
        private ILGenerator CreateDummyGenerator(Type[] parameterTypes)
        {
            return new LightInject.DynamicMethod(typeof(object), parameterTypes).GetILGenerator();
        }
#endif

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