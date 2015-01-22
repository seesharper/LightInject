using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LightInject.WinRT.Tests
{
    using System.Linq.Expressions;
    using System.Reflection.Emit;

    [TestClass]
    public class ILGeneratorTests
    {
        [TestMethod]
        public void Emit_ArrayAccessUsingSByte_DoesNotThrowException()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(string[]));
            ILGenerator ilGenerator = new ILGenerator(new ParameterExpression[]{parameterExpression});
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)9);
            ilGenerator.Emit(OpCodes.Ldelem_Ref);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
