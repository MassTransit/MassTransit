namespace MassTransit.Initializers.PropertyConverters
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using MessageData.Values;
    using Util;


    public class MessageDataPropertyConverter :
        IPropertyConverter<MessageData<byte[]>, MessageData<byte[]>>,
        IPropertyConverter<MessageData<byte[]>, MessageData<string>>,
        IPropertyConverter<MessageData<string>, MessageData<string>>,
        IPropertyConverter<MessageData<Stream>, MessageData<Stream>>,
        IPropertyConverter<MessageData<string>, string>,
        IPropertyConverter<MessageData<byte[]>, string>,
        IPropertyConverter<MessageData<byte[]>, byte[]>,
        IPropertyConverter<MessageData<Stream>, Stream>
    {
        public static readonly MessageDataPropertyConverter Instance = new MessageDataPropertyConverter();

        MessageDataPropertyConverter()
        {
        }

        public Task<MessageData<byte[]>> Convert<T>(InitializeContext<T> context, byte[] input)
            where T : class
        {
            return input == null
                ? TaskUtil.Default<MessageData<byte[]>>()
                : Task.FromResult<MessageData<byte[]>>(new PutMessageData<byte[]>(input));
        }

        public Task<MessageData<byte[]>> Convert<T>(InitializeContext<T> context, MessageData<byte[]> input)
            where T : class
        {
            return input == null
                ? TaskUtil.Default<MessageData<byte[]>>()
                : Task.FromResult(input);
        }

        async Task<MessageData<byte[]>> IPropertyConverter<MessageData<byte[]>, MessageData<string>>.Convert<T>(InitializeContext<T> context,
            MessageData<string> input)
        {
            if (input == null || !input.HasValue)
                return null;

            var text = await input.Value.ConfigureAwait(false);

            var bytes = Encoding.UTF8.GetBytes(text);

            return bytes.Length < MessageDataDefaults.Threshold
                ? (MessageData<byte[]>)new BytesInlineMessageData(bytes, input.Address)
                : new StoredMessageData<byte[]>(input.Address, bytes);
        }

        Task<MessageData<byte[]>> IPropertyConverter<MessageData<byte[]>, string>.Convert<T>(InitializeContext<T> context, string input)
        {
            if (input == null)
                return TaskUtil.Default<MessageData<byte[]>>();

            var bytes = Encoding.UTF8.GetBytes(input);

            return Task.FromResult(bytes.Length < MessageDataDefaults.Threshold
                ? (MessageData<byte[]>)new BytesInlineMessageData(bytes)
                : new PutMessageData<byte[]>(bytes));
        }

        public Task<MessageData<Stream>> Convert<T>(InitializeContext<T> context, MessageData<Stream> input)
            where T : class
        {
            return input == null
                ? TaskUtil.Default<MessageData<Stream>>()
                : Task.FromResult(input);
        }

        public Task<MessageData<Stream>> Convert<T>(InitializeContext<T> context, Stream input)
            where T : class
        {
            return input == null
                ? TaskUtil.Default<MessageData<Stream>>()
                : Task.FromResult<MessageData<Stream>>(new PutMessageData<Stream>(input));
        }

        Task<MessageData<string>> IPropertyConverter<MessageData<string>, MessageData<string>>.Convert<T>(InitializeContext<T> context,
            MessageData<string> input)
        {
            return Task.FromResult(input);
        }

        Task<MessageData<string>> IPropertyConverter<MessageData<string>, string>.Convert<T>(InitializeContext<T> context, string input)
        {
            return input == null
                ? TaskUtil.Default<MessageData<string>>()
                : Task.FromResult<MessageData<string>>(new PutMessageData<string>(input));
        }
    }


    public class MessageDataPropertyConverter<TValue> :
        IPropertyConverter<MessageData<TValue>, MessageData<TValue>>,
        IPropertyConverter<MessageData<TValue>, TValue>
        where TValue : class
    {
        public Task<MessageData<TValue>> Convert<T>(InitializeContext<T> context, MessageData<TValue> input)
            where T : class
        {
            return input == null
                ? TaskUtil.Default<MessageData<TValue>>()
                : Task.FromResult(input);
        }

        public Task<MessageData<TValue>> Convert<T1>(InitializeContext<T1> context, TValue input)
            where T1 : class
        {
            return input == null
                ? TaskUtil.Default<MessageData<TValue>>()
                : Task.FromResult<MessageData<TValue>>(new PutMessageData<TValue>(input));
        }
    }
}
