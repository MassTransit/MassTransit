namespace MassTransit
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
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
        /// Create a message scheduler that uses the Azure Service Bus ScheduleEnqueueTimeUtc property to
        /// schedule messages.
        /// NOTE that this should only be used to schedule messages outside of a message consumer. Consumers should
        /// use the ScheduleSend extensions on ConsumeContext.
        /// </summary>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="busTopology"></param>
        /// <returns></returns>
        public static IMessageScheduler CreateServiceBusMessageScheduler(this ISendEndpointProvider sendEndpointProvider, IBusTopology busTopology)
        {
            return new MessageScheduler(new ServiceBusScheduleMessageProvider(sendEndpointProvider), busTopology);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the Azure message enqueue time to schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddServiceBusMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
                return sendEndpointProvider.CreateServiceBusMessageScheduler(bus.Topology);
            });
        }
    }
}
