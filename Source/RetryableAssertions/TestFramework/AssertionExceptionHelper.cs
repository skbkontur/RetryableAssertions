using System;

namespace Kontur.RetryableAssertions.TestFramework
{
    public class AssertionExceptionHelper
    {
        private static Type Type => AssertionExceptionProvider.GetExceptionType();

        public static Type GetExceptionType()
        {
            return Type;
        }

        public static bool IsAssertionException(Exception exception)
        {
            return Type.IsInstanceOfType(exception);
        }

        public static Exception CreateException(string message)
        {
            return (Exception) Activator.CreateInstance(Type, message);
        }
    }
}