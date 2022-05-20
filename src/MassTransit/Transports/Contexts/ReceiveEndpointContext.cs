#nullable enable
namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using Logging;


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

        IReceiveObserver ReceiveObservers { get; }

        IReceiveTransportObserver TransportObservers { get; }

        ILogContext LogContext { get; }

        IPublishTopology Publish { get; }

        IReceivePipe ReceivePipe { get; }

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

        int PrefetchCount { get; }

        int? ConcurrentMessageLimit { get; }

        ISerialization Serialization { get; }

        /// <summary>
        /// Convert an unknown exception to a <see cref="ConnectionException" />, so that it can be used by
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
        /// Add an consume-side agent, which should be stopped during shutdown
        /// </summary>
        /// <param name="agent"></param>
        void AddConsumeAgent(IAgent agent);

        /// <summary>
        /// Add an agent, which should be stopped during shutdown after consume/send agents have been stopped
        /// </summary>
        /// <param name="agent"></param>
        void AddSendAgent(IAgent agent);
    }
}
