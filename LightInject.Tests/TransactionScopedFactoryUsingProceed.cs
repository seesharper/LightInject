namespace LightInject.Tests
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
                return this.instances.GetOrAdd(Transaction.Current, t => this.CreateTransactionScopedInstance(t, serviceRequest));
            }

            return this.CreateInstance(serviceRequest);
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof(IFoo);
        }

        private IFoo CreateTransactionScopedInstance(Transaction transaction, ServiceRequest serviceRequest)
        {
            transaction.TransactionCompleted += this.OnTransactionCompleted;
            return this.CreateInstance(serviceRequest);
        }

        private IFoo CreateInstance(ServiceRequest serviceRequest)
        {
            return (IFoo)serviceRequest.Proceed();
        }

        private void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            e.Transaction.TransactionCompleted -= this.OnTransactionCompleted;
            IFoo foo;
            this.instances.TryRemove(e.Transaction, out foo);
        }
    }
}