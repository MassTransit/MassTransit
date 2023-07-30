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
        readonly IReliableService _service;

        public ReliableConsumer(ILogger<ReliableConsumer> logger, IReliableService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task Consume(ConsumeContext<Command> context)
        {
            await context.Publish<Event>(new
            {
                context.MessageId,
                Text = "First"
            }, x => x.SetRoutingKey("alpha"));

            await _service.ProduceSecondEvent(context.MessageId.Value);

            if (context.Message.FailWhenConsuming && context.GetRetryAttempt() == 0)
                throw new ApplicationException("You asked me to fail, so I failed");
        }
    }


    public class ReliableService :
        IReliableService
    {
        readonly IPublishEndpoint _publishEndpoint;

        public ReliableService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task ProduceSecondEvent(Guid messageId)
        {
            await _publishEndpoint.Publish<Event>(new
            {
                messageId,
                Text = "Second"
            }, x => x.SetRoutingKey("beta"));
        }
    }


    public interface IReliableService
    {
        Task ProduceSecondEvent(Guid messageId);
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
            IConsumerConfigurator<ReliableEventConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            if (endpointConfigurator is IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.ConfigureConsumeTopology = false;

                configurator.Bind<Event>(ExchangeType.Direct, "alpha");
            }
        }
    }
}
