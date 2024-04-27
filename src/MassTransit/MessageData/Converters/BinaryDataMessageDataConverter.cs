namespace MassTransit.MessageData.Converters
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;


    public class BinaryDataMessageDataConverter :
        IMessageDataConverter<BinaryData>
    {
        public async Task<BinaryData> Convert(Stream stream, CancellationToken cancellationToken)
        {
            var result = await BinaryData.FromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
