namespace MassTransit
{
    using System;
    using Scheduling;
    using Topology;
    using Transports.Scheduling;


    public static class ActiveMqMessageSchedulerExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the built-in ActiveMQ scheduler to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        [Obsolete("Use the transport independent CreateDelayedMessageScheduler")]
        public static IMessageScheduler CreateActiveMqMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(bus), bus.Topology);
        }

        /// <summary>
        /// Create a message scheduler that uses the built-in ActiveMQ scheduler to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="busTopology"></param>
        /// <returns></returns>
        [Obsolete("Use the transport independent CreateDelayedMessageScheduler")]
        public static IMessageScheduler CreateActiveMqMessageScheduler(this ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(sendEndpointProvider), busTopology);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the ActiveMQ scheduler
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use the transport independent AddDelayedMessageScheduler")]
        public static void AddActiveMqMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddDelayedMessageScheduler();
        }
    }
}
