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
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (schedulerFactory == null)
                throw new ArgumentNullException(nameof(schedulerFactory));

            var scheduler = TaskUtil.Await(() => schedulerFactory.GetScheduler());

            Uri inputAddress = null;

            configurator.ReceiveEndpoint(queueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(scheduler), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));

                e.Consumer(() => new CancelScheduledMessageConsumer(scheduler), x =>
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                var observer = new SchedulerBusObserver(scheduler, e.InputAddress);
                configurator.ConnectBusObserver(observer);

                inputAddress = e.InputAddress;
            });

            return inputAddress;
        }
    }
}
