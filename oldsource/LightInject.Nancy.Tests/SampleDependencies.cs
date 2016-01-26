namespace LightInject.Nancy.Tests
{
    public interface ITransient
    {
        
    }

    public class Transient : ITransient
    {
    }

    public interface ISingleton
    {
        
    }

    public class Singleton : ISingleton
    {

    }

    public interface IPerRequest
    {
        
    }

    public class PerRequest : IPerRequest
    {
    }

    public interface ICollectionTypeSingleton
    {

    }

    public class CollectionTypeSingleton1 : ICollectionTypeSingleton
    {

    }

    public class CollectionTypeSingleton2 : ICollectionTypeSingleton
    {

    }

    public interface ICollectionTypePerRequest
    {

    }

    public class CollectionTypePerRequest1 : ICollectionTypePerRequest
    {

    }

    public class CollectionTypePerRequest2 : ICollectionTypePerRequest
    {

    }
}