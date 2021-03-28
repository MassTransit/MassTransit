namespace MassTransit
{
    using System;
    using Scheduling;
    using Topology;
    using Transports.Scheduling;


    public static class RabbitMqMessageSchedulerExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the RabbitMQ Delayed Exchange plug-in to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        [Obsolete("Use the transport independent CreateDelayedMessageScheduler")]
        public static IMessageScheduler CreateRabbitMqMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(bus), bus.Topology);
        }

        /// <summary>
        /// Create a message scheduler that uses the RabbitMQ Delayed Exchange plug-in to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use the transport independent CreateDelayedMessageScheduler")]
        public static IMessageScheduler CreateRabbitMqMessageScheduler(this ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(sendEndpointProvider), busTopology);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses RabbitMq delayed exchange plug-in to
        /// schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use the transport independent AddDelayedMessageScheduler")]
        public static void AddRabbitMqMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddDelayedMessageScheduler();
        }
    }
}
