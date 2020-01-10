namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Util;
    using MassTransit.Saga;
    using NHibernate;
    using NHibernate.Exceptions;


    public class NHibernateSagaRepositoryContext<TSaga, TMessage> :
        SagaRepositoryContext<TSaga, TMessage>,
        IAsyncDisposable
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly NHibernateContext _context;
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<NHibernateContext, TSaga> _factory;

        public NHibernateSagaRepositoryContext(NHibernateContext context, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<NHibernateContext, TSaga> factory)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context,_consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _context.Session.SaveAsync(instance).ConfigureAwait(false);
                await _context.Session.FlushAsync().ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (GenericADOException ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _context.Session.GetAsync<TSaga>(correlationId, LockMode.Upgrade).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public async Task<SagaRepositoryQueryContext<TSaga, TMessage>> Query(ISagaQuery<TSaga> query)
        {
            IList<Guid> instances = await _context.Session.QueryOver<TSaga>()
                .Where(query.FilterExpression)
                .Select(x => x.CorrelationId)
                .ListAsync<Guid>()
                .ConfigureAwait(false);

            return new DefaultSagaRepositoryQueryContext<TSaga, TMessage>(this, instances);
        }

        public async Task Faulted(Exception exception)
        {
            try
            {
                await _context.Transaction.RollbackAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(ex, "Transaction rollback faulted");
            }
        }

        async Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            if (_context.Transaction.IsActive)
                await _context.Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            _context.Transaction.Dispose();
            _context.Session.Dispose();
        }
    }


    public class NHibernateSagaRepositoryContext<TSaga> :
        SagaRepositoryContext<TSaga>,
        IAsyncDisposable
        where TSaga : class, ISaga
    {
        readonly NHibernateContext _context;

        public NHibernateSagaRepositoryContext(NHibernateContext context)
        {
            _context = context;
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Session.GetAsync<TSaga>(correlationId, LockMode.None);
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query)
        {
            IList<Guid> instances = await _context.Session.QueryOver<TSaga>()
                .Where(query.FilterExpression)
                .Select(x => x.CorrelationId)
                .ListAsync<Guid>()
                .ConfigureAwait(false);

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, instances);
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        async Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _context.Session.Dispose();
        }
    }
}
