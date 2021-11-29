namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;

            value = valueFactory(key);
            dictionary.Add(key, value);

            return value;
        }

        public static IDictionary<TKey, TValue> MergeLeft<TKey, TValue>(this IDictionary<TKey, TValue> source, params IDictionary<TKey, TValue>[] others)
        {
            var result = new Dictionary<TKey, TValue>(source.Count);

            foreach (IDictionary<TKey, TValue> dictionary in new[] {source}.Concat(others))
            {
                foreach (KeyValuePair<TKey, TValue> element in dictionary)
                    result[element.Key] = element.Value;
            }

            return result;
        }
    }
}
