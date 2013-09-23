namespace LightInject.Interception.Tests
{
    using System;

    public interface ITarget
    {        
    }

    public class TargetWithGetHashCodeOverride : ITarget
    {
        private readonly int hashCode;

        public TargetWithGetHashCodeOverride(int hashCode)
        {
            this.hashCode = hashCode;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }


    public interface IAdditionalInterface
    {
    }

    public interface IMethodWithNoParameters
    {
        void Execute();
    }

    public interface IClassWithGenericClassContraint<T>
        where T : class, new()
    {
    }

    public interface IClassWithReferenceTypeProperty
    {
        string Value { get; set; }
    }

    public class ClassWithReferenceTypeProperty : IClassWithReferenceTypeProperty
    {
        string IClassWithReferenceTypeProperty.Value { get; set; }
    }

    public interface IClassWithValueTypeProperty
    {
        string Value { get; set; }
    }

    public interface IClassWithEvent
    {
        event EventHandler<EventArgs> SomeEvent;
    }

    public class ClassWithEvent : IClassWithEvent
    {
        event EventHandler<EventArgs> IClassWithEvent.SomeEvent
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }
    }

    public interface IMethodWithGenericConstraint
    {
        void Execute<T>(T value) where T : class, new();
    }

    

    public interface IMethodWithReferenceTypeParameter
    {
        void Execute(string value);
    }

    public class MethodWithReferenceTypeParameter : IMethodWithReferenceTypeParameter
    {
        public void Execute(string value)
        {

        }
    }

    public interface IMethodWithValueTypeParameter
    {
        void Execute(int value);
    }

    public class MethodWithValueTypeParameter : IMethodWithValueTypeParameter
    {
        public void Execute(int value)
        {
            
        }
    }

    public interface IMethodWithNullableParameter
    {
        void Execute(int? value);
    }

    public interface IMethodWithReferenceTypeRefParameter
    {
        void Execute(ref ReferenceTypeFoo value);
    }

    public class MethodWithReferenceTypeRefParameter : IMethodWithReferenceTypeRefParameter
    {
        public void Execute(ref ReferenceTypeFoo value)
        {
            value = new ReferenceTypeFoo() { Value = "AnotherValue" };
        }
    }

    public interface IMethodWithValueTypeRefParameter
    {
        void Execute(ref ValueTypeFoo value);
    }

    public class MethodWithValueTypeRefParameter : IMethodWithValueTypeRefParameter
    {
        public void Execute(ref ValueTypeFoo value)
        {
            value = new ValueTypeFoo { Value = "AnotherValue" };
        }
    }

    public interface IMethodWithValueTypeOutParameter
    {
        void Execute(out int value);
    }

    public interface IMethodWithReferenceTypeOutParameter
    {
        void Execute(out string value);
    }

    public interface IMethodWithReferenceTypeReturnValue
    {
        string Execute();
    }

    public interface IMethodWithValueTypeReturnValue
    {
        int Execute();
    }

    public interface IMethodWithGenericParameter
    {
        void Execute<T>(T value);
    }

    public class ReferenceTypeFoo
    {
        public string Value { get; set; }
    }

    public struct ValueTypeFoo
    {
        public string Value { get; set; }
    }


    public interface IMethodWithTargetReturnType
    {
        IMethodWithTargetReturnType Execute();
    }
}