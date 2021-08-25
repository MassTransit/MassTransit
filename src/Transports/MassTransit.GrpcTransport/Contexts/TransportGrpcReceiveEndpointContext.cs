namespace MassTransit.GrpcTransport.Contexts
{
    using System;
    using Builders;
    using Configuration;
    using Context;
    using Contracts;
    using Fabric;
    using GreenPipes.Agents;
    using Integration;
    using MassTransit.Transports.Outbox;
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

            var name = _configuration.InputAddress.GetQueueOrExchangeName();

            builder.Exchange = name;
            builder.ExchangeDeclare(name, ExchangeType.FanOut);

            builder.Queue = name;
            builder.QueueDeclare(name);

            builder.QueueBind(builder.Exchange, builder.Queue);

            _configuration.Topology.Consume.Apply(builder);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.TransportProvider;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.TransportProvider;
        }

        protected override IPublishTransportProvider CreateDecoratedPublishTransportProvider()
        {
            var transport = CreatePublishTransportProvider();

            if (_hostConfiguration.UseOutbox)
            {
                return new OutboxPublishTransportProvider(_hostConfiguration, transport);
            }
            else
            {
                return transport;
            }
        }

        protected override ISendTransportProvider CreateDecoratedSendTransportProvider()
        {
            var transport = CreateSendTransportProvider();

            if (_hostConfiguration.UseOutbox)
            {
                return new OutboxSendTransportProvider(_hostConfiguration, transport);
            }
            else
            {
                return transport;
            }
        }
    }
}
