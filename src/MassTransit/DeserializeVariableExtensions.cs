namespace MassTransit
{
    using System.Collections.Generic;
    using Util;


    public static class DeserializeVariableExtensions
    {
        /// <summary>
        /// Return an object from the dictionary converted to T using the message deserializer
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            if (!dictionary.TryGetValue(key, out var obj))
            {
                value = default;
                return false;
            }

            value = ObjectTypeDeserializer.Deserialize<T>(obj, default);
            return true;
        }

        /// <summary>
        /// Converts a PascalCase key to camelCase and attempts to get the value from the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValueCamelCase<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            if (TryGetValueCamelCase(dictionary, key, out var obj))
            {
                value = ObjectTypeDeserializer.Deserialize<T>(obj, default);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Converts a PascalCase key to camelCase and attempts to get the value from the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValueCamelCase(this IDictionary<string, object> dictionary, string key, out object value)
        {
            if (char.IsUpper(key[0]))
            {
                char[] chars = key.ToCharArray();
                chars[0] = char.ToLower(chars[0]);

                key = new string(chars);
                return dictionary.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }
    }
}
