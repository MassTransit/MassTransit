namespace MassTransit.Serialization
{
    using System.Collections.Generic;


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
            where T : class
        {
            if (dictionary is null)
            {
                value = null;
                return false;
            }

            return SystemTextJsonMessageSerializer.Instance.TryGetValue(dictionary, key, out value);
        }

        /// <summary>
        /// Return an object from the dictionary converted to T using the message deserializer
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T? value)
            where T : struct
        {
            if (dictionary is null)
            {
                value = null;
                return false;
            }

            return SystemTextJsonMessageSerializer.Instance.TryGetValue(dictionary, key, out value);
        }
    }
}
