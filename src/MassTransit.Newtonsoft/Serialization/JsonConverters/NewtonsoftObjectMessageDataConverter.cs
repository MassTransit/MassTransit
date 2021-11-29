namespace MassTransit.Serialization.JsonConverters
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;
    using Newtonsoft.Json;


    public class NewtonsoftObjectMessageDataConverter<T> :
        IMessageDataConverter<T>
    {
        public Task<T> Convert(Stream stream, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
            using var jsonReader = new JsonTextReader(reader);

            var value = NewtonsoftJsonMessageSerializer.Deserializer.Deserialize<T>(jsonReader);

            return Task.FromResult(value);
        }
    }
}
