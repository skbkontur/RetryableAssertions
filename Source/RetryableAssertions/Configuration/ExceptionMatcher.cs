using System;
using System.Linq;

namespace Kontur.RetryableAssertions.Configuration
{
    public static class ExceptionMatcher
    {
        private static readonly DelegateExceptionMatcher empty = new DelegateExceptionMatcher(x => false);

        public static IExceptionMatcher FromLambda(Func<Exception, bool> retryOnException)
        {
            return new DelegateExceptionMatcher(retryOnException);
        }

        public static IExceptionMatcher FromTypes(params Type[] exceptionTypes)
        {
            return new TypeExceptionMatcher(exceptionTypes);
        }

        public static IExceptionMatcher Empty()
        {
            return empty;
        }

        private class DelegateExceptionMatcher : IExceptionMatcher
        {
            private readonly Func<Exception, bool> retryOnException;

            public DelegateExceptionMatcher(Func<Exception, bool> retryOnException)
            {
                this.retryOnException = retryOnException;
            }

            public bool RetryOnException(Exception exception)
            {
                return retryOnException(exception);
            }
        }

        private class TypeExceptionMatcher : IExceptionMatcher
        {
            private readonly Type[] exceptionTypes;

            public TypeExceptionMatcher(Type[] exceptionTypes)
            {
                this.exceptionTypes = exceptionTypes;
            }

            public bool RetryOnException(Exception exception)
            {
                var type = exception.GetType();
                return exceptionTypes.Any(x => x.IsAssignableFrom(type));
            }
        }
    }
}