namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Scheduling;


    public static class MessageSchedulerBusExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
        /// schedule messages. This should not be used with the broker-specific message schedulers.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="schedulerEndpointAddress">The endpoint address of the scheduler service</param>
        /// <returns></returns>
        public static IMessageScheduler CreateMessageScheduler(this IBus bus, Uri schedulerEndpointAddress)
        {
            Task<ISendEndpoint> GetSchedulerEndpoint()
            {
                return bus.GetSendEndpoint(schedulerEndpointAddress);
            }

            return new MessageScheduler(new EndpointScheduleMessageProvider(GetSchedulerEndpoint), bus.Topology);
        }

        /// <summary>
        /// Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
        /// schedule messages. This should not be used with the broker-specific message schedulers.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="busTopology"></param>
        /// <param name="schedulerEndpointAddress">The endpoint address of the scheduler service</param>
        /// <param name="sendEndpointProvider"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateMessageScheduler(this ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology,
            Uri schedulerEndpointAddress)
        {
            Task<ISendEndpoint> GetSchedulerEndpoint()
            {
                return sendEndpointProvider.GetSendEndpoint(schedulerEndpointAddress);
            }

            return new MessageScheduler(new EndpointScheduleMessageProvider(GetSchedulerEndpoint), busTopology);
        }

        /// <summary>
        /// Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
        /// schedule messages. This should not be used with the broker-specific message schedulers. Scheduled messages
        /// are published to the external message scheduler, rather than uses a preconfigured endpoint address.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new PublishScheduleMessageProvider(bus), bus.Topology);
        }

        /// <summary>
        /// Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
        /// schedule messages. This should not be used with the broker-specific message schedulers. Scheduled messages
        /// are published to the external message scheduler, rather than uses a preconfigured endpoint address.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="busTopology"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateMessageScheduler(this IPublishEndpoint publishEndpoint, IBusTopology busTopology)
        {
            return new MessageScheduler(new PublishScheduleMessageProvider(publishEndpoint), busTopology);
        }

        /// <summary>
        /// Create a message scheduler that uses the built-in transport message delay to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateDelayedMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(bus), bus.Topology);
        }

        /// <summary>
        /// Create a message scheduler that uses the built-in transport message delay to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="busTopology"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateDelayedMessageScheduler(this ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(sendEndpointProvider), busTopology);
        }
    }
}
