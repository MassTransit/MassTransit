namespace MassTransit.RabbitMqTransport
{
    using System;
    using MassTransit.Scheduling;
    using Scheduling;
    using Topology;


    public static class RabbitMqMessageSchedulerBusExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the RabbitMQ Delayed Exchange plug-in to schedule messages.
        ///
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateRabbitMqMessageScheduler(this IBus bus)
        {
            if (bus.Topology is IRabbitMqHostTopology topology)
                return new MessageScheduler(new DelayedExchangeScheduleMessageProvider(bus, topology));

            throw new ArgumentException("A RabbitMQ bus is required to use the RabbitMQ message scheduler");
        }
    }
}
