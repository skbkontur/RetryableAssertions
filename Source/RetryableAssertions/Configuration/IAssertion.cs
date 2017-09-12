using System;

namespace Kontur.RetryableAssertions.Configuration
{
    public interface IAssertion<in T>
    {
        void Assert(T value);
        bool TryAssert(T value, out Exception assertionException);
        bool Satisfied(T value);
    }
}