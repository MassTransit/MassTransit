namespace Grid.Distributor.Worker
{
    using System;
    using System.Configuration;
    using MassTransit;
    using Shared;

    public class DoWork :
        IServiceInterface,
        IDisposable
    {
        public DoWork()
        {
            DataBus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"]);
                    x.SetPurgeOnStartup(true);

                    x.UseMsmq();
                    x.UseMulticastSubscriptionClient();
                    x.UseControlBus();

                    x.SetConcurrentConsumerLimit(4);

                    x.Worker(w => w.Consumer<SimpleWorkItemConsumer>());
                });

            ControlBus = DataBus.ControlBus;
        }

        public IServiceBus ControlBus { get; set; }
        public IServiceBus DataBus { get; set; }

        public void Dispose()
        {
        }

        public void Start()
        {
            DataBus.InboundPipeline.View(Console.WriteLine);
        }

        public void Stop()
        {
        }
    }
}