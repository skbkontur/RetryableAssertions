using System;
using System.Linq;
using Kontur.RetryableAssertions.Configuration;
using Kontur.RetryableAssertions.TestFramework;
using Kontur.RetryableAssertions.ValueProviding;

namespace Kontur.RetryableAssertions.Extensions
{
    public static class LinqExtensions
    {
        public static IValueProvider<TItem[], TSource> Converted<T, TSource, TItem>(this IValueProvider<T[], TSource> valueProvider, Func<T, TItem> selector)
        {
            return valueProvider.Transformed(x => x.Select(selector).ToArray());
        }

        public static IValueProvider<T[], TSource> Filtered<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion)
        {
            return valueProvider.Filtered(Assertion.FromDelegate(assertion));
        }

        public static IValueProvider<TItem[], TSource> Select<T, TSource, TItem>(this IValueProvider<T[], TSource> valueProvider, Func<T, TItem> selector)
        {
            return valueProvider.Transformed(x => x.Select(selector).ToArray());
        }

        public static IValueProvider<T[], TSource> Where<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion)
        {
            return valueProvider.Filtered(Assertion.FromDelegate(assertion));
        }

        public static IValueProvider<T[], TSource> SkipWhile<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion)
        {
            return valueProvider.Skipped(Assertion.FromDelegate(assertion));
        }

        public static IValueProvider<T[], TSource> TakeWhile<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion)
        {
            return valueProvider.Taken(Assertion.FromDelegate(assertion));
        }

        public static IValueProvider<T[], TSource> Skip<T, TSource>(this IValueProvider<T[], TSource> valueProvider, int count)
        {
            return valueProvider.Transformed(x => x.Skip(count).ToArray());
        }

        public static IValueProvider<T[], TSource> Take<T, TSource>(this IValueProvider<T[], TSource> valueProvider, int count)
        {
            return valueProvider.Transformed(x => x.Take(count).ToArray());
        }

        public static T ElementAt<T, TSource>(this IValueProvider<T[], TSource> valueProvider, int index, IAssertionConfiguration configuration)
        {
            return valueProvider.Assert(x => AssertLengthGreaterOrEqual(x, index + 1), configuration).GetAsserted()[index];
        }

        public static T Single<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertionConfiguration configuration)
        {
            return valueProvider.Assert(x => AssertLengthEqual(x, 1), configuration).GetAsserted().Single();
        }

        public static T[] Any<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertionConfiguration configuration)
        {
            return valueProvider.Assert(x => AssertLengthGreater(x, 0), configuration).GetAsserted();
        }

        public static T First<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertionConfiguration configuration)
        {
            return valueProvider.Any(configuration).First();
        }

        public static T Last<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertionConfiguration configuration)
        {
            return valueProvider.Any(configuration).Last();
        }

        public static T Single<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion, IAssertionConfiguration configuration)
        {
            return valueProvider.Filtered(assertion).Single(configuration);
        }

        public static T[] All<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion, IAssertionConfiguration configuration)
        {
            return valueProvider.Filtered(assertion).Assert(x => AssertLengthEqual(x, valueProvider.LastValue().Length), configuration).GetAsserted();
        }

        public static T[] Any<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion, IAssertionConfiguration configuration)
        {
            return valueProvider.Filtered(assertion).Any(configuration);
        }

        public static T First<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion, IAssertionConfiguration configuration)
        {
            return valueProvider.Filtered(assertion).First(configuration);
        }

        public static T Last<T, TSource>(this IValueProvider<T[], TSource> valueProvider, Action<T> assertion, IAssertionConfiguration configuration)
        {
            return valueProvider.Filtered(assertion).Last(configuration);
        }

        private static void AssertLengthEqual<T>(T[] array, int expected)
        {
            if (array.Length != expected)
            {
                throw AssertionExceptionHelper.CreateException($"Expected array length equal to {expected} but was {array.Length}");
            }
        }

        private static void AssertLengthGreater<T>(T[] array, int expected)
        {
            if (array.Length <= expected)
            {
                throw AssertionExceptionHelper.CreateException($"Expected array length greater then {expected} but was {array.Length}");
            }
        }

        private static void AssertLengthGreaterOrEqual<T>(T[] array, int expected)
        {
            if (array.Length < expected)
            {
                throw AssertionExceptionHelper.CreateException($"Expected array length greater then or equal to {expected} but was {array.Length}");
            }
        }
    }
}