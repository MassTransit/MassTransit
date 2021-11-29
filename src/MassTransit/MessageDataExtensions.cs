namespace MassTransit
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageData.Values;
    using Serialization;


    public static class MessageDataExtensions
    {
        public static Task<MessageData<string>> PutString(this IMessageDataRepository repository, string value,
            CancellationToken cancellationToken = default)
        {
            return PutString(repository, value, default, cancellationToken);
        }

        public static async Task<MessageData<string>> PutString(this IMessageDataRepository repository, string value, TimeSpan? timeToLive,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (value == null)
                return EmptyMessageData<string>.Instance;

            var bytesCount = Encoding.UTF8.GetByteCount(value);
            if (bytesCount < MessageDataDefaults.Threshold && !MessageDataDefaults.AlwaysWriteToRepository)
                return new StringInlineMessageData(value);

            var bytes = Encoding.UTF8.GetBytes(value);

            using var ms = new MemoryStream(bytes, false);

            var address = await repository.Put(ms, timeToLive, cancellationToken).ConfigureAwait(false);

            if (bytesCount < MessageDataDefaults.Threshold)
                return new StringInlineMessageData(value, address);

            return new StoredMessageData<string>(address, value);
        }

        public static Task<MessageData<byte[]>> PutBytes(this IMessageDataRepository repository, byte[] bytes,
            CancellationToken cancellationToken = default)
        {
            return PutBytes(repository, bytes, default, cancellationToken);
        }

        public static async Task<MessageData<byte[]>> PutBytes(this IMessageDataRepository repository, byte[] bytes, TimeSpan? timeToLive,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (bytes == null)
                return EmptyMessageData<byte[]>.Instance;

            if (bytes.Length < MessageDataDefaults.Threshold && !MessageDataDefaults.AlwaysWriteToRepository)
                return new BytesInlineMessageData(bytes);

            using var ms = new MemoryStream(bytes, false);

            var address = await repository.Put(ms, timeToLive, cancellationToken).ConfigureAwait(false);

            if (bytes.Length < MessageDataDefaults.Threshold)
                return new BytesInlineMessageData(bytes, address);

            return new StoredMessageData<byte[]>(address, bytes);
        }

        public static Task<IMessageData> PutObject(this IMessageDataRepository repository, object value, Type objectType,
            CancellationToken cancellationToken =
                default)
        {
            return PutObject(repository, value, objectType, default, cancellationToken);
        }

        public static async Task<IMessageData> PutObject(this IMessageDataRepository repository, object value, Type objectType, TimeSpan? timeToLive,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (value == null)
                return EmptyMessageData<byte[]>.Instance;

            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value, objectType, SystemTextJsonMessageSerializer.Options);

            if (bytes.Length < MessageDataDefaults.Threshold && !MessageDataDefaults.AlwaysWriteToRepository)
                return new BytesInlineMessageData(bytes);

            using var ms = new MemoryStream(bytes, false);

            var address = await repository.Put(ms, timeToLive, cancellationToken).ConfigureAwait(false);

            if (bytes.Length < MessageDataDefaults.Threshold)
                return new BytesInlineMessageData(bytes, address);

            return new StoredMessageData<byte[]>(address, bytes);
        }

        public static Task<MessageData<Stream>> PutStream(this IMessageDataRepository repository, Stream stream,
            CancellationToken cancellationToken = default)
        {
            return PutStream(repository, stream, default, cancellationToken);
        }

        public static async Task<MessageData<Stream>> PutStream(this IMessageDataRepository repository, Stream stream, TimeSpan? timeToLive,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (stream == null)
                return EmptyMessageData<Stream>.Instance;

            var address = await repository.Put(stream, timeToLive, cancellationToken).ConfigureAwait(false);

            return new StoredMessageData<Stream>(address, stream);
        }

        public static async Task<MessageData<string>> GetString(this IMessageDataRepository repository, Uri address,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            using var ms = new MemoryStream();

            var stream = await repository.Get(address, cancellationToken).ConfigureAwait(false);

            await stream.CopyToAsync(ms).ConfigureAwait(false);

            return new StoredMessageData<string>(address, Encoding.UTF8.GetString(ms.ToArray()));
        }

        public static async Task<MessageData<byte[]>> GetBytes(this IMessageDataRepository repository, Uri address,
            CancellationToken cancellationToken = default)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            using var ms = new MemoryStream();

            var stream = await repository.Get(address, cancellationToken).ConfigureAwait(false);

            await stream.CopyToAsync(ms).ConfigureAwait(false);

            return new StoredMessageData<byte[]>(address, ms.ToArray());
        }
    }
}
