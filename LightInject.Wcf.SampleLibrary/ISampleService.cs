namespace LightInject.Wcf.SampleServices
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ICalculator
    {
        [OperationContract]
        int Add(int value1, int value2);
    }

    [ServiceContract]
    public interface IServiceWithServiceContractAttribute
    {
    }

    public interface IServiceWithoutServiceContractAttribute
    {
    }

    [ServiceContract]
    public class Service
    {
    }
}
