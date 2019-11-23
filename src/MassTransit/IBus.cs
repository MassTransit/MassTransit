namespace MassTransit
{
    using System;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transports;


    /// <summary>
    /// A bus is a logical element that includes a local endpoint and zero or more receive endpoints
    /// </summary>
    public interface IBus :
        IPublishEndpoint,
        ISendEndpointProvider,
        IConsumePipeConnector,
        IRequestPipeConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IReceiveObserverConnector,
        IReceiveEndpointObserverConnector,
        IReceiveConnector,
        IProbeSite
    {
        /// <summary>
        /// The InputAddress of the default bus endpoint
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The bus topology
        /// </summary>
        IBusTopology Topology { get; }
    }
}
