using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kontur.RetryableAssertions.Configuration;

namespace Kontur.RetryableAssertions.ValueProviding
{
    public static class ValueProviderExtensions
    {
        public static IValueProvider<T[], TSource> Taken<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertion<T> assertion)
        {
            return valueProvider.Transformed(x => x.TakeWhile(assertion.Satisfied).ToArray());
        }

        public static IValueProvider<T[], TSource> Skipped<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertion<T> assertion)
        {
            return valueProvider.Transformed(x => x.SkipWhile(assertion.Satisfied).ToArray());
        }

        public static IValueProvider<T[], TSource> Filtered<T, TSource>(this IValueProvider<T[], TSource> valueProvider, IAssertion<T> assertion)
        {
            var filter = new Filter<T>(assertion);
            return valueProvider.Transformed(filter.TransformValue, filter.TransformMessage);
        }

        private class Filter<T>
        {
            private readonly IAssertion<T> assertion;
            private List<Exception> exceptions;

            public Filter(IAssertion<T> assertion)
            {
                this.assertion = assertion;
            }

            public T[] TransformValue(T[] source)
            {
                var result = new List<T>();
                exceptions = new List<Exception>();
                foreach (var item in source)
                {
                    if (assertion.TryAssert(item, out Exception exception))
                    {
                        result.Add(item);
                    }
                    exceptions.Add(exception);
                }
                return result.ToArray();
            }

            public string TransformMessage(string message)
            {
                var result = new StringBuilder();
                result.AppendLine(message);
                result.AppendLine($"Items in source: {exceptions.Count}");
                for (var i = 0; i < exceptions.Count; i++)
                {
                    var exception = exceptions[i];
                    if (exception == null)
                    {
                        result.AppendLine($"Item[{i}]: Satisfied");
                    }
                    else
                    {
                        result.AppendLine($"Item[{i}]: Failed");
                        result.AppendLine(exception.Message);
                    }
                }
                return result.ToString();
            }
        }
    }
}