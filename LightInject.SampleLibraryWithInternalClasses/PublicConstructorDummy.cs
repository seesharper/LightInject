
namespace LightInject.SampleLibraryWithInternalClasses
{
    public interface IPublicConstructorDummy
    {
    }

    internal class PublicConstructorDummy : IPublicConstructorDummy
    {
        public PublicConstructorDummy()
        {
        }
    }
}
