namespace LightInject.Interception.Tests
{
    public class ClassWithVirtualMethod
    {
        public virtual int Execute()
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
}