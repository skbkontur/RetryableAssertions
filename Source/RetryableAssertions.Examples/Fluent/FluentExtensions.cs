using System;
using Kontur.RetryableAssertions.Examples.Emulation;
using Kontur.RetryableAssertions.Extensions;
using Kontur.RetryableAssertions.ValueProviding;

namespace Kontur.RetryableAssertions.Examples.Fluent
{
    public static class FluentExtensions
    {
        public static IAssertionResult<TT, TS> ThatFluent<TT, TS>(this IValueProvider<TT, TS> valueProvider, Action<TT> assertion)
        {
            return valueProvider.Assert(assertion, Helper.AssertionConfiguration);
        }
    }
}