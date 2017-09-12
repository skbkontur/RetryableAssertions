using System;

namespace Kontur.RetryableAssertions.Configuration
{
    public class AssertionConfiguration<T> : AssertionConfiguration, IAssertionConfiguration<T>
    {
        private IAssertion<T> assertion;

        public IAssertion<T> Assertion { get => assertion ?? throw new Exception(); set => assertion = value; }
    }

    public class AssertionConfiguration : IAssertionConfiguration
    {
        private IExceptionMatcher exceptionMatcher;

        public int Timeout { get; set; }

        public int Interval { get; set; }

        public IExceptionMatcher ExceptionMatcher { get => exceptionMatcher ?? Configuration.ExceptionMatcher.Empty(); set => exceptionMatcher = value; }
    }
}