namespace MassTransit
{
    using System;
    using DependencyInjection;
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
        public static void AddMessageScheduler(this IBusRegistrationConfigurator configurator, Uri schedulerEndpointAddress)
        {
            if (schedulerEndpointAddress == null)
                throw new ArgumentNullException(nameof(schedulerEndpointAddress));

            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
                return sendEndpointProvider.CreateMessageScheduler(bus.Topology, schedulerEndpointAddress);
            });

            configurator.TryAddScoped<IRecurringMessageScheduler>(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
                return new EndpointRecurringMessageScheduler(sendEndpointProvider, schedulerEndpointAddress, bus.Topology);
            });
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that sends <see cref="ScheduleMessage" />
        /// to an external message scheduler on the specified endpoint address, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="schedulerEndpointAddress">The endpoint address where the scheduler is running</param>
        public static void AddMessageScheduler<TBus>(this IBusRegistrationConfigurator<TBus> configurator, Uri schedulerEndpointAddress)
            where TBus : class, IBus
        {
            if (schedulerEndpointAddress == null)
                throw new ArgumentNullException(nameof(schedulerEndpointAddress));

            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<TBus>();
                var sendEndpointProvider = provider.GetRequiredService<Bind<TBus, ISendEndpointProvider>>().Value;
                return Bind<TBus>.Create(sendEndpointProvider.CreateMessageScheduler(bus.Topology, schedulerEndpointAddress));
            });

            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<TBus>();
                var sendEndpointProvider = provider.GetRequiredService<Bind<TBus, ISendEndpointProvider>>().Value;
                return Bind<TBus>.Create<IRecurringMessageScheduler>(
                    new EndpointRecurringMessageScheduler(sendEndpointProvider, schedulerEndpointAddress, bus.Topology));
            });
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that publishes <see cref="ScheduleMessage" />
        /// to an external message scheduler, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddPublishMessageScheduler(this IBusRegistrationConfigurator configurator)
        {
            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var publishEndpoint = provider.GetRequiredService<IPublishEndpoint>();
                return publishEndpoint.CreateMessageScheduler(bus.Topology);
            });

            configurator.TryAddScoped<IRecurringMessageScheduler>(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var publishEndpoint = provider.GetRequiredService<IPublishEndpoint>();
                return new PublishRecurringMessageScheduler(publishEndpoint, bus.Topology);
            });
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that publishes <see cref="ScheduleMessage" />
        /// to an external message scheduler, such as Quartz or Hangfire.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddPublishMessageScheduler<TBus>(this IBusRegistrationConfigurator<TBus> configurator)
            where TBus : class, IBus
        {
            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<TBus>();
                var publishEndpoint = provider.GetRequiredService<Bind<TBus, IPublishEndpoint>>().Value;
                return Bind<TBus>.Create(publishEndpoint.CreateMessageScheduler(bus.Topology));
            });

            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<TBus>();
                var publishEndpoint = provider.GetRequiredService<Bind<TBus, IPublishEndpoint>>().Value;
                return Bind<TBus>.Create<IRecurringMessageScheduler>(new PublishRecurringMessageScheduler(publishEndpoint, bus.Topology));
            });
        }
    }
}
