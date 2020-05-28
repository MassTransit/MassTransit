namespace MassTransit.Transports.InMemory.Builders
{
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;


    public class InMemoryReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IInMemoryReceiveEndpointConfiguration _configuration;
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryReceiveEndpointBuilder(IInMemoryHostConfiguration hostConfiguration, IInMemoryReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            _configuration.Topology.Consume
                .GetMessageTopology<T>()
                .Bind();

            return base.ConnectConsumePipe(pipe);
        }

        public ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var builder = _hostConfiguration.TransportProvider.CreateConsumeTopologyBuilder();

            var queueName = _configuration.InputAddress.GetQueueOrExchangeName();

            builder.Queue = queueName;
            builder.QueueDeclare(queueName, _configuration.ConcurrencyLimit);
            builder.Exchange = queueName;
            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);

            var context = new InMemoryReceiveEndpointContext(_hostConfiguration, _configuration);

            context.GetOrAddPayload(() => _hostConfiguration.HostTopology);

            return context;
        }
    }
}
