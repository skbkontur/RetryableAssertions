using Kontur.RetryableAssertions.Configuration;
using Kontur.RetryableAssertions.Examples.Emulation;
using Kontur.RetryableAssertions.Extensions;
using Kontur.RetryableAssertions.ValueProviding;
using NUnit.Framework.Constraints;

namespace Kontur.RetryableAssertions.Examples.Nunit
{
    public static class NunitExtensions
    {
        public static void Assert<T>(this DeferredValue<T> valueProvider, IResolveConstraint constraint)
        {
            NUnit.Framework.Assert.That(valueProvider.GetValue(), constraint, valueProvider.GetDescription());
        }

        public static void Assert<T>(this T value, IResolveConstraint constraint)
        {
            NUnit.Framework.Assert.That(value, constraint);
        }

        public static IAssertionResult<TT, TS> ThatNunit<TT, TS>(this IValueProvider<TT, TS> valueProvider, IResolveConstraint constraint)
        {
            var reusableConstraint = new ReusableConstraint(constraint);
            var assertion = Assertion.FromDelegate<TT>(x => NUnit.Framework.Assert.That(x, reusableConstraint));
            return valueProvider.Assert(assertion, Helper.AssertionConfiguration);
        }
    }
}