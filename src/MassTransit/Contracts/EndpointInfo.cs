namespace MassTransit.Contracts
{
    using System;


    public interface EndpointInfo
    {
        /// <summary>
        /// Uniquely identifies the node hosting the service endpoint
        /// </summary>
        Guid EndpointId { get; }

        /// <summary>
        /// When the endpoint was started
        /// </summary>
        DateTime Started { get; }

        /// <summary>
        /// The control address, where messages related to the service are sent
        /// </summary>
        Uri InstanceAddress { get; }

        /// <summary>
        /// The endpoint address, when actual requests are sent to be processed by this endpoint
        /// </summary>
        Uri EndpointAddress { get; }
    }
}