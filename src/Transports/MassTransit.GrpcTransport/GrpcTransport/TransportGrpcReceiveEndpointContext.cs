namespace MassTransit.GrpcTransport
{
    using System;
    using Configuration;
    using Fabric;
    using Transports;
    using Transports.Fabric;


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

        public IMessageFabric<NodeContext, GrpcTransportMessage> MessageFabric => _hostConfiguration.TransportProvider.MessageFabric;

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

        public IGrpcTransportProvider TransportProvider => _hostConfiguration.TransportProvider;

        public void ConfigureTopology(NodeContext nodeContext)
        {
            var builder = new MessageFabricConsumeTopologyBuilder<NodeContext, GrpcTransportMessage>(nodeContext, MessageFabric);

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
            return new GrpcSendTransportProvider(_hostConfiguration.TransportProvider, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new GrpcPublishTransportProvider(_hostConfiguration.TransportProvider, this);
        }
    }
}
