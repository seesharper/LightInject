namespace LightInject.SampleLibraryWithCompositionRoot
{
    public class SampleCompositionRoot : ICompositionRoot
    {
        public static int CallCount { get; set; }
        
        public void Compose(IServiceRegistry serviceRegistry)
        {
            CallCount++;
            serviceRegistry.Register(42);
        }
    }
}
