using System;
using Kontur.RetryableAssertions.TestFramework;

namespace Kontur.RetryableAssertions.Configuration
{
    public class Assertion
    {
        public static IAssertion<T> FromDelegate<T>(Action<T> action)
        {
            return new DelegateAssertionImplementation<T>(action);
        }

        private class DelegateAssertionImplementation<T> : IAssertion<T>
        {
            private readonly Action<T> assert;

            public DelegateAssertionImplementation(Action<T> assert)
            {
                this.assert = assert;
            }

            public void Assert(T value)
            {
                assert(value);
            }

            public bool TryAssert(T value, out Exception assertionException)
            {
                try
                {
                    assert(value);
                    assertionException = null;
                    return true;
                }
                catch (Exception exception)
                {
                    if (AssertionExceptionHelper.IsAssertionException(exception))
                    {
                        assertionException = exception;
                        return false;
                    }
                    throw;
                }
            }

            public bool Satisfied(T value)
            {
                try
                {
                    assert(value);
                    return true;
                }
                catch (Exception exception)
                {
                    if (AssertionExceptionHelper.IsAssertionException(exception))
                    {
                        return false;
                    }
                    throw;
                }
            }
        }
    }
}