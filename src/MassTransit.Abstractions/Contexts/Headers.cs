#nullable enable
namespace MassTransit
{
    using System.Collections.Generic;


    /// <summary>
    /// Headers are values outside of a message body transferred with the message.
    /// </summary>
    public interface Headers :
        IEnumerable<HeaderValue>
    {
        /// <summary>
        /// Returns all available headers
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> GetAll();

        /// <summary>
        /// If the specified header name is found, returns the value of the header as an object
        /// </summary>
        /// <param name="key">The header name</param>
        /// <param name="value">The output header value</param>
        /// <returns>True if the header is present, otherwise false</returns>
        bool TryGetHeader(string key, out object? value);

        /// <summary>
        /// Returns the specified header as the type, returning a default value is the header is not found
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="key">The header name</param>
        /// <param name="defaultValue">The default value of the header if not found</param>
        /// <returns>The header value</returns>
        T? Get<T>(string key, T? defaultValue = default)
            where T : class;

        /// <summary>
        /// Returns the specified header as the type, returning a default value is the header is not found
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="key">The header name</param>
        /// <param name="defaultValue">The default value of the header if not found</param>
        /// <returns>The header value</returns>
        T? Get<T>(string key, T? defaultValue = default)
            where T : struct;
    }
}
