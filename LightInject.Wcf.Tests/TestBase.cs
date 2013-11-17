namespace LightInject.Wcf.Tests
{
    using System.ServiceModel;

    public class TestBase
    {
        protected ServiceHost StartService<TService>()
        {
            var serviceHost = new LightInjectServiceHostFactory().CreateServiceHost<TService>("http://localhost:6000/" + typeof(TService).FullName + ".svc");
            serviceHost.Open();
            return serviceHost;
        } 
    }
}