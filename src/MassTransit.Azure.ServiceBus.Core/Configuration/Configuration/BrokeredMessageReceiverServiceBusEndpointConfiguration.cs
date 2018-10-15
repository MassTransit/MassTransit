namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Context;
    using MassTransit.Configuration;
    using Transports;


    public class BrokeredMessageReceiverServiceBusEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        public BrokeredMessageReceiverServiceBusEndpointConfiguration(IHostConfiguration hostConfiguration, IEndpointConfiguration configuration,
            Uri hostAddress, Uri inputAddress)
            : base(hostConfiguration, configuration)
        {
            HostAddress = hostAddress;
            InputAddress = inputAddress;
        }

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        protected override IReceiveEndpoint CreateReceiveEndpoint(string endpointName, IReceiveTransport receiveTransport,
            ReceiveEndpointContext topology)
        {
            throw new NotImplementedException();
        }

        public override IReceiveEndpoint Build()
        {
            throw new NotImplementedException();
        }
    }
}