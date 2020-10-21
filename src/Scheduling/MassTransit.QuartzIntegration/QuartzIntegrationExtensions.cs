namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using QuartzIntegration;
    using QuartzIntegration.Configuration;
    using Scheduling;


    public static class QuartzIntegrationExtensions
    {
        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

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
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

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
            return UseInMemoryScheduler(configurator, out schedulerTask, options =>
            {
                options.SchedulerFactory = schedulerFactory;
                options.QueueName = queueName;
                options.JobFactory = jobFactory;
            });
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
