namespace MassTransit
{
    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> input, out TKey key, out TValue value)
        {
            key = input.Key;
            value = input.Value;
        }
    }
}
