namespace MassTransit.WebJobs.ServiceBusIntegration.Contexts
{
    using System.Threading;
    using Context;
    using MassTransit.Configuration;
    using Microsoft.Azure.WebJobs;
    using Topology;
    using Transports;


    public class WebJobMessageReceiverEndpointContext :
        BaseReceiveEndpointContext
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;
        readonly IPublishTopology _publishTopology;

        public WebJobMessageReceiverEndpointContext(IReceiveEndpointConfiguration configuration, IBinder binder, CancellationToken cancellationToken)
            : base(configuration)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;

            _publishTopology = configuration.Topology.Publish;
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            ISendTransportProvider sendTransportProvider = new ServiceBusAttributeSendTransportProvider(_binder, _cancellationToken);

            return new SendEndpointProvider(sendTransportProvider, SendObservers, Serializer, InputAddress, SendPipe);
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            var publishTransportProvider = new ServiceBusAttributePublishTransportProvider(_binder, _cancellationToken);

            return new PublishEndpointProvider(publishTransportProvider, HostAddress, PublishObservers, Serializer, InputAddress, PublishPipe,
                _publishTopology);
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
