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
        ///
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="schedulerEndpointAddress">The endpoint address of the scheduler service</param>
        /// <returns></returns>
        public static IMessageScheduler CreateMessageScheduler(this IBus bus, Uri schedulerEndpointAddress)
        {
            Task<ISendEndpoint> GetSchedulerEndpoint() => bus.GetSendEndpoint(schedulerEndpointAddress);

            return new MessageScheduler(new EndpointScheduleMessageProvider(GetSchedulerEndpoint));
        }

        /// <summary>
        /// Create a message scheduler that uses an external message scheduler, such as Quartz.NET or Hangfire, to
        /// schedule messages. This should not be used with the broker-specific message schedulers. Scheduled messages
        /// are published to the external message scheduler, rather than uses a preconfigured endpoint address.
        ///
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new PublishScheduleMessageProvider(bus));
        }
    }
}
