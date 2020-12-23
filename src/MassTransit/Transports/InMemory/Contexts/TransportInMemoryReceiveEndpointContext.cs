namespace MassTransit.Transports.InMemory.Contexts
{
    using System;
    using Builders;
    using Configuration;
    using Context;
    using Fabric;
    using GreenPipes.Agents;


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

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.TransportProvider;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.TransportProvider;
        }

        public void ConfigureTopology()
        {
            var builder = new InMemoryConsumeTopologyBuilder(MessageFabric);

            var queueName = _configuration.InputAddress.GetQueueOrExchangeName();

            builder.Queue = queueName;
            builder.QueueDeclare(queueName, _configuration.ConcurrencyLimit);
            builder.Exchange = queueName;
            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);
        }
    }
}
