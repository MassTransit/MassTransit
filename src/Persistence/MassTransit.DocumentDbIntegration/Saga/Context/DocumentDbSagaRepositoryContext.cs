namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Context;
    using MassTransit.Saga;


    public class DocumentDbSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, IVersionedSaga
        where TMessage : class
    {
        readonly DatabaseContext<TSaga> _context;
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public DocumentDbSagaRepositoryContext(DatabaseContext<TSaga> context, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_context, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _context.Insert(instance, _consumeContext.CancellationToken).ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);
            }

            return null;
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _context.Load(correlationId, CancellationToken).ConfigureAwait(false);
            if (instance == null)
                return null;

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }
    }


    public class DocumentDbSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly DatabaseContext<TSaga> _context;

        public DocumentDbSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            // This will not work for Document Db because the .Where needs to look for [JsonProperty("id")],
            // and if you pass in CorrelationId property, the ISaga doesn't have that property. Can we .Select() it out?
            IEnumerable<TSaga> instances = await _context.Client.CreateDocumentQuery<TSaga>(_context.Collection, _context.FeedOptions)
                .Where(query.FilterExpression)
                .QueryAsync(cancellationToken)
                .ConfigureAwait(false);

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances.ToList());
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Load(correlationId, CancellationToken);
        }
    }
}
