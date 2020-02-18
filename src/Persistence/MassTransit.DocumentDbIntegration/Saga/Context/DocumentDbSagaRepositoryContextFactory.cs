namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Saga;


    public class DocumentDbSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly DatabaseContext<TSaga> _context;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public DocumentDbSagaRepositoryContextFactory(DatabaseContext<TSaga> context, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
        {
            _context = context;
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "documentdb");
            context.Add("properties", TypeCache<TSaga>.ReadWritePropertyCache.Select(x => x.Property.Name).ToArray());
        }

        public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var repositoryContext = new DocumentDbSagaRepositoryContext<TSaga, T>(_context, context, _factory);

            return next.Send(repositoryContext);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            // This will not work for Document Db because the .Where needs to look for [JsonProperty("id")],
            // and if you pass in CorrelationId property, the ISaga doesn't have that property. Can we .Select() it out?
            IEnumerable<TSaga> sagas = await _context.Client.CreateDocumentQuery<TSaga>(_context.Collection, _context.FeedOptions)
                .Where(query.FilterExpression)
                .QueryAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var repositoryContext = new DocumentDbSagaRepositoryContext<TSaga, T>(_context, context, _factory);

            var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, sagas.ToList());

            await next.Send(queryContext).ConfigureAwait(false);
        }

        public Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var repositoryContext = new DocumentDbSagaRepositoryContext<TSaga>(_context, cancellationToken);

            return asyncMethod(repositoryContext);
        }
    }
}
