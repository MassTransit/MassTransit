namespace MassTransit
{
    using System;
    using RabbitMqTransport.Specifications;


    public static class DelayedExchangeSchedulerExtensions
    {
        /// <summary>
        /// Uses the RabbitMQ Delayed ExchangeName plugin to schedule messages for future delivery. A lightweight
        /// alternative to Quartz, which does not require any storage outside of RabbitMQ.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseDelayedExchangeMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedExchangeMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }
    }
}
