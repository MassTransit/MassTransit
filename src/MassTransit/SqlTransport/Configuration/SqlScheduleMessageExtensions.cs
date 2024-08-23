namespace MassTransit
{
    using System;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Scheduling;
    using SqlTransport.Configuration;
    using Transports;


    public static class SqlScheduleMessageExtensions
    {
        /// <summary>
        /// Uses the SQL transport's built-in message scheduler
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use the renamed UseSqlMessageScheduler instead")]
        public static void UseDbMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            UseSqlMessageScheduler(configurator);
        }

        /// <summary>
        /// Uses the SQL transport's built-in message scheduler
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseSqlMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new SqlMessageSchedulerSpecification();

            configurator.AddPrePipeSpecification(pipeBuilderConfigurator);
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the SQL Transport message enqueue time to schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddSqlMessageScheduler(this IBusRegistrationConfigurator configurator)
        {
            configurator.TryAddScoped<IMessageScheduler>(provider =>
            {
                var busInstance = provider.GetRequiredService<Bind<IBus,IBusInstance>>().Value;
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();

                var hostConfiguration = busInstance.HostConfiguration as ISqlHostConfiguration
                    ?? throw new ArgumentException("The SQL transport configuration was not found");

                return new MessageScheduler(new SqlScheduleMessageProvider(hostConfiguration, sendEndpointProvider), busInstance.Bus.Topology);
            });
        }

        /// <summary>
        /// Add a <see cref="IMessageScheduler" /> to the container that uses the SQL Transport message enqueue time to schedule messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void AddSqlMessageScheduler<TBus>(this IBusRegistrationConfigurator<TBus> configurator)
            where TBus : class, IBus
        {
            configurator.TryAddScoped(provider =>
            {
                var busInstance = provider.GetRequiredService<Bind<TBus, IBusInstance>>().Value;
                var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();

                var hostConfiguration = busInstance.HostConfiguration as ISqlHostConfiguration
                    ?? throw new ArgumentException("The SQL transport configuration was not found");

                return Bind<TBus>.Create<IMessageScheduler>(
                    new MessageScheduler(new SqlScheduleMessageProvider(hostConfiguration, sendEndpointProvider), busInstance.Bus.Topology));
            });
        }
    }
}
