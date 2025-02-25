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

        public static IDictionary<string, TValue> MergeLeft<TValue>(this IDictionary<string, TValue> source, params IDictionary<string, TValue>[] others)
        {
            var result = new Dictionary<string, TValue>(source.Count, StringComparer.OrdinalIgnoreCase);

            foreach (IDictionary<string, TValue> dictionary in new[] { source }.Concat(others))
            {
                foreach (KeyValuePair<string, TValue> element in dictionary)
                {
                    if (result.ContainsKey(element.Key))
                    {
                        if (element.Value != null)
                            result[element.Key] = element.Value;
                    }
                    else
                        result[element.Key] = element.Value;
                }
            }

            return result;
        }

        public static Dictionary<string, object> SetValue(this Dictionary<string, object> dictionary, string key, string? value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                dictionary.Remove(key);
            else
                dictionary[key] = value;

            return dictionary;
        }

        public static Dictionary<string, object> SetValue(this Dictionary<string, object> dictionary, string key, object? value, bool overwrite = true)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (overwrite)
            {
                if (value == null)
                    dictionary.Remove(key);
                else
                    dictionary[key] = value;
            }
            else if (!dictionary.ContainsKey(key) && value != null)
                dictionary.Add(key, value);

            return dictionary;
        }

        public static Dictionary<string, object> SetValues(this Dictionary<string, object> dictionary, IEnumerable<KeyValuePair<string, object?>>? properties,
            bool overwrite = true)
        {
            if (properties != null)
            {
                foreach (KeyValuePair<string, object?> header in properties)
                    SetValue(dictionary, header.Key, header.Value, overwrite);
            }

            return dictionary;
        }
    }
}
