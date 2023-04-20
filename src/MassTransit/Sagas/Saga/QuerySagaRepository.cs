namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// The modern query saga repository, which can be used with any storage engine. Leverages the new interfaces for query context.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class QuerySagaRepository<TSaga> :
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IQuerySagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public QuerySagaRepository(IQuerySagaRepositoryContextFactory<TSaga> repositoryContextFactory)
        {
            _repositoryContextFactory = repositoryContextFactory;
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _repositoryContextFactory.Execute<IEnumerable<Guid>>(async context => await context.Query(query).ConfigureAwait(false));
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("querySagaRepository");

            _repositoryContextFactory.Probe(scope);
        }
    }
}
