using System;
using System.Collections.Concurrent;
using System.Transactions;
using LightInject;
using LightInject.SampleLibrary;

namespace DependencyInjector.Tests
{
    internal class TransactionScopedFactory : IFactory
    {
        private readonly ConcurrentDictionary<Transaction, IFoo> instances 
            = new ConcurrentDictionary<Transaction, IFoo>();

        public object GetInstance(ServiceRequest serviceRequest)
        {
            if (Transaction.Current != null)
            {                
                return instances.GetOrAdd(Transaction.Current, CreateTransactionScopedInstance);
            }
            return CreateInstance();
        }

        private IFoo CreateTransactionScopedInstance(Transaction transaction)
        {
            transaction.TransactionCompleted += OnTransactionCompleted;
            return CreateInstance();
        }

        private IFoo CreateInstance()
        {
            return new Foo();
        }

        private void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {            
            e.Transaction.TransactionCompleted -= OnTransactionCompleted;
            IFoo foo;
            instances.TryRemove(e.Transaction, out foo);
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof(IFoo);
        }
    }
}