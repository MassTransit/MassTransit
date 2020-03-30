namespace MassTransit.MessageData.Values
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Gets the message data when accessed via Value, using the specified repository and converter.
    /// </summary>
    /// <typeparam name="T">The message data property type</typeparam>
    public class GetMessageData<T> :
        MessageData<T>
    {
        readonly Uri _address;
        readonly IMessageDataConverter<T> _converter;
        readonly IMessageDataRepository _repository;
        readonly Lazy<Task<T>> _value;
        readonly CancellationToken _cancellationToken;

        public GetMessageData(Uri address, IMessageDataRepository repository, IMessageDataConverter<T> converter, CancellationToken cancellationToken)
        {
            _address = address;
            _repository = repository;
            _converter = converter;

            _cancellationToken = cancellationToken;

            _value = new Lazy<Task<T>>(GetValue);
        }

        public Uri Address => _address;

        public bool HasValue => true;

        public Task<T> Value => _value.Value;

        async Task<T> GetValue()
        {
            using var valueStream = await _repository.Get(_address, _cancellationToken).ConfigureAwait(false);

            return await _converter.Convert(valueStream, _cancellationToken).ConfigureAwait(false);
        }
    }
}
