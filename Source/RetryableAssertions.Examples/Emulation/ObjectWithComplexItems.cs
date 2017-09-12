using System.Collections.Generic;
using System.Linq;

namespace Kontur.RetryableAssertions.Examples.Emulation
{
    public class ObjectWithComplexItems
    {
        public IEnumerable<ObjectWithDeferedProperties> Items => GetItems();

        public IEnumerable<ObjectWithDeferedProperties> GetItems()
        {
            var times = 0;
            return new[] {0}.SelectMany(x => Enumerable.Range(0, ++times)).Select(x => new ObjectWithDeferedProperties());
        }

        public class ObjectWithDeferedProperties
        {
            public DeferredValue<string> Xdeferred { get; } = DeferredValue.Create("12", "Months in year");
            public DeferredValue<string> Ydeferred { get; } = DeferredValue.Create("42", "Answer to the Ultimate Question of Life, The Universe, and Everything");
            public DeferredValue<string> Zdeferred { get; } = DeferredValue.Create("56", "Random int");
        }
    }
}