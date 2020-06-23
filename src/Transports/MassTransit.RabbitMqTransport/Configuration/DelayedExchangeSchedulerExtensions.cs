namespace MassTransit
{
    using System;
    using RabbitMqTransport.Specifications;


    public static class DelayedExchangeSchedulerExtensions
    {
        /// <summary>
        /// Uses the RabbitMQ delayed-exchange plug-in for message scheduling.
        /// </summary>
        public static void UseDelayedExchangeMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedExchangeMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Uses the RabbitMQ delayed-exchange plug-in for message scheduling.
        /// </summary>
        public static void UseRabbitMqMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            UseDelayedExchangeMessageScheduler(configurator);
        }
    }
}
