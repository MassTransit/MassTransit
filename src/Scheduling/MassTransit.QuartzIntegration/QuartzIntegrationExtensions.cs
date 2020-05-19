namespace MassTransit
{
    using System;
    using GreenPipes;
    using Quartz;
    using Quartz.Impl;
    using QuartzIntegration;
    using QuartzIntegration.Configuration;
    using Scheduling;
    using Util;


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

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, out IScheduler scheduler, string queueName = "quartz")
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            return UseInMemoryScheduler(configurator, schedulerFactory, out scheduler, queueName);
        }

        public static Uri UseInMemoryScheduler(this IBusFactoryConfigurator configurator, ISchedulerFactory schedulerFactory, out IScheduler scheduler,
            string queueName = "quartz")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (schedulerFactory == null)
                throw new ArgumentNullException(nameof(schedulerFactory));

            scheduler = TaskUtil.Await(() => schedulerFactory.GetScheduler());

            Uri inputAddress = null;

            var schedulerInstance = scheduler;

            configurator.ReceiveEndpoint(queueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(schedulerInstance), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));

                e.Consumer(() => new CancelScheduledMessageConsumer(schedulerInstance), x =>
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                var observer = new SchedulerBusObserver(schedulerInstance, e.InputAddress);
                configurator.ConnectBusObserver(observer);

                inputAddress = e.InputAddress;
            });

            return inputAddress;
        }
    }
}
