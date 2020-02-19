namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using ConsumePipeSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
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

        ReceiveObservable ReceiveObservers { get; }

        IReceiveTransportObserver TransportObservers { get; }

        IConsumePipeSpecification ConsumePipeSpecification { get; }

        ILogContext LogContext { get; }

        IPublishTopology Publish { get; }

        IReceivePipe ReceivePipe { get; }

        ISendPipe SendPipe { get; }

        IMessageSerializer Serializer { get; }

        IPublishEndpointProvider PublishEndpointProvider { get; }

        ISendEndpointProvider SendEndpointProvider { get; }

        /// <summary>
        /// Task completed when dependencies are ready
        /// </summary>
        Task Dependencies { get; }

        IReceivePipeDispatcher CreateReceivePipeDispatcher();
    }
}
