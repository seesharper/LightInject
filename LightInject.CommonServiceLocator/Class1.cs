using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject.CommonServiceLocator
{
    using Microsoft.Practices.ServiceLocation;

    public class LightInjectServiceLocator : ServiceLocatorImplBase 
    {
        protected override object DoGetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
