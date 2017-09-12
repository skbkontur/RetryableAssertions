using System;
using System.Collections.Generic;

namespace Kontur.RetryableAssertions.ValueProviding
{
    public class ValueProvider
    {
        public static IValueProvider<T, T> Create<T>(Func<T> getValue, Func<string> getMessage)
        {
            return new ValueProviderImplementation<T, T>(getValue, getMessage);
        }

        public static IValueProvider<T, T> Create<T>(Func<T> getValue, string message)
        {
            return new ValueProviderImplementation<T, T>(getValue, () => message);
        }

        public static IValueProvider<T, T> Create<T>(Func<T> getValue)
        {
            return new ValueProviderImplementation<T, T>(getValue, () => string.Empty);
        }

        private class ValueProviderImplementation<T, TSource> : IValueProvider<T, TSource>
        {
            private readonly IValueProvider<TSource> source;
            private readonly Func<T> getValue;
            private readonly Func<string> getMessage;
            private readonly IValueProvider parent;
            private bool isValueRetrieved;
            private T lastValue;
            private Exception exception;

            internal ValueProviderImplementation(Func<T> getValue, Func<string> getMessage)
                : this(getValue, getMessage, null, null)
            {
            }

            private ValueProviderImplementation(Func<T> getValue, Func<string> getMessage, IValueProvider<TSource> source, IValueProvider parent)
            {
                this.getValue = getValue;
                this.getMessage = getMessage;
                this.parent = parent;
                this.source = source;
            }

            public IValueProvider<TSource> Source => source ?? this as IValueProvider<TSource>;

            public string GetMessage()
            {
                return getMessage();
            }

            public IEnumerable<object> LastValues()
            {
                AssertValueWasRetrieved();
                return LastValuesInternal();
            }

            public IValueProvider<TTransformed, TSource> Transformed<TTransformed>(Func<T, TTransformed> transform)
            {
                return TransformedInternal(transform);
            }

            public IValueProvider<TTransformed, TSource> Transformed<TTransformed>(Func<T, TTransformed> transform, Func<string, string> transformMessage)
            {
                return TransformedInternal(transform, transformMessage);
            }

            public T GetValue()
            {
                try
                {
                    exception = null;
                    return lastValue = getValue();
                }
                catch (Exception e)
                {
                    exception = e;
                    throw;
                }
                finally
                {
                    isValueRetrieved = true;
                }
            }

            public T LastValue()
            {
                AssertValueWasRetrieved();
                return exception != null ? throw exception : lastValue;
            }

            private IValueProvider<TTransformed, TSource> TransformedInternal<TTransformed>(Func<T, TTransformed> transform, Func<string, string> transformMessage = null)
            {
                return new ValueProviderImplementation<TTransformed, TSource>(
                    () => transform(GetValue()),
                    transformMessage == null ? getMessage : () => transformMessage(getMessage()),
                    Source,
                    this
                );
            }

            private void AssertValueWasRetrieved()
            {
                if (!isValueRetrieved)
                {
                    throw new ValueNotRetrievedException();
                }
            }

            private IEnumerable<object> LastValuesInternal()
            {
                yield return LastValue();
                if (parent != null)
                {
                    foreach (var value in parent.LastValues())
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}