namespace DependencyInjector.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Transactions;
    using LightInject;
    using LightInject.SampleLibrary;

    internal class TransactionScopedFactoryUsingProceed : IFactory
    {
        private readonly ConcurrentDictionary<Transaction, IFoo> instances 
            = new ConcurrentDictionary<Transaction, IFoo>();

        public object GetInstance(ServiceRequest serviceRequest)
        {
            if (Transaction.Current != null)
            {
                return instances.GetOrAdd(Transaction.Current, t => CreateTransactionScopedInstance(t, serviceRequest));
            }

            return CreateInstance(serviceRequest);
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof(IFoo);
        }

        private IFoo CreateTransactionScopedInstance(Transaction transaction, ServiceRequest serviceRequest)
        {
            transaction.TransactionCompleted += OnTransactionCompleted;
            return CreateInstance(serviceRequest);
        }

        private IFoo CreateInstance(ServiceRequest serviceRequest)
        {
            return (IFoo)serviceRequest.Proceed();
        }

        private void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            e.Transaction.TransactionCompleted -= OnTransactionCompleted;
            IFoo foo;
            instances.TryRemove(e.Transaction, out foo);
        }
    }
}