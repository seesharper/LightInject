using System;

namespace LightInject.SampleLibrary
{
    public interface IBar
    {
    }

    public class Bar : IBar
    {
    }

    public class AnotherBar : IBar
    {
    }

    public class BarFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            CallCount++;
            ServiceName = serviceRequest.ServiceName;
            return serviceRequest.CanProceed ? serviceRequest.Proceed() : new Bar();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof (IBar).IsAssignableFrom(serviceType);
        }

        public string ServiceName { get; set; }

        public int CallCount { get; private set; }
    }
}