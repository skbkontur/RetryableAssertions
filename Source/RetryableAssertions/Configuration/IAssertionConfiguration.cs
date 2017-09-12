namespace Kontur.RetryableAssertions.Configuration
{
    public interface IAssertionConfiguration<T> : IAssertionConfiguration
    {
        IAssertion<T> Assertion { get; }
    }

    public interface IAssertionConfiguration
    {
        int Timeout { get; }
        int Interval { get; }
        IExceptionMatcher ExceptionMatcher { get; }
    }
}