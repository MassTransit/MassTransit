namespace MassTransit.MessageData
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    public class InMemoryMessageDataRepository :
        IMessageDataRepository
    {
        readonly ConcurrentDictionary<Uri, byte[]> _values;

        public InMemoryMessageDataRepository()
        {
            _values = new ConcurrentDictionary<Uri, byte[]>();
        }

        Task<Stream> IMessageDataRepository.Get(Uri address, CancellationToken cancellationToken)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            if (_values.TryGetValue(address, out byte[] value))
                return Task.FromResult<Stream>(new MemoryStream(value, false));

            throw new MessageDataNotFoundException(address);
        }

        async Task<Uri> IMessageDataRepository.Put(Stream stream, TimeSpan? timeToLive, CancellationToken cancellationToken)
        {
            Uri address = new InMemoryMessageDataId().Uri;

            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms).ConfigureAwait(false);

            _values.TryAdd(address, ms.ToArray());

            return address;
        }
    }
}
