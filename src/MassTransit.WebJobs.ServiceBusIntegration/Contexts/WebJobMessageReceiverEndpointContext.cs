namespace MassTransit.WebJobs.ServiceBusIntegration.Contexts
{
    using System;
    using System.Threading;
    using Context;
    using MassTransit.Configuration;
    using Microsoft.Azure.WebJobs;


    public class WebJobMessageReceiverEndpointContext :
        BaseReceiveEndpointContext
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;

        public WebJobMessageReceiverEndpointContext(IReceiveEndpointConfiguration configuration, Uri inputAddress, IBinder binder,
            CancellationToken cancellationToken)
            : base(configuration)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;

            InputAddress = inputAddress ?? configuration.InputAddress;
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ServiceBusAttributeSendTransportProvider(_binder, _cancellationToken);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new ServiceBusAttributePublishTransportProvider(_binder, _cancellationToken);
        }
    }
}
