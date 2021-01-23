namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Hangfire;
    using Hangfire.Common;
    using Hangfire.Server;
    using HangfireIntegration;
    using HangfireIntegration.Configuration;
    using Registration;
    using Scheduling;


    public static class HangfireIntegrationExtensions
    {
        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, IBusRegistrationContext context, string queueName = "hangfire",
            Action<BackgroundJobServerOptions> configureServer = null)
        {
            configurator.UseHangfireScheduler(new BusRegistrationContextComponentResolver(context, DefaultHangfireComponentResolver.Instance), queueName,
                configureServer);
        }

        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, string queueName = "hangfire",
            Action<BackgroundJobServerOptions> configureServer = null)
        {
            configurator.UseHangfireScheduler(DefaultHangfireComponentResolver.Instance, queueName, configureServer);
        }

        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, IHangfireComponentResolver hangfireComponentResolver,
            string queueName = "hangfire",
            Action<BackgroundJobServerOptions> configureServer = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (hangfireComponentResolver == null)
                throw new ArgumentNullException(nameof(hangfireComponentResolver));

            configurator.ReceiveEndpoint(queueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(hangfireComponentResolver), x =>
                {
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId));
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId));
                });
                e.Consumer(() => new ScheduleRecurringMessageConsumer(hangfireComponentResolver));

                var observer = new SchedulerBusObserver(hangfireComponentResolver, e.InputAddress, configureServer);
                configurator.ConnectBusObserver(observer);

                configurator.UseMessageScheduler(e.InputAddress);
            });
        }


        class BusRegistrationContextComponentResolver :
            IHangfireComponentResolver
        {
            readonly Lazy<IBackgroundJobClient> _backgroundJobClient;
            readonly Lazy<IEnumerable<IBackgroundProcess>> _backgroundProcesses;
            readonly Lazy<IJobFilterProvider> _jobFilterProvider;
            readonly Lazy<JobStorage> _jobStorage;
            readonly Lazy<IRecurringJobManager> _recurringJobManager;
            readonly Lazy<ITimeZoneResolver> _timeZoneResolver;

            public BusRegistrationContextComponentResolver(IConfigurationServiceProvider provider, IHangfireComponentResolver resolver)
            {
                _backgroundJobClient = new Lazy<IBackgroundJobClient>(() => provider.GetService<IBackgroundJobClient>() ?? resolver.BackgroundJobClient);
                _recurringJobManager = new Lazy<IRecurringJobManager>(() => provider.GetService<IRecurringJobManager>() ?? resolver.RecurringJobManager);
                _timeZoneResolver = new Lazy<ITimeZoneResolver>(() => provider.GetService<ITimeZoneResolver>() ?? resolver.TimeZoneResolver);
                _jobFilterProvider = new Lazy<IJobFilterProvider>(() => provider.GetService<IJobFilterProvider>() ?? resolver.JobFilterProvider);
                _backgroundProcesses =
                    new Lazy<IEnumerable<IBackgroundProcess>>(() => provider.GetService<IEnumerable<IBackgroundProcess>>() ?? resolver.BackgroundProcesses);
                _jobStorage = new Lazy<JobStorage>(() => provider.GetService<JobStorage>() ?? resolver.JobStorage);
            }

            public IBackgroundJobClient BackgroundJobClient => _backgroundJobClient.Value;
            public IRecurringJobManager RecurringJobManager => _recurringJobManager.Value;
            public ITimeZoneResolver TimeZoneResolver => _timeZoneResolver.Value;
            public IJobFilterProvider JobFilterProvider => _jobFilterProvider.Value;
            public IEnumerable<IBackgroundProcess> BackgroundProcesses => _backgroundProcesses.Value;
            public JobStorage JobStorage => _jobStorage.Value;
        }
    }
}
