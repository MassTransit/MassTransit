namespace MassTransit
{
    using System;
    using Configuration;
    using HangfireIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;


    public static class HangfireRegistrationExtensions
    {
        /// <summary>
        /// Add the Hangfire consumers to the bus, using <see cref="HangfireEndpointOptions" /> for configuration. Also registers the
        /// Hangfire Bus Observer, so that Hangfire is started/stopped with the bus.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure">Configure the Hangfire options</param>
        public static void AddHangfireConsumers(this IBusRegistrationConfigurator configurator, Action<HangfireEndpointOptions>? configure = null)
        {
            OptionsBuilder<HangfireEndpointOptions> options = configurator.AddOptions<HangfireEndpointOptions>();
            if (configure != null)
                options.Configure(configure);

            configurator.AddBusObserver<HangfireBusObserver>();

            configurator.TryAddSingleton<HangfireEndpointDefinition>();

            configurator.AddConsumer<ScheduleMessageConsumer, ScheduleMessageConsumerDefinition>();
            configurator.AddConsumer<ScheduleRecurringMessageConsumer, ScheduleRecurringMessageConsumerDefinition>();
        }

        /// <summary>
        /// When manually configuring a receive endpoint, configure the Hangfire consumers for this endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        public static void ConfigureHangfireConsumers(this IReceiveEndpointConfigurator configurator, IBusRegistrationContext context)
        {
            configurator.ConfigureConsumer<ScheduleMessageConsumer>(context);
            configurator.ConfigureConsumer<ScheduleRecurringMessageConsumer>(context);
        }
    }
}
