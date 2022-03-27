namespace MassTransit
{
    using System;
    using Hangfire;
    using HangfireIntegration;
    using Scheduling;


    public static class HangfireIntegrationExtensions
    {
        [Obsolete("Use the new .AddHangfireConsumers() method, combined with AddHangfire(), to configure the Quartz scheduler")]
        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, IBusRegistrationContext context, string queueName = "hangfire",
            Action<BackgroundJobServerOptions>? configureServer = null)
        {
            UseHangfireScheduler(configurator, new BusRegistrationContextComponentResolver(context, DefaultHangfireComponentResolver.Instance), queueName,
                configureServer);
        }

        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, string queueName = "hangfire",
            Action<BackgroundJobServerOptions>? configureServer = null)
        {
            UseHangfireScheduler(configurator, DefaultHangfireComponentResolver.Instance, queueName, configureServer);
        }

        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, IHangfireComponentResolver hangfireComponentResolver,
            string queueName = "hangfire",
            Action<BackgroundJobServerOptions>? configureServer = null)
        {
            UseHangfireScheduler(configurator, options =>
            {
                options.QueueName = queueName;
                options.ComponentResolver = hangfireComponentResolver;
                options.ConfigureServer = configureServer;
            });
        }

        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, Action<HangfireSchedulerOptions>? configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var options = new HangfireSchedulerOptions();
            configure?.Invoke(options);

            configurator.ReceiveEndpoint(options.QueueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(options.ComponentResolver.BackgroundJobClient, options.ComponentResolver.JobStorage), x =>
                {
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId));
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId));
                });
                e.Consumer(() =>
                    new ScheduleRecurringMessageConsumer(options.ComponentResolver.RecurringJobManager, options.ComponentResolver.TimeZoneResolver));

                var observer = new SchedulerBusObserver(options);
                configurator.ConnectBusObserver(observer);

                configurator.UseMessageScheduler(e.InputAddress);
            });
        }
    }
}
