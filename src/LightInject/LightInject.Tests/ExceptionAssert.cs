namespace LightInject.Tests
{
    using System;
    using Xunit;

    public static class ExceptionAssert
    {
        public static TException Throws<TException>(Action action) where TException : Exception
        {
            return (TException)Execute<TException>(action);
        }

        public static TException Throws<TException>(Action action, string message) where TException : Exception
        {
            return Throws<TException>(action, e => e.Message == message);
        }

        public static TException Throws<TException>(Action action, Func<TException, bool> predicate) where TException : Exception
        {
            var exception = (TException)Execute<TException>(action);
            if (!predicate(exception))
            {
                throw exception;
            }

            return exception;
        }

        private static Exception Execute<TException>(Action action) where TException : Exception
        {
            try
            {
                action();                                
                throw new InvalidOperationException("The action did not throw an exception");
            }
            catch (Exception ex)
            {
                if (!(ex is TException))
                {
                    throw;
                }

                return ex;
            }
        }
    }
}