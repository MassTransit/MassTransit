namespace MassTransit.InMemoryTransport
{
    using System;
    using Configuration;
    using Transports;
    using Transports.Fabric;


    public class TransportInMemoryReceiveEndpointContext :
        BaseReceiveEndpointContext,
        InMemoryReceiveEndpointContext
    {
        readonly IInMemoryReceiveEndpointConfiguration _configuration;
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public TransportInMemoryReceiveEndpointContext(IInMemoryHostConfiguration hostConfiguration, IInMemoryReceiveEndpointConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric => _hostConfiguration.TransportProvider.MessageFabric;
        public InMemoryTransportContext TransportContext => _hostConfiguration.TransportProvider;

        public override void AddSendAgent(IAgent agent)
        {
            throw new NotSupportedException();
        }

        public override void AddConsumeAgent(IAgent agent)
        {
            throw new NotSupportedException();
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return exception;
        }

        public void ConfigureTopology()
        {
            var builder = new MessageFabricConsumeTopologyBuilder<InMemoryTransportContext, InMemoryTransportMessage>(_hostConfiguration.TransportProvider,
                MessageFabric);

            var name = _configuration.InputAddress.GetEndpointName();

            builder.Exchange = name;
            builder.ExchangeDeclare(name, ExchangeType.FanOut);

            builder.Queue = name;
            builder.QueueDeclare(name);

            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new InMemorySendTransportProvider(_hostConfiguration.TransportProvider, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new InMemoryPublishTransportProvider(_hostConfiguration.TransportProvider, this);
        }
    }
}
