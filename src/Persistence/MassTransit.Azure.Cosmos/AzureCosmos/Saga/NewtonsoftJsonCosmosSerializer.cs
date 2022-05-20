namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using System.IO;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    // https://github.com/Azure/azure-cosmos-dotnet-v3/issues/551


    /// <summary>
    /// The default Cosmos JSON.NET serializer.
    /// </summary>
    public class NewtonsoftJsonCosmosSerializer :
        CosmosSerializer
    {
        readonly JsonSerializer _serializer;

        /// <summary>
        /// Create a serializer that uses the JSON.net serializer
        /// </summary>
        /// <remarks>
        /// This is internal to reduce exposure of JSON.net types so
        /// it is easier to convert to System.Text.Json
        /// </remarks>
        public NewtonsoftJsonCosmosSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _serializer = JsonSerializer.Create(jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings)));
        }

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
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<T>(reader);
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
            using (var streamWriter = new StreamWriter(stream, MessageDefaults.Encoding, 1024, true))
            using (var writer = new JsonTextWriter(streamWriter) { Formatting = Formatting.None })
            {
                _serializer.Serialize(writer, input);

                writer.Flush();
                streamWriter.Flush();
            }

            stream.Position = 0;
            return stream;
        }
    }
}
