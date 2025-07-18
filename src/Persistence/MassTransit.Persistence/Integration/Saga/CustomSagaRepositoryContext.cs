namespace MassTransit.Persistence.Integration.Saga
{
    using Context;
    using Internals;
    using MassTransit.Saga;
    using Middleware;


    public class CustomSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>,
        IProbeSite
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly DatabaseContext<TSaga> _context;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public CustomSagaRepositoryContext(DatabaseContext<TSaga> context,
            ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(consumeContext, context)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("TSaga", typeof(TSaga).Name);
            context.Add("TMessage", typeof(TMessage).Name);
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_context, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            LogContext.Debug?.Log("Adding saga instance {correlationId}", instance.CorrelationId);

            return _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            LogContext.Debug?.Log("Inserting saga instance {correlationId}", instance.CorrelationId);

            await _context.InsertAsync(instance, CancellationToken)
                .ConfigureAwait(false);

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>?> Load(Guid correlationId)
        {
            LogContext.Debug?.Log("Attempting to load saga instance {correlationId}", correlationId);

            var instance = await _context.LoadAsync(correlationId, CancellationToken)
                .ConfigureAwait(false);

            if (instance == null)
            {
                LogContext.Debug?.Log("No saga instance found for {correlationId}", correlationId);
                return null;
            }

            LogContext.Debug?.Log("Saga instance found for {correlationId}", correlationId);
            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            LogContext.Debug?.Log("Saving saga instance {correlationId}", context.Saga.CorrelationId);

            return _context.InsertAsync(context.Saga, CancellationToken);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            LogContext.Debug?.Log("Updating saga instance {correlationId}", context.Saga.CorrelationId);

            return _context.UpdateAsync(context.Saga, CancellationToken);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            LogContext.Debug?.Log("Deleting saga instance {correlationId}", context.Saga.CorrelationId);

            return _context.DeleteAsync(context.Saga, CancellationToken);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            LogContext.Debug?.Log("Discarding saga instance {correlationId}", context.Saga.CorrelationId);

            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            LogContext.Debug?.Log("Reverting saga instance {correlationId}", context.Saga.CorrelationId);

            return Task.CompletedTask;
        }
    }


    public class CustomSagaRepositoryContext<TSaga> :
        BasePipeContext,
        QuerySagaRepositoryContext<TSaga>,
        LoadSagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;

        public CustomSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            var instance = await _context.LoadAsync(correlationId, CancellationToken)
                .ConfigureAwait(false);

            if (instance is null)
                LogContext.Debug?.Log("Missing saga instance {correlationId}", correlationId);
            else
                LogContext.Debug?.Log("Loaded saga instance {correlationId}", correlationId);

            // might still be null, but we can't change the upstream nullability without a major impact
            return instance!;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            var instances = await _context.QueryAsync(query.FilterExpression, cancellationToken).ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (LogContext.Debug.HasValue)
            {
                var expression = query.FilterExpression.ToExpressionString();

                LogContext.Debug?.Log("Loaded {count} matching saga instances for {expression}", instances.Count, expression);
            }

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances);
        }
    }
}
