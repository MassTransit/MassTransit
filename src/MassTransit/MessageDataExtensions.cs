namespace MassTransit
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageData;
    using MessageData.Values;


    public static class MessageDataExtensions
    {
        public static async Task<MessageData<string>> PutString(this IMessageDataRepository repository, string value,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (value == null)
                return new EmptyMessageData<string>();

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(value), false);

            Uri address = await repository.Put(ms, default, cancellationToken).ConfigureAwait(false);

            return new StoredMessageData<string>(address, value);
        }

        public static async Task<MessageData<byte[]>> PutBytes(this IMessageDataRepository repository, byte[] bytes,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            using var ms = new MemoryStream(bytes, false);

            Uri address = await repository.Put(ms, default, cancellationToken).ConfigureAwait(false);

            return new StoredMessageData<byte[]>(address, bytes);
        }

        public static async Task<MessageData<string>> PutString(this IMessageDataRepository repository, string value, TimeSpan timeToLive,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (value == null)
                return new EmptyMessageData<string>();

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(value), false);

            Uri address = await repository.Put(ms, timeToLive, cancellationToken).ConfigureAwait(false);

            return new StoredMessageData<string>(address, value);
        }

        public static async Task<MessageData<byte[]>> PutBytes(this IMessageDataRepository repository, byte[] bytes, TimeSpan timeToLive,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            using var ms = new MemoryStream(bytes, false);

            Uri address = await repository.Put(ms, timeToLive, cancellationToken).ConfigureAwait(false);

            return new StoredMessageData<byte[]>(address, bytes);
        }

        public static async Task<MessageData<string>> GetString(this IMessageDataRepository repository, Uri address,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            using var ms = new MemoryStream();

            Stream stream = await repository.Get(address, cancellationToken).ConfigureAwait(false);

            await stream.CopyToAsync(ms).ConfigureAwait(false);

            return new StoredMessageData<string>(address, Encoding.UTF8.GetString(ms.ToArray()));
        }

        public static async Task<MessageData<byte[]>> GetBytes(this IMessageDataRepository repository, Uri address,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            using var ms = new MemoryStream();

            Stream stream = await repository.Get(address, cancellationToken).ConfigureAwait(false);

            await stream.CopyToAsync(ms).ConfigureAwait(false);

            return new StoredMessageData<byte[]>(address, ms.ToArray());
        }
    }
}
