namespace MassTransit
{
    using System;
    using GreenPipes;
    using HangfireIntegration;
    using Scheduling;


    public static class HangfireIntegrationExtensions
    {
        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, string queueName = "hangfire")
        {
            configurator.UseHangfireScheduler(DefaultHangfireComponentResolver.Instance.Value, queueName);
        }

        public static void UseHangfireScheduler(this IBusFactoryConfigurator configurator, IHangfireComponentResolver hangfireComponentResolver,
            string queueName = "hangfire")
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

                configurator.UseMessageScheduler(e.InputAddress);
            });
        }
    }
}
