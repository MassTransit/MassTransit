namespace MassTransit
{
    using System;
    using Configuration;


    public static class ServiceBusScheduleMessageExtensions
    {
        /// <summary>
        /// Uses the Enqueue time of Service Bus messages to schedule future delivery of messages instead
        /// of using Quartz. A natively supported feature that is highly reliable.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseServiceBusMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new ServiceBusMessageSchedulerSpecification();

            configurator.AddPrePipeSpecification(pipeBuilderConfigurator);
        }
    }
}
