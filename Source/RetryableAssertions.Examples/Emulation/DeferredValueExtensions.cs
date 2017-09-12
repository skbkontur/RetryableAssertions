using System.Collections.Generic;
using System.Linq;
using Kontur.RetryableAssertions.ValueProviding;

namespace Kontur.RetryableAssertions.Examples.Emulation
{
    public static class DeferredValueExtensions
    {
        public static IValueProvider<T, T> Wait<T>(this DeferredValue<T> deferredValue)
        {
            return ValueProvider.Create(deferredValue.GetValue, deferredValue.GetDescription);
        }

        public static IValueProvider<T[], T[]> Wait<T>(this IEnumerable<DeferredValue<T>> deferredValues)
        {
            var deferredEnumerable = new DeferredEnumerable<T>(deferredValues);
            return ValueProvider.Create(deferredEnumerable.GetValue, deferredEnumerable.GetDescription);
        }

        public static IValueProvider<T[], T[]> Wait<T>(this IEnumerable<T> source)
        {
            return ValueProvider.Create(source.ToArray);
        }

        private class DeferredEnumerable<T>
        {
            private readonly IEnumerable<DeferredValue<T>> source;
            private DeferredValue<T>[] cache;

            public DeferredEnumerable(IEnumerable<DeferredValue<T>> source)
            {
                this.source = source;
            }

            public T[] GetValue()
            {
                cache = source.ToArray();
                return cache.Select(x => x.GetValue()).ToArray();
            }

            public string GetDescription()
            {
                return string.Join("\n", cache.Select(x => x.GetDescription()));
            }
        }
    }
}