using System;
using Xunit;

namespace LightInject.Tests
{
    public class DefaultValueTests : TestBase
    {
        [Fact]
        public void ShouldHandleBoolWithDefault()
        {
            Assert.False(RegisterAndGet<FooWithBoolAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleBoolWithTrueValue()
        {
            Assert.True(RegisterAndGet<FooWithBoolAsTrue>().Value);
        }

        [Fact]
        public void ShouldHandleBoolWithFalseValue()
        {
            Assert.False(RegisterAndGet<FooWithBoolAsFalse>().Value);
        }

        [Fact]
        public void ShouldHandleByteWithDefault()
        {
            Assert.Equal((byte)0, RegisterAndGet<FooWithByteAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleByteWithMaxValue()
        {
            Assert.Equal(byte.MaxValue, RegisterAndGet<FooWithByteAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleByteWithMinValue()
        {
            Assert.Equal(byte.MinValue, RegisterAndGet<FooWithByteAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleSByteWithDefault()
        {
            Assert.Equal((sbyte)0, RegisterAndGet<FooWithSByteAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleSByteWithMaxValue()
        {
            Assert.Equal(sbyte.MaxValue, RegisterAndGet<FooWithSByteAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleSByteWithMinValue()
        {
            Assert.Equal(sbyte.MinValue, RegisterAndGet<FooWithSByteAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleShortWithDefault()
        {
            Assert.Equal((short)0, RegisterAndGet<FooWithShortAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleShortWithMaxValue()
        {
            Assert.Equal(short.MaxValue, RegisterAndGet<FooWithShortAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleShortWithMinValue()
        {
            Assert.Equal(short.MinValue, RegisterAndGet<FooWithShortAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleUShortWithDefault()
        {
            Assert.Equal((ushort)0, RegisterAndGet<FooWithUShortAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleUShortWithMaxValue()
        {
            Assert.Equal(ushort.MaxValue, RegisterAndGet<FooWithUShortAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleUShortWithMinValue()
        {
            Assert.Equal(ushort.MinValue, RegisterAndGet<FooWithUShortAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleIntWithDefault()
        {
            Assert.Equal((int)0, RegisterAndGet<FooWithIntAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleIntWithMaxValue()
        {
            Assert.Equal(int.MaxValue, RegisterAndGet<FooWithIntAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleIntWithMinValue()
        {
            Assert.Equal(int.MinValue, RegisterAndGet<FooWithIntAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleUIntWithDefault()
        {
            Assert.Equal((uint)0, RegisterAndGet<FooWithUIntAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleUIntWithMaxValue()
        {
            Assert.Equal(uint.MaxValue, RegisterAndGet<FooWithUIntAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleUIntWithMinValue()
        {
            Assert.Equal(uint.MinValue, RegisterAndGet<FooWithUIntAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleLongWithDefault()
        {
            Assert.Equal((long)0, RegisterAndGet<FooWithLongAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleLongWithMaxValue()
        {
            Assert.Equal(long.MaxValue, RegisterAndGet<FooWithLongAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleLongWithMinValue()
        {
            Assert.Equal(long.MinValue, RegisterAndGet<FooWithLongAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleULongWithDefault()
        {
            Assert.Equal((ulong)0, RegisterAndGet<FooWithULongAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleULongWithMaxValue()
        {
            Assert.Equal(ulong.MaxValue, RegisterAndGet<FooWithULongAsMax>().Value);
        }

        [Fact]
        public void ShouldHandleULongWithMinValue()
        {
            Assert.Equal(ulong.MinValue, RegisterAndGet<FooWithULongAsMin>().Value);
        }

        [Fact]
        public void ShouldHandleIntEnumWithDefault()
        {
            Assert.Equal(IntEnum.None, RegisterAndGet<FooWithIntEnumAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleIntEnumWithValueSet()
        {
            Assert.Equal(IntEnum.SomeValue, RegisterAndGet<FooWithIntEnumSet>().Value);
        }


        [Fact]
        public void ShouldHandleStringWithDefault()
        {
            Assert.Equal((string)null, RegisterAndGet<FooWithStringAsDefault>().Value);
        }

        [Fact]
        public void ShouldHandleStringWithValueSet()
        {
            Assert.Equal("SomeValue", RegisterAndGet<FooWithStringSet>().Value);
        }

        [Fact]
        public void ShouldHandleDefaultReferenceType()
        {
            Assert.Null(RegisterAndGet<FooWithCustomReferenceTypeSetToNull>().Value);
        }

        [Fact]
        public void ShouldHandleValueTypeSetToDefault()
        {
            Assert.Equal(0, RegisterAndGet<FooWithCustomValueTypeSetToDefault>().Value.Value);
        }

        [Fact]
        public void ShouldHandleValueTypeSetToNewInstance()
        {
            Assert.Equal(0, RegisterAndGet<FooWithCustomValueTypeSetToNewInstance>().Value.Value);
        }

        [Fact]
        public void ShouldUseConstructorWithTheMostResolvableParameters()
        {
            var container = CreateContainer(new ContainerOptions() { EnableOptionalArguments = true });
            container.Register<ServiceA>();
            container.Register<ServiceB>();
            container.Register<FooWithMultipleConstructors>();

            var instance = container.GetInstance<FooWithMultipleConstructors>();

            Assert.True(instance.ConstructorCalled);
        }

        [Fact]
        public void ShouldHandleInterfaceSetToNull()
        {
            Assert.Null(RegisterAndGet<FooWithInterfaceSetToNull>().Value);
        }

        [Fact]
        public void ShouldHandleInterfaceSetToDefault()
        {
            Assert.Null(RegisterAndGet<FooWithInterfaceSetToDefault>().Value);
        }

        private T RegisterAndGet<T>()
        {
            var container = CreateContainer(new ContainerOptions() { EnableOptionalArguments = true });
            container.Register<T>();
            return container.GetInstance<T>();
        }


        public class FooWithMultipleConstructors
        {
            public bool ConstructorCalled;

            public FooWithMultipleConstructors(ServiceA service, ServiceB serviceB)
            {
            }

            public FooWithMultipleConstructors(ServiceA service, ServiceB serviceB, ServiceC serviceC = null)
            {
                ConstructorCalled = true;
            }
        }

        public class ServiceA
        {

        }

        public class ServiceB
        {

        }

        public class ServiceC
        {

        }

        public class FooWithGenericDefaultValue<T>
        {
            public FooWithGenericDefaultValue(T value = default)
            {
            }
        }

        public class FooWithBoolAsDefault
        {
            public FooWithBoolAsDefault(bool value = default)
            {
                Value = value;
            }

            public bool Value { get; }
        }

        public class FooWithBoolAsTrue
        {
            public FooWithBoolAsTrue(bool value = true)
            {
                Value = value;
            }

            public bool Value { get; }
        }

        public class FooWithBoolAsFalse
        {
            public FooWithBoolAsFalse(bool value = false)
            {
                Value = value;
            }

            public bool Value { get; }
        }

        public class FooWithByteAsDefault
        {
            public FooWithByteAsDefault(byte value = default)
            {
                Value = value;
            }

            public byte Value { get; }
        }

        public class FooWithByteAsMax
        {
            public FooWithByteAsMax(byte value = byte.MaxValue)
            {
                Value = value;
            }

            public byte Value { get; }
        }

        public class FooWithByteAsMin
        {
            public FooWithByteAsMin(byte value = byte.MinValue)
            {
                Value = value;
            }

            public byte Value { get; }
        }


        public class FooWithSByteAsDefault
        {
            public FooWithSByteAsDefault(sbyte value = default)
            {
                Value = value;
            }

            public sbyte Value { get; }
        }


        public class FooWithSByteAsMax
        {
            public FooWithSByteAsMax(sbyte value = sbyte.MaxValue)
            {
                Value = value;
            }

            public sbyte Value { get; }
        }

        public class FooWithSByteAsMin
        {
            public FooWithSByteAsMin(sbyte value = sbyte.MinValue)
            {
                Value = value;
            }

            public sbyte Value { get; }
        }

        public class FooWithShortAsDefault
        {
            public FooWithShortAsDefault(short value = default)
            {
                Value = value;
            }

            public short Value { get; }
        }


        public class FooWithShortAsMax
        {
            public FooWithShortAsMax(short value = short.MaxValue)
            {
                Value = value;
            }

            public short Value { get; }
        }

        public class FooWithShortAsMin
        {
            public FooWithShortAsMin(short value = short.MinValue)
            {
                Value = value;
            }

            public short Value { get; }
        }

        public class FooWithUShortAsDefault
        {
            public FooWithUShortAsDefault(ushort value = default)
            {
                Value = value;
            }

            public ushort Value { get; }
        }


        public class FooWithUShortAsMax
        {
            public FooWithUShortAsMax(ushort value = ushort.MaxValue)
            {
                Value = value;
            }

            public ushort Value { get; }
        }

        public class FooWithUShortAsMin
        {
            public FooWithUShortAsMin(ushort value = ushort.MinValue)
            {
                Value = value;
            }

            public ushort Value { get; }
        }

        public class FooWithIntAsDefault
        {
            public FooWithIntAsDefault(int value = default)
            {
                Value = value;
            }

            public int Value { get; }
        }


        public class FooWithIntAsMax
        {
            public FooWithIntAsMax(int value = int.MaxValue)
            {
                Value = value;
            }

            public int Value { get; }
        }

        public class FooWithIntAsMin
        {
            public FooWithIntAsMin(int value = int.MinValue)
            {
                Value = value;
            }

            public int Value { get; }
        }


        public class FooWithUIntAsDefault
        {
            public FooWithUIntAsDefault(uint value = default)
            {
                Value = value;
            }

            public uint Value { get; }
        }


        public class FooWithUIntAsMax
        {
            public FooWithUIntAsMax(uint value = uint.MaxValue)
            {
                Value = value;
            }

            public uint Value { get; }
        }

        public class FooWithUIntAsMin
        {
            public FooWithUIntAsMin(uint value = uint.MinValue)
            {
                Value = value;
            }

            public uint Value { get; }
        }

        public class FooWithLongAsDefault
        {
            public FooWithLongAsDefault(long value = default)
            {
                Value = value;
            }

            public long Value { get; }
        }


        public class FooWithLongAsMax
        {
            public FooWithLongAsMax(long value = long.MaxValue)
            {
                Value = value;
            }

            public long Value { get; }
        }

        public class FooWithLongAsMin
        {
            public FooWithLongAsMin(long value = long.MinValue)
            {
                Value = value;
            }

            public long Value { get; }
        }

        public class FooWithULongAsDefault
        {
            public FooWithULongAsDefault(ulong value = default)
            {
                Value = value;
            }

            public ulong Value { get; }
        }


        public class FooWithULongAsMax
        {
            public FooWithULongAsMax(ulong value = ulong.MaxValue)
            {
                Value = value;
            }

            public ulong Value { get; }
        }

        public class FooWithULongAsMin
        {
            public FooWithULongAsMin(ulong value = ulong.MinValue)
            {
                Value = value;
            }

            public ulong Value { get; }
        }

        public class FooWithIntEnumAsDefault
        {
            public FooWithIntEnumAsDefault(IntEnum value = default)
            {
                Value = value;
            }

            public IntEnum Value { get; }
        }


        public class FooWithIntEnumSet
        {
            public FooWithIntEnumSet(IntEnum value = IntEnum.SomeValue)
            {
                Value = value;
            }

            public IntEnum Value { get; }
        }

        public class FooWithStringAsDefault
        {
            public FooWithStringAsDefault(string value = default)
            {
                Value = value;
            }

            public string Value { get; }
        }

        public class FooWithStringSet
        {
            public FooWithStringSet(string value = "SomeValue")
            {
                Value = value;
            }

            public string Value { get; }
        }

        public class FooWithCustomReferenceTypeSetToNull
        {
            public FooWithCustomReferenceTypeSetToNull(CustomReferenceType value = null)
            {
                Value = value;
            }

            public CustomReferenceType Value { get; }
        }

        public class FooWithCustomValueTypeSetToDefault
        {
            public FooWithCustomValueTypeSetToDefault(CustomValueType value = default(CustomValueType))
            {
                Value = value;
            }

            public CustomValueType Value { get; }
        }


        public class FooWithCustomValueTypeSetToNewInstance
        {
            public FooWithCustomValueTypeSetToNewInstance(CustomValueType value = new CustomValueType())
            {
                Value = value;
            }

            public CustomValueType Value { get; }
        }

        public class CustomReferenceType
        {

        }

        public class FooWithCustomValueType
        {

        }


        public struct CustomValueType
        {
            public int Value;

            public CustomValueType(int value)
            {
                Value = value;
            }
        }

        public class FooWithInterfaceSetToNull
        {
            public FooWithInterfaceSetToNull(IFoo foo = null)
            {
                Value = foo;
            }

            public IFoo Value { get; }
        }

        public class FooWithInterfaceSetToDefault
        {
            public FooWithInterfaceSetToDefault(IFoo foo = default)
            {
                Value = foo;
            }

            public IFoo Value { get; }
        }

        public interface IFoo
        {
        }

        public class Foo : IFoo
        {

        }

        public enum IntEnum
        {
            None,
            SomeValue
        }


        /// <summary>
        /// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
        /// </summary>
        public class FooWithPrimitiveTypes
        {
            public FooWithPrimitiveTypes(bool boolValue = default,
                                         byte byteValue = default,
                                         sbyte sbyteValue = default,
                                         short shortValue = default,
                                         ushort ushortValue = default,
                                         int intvalue = default,
                                         uint uintValue = default,
                                         long longValue = default,
                                         ulong ulongValue = default,
                                         IntPtr intPtrValue = default,
                                         UIntPtr uIntPtrValue = default)
            {
                BoolValue = boolValue;
                ByteValue = byteValue;
                SbyteValue = sbyteValue;
                ShortValue = shortValue;
                UshortValue = ushortValue;
                Intvalue = intvalue;
                UintValue = uintValue;
                LongValue = longValue;
                UlongValue = ulongValue;
                IntPtrValue = intPtrValue;
                UIntPtrValue = uIntPtrValue;
            }

            public bool BoolValue { get; }
            public byte ByteValue { get; }
            public sbyte SbyteValue { get; }
            public short ShortValue { get; }
            public ushort UshortValue { get; }
            public int Intvalue { get; }
            public uint UintValue { get; }
            public long LongValue { get; }
            public ulong UlongValue { get; }
            public IntPtr IntPtrValue { get; }
            public UIntPtr UIntPtrValue { get; }
        }
    }
}