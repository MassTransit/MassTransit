namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Transports;


    public class BrokeredMessageReceiverServiceBusEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        public BrokeredMessageReceiverServiceBusEndpointConfiguration(IEndpointConfiguration configuration, Uri hostAddress, Uri inputAddress)
            : base(configuration)
        {
            HostAddress = hostAddress;
            InputAddress = inputAddress;
        }

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override IReceiveEndpoint CreateReceiveEndpoint(string endpointName, IReceiveTransport receiveTransport, IReceivePipe receivePipe,
            IReceiveEndpointTopology topology)
        {
            throw new NotImplementedException();
        }

        public override IReceiveEndpoint Build()
        {
            throw new NotImplementedException();
        }
    }
}