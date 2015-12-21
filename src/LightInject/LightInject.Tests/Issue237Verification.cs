namespace LightInject.Tests
{
    public class Issue237Verification : Issue237
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}