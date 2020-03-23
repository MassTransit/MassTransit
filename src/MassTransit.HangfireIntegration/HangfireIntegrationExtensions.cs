namespace MassTransit
{
    using System;
    using GreenPipes;
    using HangfireIntegration;
    using Scheduling;


    public static class HangfireIntegrationExtensions
    {
        public static Uri UseHangfireScheduler(this IBusFactoryConfigurator configurator, string queueName = "hangfire")
        {
            return configurator.UseHangfireScheduler(DefaultHangfireComponentResolver.Instance.Value, queueName);
        }

        public static Uri UseHangfireScheduler(this IBusFactoryConfigurator configurator, IHangfireComponentResolver hangfireComponentResolver,
            string queueName = "hangfire")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (hangfireComponentResolver == null)
                throw new ArgumentNullException(nameof(hangfireComponentResolver));

            Uri inputAddress = null;

            configurator.ReceiveEndpoint(queueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(hangfireComponentResolver), x =>
                {
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId));
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId));
                });
                e.Consumer(() => new ScheduleRecurringMessageConsumer(hangfireComponentResolver));

                configurator.UseMessageScheduler(e.InputAddress);

                // var observer = new SchedulerBusObserver(schedulerInstance, e.InputAddress);
                // configurator.ConnectBusObserver(observer);

                inputAddress = e.InputAddress;
            });

            return inputAddress;
        }
    }
}
