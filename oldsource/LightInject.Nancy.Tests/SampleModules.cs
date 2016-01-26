namespace LightInject.Nancy.Tests
{
    using System.Collections.Generic;

    using global::Nancy;

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            
        }
    }

    public class SampleModuleWithTransientDependency : NancyModule
    {
        public ITransient Transient { get; private set; }

        public SampleModuleWithTransientDependency(ITransient transient)
        {
            Transient = transient;
        }
    }

    public class SampleModuleWithPerRequestDependency : NancyModule
    {
        public IPerRequest PerRequest { get; private set; }

        public SampleModuleWithPerRequestDependency(IPerRequest perRequest)
        {
            PerRequest = perRequest;
        }
    }

    public class SampleModuleWithSingletonDependency : NancyModule
    {
        public ISingleton Singleton { get; private set; }

        public SampleModuleWithSingletonDependency(ISingleton singleton)
        {
            Singleton = singleton;
        }
    }

    public class SampleModuleWithPerRequestCollectionDependency : NancyModule
    {
        public IEnumerable<ICollectionTypePerRequest> Instances { get; private set; }

        public SampleModuleWithPerRequestCollectionDependency(IEnumerable<ICollectionTypePerRequest> instances)
        {
            this.Instances = instances;
        }
    }

}