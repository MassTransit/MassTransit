namespace MassTransit.MessageData.PropertyProviders
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    public class StreamMessageDataConverter :
        IMessageDataConverter<Stream>
    {
        public Task<Stream> Convert(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(stream);
        }
    }
}
