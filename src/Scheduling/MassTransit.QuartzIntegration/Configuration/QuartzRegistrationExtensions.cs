namespace MassTransit
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using QuartzIntegration;


    public static class QuartzRegistrationExtensions
    {
        /// <summary>
        /// Add the Quartz consumers to the bus, using <see cref="QuartzEndpointOptions" /> for configuration. Also registers the
        /// Quartz Bus Observer, so that Quartz is started/stopped with the bus.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure">Configure the Quartz options</param>
        public static void AddQuartzConsumers(this IBusRegistrationConfigurator configurator, Action<QuartzEndpointOptions>? configure = null)
        {
            OptionsBuilder<QuartzEndpointOptions> options = configurator.AddOptions<QuartzEndpointOptions>();
            if (configure != null)
                options.Configure(configure);

            configurator.AddBusObserver<QuartzBusObserver>();

            configurator.TryAddSingleton<QuartzEndpointDefinition>();

            configurator.AddConsumer<ScheduleMessageConsumer, ScheduleMessageConsumerDefinition>();
            configurator.AddConsumer<CancelScheduledMessageConsumer, CancelScheduledMessageConsumerDefinition>();
        }

        /// <summary>
        /// When manually configuring a receive endpoint, configure the Quartz consumers for this endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        public static void ConfigureQuartzConsumers(this IReceiveEndpointConfigurator configurator, IBusRegistrationContext context)
        {
            configurator.ConfigureConsumer<ScheduleMessageConsumer>(context);
            configurator.ConfigureConsumer<CancelScheduledMessageConsumer>(context);
        }
    }
}
