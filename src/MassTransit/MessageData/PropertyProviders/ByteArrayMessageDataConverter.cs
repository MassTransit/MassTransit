namespace MassTransit.MessageData.PropertyProviders
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    public class ByteArrayMessageDataConverter :
        IMessageDataConverter<byte[]>
    {
        public async Task<byte[]> Convert(Stream stream, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms, 4096, cancellationToken).ConfigureAwait(false);

            return ms.ToArray();
        }
    }
}
