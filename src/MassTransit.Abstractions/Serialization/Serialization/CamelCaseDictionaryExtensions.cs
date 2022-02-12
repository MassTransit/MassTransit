namespace MassTransit.Serialization
{
    using System.Collections.Generic;


    public static class CamelCaseDictionaryExtensions
    {
        /// <summary>
        /// Converts a PascalCase key to camelCase and attempts to get the value from the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValueCamelCase(this IDictionary<string, object>? dictionary, string key, out object? value)
        {
            if (dictionary != null && char.IsUpper(key[0]))
            {
                var chars = key.ToCharArray();
                chars[0] = char.ToLower(chars[0]);

                key = new string(chars);
                return dictionary.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Converts a PascalCase key to camelCase and attempts to get the value from the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValueCamelCase(this IReadOnlyDictionary<string, object>? dictionary, string key, out object? value)
        {
            if (dictionary != null && char.IsUpper(key[0]))
            {
                var chars = key.ToCharArray();
                chars[0] = char.ToLower(chars[0]);

                key = new string(chars);
                return dictionary.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }
    }
}
