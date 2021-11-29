namespace MassTransit.MessageData.Values
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;


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

        public void Set(IMessageDataReference reference)
        {
            reference.Text = default;
            reference.Data = _value;
        }

        public Uri Address { get; }

        public bool HasValue => true;

        public Task<byte[]> Value { get; }
    }


    public class BytesInlineMessageData<T> :
        MessageData<T>,
        IInlineMessageData
    {
        readonly IMessageDataConverter<T> _converter;
        readonly byte[] _value;
        readonly Lazy<Task<T>> _valueTask;

        public BytesInlineMessageData(IMessageDataConverter<T> converter, byte[] value, Uri address = null)
        {
            Address = address;
            _value = value;

            _valueTask = new Lazy<Task<T>>(() => GetValue());

            _converter = converter;
        }

        public void Set(IMessageDataReference reference)
        {
            reference.Text = default;
            reference.Data = _value;
        }

        public Uri Address { get; }

        public bool HasValue => true;

        public Task<T> Value => _valueTask.Value;

        async Task<T> GetValue()
        {
            using var stream = new MemoryStream(_value, false);

            return await _converter.Convert(stream, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
