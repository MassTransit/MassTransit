namespace MassTransit
{
    using System;
    using System.Collections.Specialized;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using QuartzIntegration;
    using Scheduling;
    using Util;


    public static class QuartzIntegrationExtensions
    {
        [Obsolete("Use the new .AddQuartzConsumers() method, combined with AddQuartz(), to configure the Quartz scheduler")]
        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, IBusRegistrationContext context, string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return configurator.UseInMemoryScheduler(options =>
            {
                options.SchedulerFactory = context.GetService<ISchedulerFactory>() ?? new StdSchedulerFactory();
                options.QueueName = queueName;
            });
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var schedulerFactory = new StdSchedulerFactory(GetDefaultConfiguration());

            return configurator.UseInMemoryScheduler(schedulerFactory, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, out ISchedulerFactory schedulerFactory, string queueName = "quartz")
        {
            schedulerFactory = new StdSchedulerFactory(GetDefaultConfiguration());

            return UseInMemoryScheduler(configurator, schedulerFactory, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, ISchedulerFactory schedulerFactory, string queueName = "quartz")
        {
            return configurator.UseInMemoryScheduler(options =>
            {
                options.SchedulerFactory = schedulerFactory;
                options.JobFactoryFactory = bus => new MassTransitJobFactory(bus);
                options.QueueName = queueName;
            });
        }

        static NameValueCollection GetDefaultConfiguration()
        {
            var configuration = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", $"MassTransit-{NewId.Next().ToString(FormatUtil.Formatter)}" },
                { "quartz.threadPool.maxConcurrency", Environment.ProcessorCount.ToString("F0") }
            };

            return configuration;
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, Action<QuartzSchedulerOptions>? configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var options = new QuartzSchedulerOptions();
            configure?.Invoke(options);

            if (options.SchedulerFactory == null)
                throw new ArgumentNullException(nameof(options.SchedulerFactory));

            Uri? inputAddress = null;

            var observer = new SchedulerBusObserver(options);

            configurator.ReceiveEndpoint(options.QueueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(options.SchedulerFactory), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));

                e.Consumer(() => new CancelScheduledMessageConsumer(options.SchedulerFactory), x =>
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                configurator.ConnectBusObserver(observer);

                inputAddress = e.InputAddress;
            });

            return inputAddress!;
        }
    }
}
