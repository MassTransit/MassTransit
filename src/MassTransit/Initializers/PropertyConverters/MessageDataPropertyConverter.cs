namespace MassTransit.Initializers.PropertyConverters
{
    using System.Text;
    using System.Threading.Tasks;
    using MessageData.Values;
    using Util;


    public class MessageDataPropertyConverter :
        IPropertyConverter<MessageData<byte[]>, MessageData<byte[]>>,
        IPropertyConverter<MessageData<byte[]>, MessageData<string>>,
        IPropertyConverter<MessageData<string>, MessageData<string>>,
        IPropertyConverter<MessageData<string>, string>,
        IPropertyConverter<MessageData<byte[]>, string>,
        IPropertyConverter<MessageData<byte[]>, byte[]>
    {
        public Task<MessageData<byte[]>> Convert<T>(InitializeContext<T> context, MessageData<byte[]> input)
            where T : class
        {
            return input != null
                ? Task.FromResult(input)
                : TaskUtil.Default<MessageData<byte[]>>();
        }

        Task<MessageData<byte[]>> IPropertyConverter<MessageData<byte[]>, MessageData<string>>.Convert<T>(InitializeContext<T> context,
            MessageData<string> input)
        {
            return input != null
                ? Task.FromResult<MessageData<byte[]>>(new ToByteArrayMessageData(input))
                : TaskUtil.Default<MessageData<byte[]>>();
        }

        Task<MessageData<string>> IPropertyConverter<MessageData<string>, MessageData<string>>.Convert<T>(InitializeContext<T> context,
            MessageData<string> input)
        {
            return Task.FromResult(input);
        }

        Task<MessageData<string>> IPropertyConverter<MessageData<string>, string>.Convert<T>(InitializeContext<T> context, string input)
        {
            return input != null
                ? Task.FromResult<MessageData<string>>(new PutMessageData<string>(input))
                : TaskUtil.Default<MessageData<string>>();
        }

        Task<MessageData<byte[]>> IPropertyConverter<MessageData<byte[]>, string>.Convert<T>(InitializeContext<T> context, string input)
        {
            return input != null
                ? Task.FromResult<MessageData<byte[]>>(new PutMessageData<byte[]>(Encoding.UTF8.GetBytes(input)))
                : TaskUtil.Default<MessageData<byte[]>>();
        }

        public Task<MessageData<byte[]>> Convert<T>(InitializeContext<T> context, byte[] input)
            where T : class
        {
            return input != null
                ? Task.FromResult<MessageData<byte[]>>(new PutMessageData<byte[]>(input))
                : TaskUtil.Default<MessageData<byte[]>>();
        }
    }
}
