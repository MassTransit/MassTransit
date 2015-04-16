namespace Grid.Distributor.Worker
{
    using System.Diagnostics;
    using MassTransit;
    using Topshelf;


    public class WorkerService :
        ServiceControl
    {
        IServiceBus _dataBus;

        public bool Start(HostControl hostControl)
        {
            _dataBus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom(string.Format("rabbitmq://localhost/mt_grid_worker{0}?prefetch=20", Process.GetCurrentProcess().Id));

                    x.UseRabbitMq();
                    x.SetConcurrentConsumerLimit(20);

                    x.Worker(w => w.Consumer<SimpleWorkItemConsumer>());
                });

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _dataBus.Dispose();

            return true;
        }
    }
}