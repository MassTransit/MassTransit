namespace MassTransit
{
    using System;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transports;


    /// <summary>
    /// A Bus Host is a transport-neutral reference to a host
    /// </summary>
    public interface IHost :
        IReceiveConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IReceiveEndpointObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// An address that identifies the host
        /// </summary>
        Uri Address { get; }

        IHostTopology Topology { get; }
    }
}
