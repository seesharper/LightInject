namespace LightInject.Interception.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]    
    public class ContainerInterceptorVerificationTests : ContainerInterceptionTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }

        public override void GetInstance_InterceptorAfterDecorator_ReturnsProxy()
        {
            
        }
    }
}