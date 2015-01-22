namespace LightInject.Tests
{
    using System;

#if NET || NET45 || NET45TEST
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

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
                throw new AssertFailedException("The action did not throw an exception");
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