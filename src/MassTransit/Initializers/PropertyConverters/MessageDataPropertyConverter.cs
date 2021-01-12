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
        public Task<MessageData<byte[]>> Convert<T>(InitializeContext<T> context, byte[] input)
            where T : class
        {
            if (input == null)
                return TaskUtil.Default<MessageData<byte[]>>();

            return Task.FromResult<MessageData<byte[]>>(new PutMessageData<byte[]>(input));
        }

        public Task<MessageData<byte[]>> Convert<T>(InitializeContext<T> context, MessageData<byte[]> input)
            where T : class
        {
            return input != null
                ? Task.FromResult(input)
                : TaskUtil.Default<MessageData<byte[]>>();
        }

        async Task<MessageData<byte[]>> IPropertyConverter<MessageData<byte[]>, MessageData<string>>.Convert<T>(InitializeContext<T> context,
            MessageData<string> input)
        {
            if (input == null || !input.HasValue)
                return default;

            var text = await input.Value.ConfigureAwait(false);

            byte[] bytes = Encoding.UTF8.GetBytes(text);

            if (bytes.Length < MessageDataDefaults.Threshold)
                return new BytesInlineMessageData(bytes, input.Address);

            return new StoredMessageData<byte[]>(input.Address, bytes);
        }

        Task<MessageData<byte[]>> IPropertyConverter<MessageData<byte[]>, string>.Convert<T>(InitializeContext<T> context, string input)
        {
            if (input == null)
                return TaskUtil.Default<MessageData<byte[]>>();

            byte[] bytes = Encoding.UTF8.GetBytes(input);

            if (bytes.Length < MessageDataDefaults.Threshold)
                return Task.FromResult<MessageData<byte[]>>(new BytesInlineMessageData(bytes));

            return Task.FromResult<MessageData<byte[]>>(new PutMessageData<byte[]>(bytes));
        }

        Task<MessageData<string>> IPropertyConverter<MessageData<string>, MessageData<string>>.Convert<T>(InitializeContext<T> context,
            MessageData<string> input)
        {
            return Task.FromResult(input);
        }

        Task<MessageData<string>> IPropertyConverter<MessageData<string>, string>.Convert<T>(InitializeContext<T> context, string input)
        {
            if (input == null)
                return TaskUtil.Default<MessageData<string>>();

            return Task.FromResult<MessageData<string>>(new PutMessageData<string>(input));
        }

        public Task<MessageData<Stream>> Convert<T>(InitializeContext<T> context, Stream input)
            where T : class
        {
            if (input == null)
                return TaskUtil.Default<MessageData<Stream>>();

            return Task.FromResult<MessageData<Stream>>(new PutMessageData<Stream>(input));
        }

        public Task<MessageData<Stream>> Convert<T>(InitializeContext<T> context, MessageData<Stream> input)
            where T : class
        {
            return input != null
                ? Task.FromResult(input)
                : TaskUtil.Default<MessageData<Stream>>();
        }
    }
}
