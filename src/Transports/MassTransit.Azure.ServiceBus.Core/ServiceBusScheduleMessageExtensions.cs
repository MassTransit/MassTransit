namespace MassTransit
{
    using System;
    using Azure.ServiceBus.Core.Specifications;
    using GreenPipes;


    public static class ServiceBusScheduleMessageExtensions
    {
        /// <summary>
        /// Uses the Enqueue time of Service Bus messages to schedule future delivery of messages instead
        /// of using Quartz. A natively supported feature that is highly reliable.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseServiceBusMessageScheduler(this IPipeConfigurator<ConsumeContext> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new ServiceBusMessageSchedulerSpecification();

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}
