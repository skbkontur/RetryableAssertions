using Kontur.RetryableAssertions.Configuration;

namespace Kontur.RetryableAssertions.Examples.Emulation
{
    public static class Helper
    {
        public static AssertionConfiguration AssertionConfiguration { get; } = new AssertionConfiguration
        {
            Timeout = 1000,
            Interval = 10,
            ExceptionMatcher = ExceptionMatcher.FromTypes(typeof(OneMoreAttemptPleaseException))
        };
    }
}