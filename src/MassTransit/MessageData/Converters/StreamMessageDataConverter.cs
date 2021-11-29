namespace MassTransit.MessageData.Converters
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;


    public class StreamMessageDataConverter :
        IMessageDataConverter<Stream>
    {
        public Task<Stream> Convert(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(stream);
        }
    }
}
