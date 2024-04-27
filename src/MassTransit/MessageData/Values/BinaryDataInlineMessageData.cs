namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;


    public class BinaryDataInlineMessageData :
        MessageData<BinaryData>,
        IInlineMessageData
    {
        readonly BinaryData _value;

        public BinaryDataInlineMessageData(BinaryData value, Uri address = null)
        {
            Address = address;
            _value = value;

            Value = Task.FromResult(value);
        }

        public void Set(IMessageDataReference reference)
        {
            reference.Text = default;
            reference.Data = _value.ToArray();
        }

        public Uri Address { get; }

        public bool HasValue => true;

        public Task<BinaryData> Value { get; }
    }
}
