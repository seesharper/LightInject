namespace LightInject.Wcf.SampleLibrary
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface IServiceWithSameDependencyTwice
    {
        [OperationContract]
        int Execute();
    }
        
    [ServiceContract]
    public interface IServiceWithServiceContractAttribute
    {
    }

    public interface IServiceWithoutServiceContractAttribute
    {
    }

    [ServiceContract]
    public class ServiceWithoutInterface
    {
    }
}
