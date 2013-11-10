namespace LightInject.Wcf.SampleLibrary.Implementation
{
    using LightInject.Wcf.SampleServices;

    public class Calculator : ICalculator
    {
        public int Add(int value1, int value2)
        {
            return value1 + value2;
        }
    }
}