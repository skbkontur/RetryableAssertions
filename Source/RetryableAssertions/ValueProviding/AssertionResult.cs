namespace Kontur.RetryableAssertions.ValueProviding
{
    internal class AssertionResult<TAsserted, TSource> : IAssertionResult<TAsserted, TSource>
    {
        private readonly TAsserted asserted;
        private readonly TSource source;

        public AssertionResult(IValueProvider<TAsserted, TSource> valueProvider)
            : this(valueProvider.LastValue(), valueProvider.Source.LastValue())
        {
        }

        public AssertionResult(TAsserted asserted, TSource source)
        {
            this.asserted = asserted;
            this.source = source;
        }

        public TAsserted GetAsserted()
        {
            return asserted;
        }

        public TSource GetSource()
        {
            return source;
        }
    }
}