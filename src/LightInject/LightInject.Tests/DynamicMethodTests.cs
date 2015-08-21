#if PCL_111
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Tests
{
    using System.Reflection;    

    using Xunit;

    using DynamicMethod = LightInject.DynamicMethod;
    using ILGenerator = LightInject.ILGenerator;
    using LocalBuilder = LightInject.LocalBuilder;

    
    public class DynamicMethodTests
    {
        [Fact]
        public void Emit_UnknownOpCode_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add));
        }
        
        [Fact]
        public void Emit_UnknownOpCodeWithConstructorInfo_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[]{typeof(object[])});
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, (ConstructorInfo)null));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithLocalBuilder_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, (LocalBuilder)null));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithInt_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, 42));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithSByte_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, (SByte)42));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithByte_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, (byte)42));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithString_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, (string)null));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithType_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, typeof(string)));
        }

        [Fact]
        public void Emit_UnknownOpCodeWithMethodInfo_ThrowsNotSupportedException()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(typeof(object), new Type[] { typeof(object[]) });
            ILGenerator generator = dynamicMethod.GetILGenerator();
            ExceptionAssert.Throws<NotSupportedException>(() => generator.Emit(OpCodes.Add, (MethodInfo)null));
        }

    }

}
#endif