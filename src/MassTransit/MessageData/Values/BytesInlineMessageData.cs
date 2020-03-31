namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;
    using Serialization.JsonConverters;


    public class BytesInlineMessageData :
        MessageData<byte[]>,
        IInlineMessageData
    {
        readonly byte[] _value;

        public BytesInlineMessageData(byte[] value, Uri address = null)
        {
            Address = address;
            _value = value;

            Value = Task.FromResult(value);
        }

        public Uri Address { get; }

        public bool HasValue => true;

        public Task<byte[]> Value { get; }

        public void Set(MessageDataReference reference)
        {
            reference.Text = default;
            reference.Data = _value;
        }
    }
}
