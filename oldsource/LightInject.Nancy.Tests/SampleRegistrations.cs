namespace LightInject.Nancy.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Nancy.Bootstrapper;

    public class SampleRegistrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                yield return new TypeRegistration(typeof(ITransient), typeof(Transient), Lifetime.Transient);
                yield return new TypeRegistration(typeof(IPerRequest), typeof(PerRequest), Lifetime.PerRequest);
                yield return new TypeRegistration(typeof(ISingleton), typeof(Singleton), Lifetime.Singleton);
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                Type[] implementingTypes = { typeof(CollectionTypePerRequest1), typeof(CollectionTypePerRequest2) };
                var registration = new CollectionTypeRegistration(
                    typeof(ICollectionTypePerRequest),
                    implementingTypes,
                    Lifetime.PerRequest);
                yield return registration;
            }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }
    }

    
}