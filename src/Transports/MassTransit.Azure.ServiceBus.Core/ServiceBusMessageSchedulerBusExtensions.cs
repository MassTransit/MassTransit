namespace MassTransit
{
    using Azure.ServiceBus.Core.Scheduling;
    using Registration;
    using Scheduling;


    public static class ServiceBusMessageSchedulerBusExtensions
    {
        /// <summary>
        /// Create a message scheduler that uses the Azure Service Bus ScheduleEnqueueTimeUtc property to
        /// schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateServiceBusMessageScheduler(this IBus bus)
        {
            return new MessageScheduler(new ServiceBusScheduleMessageProvider(bus), bus.Topology);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the Azure message enqueue time to schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddServiceBusMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.AddMessageScheduler(new MessageSchedulerRegistration());
        }


        class MessageSchedulerRegistration :
            IMessageSchedulerRegistration
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterSingleInstance(provider => provider.GetRequiredService<IBus>().CreateServiceBusMessageScheduler());
            }
        }
    }
}
