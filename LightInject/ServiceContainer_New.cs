using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightInject
{
    public class ServiceContainer_New
    {
        private readonly ConcurrentDictionary<Tuple<Type, string>, Func<object>> _factories =
           new ConcurrentDictionary<Tuple<Type, string>, Func<object>>();

        public object GetInstance(Type serviceType, string serviceName)
        {
            return null;
        }
    }
}
