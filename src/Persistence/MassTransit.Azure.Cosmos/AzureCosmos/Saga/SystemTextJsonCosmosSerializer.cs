namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using System.IO;
    using System.Text.Json;
    using Microsoft.Azure.Cosmos;


    /// <summary>
    /// The default serializer, using System.Text.Json
    /// </summary>
    public class SystemTextJsonCosmosSerializer :
        CosmosSerializer
    {
        /// <summary>
        /// Create a serializer that uses the JSON.net serializer
        /// </summary>
        /// <remarks>
        /// This is internal to reduce exposure of JSON.net types so
        /// it is easier to convert to System.Text.Json
        /// </remarks>
        public SystemTextJsonCosmosSerializer(JsonSerializerOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public JsonSerializerOptions Options { get; }

        /// <summary>
        /// Convert a Stream to the passed in type.
        /// </summary>
        /// <typeparam name="T">The type of object that should be deserialized</typeparam>
        /// <param name="stream">An open stream that is readable that contains JSON</param>
        /// <returns>The object representing the deserialized stream</returns>
        public override T FromStream<T>(Stream stream)
        {
            if (typeof(Stream).IsAssignableFrom(typeof(T)))
                return (T)(object)stream;

            using (stream)
            {
                return JsonSerializer.Deserialize<T>(stream, Options);
            }
        }

        /// <summary>
        /// Converts an object to a open readable stream
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="input">The object to be serialized</param>
        /// <returns>An open readable stream containing the JSON of the serialized object</returns>
        public override Stream ToStream<T>(T input)
        {
            var stream = new MemoryStream();

            JsonSerializer.Serialize(stream, input, Options);

            stream.Position = 0;
            return stream;
        }
    }
}
