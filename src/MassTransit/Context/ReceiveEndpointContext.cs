namespace MassTransit.Context
{
    using System;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transports;


    /// <summary>
    /// The context of a receive endpoint
    /// </summary>
    public interface ReceiveEndpointContext :
        PipeContext,
        ISendObserverConnector,
        IPublishObserverConnector,
        IReceiveTransportObserverConnector,
        IReceiveObserverConnector,
        IReceiveEndpointObserverConnector
    {
        Uri InputAddress { get; }

        IReceiveEndpointObserver EndpointObservers { get; }
        IReceiveObserver ReceiveObservers { get; }
        IReceiveTransportObserver TransportObservers { get; }

        ILogContext LogContext { get; }

        IPublishTopology Publish { get; }

        IReceivePipe ReceivePipe { get; }

        ISendPipe SendPipe { get; }

        IMessageSerializer Serializer { get; }

        IPublishEndpointProvider PublishEndpointProvider { get; }

        ISendEndpointProvider SendEndpointProvider { get; }
    }
}
