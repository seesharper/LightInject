namespace LightInject.Wcf.Tests
{
    using System;
    using System.ServiceModel;

    public class TestBase
    {
        protected ServiceHost StartService<TService>()
        {
            var container = new ServiceContainer();
            container.EnableWcf();
            var serviceHost = new LightInjectServiceHostFactory().CreateServiceHost<TService>("http://localhost:6000/" + typeof(TService).FullName + ".svc");
            serviceHost.Open();
            return serviceHost;
        } 
    }
}