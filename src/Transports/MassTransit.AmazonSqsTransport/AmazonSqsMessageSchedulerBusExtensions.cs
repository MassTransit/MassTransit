namespace MassTransit
{
    using AmazonSqsTransport.Scheduling;
    using Registration;
    using Scheduling;


    public static class AmazonSqsMessageSchedulerBusExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the built-in AmazonSQS message delay to schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateAmazonSqsMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new DelayedMessageScheduleMessageProvider(bus), bus.Topology);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the SQS message delay to schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddAmazonSqsMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddMessageScheduler(new MessageSchedulerRegistration());
        }


        class MessageSchedulerRegistration :
            IMessageSchedulerRegistration
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.Register(provider => provider.GetRequiredService<IBus>().CreateAmazonSqsMessageScheduler());
            }
        }
    }
}
