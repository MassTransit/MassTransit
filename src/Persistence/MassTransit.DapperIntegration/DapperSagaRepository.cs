namespace MassTransit.DapperIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Saga;


    public class DapperSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaRepository<TSaga> _repository;

        public DapperSagaRepository(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            var consumeContextFactory = new DapperSagaConsumeContextFactory<TSaga>();

            var options = new DapperOptions<TSaga>(connectionString, isolationLevel);
            var repositoryContextFactory = new DapperSagaRepositoryContextFactory<TSaga>(options, consumeContextFactory);

            _repository = new SagaRepository<TSaga>(repositoryContextFactory);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _repository.Load(correlationId);
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _repository.Find(query);
        }

        public void Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.Send(context, policy, next);
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.SendQuery(context, query, policy, next);
        }
    }
}
