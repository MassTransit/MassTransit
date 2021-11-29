namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Scheduling;


    public static class MessageSchedulerRegistrationExtensions
    {
        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that sends <see cref="ScheduleMessage" />
        /// to an external message scheduler on the specified endpoint address, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schedulerEndpointAddress">The endpoint address where the scheduler is running</param>
        public static void AddMessageScheduler(this IRegistrationConfigurator configurator, Uri schedulerEndpointAddress)
        {
            if (schedulerEndpointAddress == null)
                throw new ArgumentNullException(nameof(schedulerEndpointAddress));

            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
                return sendEndpointProvider.CreateMessageScheduler(bus.Topology, schedulerEndpointAddress);
            });
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that publishes <see cref="ScheduleMessage" />
        /// to an external message scheduler, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddPublishMessageScheduler(this IRegistrationConfigurator configurator)
        {
            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var publishEndpoint = provider.GetRequiredService<IPublishEndpoint>();
                return publishEndpoint.CreateMessageScheduler(bus.Topology);
            });
        }
    }
}
