using System;
using System.Diagnostics;
using System.Threading;
using Kontur.RetryableAssertions.Configuration;
using Kontur.RetryableAssertions.TestFramework;
using Kontur.RetryableAssertions.ValueProviding;

namespace Kontur.RetryableAssertions
{
    public static class Wait
    {
        public static IAssertionResult<T, TSource> Assertion<T, TSource>(IValueProvider<T, TSource> valueProvider, IAssertionConfiguration<T> configuration)
        {
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                try
                {
                    var actualValue = valueProvider.GetValue();
                    configuration.Assertion.Assert(actualValue);
                    return new AssertionResult<T, TSource>(valueProvider);
                }
                catch (Exception exception)
                {
                    var isAssertionException = AssertionExceptionHelper.IsAssertionException(exception);

                    if (configuration.Timeout > 0 && configuration.Timeout > stopwatch.ElapsedMilliseconds)
                    {
                        if (isAssertionException || configuration.ExceptionMatcher.RetryOnException(exception))
                        {
                            Thread.Sleep(configuration.Interval);
                            continue;
                        }
                    }

                    if (!isAssertionException)
                    {
                        throw;
                    }

                    var message = $"[timeout: {configuration.Timeout}]\n{valueProvider.GetMessage()}\n\n{exception.Message}";
                    throw AssertionExceptionHelper.CreateException(message);
                }
            }
        }
    }
}