namespace LightInject.Wcf.SampleLibrary.Implementation
{
    using LightInject.Wcf.SampleServices;

    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ICalculator, Calculator>();
        }
    }
}