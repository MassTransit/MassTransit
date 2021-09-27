namespace MassTransit.MessageData.PropertyProviders
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Serialization;


    public class ObjectMessageDataConverter<T> :
        IMessageDataConverter<T>
    {
        public Task<T> Convert(Stream stream, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
            using var jsonReader = new JsonTextReader(reader);

            var value = JsonMessageSerializer.Deserializer.Deserialize<T>(jsonReader);

            return Task.FromResult(value);
        }
    }
}
