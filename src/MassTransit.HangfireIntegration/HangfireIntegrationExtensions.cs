namespace MassTransit
{
    using System;
    using System.Threading;
    using GreenPipes;
    using Hangfire;
    using HangfireIntegration;
    using Scheduling;


    public static class HangfireIntegrationExtensions
    {
        static readonly Lazy<IBackgroundJobClient> Cached =
            new Lazy<IBackgroundJobClient>(() => new BackgroundJobClient(), LazyThreadSafetyMode.PublicationOnly);

        static readonly Func<IBackgroundJobClient> ClientFactory = () => Cached.Value;

        public static Uri UseHangfireScheduler(this IBusFactoryConfigurator configurator, string queueName = "hangfire")
        {
            return configurator.UseHangfireScheduler(ClientFactory, queueName);
        }

        public static Uri UseHangfireScheduler(this IBusFactoryConfigurator configurator, Func<IBackgroundJobClient> backgroundJobClient,
            string queueName = "hangfire")
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (backgroundJobClient == null)
                throw new ArgumentNullException(nameof(backgroundJobClient));

            Uri inputAddress = null;

            configurator.ReceiveEndpoint(queueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(backgroundJobClient), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));
                //
                // e.Consumer(() => new CancelScheduledMessageConsumer(schedulerInstance), x =>
                //     x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                // var observer = new SchedulerBusObserver(schedulerInstance, e.InputAddress);
                // configurator.ConnectBusObserver(observer);

                inputAddress = e.InputAddress;
            });

            return inputAddress;
        }
    }
}
