namespace MassTransit.Transports.InMemory.Builders
{
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;


    public class InMemoryReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IInMemoryHostControl _host;
        readonly IInMemoryReceiveEndpointConfiguration _configuration;

        public InMemoryReceiveEndpointBuilder(IInMemoryHostControl host, IInMemoryReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
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
            var builder = _host.CreateConsumeTopologyBuilder();

            var queueName = _configuration.InputAddress.GetQueueOrExchangeName();

            builder.Queue = queueName;
            builder.QueueDeclare(queueName, _configuration.ConcurrencyLimit);
            builder.Exchange = queueName;
            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);

            return new InMemoryReceiveEndpointContext(_configuration, _host);
        }
    }
}
