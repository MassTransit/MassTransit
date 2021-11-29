namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public class CosmosSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public CosmosSagaRepositoryContextFactory(DatabaseContext<TSaga> context, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
        {
            _context = context;
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "cosmosdb");
            context.Add("properties", TypeCache<TSaga>.ReadWritePropertyCache.Select(x => x.Property.Name).ToArray());
        }

        public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var repositoryContext = new CosmosSagaRepositoryContext<TSaga, T>(_context, context, _factory);

            return next.Send(repositoryContext);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            IEnumerable<TSaga> sagas = await _context.Container.GetItemLinqQueryable<TSaga>()
                .Where(query.FilterExpression)
                .QueryAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var repositoryContext = new CosmosSagaRepositoryContext<TSaga, T>(_context, context, _factory);

            var queryContext = new DefaultSagaRepositoryQueryContext<TSaga, T>(repositoryContext, sagas.Select(x => x.CorrelationId).ToList());

            await next.Send(queryContext).ConfigureAwait(false);
        }

        public Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var repositoryContext = new CosmosSagaRepositoryContext<TSaga>(_context, cancellationToken);

            return asyncMethod(repositoryContext);
        }
    }
}
