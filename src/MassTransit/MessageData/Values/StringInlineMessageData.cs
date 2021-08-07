namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading.Tasks;
    using Serialization.JsonConverters;


    public class StringInlineMessageData :
        MessageData<string>,
        IInlineMessageData
    {
        readonly string _value;

        public StringInlineMessageData(string value, Uri address = null)
        {
            Address = address;
            _value = value;

            Value = Task.FromResult(value);
        }

        public void Set(IMessageDataReference reference)
        {
            reference.Text = _value;
            reference.Data = default;
        }

        public Uri Address { get; }

        public bool HasValue => true;

        public Task<string> Value { get; }
    }
}
