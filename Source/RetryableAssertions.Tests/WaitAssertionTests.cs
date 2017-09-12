using System;
using System.Linq;
using Kontur.RetryableAssertions.Configuration;
using Kontur.RetryableAssertions.ValueProviding;
using NSubstitute;
using NUnit.Framework;

namespace Kontur.RetryableAssertions.Tests
{
    public class WaitAssertionTests
    {
        [Test]
        public void ZeroTimeoutAndAssertionSatisfied_AssertionCalledOnce()
        {
            var configuration = Substitute.For<IAssertionConfiguration<int>>();
            configuration.Timeout.Returns(0);

            Wait.Assertion(ValueProvider.Create(() => 1), configuration);

            configuration.Assertion.ReceivedWithAnyArgs().Assert(default(int));
            configuration.ExceptionMatcher.DidNotReceiveWithAnyArgs().RetryOnException(default(Exception));
        }

        [Test]
        public void ZeroTimeoutAndAssertionFailed_AssertionCalledOnce()
        {
            var configuration = Substitute.For<IAssertionConfiguration<int>>();
            configuration.Timeout.Returns(0);
            configuration.ExceptionMatcher.RetryOnException(default(Exception)).ReturnsForAnyArgs(true);
            configuration.Assertion.WhenForAnyArgs(x => x.Assert(default(int))).Do(x => throw new Exception());

            Assert.Throws<Exception>(() => Wait.Assertion(ValueProvider.Create(() => 1), configuration));

            configuration.Assertion.ReceivedWithAnyArgs().Assert(default(int));
        }

        [Test]
        public void AssertionSatisfiedFirstTime_RetryUntillAssertionSatisfied()
        {
            var configuration = Substitute.For<IAssertionConfiguration<int>>();
            configuration.Timeout.Returns(100);
            configuration.Interval.Returns(10);

            Wait.Assertion(ValueProvider.Create(() => 1), configuration);

            configuration.Assertion.ReceivedWithAnyArgs().Assert(default(int));
            configuration.ExceptionMatcher.DidNotReceiveWithAnyArgs().RetryOnException(default(Exception));
        }

        [Test]
        public void AssertionSatisfiedDeferred_RetryUntillAssertionSatisfied()
        {
            var times = 3;

            var configuration = Substitute.For<IAssertionConfiguration<int>>();
            configuration.Timeout.Returns(100);
            configuration.Interval.Returns(10);
            configuration.ExceptionMatcher.RetryOnException(null).ReturnsForAnyArgs(true);
            configuration.Assertion.WhenForAnyArgs(x => x.Assert(default(int)))
                         .Do(x =>
                         {
                             if (--times != 0)
                             {
                                 throw new Exception();
                             }
                         });

            Wait.Assertion(ValueProvider.Create(() => 1), configuration);

            configuration.Assertion.ReceivedWithAnyArgs(3).Assert(default(int));
        }

        [Test]
        public void AssertionAlwaysFailed_RetryUntillTimeout()
        {
            var configuration = Substitute.For<IAssertionConfiguration<int>>();
            configuration.Timeout.Returns(100);
            configuration.Interval.Returns(10);
            configuration.ExceptionMatcher.RetryOnException(null).ReturnsForAnyArgs(true);
            configuration.Assertion.WhenForAnyArgs(x => x.Assert(default(int)))
                         .Do(x => throw new Exception());

            Assert.Throws<Exception>(() => Wait.Assertion(ValueProvider.Create(() => 1), configuration));

            Assert.That(configuration.Assertion.ReceivedCalls().Count(), Is.GreaterThan(5).And.LessThan(15));
        }

        [Test]
        public void AssertionAlwaysFailed_RetryUntillException()
        {
            var times = 3;
            var configuration = Substitute.For<IAssertionConfiguration<int>>();
            configuration.Timeout.Returns(100);
            configuration.Interval.Returns(10);
            configuration.ExceptionMatcher.RetryOnException(null).ReturnsForAnyArgs(x => --times != 0);
            configuration.Assertion.WhenForAnyArgs(x => x.Assert(default(int))).Do(x => throw new Exception());

            Assert.Throws<Exception>(() => Wait.Assertion(ValueProvider.Create(() => 1), configuration));

            configuration.Assertion.ReceivedWithAnyArgs(3).Assert(default(int));
        }

        [Test]
        public void AssertionReceivesActualValue()
        {
            var configuration = Substitute.For<IAssertionConfiguration<string>>();
            configuration.Timeout.Returns(0);

            var valueProvider = Substitute.For<IValueProvider<string, int>>();
            valueProvider.GetValue().Returns("zzz");

            Wait.Assertion(valueProvider, configuration);

            configuration.Assertion.Received().Assert("zzz");
        }
    }
}