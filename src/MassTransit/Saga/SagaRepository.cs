namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// The modern saga repository, which can be used with any storage engine. Leverages the new interfaces for consume and query context.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class SagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public SagaRepository(ISagaRepositoryContextFactory<TSaga> repositoryContextFactory)
        {
            _repositoryContextFactory = repositoryContextFactory;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");

            _repositoryContextFactory.Probe(scope);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _repositoryContextFactory.Execute(context => context.Load(correlationId));
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _repositoryContextFactory.Execute<IEnumerable<Guid>>(async context => await context.Query(query).ConfigureAwait(false));
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var correlationId = context.CorrelationId ??
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            try
            {
                await _repositoryContextFactory.Send(context, new SendSagaPipe<TSaga, T>(policy, next, correlationId)).ConfigureAwait(false);
            }
            catch (SagaException exception)
            {
                context.LogFault(this, exception);
                throw;
            }
            catch (DataException exception)
            {
                context.LogFault(this, exception);
                throw;
            }
            catch (Exception exception)
            {
                context.LogFault(this, exception);

                throw new SagaException(exception.Message, typeof(TSaga), typeof(T), Guid.Empty, exception);
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                await _repositoryContextFactory.SendQuery(context, query, new SendQuerySagaPipe<TSaga, T>(policy, next)).ConfigureAwait(false);
            }
            catch (SagaException exception)
            {
                context.LogFault(this, exception);
                throw;
            }
            catch (DataException exception)
            {
                context.LogFault(this, exception);
                throw;
            }
            catch (Exception exception)
            {
                context.LogFault(this, exception);

                throw new SagaException(exception.Message, typeof(TSaga), typeof(T), Guid.Empty, exception);
            }
        }
    }
}
