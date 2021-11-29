namespace MassTransit.MessageData.PropertyProviders
{
    using System.Threading.Tasks;
    using Initializers;
    using Util;
    using Values;


    public class GetMessageDataPropertyProvider<TInput, TValue> :
        IPropertyProvider<TInput, MessageData<TValue>>
        where TInput : class
    {
        readonly IPropertyProvider<TInput, MessageData<TValue>> _inputProvider;
        readonly IMessageDataReader<TValue> _reader;
        readonly IMessageDataRepository _repository;

        public GetMessageDataPropertyProvider(IPropertyProvider<TInput, MessageData<TValue>> inputProvider, IMessageDataRepository repository = default)
        {
            _repository = repository;
            _inputProvider = inputProvider;

            _reader = MessageDataReaderFactory.CreateReader<TValue>();
        }

        public Task<MessageData<TValue>> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.HasInput)
                return TaskUtil.Default<MessageData<TValue>>();

            Task<MessageData<TValue>> inputTask = _inputProvider.GetProperty(context);
            if (inputTask.IsCompleted)
            {
                MessageData<TValue> messageData = inputTask.Result;
                if (messageData is IInlineMessageData)
                    return Task.FromResult(messageData);

                if (messageData is { HasValue: true } && messageData.Address != null)
                {
                    var repository = _repository;
                    if (repository != null || context.TryGetPayload(out repository))
                        return Task.FromResult(_reader.GetMessageData(repository, messageData.Address, context.CancellationToken));
                }

                return Task.FromResult(EmptyMessageData<TValue>.Instance);
            }

            async Task<MessageData<TValue>> GetPropertyAsync()
            {
                MessageData<TValue> messageData = await inputTask.ConfigureAwait(false);
                if (messageData is IInlineMessageData)
                    return messageData;

                if (messageData?.Address != null)
                {
                    var repository = _repository;
                    if (repository != null || context.TryGetPayload(out repository))
                        return _reader.GetMessageData(repository, messageData.Address, context.CancellationToken);
                }

                return EmptyMessageData<TValue>.Instance;
            }

            return GetPropertyAsync();
        }
    }
}
