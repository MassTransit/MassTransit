namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Saga;


    public abstract class BaseSagaTestHarness<TSaga>
        where TSaga : class, ISaga
    {
        protected BaseSagaTestHarness(ISagaRepository<TSaga> repository, TimeSpan testTimeout)
        {
            QuerySagaRepository = repository as IQuerySagaRepository<TSaga>;

            TestTimeout = testTimeout;
        }

        protected TimeSpan TestTimeout { get; }

        protected IQuerySagaRepository<TSaga> QuerySagaRepository { get; }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> Exists(Guid correlationId, TimeSpan? timeout = default)
        {
            if (QuerySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            while (DateTime.Now < giveUpAt)
            {
                var saga = (await QuerySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        /// <summary>
        /// Waits until at least one saga exists matching the specified filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<IList<Guid>> Match(Expression<Func<TSaga, bool>> filter, TimeSpan? timeout = default)
        {
            if (QuerySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TSaga>(filter);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await QuerySagaRepository.Find(query).ConfigureAwait(false)).ToList();
                if (sagas.Count > 0)
                    return sagas;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return new List<Guid>();
        }

        /// <summary>
        /// Waits until the saga matching the specified correlationId does NOT exist
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> NotExists(Guid correlationId, TimeSpan? timeout = default)
        {
            if (QuerySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            Guid? saga = default;
            while (DateTime.Now < giveUpAt)
            {
                saga = (await QuerySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga == Guid.Empty)
                    return default;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return saga;
        }
    }
}
