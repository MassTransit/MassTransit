namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Saga;
    using Util;


    public static class ExtensionMethodsForSagas
    {
        public static Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is ILoadSagaRepository<TSaga> loadSagaRepository)
                return loadSagaRepository.ShouldContainSaga(correlationId, timeout);

            if (repository is IQuerySagaRepository<TSaga> querySagaRepository)
                return querySagaRepository.ShouldContainSaga(correlationId, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository", nameof(repository)));
        }

        static async Task<Guid?> ShouldContainSaga<TSaga>(this ILoadSagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                TSaga saga = await repository.Load(correlationId).ConfigureAwait(false);
                if (saga != null)
                    return saga.CorrelationId;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        static async Task<Guid?> ShouldContainSaga<TSaga>(this IQuerySagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            while (DateTime.Now < giveUpAt)
            {
                Guid instanceId = (await repository.Find(query).ConfigureAwait(false)).SingleOrDefault();
                if (instanceId != Guid.Empty)
                    return instanceId;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        public static Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid correlationId, Func<TSaga, bool> condition,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is ILoadSagaRepository<TSaga> loadSagaRepository)
                return loadSagaRepository.ShouldContainSaga(correlationId, condition, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository", nameof(repository)));
        }

        static async Task<Guid?> ShouldContainSaga<TSaga>(this ILoadSagaRepository<TSaga> repository, Guid correlationId, Func<TSaga, bool> condition,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                TSaga saga = await repository.Load(correlationId).ConfigureAwait(false);
                if (saga != null && condition(saga))
                    return saga.CorrelationId;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        public static Task<Guid?> ShouldNotContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is ILoadSagaRepository<TSaga> loadSagaRepository)
                return loadSagaRepository.ShouldNotContainSaga(correlationId, timeout);

            if (repository is IQuerySagaRepository<TSaga> querySagaRepository)
                return querySagaRepository.ShouldNotContainSaga(correlationId, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository", nameof(repository)));
        }

        static async Task<Guid?> ShouldNotContainSaga<TSaga>(this ILoadSagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            TSaga instance = default;
            while (DateTime.Now < giveUpAt)
            {
                instance = await repository.Load(correlationId).ConfigureAwait(false);
                if (instance == null)
                    return default;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return instance.CorrelationId;
        }

        static async Task<Guid?> ShouldNotContainSaga<TSaga>(this IQuerySagaRepository<TSaga> repository, Guid correlationId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            Guid? saga = default;
            while (DateTime.Now < giveUpAt)
            {
                saga = (await repository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga == Guid.Empty)
                    return default;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return saga;
        }

        public static Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Expression<Func<TSaga, bool>> filter,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is IQuerySagaRepository<TSaga> querySagaRepository)
                return querySagaRepository.ShouldContainSaga(filter, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository", nameof(repository)));
        }

        static async Task<Guid?> ShouldContainSaga<TSaga>(this IQuerySagaRepository<TSaga> repository, Expression<Func<TSaga, bool>> filter,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            var query = new SagaQuery<TSaga>(filter);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await repository.Find(query).ConfigureAwait(false)).ToList();
                if (sagas.Count > 0)
                    return sagas.Single();

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }
    }
}
