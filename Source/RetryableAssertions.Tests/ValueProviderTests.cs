using System;
using System.Linq;
using Kontur.RetryableAssertions.ValueProviding;
using NSubstitute;
using NUnit.Framework;

namespace Kontur.RetryableAssertions.Tests
{
    public class ValueProviderTests
    {
        [Test]
        public void RootProvider_SourceIsRoot()
        {
            var valueProvider = ValueProvider.Create(Substitute.For<Func<string>>());

            Assert.That(valueProvider.Source, Is.SameAs(valueProvider));
        }

        [Test]
        public void TransformedProvider_SourceIsRoot()
        {
            var valueProvider = ValueProvider.Create(Substitute.For<Func<string>>());
            var transformed = valueProvider.Transformed(x => x);

            Assert.That(transformed.Source, Is.SameAs(valueProvider));
        }

        [Test]
        public void TransformedX2Provider_SourceIsRoot()
        {
            var valueProvider = ValueProvider.Create(Substitute.For<Func<string>>());
            var transformed = valueProvider.Transformed(x => x).Transformed(x => x);

            Assert.That(transformed.Source, Is.SameAs(valueProvider));
        }

        [Test]
        public void RootProviderGetValue_RetrievesValueFromSource()
        {
            var valueProvider = ValueProvider.Create(() => "xxx");

            Assert.That(valueProvider.GetValue(), Is.EqualTo("xxx"));
        }

        [Test]
        public void TransformedProviderGetValue_RetrievesValueFromSource()
        {
            var valueProvider = ValueProvider.Create(() => "xxx");
            var transformed = valueProvider.Transformed(x => $"[{x}]");

            Assert.That(transformed.GetValue(), Is.EqualTo("[xxx]"));
        }

        [Test]
        public void RootProviderGetValue_RetrievesValueEveryCall()
        {
            var getValue = Substitute.For<Func<string>>();
            var valueProvider = ValueProvider.Create(getValue);

            valueProvider.GetValue();
            valueProvider.GetValue();

            getValue.Received(2).Invoke();
        }

        [Test]
        public void TransformedProviderGetValue_RetrievesValueEveryCall()
        {
            var getValue = Substitute.For<Func<string>>();
            var transform = Substitute.For<Func<string, int>>();

            var valueProvider = ValueProvider.Create(getValue);
            var transformed = valueProvider.Transformed(transform);

            transformed.GetValue();
            transformed.GetValue();

            getValue.Received(2).Invoke();
        }

        [Test]
        public void LastValue_ReturnsLastCachedValue()
        {
            var getValue = Substitute.For<Func<string>>();

            var valueProvider = ValueProvider.Create(getValue);
            getValue.Invoke().Returns(x => "yyy");
            valueProvider.GetValue();
            getValue.Invoke().Returns(x => "zzz");
            valueProvider.GetValue();
            getValue.ClearReceivedCalls();

            var lastValue = valueProvider.LastValue();

            Assert.That(lastValue, Is.EqualTo("zzz"));
            getValue.DidNotReceive().Invoke();
        }

        [Test]
        public void ValueNotRetrieved_LastValueThrowsException()
        {
            var getValue = Substitute.For<Func<string>>();

            var valueProvider = ValueProvider.Create(getValue);

            Assert.Throws<ValueNotRetrievedException>(() => valueProvider.LastValue());
        }

        [Test]
        public void GetValueThrows_LastValueRethrows()
        {
            var exception = new Exception();
            var getValue = Substitute.For<Func<string>>();
            getValue.Invoke().ReturnsForAnyArgs(x => throw exception);

            var valueProvider = ValueProvider.Create(getValue);

            Assert.Throws(Is.SameAs(exception), () => valueProvider.GetValue());
            Assert.Throws(Is.SameAs(exception), () => valueProvider.LastValue());
        }

        [Test]
        public void LastValueOverridesException()
        {
            var called = false;
            var getValue = Substitute.For<Func<string>>();
            getValue.Invoke().ReturnsForAnyArgs(x => called ? "xxx" : throw new Exception());

            var valueProvider = ValueProvider.Create(getValue);
            Assert.Throws<Exception>(() => valueProvider.GetValue());

            called = true;

            Assert.That(valueProvider.GetValue(), Is.EqualTo("xxx"));
            Assert.That(valueProvider.LastValue(), Is.EqualTo("xxx"));
        }

        [Test]
        public void LastExceptionOverridesValue()
        {
            var exception = new Exception();
            var getValue = Substitute.For<Func<string>>();
            getValue.Invoke().ReturnsForAnyArgs("xxx");

            var valueProvider = ValueProvider.Create(getValue);
            valueProvider.GetValue();

            getValue.Invoke().ReturnsForAnyArgs(x => throw exception);

            Assert.Throws(Is.SameAs(exception), () => valueProvider.GetValue());
            Assert.Throws(Is.SameAs(exception), () => valueProvider.LastValue());
        }

        [Test]
        public void ValueNotRetrieved_LastValuesThrowsException()
        {
            var valueProvider = ValueProvider.Create(() => "xxx");

            Assert.Throws<ValueNotRetrievedException>(() => valueProvider.LastValues());
        }

        [Test]
        public void LastValues()
        {
            var valueProvider = ValueProvider.Create(() => "xxx");
            valueProvider.GetValue();

            Assert.That(valueProvider.LastValues(), Is.EqualTo(new[] {"xxx"}));
        }

        [Test]
        public void LastValuesAfterTransformation()
        {
            var valueProvider = ValueProvider.Create(() => "xxx")
                                             .Transformed(x => $"[{x}]")
                                             .Transformed(x => $"({x})");

            valueProvider.GetValue();

            Assert.That(valueProvider.LastValues().ToArray(), Is.EqualTo(new[] {"([xxx])", "[xxx]", "xxx"}));
        }
    }
}