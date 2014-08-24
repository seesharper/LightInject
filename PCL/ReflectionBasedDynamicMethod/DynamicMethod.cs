namespace ReflectionBasedDynamicMethod
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DynamicMethod
    {
        private readonly Type returnType;

        private readonly Type[] parameterTypes;

        private readonly ILGenerator generator;

        public DynamicMethod(Type returnType, Type[] parameterTypes)
        {
            this.returnType = returnType;
            this.parameterTypes = parameterTypes;
            generator = new ILGenerator();
        }

        public Delegate CreateDelegate(Type delegateType)
        {
            var executeMethod = typeof(ILGenerator).GetMethod("Execute");
            var d =  executeMethod.CreateDelegate(delegateType, generator);
            return d;            
        }

        public ILGenerator GetILGenerator()
        {
            return generator;
        }
    }

    public class ILGenerator
    {
        private readonly Stack<object> stack = new Stack<object>();
        private readonly List<Action<object[]>> instructions = new List<Action<object[]>>();
        private readonly List<LocalBuilder> locals = new List<LocalBuilder>();

        public object Execute(object[] arguments)
        {
            foreach (var instruction in instructions)
            {
                instruction(arguments);
            }
            return stack.Pop();
        }

        public void Emit(OpCode code, ConstructorInfo constructor)
        {
            if (code == OpCodes.Newobj)
            {
                instructions.Add((args) => EmitInternal(code, constructor));
            }
        }

        internal void EmitInternal(OpCode code, ConstructorInfo constructor)
        {
            if (code == OpCodes.Newobj)
            {
                var parameterCount = constructor.GetParameters().Length;
                object[] arguments = Pop(parameterCount);
                Type type = constructor.DeclaringType;
                object instance = Activator.CreateInstance(type, arguments);
                stack.Push(instance);
            }
        }

        private object[] Pop(int numberOfElements)
        {
            var expressionsToPop = new object[numberOfElements];

            for (int i = 0; i < numberOfElements; i++)
            {
                expressionsToPop[i] = stack.Pop();
            }

            return expressionsToPop.Reverse().ToArray();
        }
    }


    /// <summary>
    /// Represents a local variable within a method or constructor.
    /// </summary>
    public class LocalBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalBuilder"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the variable that this <see cref="LocalBuilder"/> represents.</param>
        /// <param name="localIndex">The zero-based index of the local variable within the method body.</param> 
        public LocalBuilder(Type type, int localIndex)
        {
            Variable = Expression.Parameter(type);
            LocalType = type;
            LocalIndex = localIndex;
        }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> that represents the variable.
        /// </summary>
        public ParameterExpression Variable { get; private set; }

        /// <summary>
        /// Gets the type of the local variable.
        /// </summary>
        public Type LocalType { get; private set; }

        /// <summary>
        /// Gets the zero-based index of the local variable within the method body.
        /// </summary>
        public int LocalIndex { get; private set; }

        /// <summary>
        /// Gets or sets the value associated with this <see cref="LocalBuilder"/>.
        /// </summary>
        public object Value { get; set; }
    } 
}