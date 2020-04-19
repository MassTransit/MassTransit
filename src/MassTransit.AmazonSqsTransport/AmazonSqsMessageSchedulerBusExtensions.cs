namespace MassTransit.AmazonSqsTransport
{
    using MassTransit.Scheduling;
    using Scheduling;


    public static class AmazonSqsMessageSchedulerBusExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the built-in AmazonSQS message delay to schedule messages.
        ///
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateAmazonSqsMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new DelayedMessageScheduleMessageProvider(bus));
        }
    }
}
