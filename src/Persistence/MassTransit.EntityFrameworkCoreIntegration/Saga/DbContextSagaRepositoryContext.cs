namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Util;


    public class DbContextSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly DbContext _dbContext;
        readonly ISagaConsumeContextFactory<DbContext, TSaga> _factory;

        public DbContextSagaRepositoryContext(DbContext dbContext, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DbContext, TSaga> factory)
            : base(consumeContext)
        {
            _dbContext = dbContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            throw new NotImplementedException();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            throw new NotImplementedException();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public Task<SagaRepositoryQueryContext<TSaga, TMessage>> Query(ISagaQuery<TSaga> query)
        {
            throw new NotImplementedException();
        }
    }


    public class DbContextSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DbContext _dbContext;

        public DbContextSagaRepositoryContext(DbContext dbContext, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _dbContext = dbContext;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            List<Guid> results = await _dbContext.Set<TSaga>()
                .AsNoTracking()
                .Where(query.FilterExpression)
                .Select(x => x.CorrelationId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, results);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _dbContext.Set<TSaga>()
                .AsNoTracking()
                .Where(x => x.CorrelationId == correlationId)
                .FirstOrDefaultAsync();
        }
    }
}
