
namespace LightInject.SampleLibraryWithInternalClasses
{
    public interface IInternalConstructorDummy
    {
    }

    internal class InternalConstructorDummy : IInternalConstructorDummy
    {
        internal InternalConstructorDummy()
        {
        }
    }
}
