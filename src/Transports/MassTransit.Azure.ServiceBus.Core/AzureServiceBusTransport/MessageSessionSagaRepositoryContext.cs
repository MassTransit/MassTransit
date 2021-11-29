namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Saga;
    using Serialization;


    public class MessageSessionSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<MessageSessionContext, TSaga> _factory;
        readonly MessageSessionContext _sessionContext;

        public MessageSessionSagaRepositoryContext(ConsumeContext<TMessage> consumeContext, ISagaConsumeContextFactory<MessageSessionContext, TSaga> factory)
            : base(consumeContext)
        {
            if (!consumeContext.TryGetPayload(out MessageSessionContext sessionContext))
            {
                throw new SagaException($"The session-based saga repository requires an active message session: {TypeCache<TSaga>.ShortName}",
                    typeof(TSaga), typeof(TMessage));
            }

            _consumeContext = consumeContext;
            _sessionContext = sessionContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_sessionContext, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_sessionContext, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            return Task.FromResult<SagaConsumeContext<TSaga, TMessage>>(default);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await ReadSagaState(_sessionContext).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_sessionContext, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return WriteSagaState(_sessionContext, context.Saga);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return WriteSagaState(_sessionContext, context.Saga);
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            await _sessionContext.SetStateAsync(null).ConfigureAwait(false);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        static Task WriteSagaState(MessageSessionContext context, TSaga saga)
        {
            return context.SetStateAsync(BinaryData.FromObjectAsJson(saga, SystemTextJsonMessageSerializer.Options));
        }

        static async Task<TSaga> ReadSagaState(MessageSessionContext context)
        {
            var state = await context.GetStateAsync().ConfigureAwait(false);

            return state?.ToObjectFromJson<TSaga>(SystemTextJsonMessageSerializer.Options);
        }
    }
}
