namespace LightInject.Interception.Tests
{
    using System;


    public class ClassWithNoMethods
    {
        
    }

    public class ClassWithVirtualMethod
    {
        public virtual int Execute()
        {
            return 42;
        }
    }

    public class ClassWithVirtualAndNonVirtualMethod
    {
        public virtual void VirtualMethod()
        {
          
        }

        public void NonVirtualMethod()
        {

        }
    }

    public class ClassWithVirtualProtectedMethod
    {
        protected virtual int Execute()
        {
            return 42;
        }
    }

    public class ClassWithConstructor
    {
        public string Value { get; set; }

        public ClassWithConstructor(string value)
        {
            Value = value;
        }

        public virtual int Execute()
        {
            return 42;
        }
    }

    public class ClassWithPropertyAndVirtualMethod
    {
        public string Value { get; set; }

        public virtual int Execute()
        {
            return 42;
        }
    }

    public class ClassWithVirtualProperty
    {
        public string value;

        public virtual string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
    }


    public class ClassImplementingIDisposableWithVirtualDisposeMethod : IDisposable
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    

    public class ClassImplementingIDisposableWithNonVirtualDisposeMethod : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


    
}