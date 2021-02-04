namespace MassTransit
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using GreenPipes;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using QuartzIntegration;
    using QuartzIntegration.Configuration;
    using Scheduling;
    using Util;


    public static class QuartzIntegrationExtensions
    {
        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, IBusRegistrationContext context, string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return configurator.UseInMemoryScheduler(context.GetService<ISchedulerFactory>() ?? new StdSchedulerFactory(), context.GetService<IJobFactory>(),
                queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var schedulerFactory = new StdSchedulerFactory(GetDefaultConfiguration());

            return configurator.UseInMemoryScheduler(schedulerFactory, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, ISchedulerFactory schedulerFactory, string queueName = "quartz")
        {
            return UseInMemoryScheduler(configurator, schedulerFactory, out _, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, ISchedulerFactory schedulerFactory, IJobFactory jobFactory,
            string queueName = "quartz")
        {
            return UseInMemoryScheduler(configurator, schedulerFactory, jobFactory, out _, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, out Task<IScheduler> schedulerTask, string queueName = "quartz")
        {
            var schedulerFactory = new StdSchedulerFactory(GetDefaultConfiguration());

            return UseInMemoryScheduler(configurator, schedulerFactory, out schedulerTask, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, ISchedulerFactory schedulerFactory,
            out Task<IScheduler> schedulerTask, string queueName = "quartz")
        {
            return UseInMemoryScheduler(configurator, schedulerFactory, null, out schedulerTask, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, ISchedulerFactory schedulerFactory, IJobFactory jobFactory,
            out Task<IScheduler> schedulerTask, string queueName = "quartz")
        {
            return configurator.UseInMemoryScheduler(out schedulerTask, options =>
            {
                options.SchedulerFactory = schedulerFactory;
                options.QueueName = queueName;
                options.JobFactory = jobFactory;
            });
        }

        static NameValueCollection GetDefaultConfiguration()
        {
            var configuration = new NameValueCollection
            {
                {"quartz.scheduler.instanceName", $"MassTransit-{NewId.Next().ToString(FormatUtil.Formatter)}"},
                {"quartz.threadPool.maxConcurrency", Environment.ProcessorCount.ToString("F0")}
            };

            return configuration;
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, Action<InMemorySchedulerOptions> configure)
        {
            return configurator.UseInMemoryScheduler(out _, configure);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, out Task<IScheduler> schedulerTask,
            Action<InMemorySchedulerOptions> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var options = new InMemorySchedulerOptions();
            configure?.Invoke(options);

            if (options.SchedulerFactory == null)
                throw new ArgumentNullException(nameof(options.SchedulerFactory));

            Uri inputAddress = null;

            var observer = new SchedulerBusObserver(options);

            schedulerTask = observer.Scheduler;

            configurator.ReceiveEndpoint(options.QueueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(observer.Scheduler), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));

                e.Consumer(() => new CancelScheduledMessageConsumer(observer.Scheduler), x =>
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                configurator.ConnectBusObserver(observer);

                inputAddress = e.InputAddress;
            });

            return inputAddress;
        }
    }
}
