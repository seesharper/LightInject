namespace LightInject.Interception.Tests
{
    using Xunit;

    [Collection("Interception")]
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