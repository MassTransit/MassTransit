namespace MassTransit.Azure.ServiceBus.Core
{
    using MassTransit.Scheduling;
    using Scheduling;


    public static class ServiceBusMessageSchedulerBusExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the Azure Service Bus ScheduleEnqueueTimeUtc property to
        /// schedule messages.
        ///
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateServiceBusMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new ServiceBusScheduleMessageProvider(bus));
        }
    }
}
