using System;
using Kontur.RetryableAssertions.Configuration;
using Kontur.RetryableAssertions.ValueProviding;

namespace Kontur.RetryableAssertions.Extensions
{
    public static class AssertExtensions
    {
        public static IAssertionResult<T, TSource> Assert<T, TSource>(this IValueProvider<T, TSource> valueProvider, IAssertionConfiguration<T> configuration)
        {
            return Wait.Assertion(valueProvider, configuration);
        }

        public static IAssertionResult<T, TSource> Assert<T, TSource>(this IValueProvider<T, TSource> valueProvider, IAssertion<T> assertion, IAssertionConfiguration configuration)
        {
            return Wait.Assertion(valueProvider, new AssertionConfiguration<T>
            {
                Timeout = configuration.Timeout,
                Interval = configuration.Interval,
                ExceptionMatcher = configuration.ExceptionMatcher,
                Assertion = assertion
            });
        }

        public static IAssertionResult<T, TSource> Assert<T, TSource>(this IValueProvider<T, TSource> valueProvider, Action<T> assertion, IAssertionConfiguration configuration)
        {
            return valueProvider.Assert(Assertion.FromDelegate(assertion), configuration);
        }
    }
}