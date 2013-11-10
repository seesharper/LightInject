namespace LightInject.Wcf.Tests
{
    using System.ServiceModel;
    using LightInject.Wcf;
    using LightInject.Wcf.SampleServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Importants: Before running these test, the following command needs to be executed
    /// in a command prompt with administrator privileges.
    /// c:\netsh http add urlacl url=http://+:6000/ user=[username]
    /// </summary>
    [TestClass]
    public class InvocationTests
    {
        private ServiceHost serviceHost; 
        
        [TestInitialize]
        public void TestInitialize()
        {
            serviceHost = new LightInjectServiceHostFactory().CreateServiceHost<ICalculator>("http://localhost:6000");
            serviceHost.Open();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            serviceHost.Close();
        }

        [TestMethod]
        public void Add_TwoNumbers_ReturnsSum()
        {
            var calculatorFactory = new ChannelFactory<ICalculator>(new BasicHttpBinding(), new EndpointAddress("http://localhost:6000"));
            ICalculator calculator = calculatorFactory.CreateChannel();
            var result = calculator.Add(2, 2);
            Assert.AreEqual(4, result);     
            ((IClientChannel)calculator).Close();
        }
    }
}