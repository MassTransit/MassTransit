namespace MassTransit.MessageData.Converters
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;


    public class StringMessageDataConverter :
        IMessageDataConverter<string>
    {
        public async Task<string> Convert(Stream stream, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms, 4096, cancellationToken).ConfigureAwait(false);

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
