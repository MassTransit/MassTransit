namespace MassTransit
{
    using System;
    using Configurators;


    public static class DelayedExchangeSchedulerExtensions
    {
        /// <summary>
        /// Uses the RabbitMQ delayed-exchange plug-in for message scheduling.
        /// </summary>
        [Obsolete("Use the transport independent UseDelayedMessageScheduler")]
        public static void UseDelayedExchangeMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new DelayedMessageSchedulerSpecification();

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Uses the RabbitMQ delayed-exchange plug-in for message scheduling.
        /// </summary>
        [Obsolete("Use the transport independent UseDelayedMessageScheduler")]
        public static void UseRabbitMqMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }
    }
}
