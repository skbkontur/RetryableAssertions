using System;

namespace Kontur.RetryableAssertions.Configuration
{
    public interface IExceptionMatcher
    {
        bool RetryOnException(Exception exception);
    }
}