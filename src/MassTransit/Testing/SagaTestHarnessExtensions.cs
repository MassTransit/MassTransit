namespace MassTransit.Testing
{
    using System;
    using Saga;


    public static class SagaTestHarnessExtensions
    {
        public static SagaTestHarness<T> Saga<T>(this BusTestHarness harness, string queueName = null)
            where T : class, ISaga
        {
            var repository = new InMemorySagaRepository<T>();

            return new SagaTestHarness<T>(harness, repository, queueName);
        }

        public static SagaTestHarness<T> Saga<T>(this BusTestHarness harness, ISagaRepository<T> repository, string queueName = null)
            where T : class, ISaga
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            return new SagaTestHarness<T>(harness, repository, queueName);
        }
    }
}
