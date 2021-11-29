namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    // https://github.com/Azure/azure-cosmos-dotnet-v3/issues/551


    /// <summary>
    /// The default Cosmos JSON.NET serializer.
    /// </summary>
    public class CosmosJsonDotNetSerializer :
        CosmosSerializer
    {
        static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
        readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Create a serializer that uses the JSON.net serializer
        /// </summary>
        /// <remarks>
        /// This is internal to reduce exposure of JSON.net types so
        /// it is easier to convert to System.Text.Json
        /// </remarks>
        public CosmosJsonDotNetSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _serializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
        }

        /// <summary>
        /// Convert a Stream to the passed in type.
        /// </summary>
        /// <typeparam name="T">The type of object that should be deserialized</typeparam>
        /// <param name="stream">An open stream that is readable that contains JSON</param>
        /// <returns>The object representing the deserialized stream</returns>
        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                    return (T)(object)stream;

                using (var sr = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        var jsonSerializer = GetSerializer();
                        return jsonSerializer.Deserialize<T>(jsonTextReader);
                    }
                }
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
            var streamPayload = new MemoryStream();
            using (var streamWriter = new StreamWriter(streamPayload, DefaultEncoding, 1024, true))
            {
                using (JsonWriter writer = new JsonTextWriter(streamWriter))
                {
                    writer.Formatting = Formatting.None;
                    var jsonSerializer = GetSerializer();
                    jsonSerializer.Serialize(writer, input);
                    writer.Flush();
                    streamWriter.Flush();
                }
            }

            streamPayload.Position = 0;
            return streamPayload;
        }

        /// <summary>
        /// JsonSerializer has hit a race conditions with custom settings that cause null reference exception.
        /// To avoid the race condition a new JsonSerializer is created for each call
        /// </summary>
        JsonSerializer GetSerializer()
        {
            return JsonSerializer.Create(_serializerSettings);
        }
    }
}
