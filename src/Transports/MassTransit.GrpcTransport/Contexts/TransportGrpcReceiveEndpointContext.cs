namespace MassTransit.GrpcTransport.Contexts
{
    using System;
    using Builders;
    using Configuration;
    using Context;
    using Fabric;
    using GreenPipes.Agents;
    using Integration;
    using Transports.InMemory;


    public class TransportGrpcReceiveEndpointContext :
        BaseReceiveEndpointContext,
        GrpcReceiveEndpointContext
    {
        readonly IGrpcReceiveEndpointConfiguration _configuration;
        readonly IGrpcHostConfiguration _hostConfiguration;

        public TransportGrpcReceiveEndpointContext(IGrpcHostConfiguration hostConfiguration, IGrpcReceiveEndpointConfiguration configuration)
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

        public IGrpcTransportProvider TransportProvider => _hostConfiguration.TransportProvider;

        public void ConfigureTopology(NodeContext nodeContext)
        {
            var builder = new GrpcConsumeTopologyBuilder(nodeContext, MessageFabric);

            var queueName = _configuration.InputAddress.GetQueueOrExchangeName();

            builder.Queue = queueName;
            builder.QueueDeclare(queueName, _configuration.Transport.GetConcurrentMessageLimit());
            builder.Exchange = queueName;
            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new GrpcSendTransportProvider(_hostConfiguration, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new GrpcPublishTransportProvider(_hostConfiguration, this);
        }
    }
}
