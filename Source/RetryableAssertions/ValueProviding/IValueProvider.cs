using System;
using System.Collections.Generic;

namespace Kontur.RetryableAssertions.ValueProviding
{
    public interface IValueProvider
    {
        string GetMessage();
        IEnumerable<object> LastValues();
    }

    public interface IValueProvider<out T> : IValueProvider
    {
        T GetValue();
        T LastValue();
    }

    public interface IValueProvider<out T, out TSource> : IValueProvider<T>
    {
        IValueProvider<TSource> Source { get; }
        IValueProvider<TTransformed, TSource> Transformed<TTransformed>(Func<T, TTransformed> transform);
        IValueProvider<TTransformed, TSource> Transformed<TTransformed>(Func<T, TTransformed> transform, Func<string, string> transformMessage);
    }
}