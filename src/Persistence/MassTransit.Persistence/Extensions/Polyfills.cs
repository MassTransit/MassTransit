namespace MassTransit
{
#if !NET8_0_OR_GREATER
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> input, out TKey key, out TValue value)
        {
            key = input.Key;
            value = input.Value;
        }
    }
#endif
}
