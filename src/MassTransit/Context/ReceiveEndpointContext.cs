namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using ConsumePipeSpecifications;
    using GreenPipes;
    using GreenPipes.Agents;
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
        IReceiveEndpointObserverConnector,
        IProbeSite
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

        /// <summary>
        /// If true (the default), faults should be published when no ResponseAddress or FaultAddress are present.
        /// </summary>
        bool PublishFaults { get; }

        /// <summary>
        /// Convert an unknown exception to a <see cref="ConnectionException"/>, so that it can be used by
        /// the transport retry policy.
        /// </summary>
        /// <param name="exception">The original exception</param>
        /// <param name="message">A contextual message describing when the exception occurred</param>
        /// <returns></returns>
        Exception ConvertException(Exception exception, string message);

        IReceivePipeDispatcher CreateReceivePipeDispatcher();

        /// <summary>
        /// Reset the receive endpoint, which should clear any caches, etc.
        /// </summary>
        void Reset();

        /// <summary>
        /// Add an agent, which should be stopped during shutdown
        /// </summary>
        /// <param name="agent"></param>
        void AddConsumeAgent(IAgent agent);
    }
}
