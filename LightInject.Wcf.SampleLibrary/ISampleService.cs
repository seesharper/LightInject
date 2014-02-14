﻿namespace LightInject.Wcf.SampleLibrary
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
    public interface IPerCallInstanceAndSingleConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface IPerCallInstanceAndMultipleConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface IPerCallInstanceAndReentrantConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface IPerSessionInstanceAndSingleConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface IPerSessionInstanceAndMultipleConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface IPerSessionInstanceAndReentrantConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface ISingleInstanceAndSingleConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface ISingleInstanceAndMultipleConcurrency
    {
        [OperationContract]
        int Execute();
    }

    [ServiceContract]
    public interface ISingleInstanceAndReentrantConcurrency
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
