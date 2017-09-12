namespace Kontur.RetryableAssertions.ValueProviding
{
    public interface IAssertionResult<out TAsserted, out TSource>
    {
        TAsserted GetAsserted();
        TSource GetSource();
    }
}