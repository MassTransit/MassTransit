namespace MassTransit.Saga
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// The modern query saga repository, which can be used with any storage engine. Leverages the new interfaces for query context.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class LoadSagaRepository<TSaga> :
        ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadSagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public LoadSagaRepository(ILoadSagaRepositoryContextFactory<TSaga> repositoryContextFactory)
        {
            _repositoryContextFactory = repositoryContextFactory;
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _repositoryContextFactory.Execute(context => context.Load(correlationId));
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("loadSagaRepository");

            _repositoryContextFactory.Probe(scope);
        }
    }
}
