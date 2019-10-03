namespace MassTransit.MessageData
{
    using System.Threading.Tasks;
    using Initializers;
    using Util;


    public class LoadMessageDataPropertyProvider<TInput, TValue> :
        IPropertyProvider<TInput, MessageData<TValue>>
        where TInput : class
    {
        readonly IPropertyProvider<TInput, MessageData<TValue>> _inputProvider;
        readonly IMessageDataRepository _repository;

        public LoadMessageDataPropertyProvider(IPropertyProvider<TInput, MessageData<TValue>> inputProvider, IMessageDataRepository repository = default)
        {
            _repository = repository;
            _inputProvider = inputProvider;
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

                if (messageData?.Address != null)
                {
                    var repository = _repository;
                    if (repository != null || context.TryGetPayload(out repository))
                        return Task.FromResult(MessageDataFactory.Load<TValue>(repository, messageData.Address, context.CancellationToken));
                }

                return Task.FromResult<MessageData<TValue>>(new EmptyMessageData<TValue>());
            }

            async Task<MessageData<TValue>> GetPropertyAsync()
            {
                MessageData<TValue> messageData = await inputTask.ConfigureAwait(false);

                if (messageData?.Address != null)
                {
                    var repository = _repository;
                    if (repository != null || context.TryGetPayload(out repository))
                        return MessageDataFactory.Load<TValue>(repository, messageData.Address, context.CancellationToken);
                }

                return new EmptyMessageData<TValue>();
            }

            return GetPropertyAsync();
        }
    }
}
