namespace TopshelfListener
{
    using System;
    using System.Threading.Tasks;
    using EventContracts;
    using MassTransit;
    using Topshelf;

    public class Program
    {
        public static int Main()
        {
            return (int)HostFactory.Run(cfg => cfg.Service(x => new EventConsumerService()));
        }

        class EventConsumerService :
            ServiceControl
        {
            IBusControl _bus;

            public bool Start(HostControl hostControl)
            {
                _bus = ConfigureBus();
                _bus.Start(TimeSpan.FromSeconds(10));

                return true;
            }

            public bool Stop(HostControl hostControl)
            {
                _bus?.Stop(TimeSpan.FromSeconds(30));

                return true;
            }

            IBusControl ConfigureBus()
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        e.Consumer<EventConsumer>();
                    });
                });
            }
        }

        class EventConsumer :
            IConsumer<ValueEntered>
        {
            public async Task Consume(ConsumeContext<ValueEntered> context)
            {
                Console.WriteLine("Value: {0}", context.Message.Value);
            }
        }
    }
}
