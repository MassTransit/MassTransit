namespace MassTransit.MessageData.Values
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;


    public class ToByteArrayMessageData :
        MessageData<byte[]>
    {
        readonly MessageData<string> _messageData;

        public ToByteArrayMessageData(MessageData<string> messageData)
        {
            _messageData = messageData;
        }

        public Uri Address => _messageData.Address;

        public bool HasValue => _messageData.HasValue;

        public Task<byte[]> Value => Convert(_messageData.Value);

        Task<byte[]> Convert(Task<string> value)
        {
            if (value.IsCompletedSuccessfully())
            {
                var text = value.Result;
                return text == null
                    ? Task.FromResult<byte[]>(null)
                    : Task.FromResult(Encoding.UTF8.GetBytes(text));
            }

            async Task<byte[]> ConvertAsync()
            {
                var text = await value.ConfigureAwait(false);
                return text == null
                    ? null
                    : Encoding.UTF8.GetBytes(text);
            }

            return ConvertAsync();
        }
    }
}
