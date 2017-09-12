using System;

namespace Kontur.RetryableAssertions.Examples.Emulation
{
    public class OneMoreAttemptPleaseException : Exception
    {
        public OneMoreAttemptPleaseException()
            : base("Do it one more time")
        {
        }
    }
}