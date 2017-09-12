using System;

namespace Kontur.RetryableAssertions.Examples.Emulation
{
    public class DeferredValue
    {
        public static DeferredValue<T> Create<T>(Func<T> getBadValue, Func<T> value, string description)
        {
            return new DeferredValue<T>(getBadValue, value, description);
        }

        public static DeferredValue<T> Create<T>(Func<T> getBadValue, T value, string description)
        {
            return new DeferredValue<T>(getBadValue, () => value, description);
        }

        public static DeferredValue<T> Create<T>(T value, string description)
        {
            return new DeferredValue<T>(() => value, () => value, description);
        }
    }

    public class DeferredValue<T>
    {
        private readonly Func<T> getBadValue;
        private readonly Func<T> getValue;
        private readonly string description;
        private int counter = 3;

        public DeferredValue(Func<T> getBadValue, Func<T> getValue, string description)
        {
            this.getBadValue = getBadValue;
            this.getValue = getValue;
            this.description = description;
        }

        public T GetValue()
        {
            return --counter > 0 ? getBadValue() : getValue();
        }

        public string GetDescription()
        {
            return description;
        }
    }
}