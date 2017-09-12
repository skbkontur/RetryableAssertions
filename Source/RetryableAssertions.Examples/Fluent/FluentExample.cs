using System;
using System.Linq;
using FluentAssertions;
using Kontur.RetryableAssertions.Examples.Emulation;
using Kontur.RetryableAssertions.Extensions;
using NUnit.Framework;

namespace Kontur.RetryableAssertions.Examples.Fluent
{
    public class FluentExample
    {
        [Test]
        public void Test42Ok()
        {
            var deferredValue = DeferredValue.Create(() => 352, 42, "Answer to the Ultimate Question of Life, The Universe, and Everything");
            deferredValue.Wait().ThatFluent(x => x.Should().Be(42, "answer is 42"));
        }

        [Test]
        public void Test42Wrong()
        {
            var deferredValue = DeferredValue.Create(() => 352, 420, "Answer to the Ultimate Question of Life, The Universe, and Everything");
            deferredValue.Wait().ThatFluent(x => x.Should().Be(42, "answer is 42"));
        }

        [Test]
        public void Test42FailedByException()
        {
            var deferredValue = DeferredValue.Create(() => 352, () => throw new Exception(":("), "Answer to the Ultimate Question of Life, The Universe, and Everything");
            deferredValue.Wait().ThatFluent(x => x.Should().Be(42, "answer is 42"));
        }

        [Test]
        public void Test42FailedImmidiatly()
        {
            var deferredValue = DeferredValue.Create(() => throw new Exception(":("), 42, "Answer to the Ultimate Question of Life, The Universe, and Everything");
            deferredValue.Wait().ThatFluent(x => x.Should().Be(42, "answer is 42"));
        }

        [Test]
        public void Test42Retry()
        {
            var deferredValue = DeferredValue.Create(() => throw new OneMoreAttemptPleaseException(), 42, "Answer to the Ultimate Question of Life, The Universe, and Everything");
            deferredValue.Wait().ThatFluent(x => x.Should().Be(42, "answer is 42"));
        }

        [Test]
        public void Test42WithTransformation()
        {
            var deferredValue = DeferredValue.Create(() => "352", "42", "Answer to the Ultimate Question of Life, The Universe, and Everything");
            deferredValue.Wait().Transformed(int.Parse).ThatFluent(x => x.Should().Be(42, "answer is 42"));
        }

        [Test]
        public void Test42ExistsOk()
        {
            var deferredValue1 = DeferredValue.Create(() => 352, 12, "Months in year");
            var deferredValue2 = DeferredValue.Create(() => 352, 42, "Answer to the Ultimate Question of Life, The Universe, and Everything");
            var deferredValue3 = DeferredValue.Create(() => 352, 56, "Random int");
            var deferredValues = new[] {deferredValue1, deferredValue2, deferredValue3};
            deferredValues.Wait().ThatFluent(x => x.Should().Contain(42, "answer is 42"));
        }

        [Test]
        public void Test42ExistsFaild()
        {
            var deferredValue1 = DeferredValue.Create(() => 352, 12, "Months in year");
            var deferredValue2 = DeferredValue.Create(() => 352, 420, "Answer to the Ultimate Question of Life, The Universe, and Everything");
            var deferredValue3 = DeferredValue.Create(() => 352, 56, "Random int");
            var deferredValues = new[] {deferredValue1, deferredValue2, deferredValue3};
            deferredValues.Wait().ThatFluent(x => x.Should().Contain(42, "answer is 42"));
        }

        [Test]
        public void Test42ExistsWithTransformation()
        {
            var deferredValue1 = DeferredValue.Create(() => "352", "12", "Months in year");
            var deferredValue2 = DeferredValue.Create(() => "352", "42", "Answer to the Ultimate Question of Life, The Universe, and Everything");
            var deferredValue3 = DeferredValue.Create(() => "352", "56", "Random int");
            var deferredValues = new[] {deferredValue1, deferredValue2, deferredValue3};
            deferredValues.Wait().Transformed(x => Array.ConvertAll(x, int.Parse)).ThatFluent(x => x.Should().Contain(42, "answer is 42"));
        }

        [Test]
        public void Test42EnumerableOk()
        {
            var source = new ObjectWithComplexItems();
            source.Items.Select(x => x.Ydeferred).Wait().ThatFluent(x => x.Should().Equal(new[] {"42", "42", "42"}, "answer is 42"));
        }

        [Test]
        public void Test42EnumerableFailedBecauseNotLazy()
        {
            var source = new ObjectWithComplexItems();
            source.Items.ToArray().Select(x => x.Ydeferred).Wait().ThatFluent(x => x.Should().Equal(new[] {"42", "42", "42"}, "answer is 42"));
        }

        [Test]
        public void Test42EnumerableFailedBecauseOfValue()
        {
            var source = new ObjectWithComplexItems();
            source.Items.Select(x => x.Zdeferred).Wait().ThatFluent(x => x.Should().Equal(new[] {"42"}, "answer is 42"));
        }

        [Test]
        public void Test42Single()
        {
            var deferredValue1 = DeferredValue.Create(() => "352", "12", "Months in year");
            var deferredValue2 = DeferredValue.Create(() => "352", "42", "Answer to the Ultimate Question of Life, The Universe, and Everything");
            var deferredValue3 = DeferredValue.Create(() => "352", "56", "Random int");
            var deferredValues = new[] {deferredValue1, deferredValue2, deferredValue3};

            var result = deferredValues.Wait().Single(x => x.Should().Be("42", "answer is 42"), Helper.AssertionConfiguration);
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Test42DynamicCount()
        {
            var source = new ObjectWithComplexItems();
            var result = source.Items
                               .Wait()
                               .Filtered(x => x.Ydeferred.GetValue().Should().Be("42", "answer is 42"))
                               .ThatFluent(x => x.Should().HaveCount(3))
                               .GetAsserted();

            Assert.That(result, Is.EqualTo(new[] {"42", "42", "42"}));
        }

        [Test]
        public void Test42AnyFaild()
        {
            var source = new ObjectWithComplexItems();
            source.Items.Wait().Any(x => x.Xdeferred.GetValue().Should().Be("42", "answer is 42"), Helper.AssertionConfiguration);
        }

        [Test]
        public void Test42SingleFaild()
        {
            var source = new ObjectWithComplexItems();
            source.Items.Wait().Single(x => x.Xdeferred.GetValue().Should().Be("42", "answer is 42"), Helper.AssertionConfiguration);
        }
    }
}