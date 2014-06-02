namespace LightInject.SampleLibraryWithCompositionRoot
{
    using System.Reflection;

    public class SampleCompositionRoot : ICompositionRoot
    {
        public static int CallCount { get; set; }

        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            CallCount++;
#if NETFX_CORE || WINDOWS_PHONE
            serviceRegistry.RegisterAssembly(typeof(SampleCompositionRoot).GetTypeInfo().Assembly);
#else
            serviceRegistry.RegisterAssembly(typeof(SampleCompositionRoot).Assembly);
#endif

        }
    }
}
