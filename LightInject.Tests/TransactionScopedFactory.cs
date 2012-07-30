namespace LightInject.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Transactions;

    using LightInject;
    using LightInject.SampleLibrary;

    internal class TransactionScopedFactory : IFactory
    {
        private readonly ConcurrentDictionary<Transaction, IFoo> instances 
            = new ConcurrentDictionary<Transaction, IFoo>();

        public object GetInstance(ServiceRequest serviceRequest)
        {
            if (Transaction.Current != null)
            {                
                return this.instances.GetOrAdd(Transaction.Current, this.CreateTransactionScopedInstance);
            }
            return this.CreateInstance();
        }

        private IFoo CreateTransactionScopedInstance(Transaction transaction)
        {
            transaction.TransactionCompleted += this.OnTransactionCompleted;
            return this.CreateInstance();
        }

        private IFoo CreateInstance()
        {
            return new Foo();
        }

        private void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {            
            e.Transaction.TransactionCompleted -= this.OnTransactionCompleted;
            IFoo foo;
            this.instances.TryRemove(e.Transaction, out foo);
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof(IFoo);
        }
    }
}