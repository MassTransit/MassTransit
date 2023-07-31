namespace MassTransit
{
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DelayedMessageSchedulerRegistrationExtensions
    {
        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses transport message delay to schedule messages
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddDelayedMessageScheduler(this IBusRegistrationConfigurator configurator)
        {
            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<IBus>();
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
                return sendEndpointProvider.CreateDelayedMessageScheduler(bus.Topology);
            });
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses transport message delay to schedule messages
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddDelayedMessageScheduler<TBus>(this IBusRegistrationConfigurator<TBus> configurator)
            where TBus : class, IBus
        {
            configurator.TryAddScoped(provider =>
            {
                var bus = provider.GetRequiredService<TBus>();
                var sendEndpointProvider = provider.GetRequiredService<Bind<TBus, ISendEndpointProvider>>().Value;
                return Bind<TBus>.Create(sendEndpointProvider.CreateDelayedMessageScheduler(bus.Topology));
            });
        }
    }
}
