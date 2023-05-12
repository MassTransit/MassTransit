namespace MassTransit.Testing
{
    using System;


    public static class SagaTestHarnessExtensions
    {
        public static SagaTestHarness<T> Saga<T>(this BusTestHarness harness, string queueName = null)
            where T : class, ISaga
        {
            var repository = new InMemorySagaRepository<T>();

            return new SagaTestHarness<T>(harness, repository, repository, repository, queueName);
        }

        public static SagaTestHarness<T> Saga<T>(this BusTestHarness harness, ISagaRepository<T> repository, string queueName = null)
            where T : class, ISaga
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            var querySagaRepository = repository as IQuerySagaRepository<T>;
            var loadSagaRepository = repository as ILoadSagaRepository<T>;

            return new SagaTestHarness<T>(harness, repository, querySagaRepository, loadSagaRepository, queueName);
        }
    }
}
