namespace MassTransit.Caching
{
    /// <summary>
    /// Returns the key for a value
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public delegate TKey KeyProvider<out TKey, in TValue>(TValue value);
}
