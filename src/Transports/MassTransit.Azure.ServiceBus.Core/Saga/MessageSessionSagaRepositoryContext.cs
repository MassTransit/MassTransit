namespace MassTransit.Azure.ServiceBus.Core.Saga
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Saga;
    using Metadata;
    using Newtonsoft.Json;
    using Serialization;
    using Util;


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
                throw new SagaException($"The session-based saga repository requires an active message session: {TypeMetadataCache<TSaga>.ShortName}",
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
            return TaskUtil.Completed;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return TaskUtil.Completed;
        }

        static async Task WriteSagaState(MessageSessionContext context, TSaga saga)
        {
            using var serializeStream = new MemoryStream();
            using var writer = new StreamWriter(serializeStream, Encoding.UTF8, 1024, true);
            using var bsonWriter = new JsonTextWriter(writer);
            JsonMessageSerializer.Serializer.Serialize(bsonWriter, saga);

            await bsonWriter.FlushAsync().ConfigureAwait(false);
            await serializeStream.FlushAsync().ConfigureAwait(false);

            await context.SetStateAsync(serializeStream.ToArray()).ConfigureAwait(false);
        }

        static async Task<TSaga> ReadSagaState(MessageSessionContext context)
        {
            var state = await context.GetStateAsync().ConfigureAwait(false);
            if (state == null)
                return default;

            using var stateStream = new MemoryStream(state);
            if (stateStream.Length == 0)
                return default;

            using var reader = new StreamReader(stateStream, Encoding.UTF8, false, 1024, true);
            using var bsonReader = new JsonTextReader(reader);
            return JsonMessageSerializer.Deserializer.Deserialize<TSaga>(bsonReader);
        }
    }
}
