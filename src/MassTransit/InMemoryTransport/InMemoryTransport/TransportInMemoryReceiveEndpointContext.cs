namespace MassTransit.InMemoryTransport
{
    using System;
    using Configuration;
    using Context;
    using Fabric;
    using Transports;


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

        public override void AddConsumeAgent(IAgent agent)
        {
            throw new NotSupportedException();
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return exception;
        }

        public IMessageFabric MessageFabric => _hostConfiguration.TransportProvider.MessageFabric;

        public void ConfigureTopology()
        {
            var builder = new InMemoryConsumeTopologyBuilder(MessageFabric);

            var queueName = _configuration.InputAddress.GetEndpointName();

            builder.Queue = queueName;
            builder.QueueDeclare(queueName, _configuration.Transport.GetConcurrentMessageLimit());
            builder.Exchange = queueName;
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
