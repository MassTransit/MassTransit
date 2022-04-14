namespace MassTransit.Tests.ReliableMessaging
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Transports.Fabric;
    using Microsoft.Extensions.Logging;


    public class ReliableConsumer :
        IConsumer<Command>
    {
        readonly ILogger<ReliableConsumer> _logger;
        readonly IBusTopology _topology;

        public ReliableConsumer(ILogger<ReliableConsumer> logger, IBus bus)
        {
            _logger = logger;
            _topology = bus.Topology;
        }

        public async Task Consume(ConsumeContext<Command> context)
        {
            await context.Publish<Event>(new
            {
                context.MessageId,
                Text = "First"
            }, x => x.SetRoutingKey("alpha"));

            await context.Publish<Event>(new
            {
                context.MessageId,
                Text = "Second"
            }, x => x.SetRoutingKey("beta"));

            if (context.Message.FailWhenConsuming && context.GetRetryAttempt() == 0)
                throw new ApplicationException("You asked me to fail, so I failed");
        }
    }


    public class ReliableEventConsumer :
        IConsumer<Event>
    {
        public Task Consume(ConsumeContext<Event> context)
        {
            return Task.CompletedTask;
        }
    }


    public class ReliableEventConsumerDefinition :
        ConsumerDefinition<ReliableEventConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReliableEventConsumer> consumerConfigurator)
        {
            if (endpointConfigurator is IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.ConfigureConsumeTopology = false;

                configurator.Bind<Event>(ExchangeType.Direct, "alpha");
            }
        }
    }
}
