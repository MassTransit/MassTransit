namespace MassTransit.Testing
{
    using System;
    using Pipeline.ConsumerFactories;


    public static class ConsumerTestHarnessExtensions
    {
        public static ConsumerTestHarness<T> Consumer<T>(this BusTestHarness harness, string queueName = null)
            where T : class, IConsumer, new()
        {
            var consumerFactory = new DefaultConstructorConsumerFactory<T>();

            return new ConsumerTestHarness<T>(harness, consumerFactory, queueName);
        }

        public static ConsumerTestHarness<T> Consumer<T>(this BusTestHarness harness, IConsumerFactory<T> consumerFactory, string queueName = null)
            where T : class, IConsumer, new()
        {
            return new ConsumerTestHarness<T>(harness, consumerFactory, queueName);
        }

        public static ConsumerTestHarness<T> Consumer<T>(this BusTestHarness harness, Func<T> consumerFactoryMethod, string queueName = null)
            where T : class, IConsumer
        {
            return new ConsumerTestHarness<T>(harness, new DelegateConsumerFactory<T>(consumerFactoryMethod), queueName);
        }
    }
}
